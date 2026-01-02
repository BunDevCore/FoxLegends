using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void OnLevel1ButtonPressed()
    {
        SceneManager.LoadScene("Level 1");
    }

    public void OnExitButtonPressed()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }
}
