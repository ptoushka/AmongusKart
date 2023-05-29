using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public float StartChance = 0.8f;
    public int worth = 1;
    public bool Exist = false;
    public Transform CoinModel;
    public Checkpoints LevelController;
    public ParticleSystem CollectCoin;

    void Start()
    {
        Respawn(StartChance);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Player")) return;
        if (Exist)
        {
            CollectCoin.Play();
            LevelController.AddMoney(worth);
        }
        Exist = false;
    }

    public void Respawn(float chance = 0.8f)
    {
        if (Random.Range(0f, 1f) <= chance) Exist = true;
    }

    void Update()
    {
        if (Exist) CoinModel.localScale = Vector3.one;
        else CoinModel.localScale = Vector3.zero;
    }
}
