using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void ChangeScene(string name)
    {
        SceneManager.LoadScene(name);
        Time.timeScale = 1f;
    }

    public void Exit()
    {
        // PlayerPrefs.DeleteAll();
        // Application.Quit();
        UnityEditor.EditorApplication.isPlaying = false;
    }
}
