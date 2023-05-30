using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Checkpoints : MonoBehaviour
{
    public int CheckpointsCount = 0;
    public int CheckpointsAmount = 3;
    public int CheckpointBias = 0;
    public int CurrentLap = 1;
    public int TotalLaps = 2;
    public Collider LastCheckpoint;
    public Collider Finish;
    public bool FreezeOnFinish = false;

    public Lava[] lavaRemind; // Rework to Remind func for Reminder Script

    public GameObject Player;
    public float DeathY = -5f;
    public float DeathPenalty = 0f;

    public Vector3 DefaultRotation;

    public TMP_Text LapsText;
    public float time;
    public TMP_Text Timer;
    public AnimPlayer UIPlayer;
    public float[] StarsTimes = {0f, 1f, 2f};
    public Transform[] Stars = {};

    public int LevelID = 0;
    public int CompletedStars = 0;

    public GameObject CoinHolder;

    private bool IsPaused = false;


    IEnumerator DelayedSlow(float wait, float scale)
    {
        yield return new WaitForSeconds(wait);
        Time.timeScale = scale;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsPaused) Resume();
            else Pause();
        }
    }

    public void Pause()
    {
        UIPlayer.Play("Pause");
        IsPaused = true;
        Player.GetComponent<Player>().Pause();
    }

    public void Resume()
    {
        UIPlayer.Play("Unpause");
        IsPaused = false;
        Player.GetComponent<Player>().Resume();
    }

    void FixedUpdate()
    {
        if (IsPaused)
        {
            Time.timeScale = 0.05f;
            // return;
        }
        else
        {
            if (Time.timeScale < 1f) Time.timeScale += 0.05f;
            else Time.timeScale = 1f;
        }

        if (Player.transform.position.y < DeathY) RespawnPlayer();

        if (CurrentLap <= TotalLaps) LapsText.SetText(string.Concat("Lap ", CurrentLap, "/", TotalLaps));
        else LapsText.SetText("Finished!");

        // Timer Code
        if (CurrentLap <= TotalLaps)
        {
            time += Time.deltaTime;

            int mills = (int)((time - Mathf.Floor(time))*100);
            string rem1 = ".";
            if (mills < 10) rem1 = ".0";

            int seconds = (int)(time % 60);
            string rem2 = ":";
            if (seconds < 10) rem2 = ":0";

            int minutes = (int)(((time - seconds) - (time - seconds) % 60) / 60);

            Timer.SetText(string.Concat(minutes, rem2, seconds, rem1, mills));


            CompletedStars = 0;
            for (int i = Stars.Length-1; i >= 0; i--)
            {
                if (StarsTimes[i] < time) Stars[i].GetComponent<AnimPlayer>().Play("Star Drop");
                else CompletedStars = Stars.Length - i;
            }
        }

    }

    public void RespawnPlayer()
    {
        Player.transform.position = LastCheckpoint.transform.position + new Vector3(0, 4, 0);
        Player.transform.localRotation = LastCheckpoint.transform.rotation * Quaternion.Euler(DefaultRotation);
        Player.GetComponent<Player>().Speed = 0;
        Player.GetComponent<Rigidbody>().velocity = Vector3.zero;
        time += DeathPenalty;
        foreach (Lava lava in lavaRemind) lava.Rewind();
    }

    public void RespawnMoney()
    {
        foreach (var coin in CoinHolder.GetComponentsInChildren<Coin>()) coin.Respawn();
    }

    public void AddMoney(int worth = 1)
    {
        PlayerPrefs.SetInt("Money", PlayerPrefs.GetInt("Money", 0) + worth);
        PlayerPrefs.Save();
    }

    public void AddChecked(Collider coll)
    {
        if (LastCheckpoint == coll) return;
        CheckpointsCount++;
        if (coll == Finish && CheckpointsCount >= (CheckpointsAmount - CheckpointBias))
        {
            CurrentLap++;
            RespawnMoney();
            CheckpointsCount = 0;
            Debug.Log("Lapped!");
            if (CurrentLap > TotalLaps)
            {
                Debug.Log("Finish!");
                UIPlayer.Play("Race Complete");
                if (PlayerPrefs.GetFloat(string.Concat(LevelID, "t"), 9999f) > time)
                {
                    PlayerPrefs.SetInt(string.Concat(LevelID, "s"), CompletedStars);
                    PlayerPrefs.SetFloat(string.Concat(LevelID, "t"), time);
                }
                PlayerPrefs.Save();
                StartCoroutine(DelayedSlow(2f, 0.1f));
                if (FreezeOnFinish) Player.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                foreach (Lava lava in lavaRemind) lava.enabled = false;
            }
            LapsText.transform.GetComponent<AnimPlayer>().Play("New Lap");
        }
        LastCheckpoint = coll;
        foreach(Lava lava in lavaRemind) lava.Remind();
    }
}
