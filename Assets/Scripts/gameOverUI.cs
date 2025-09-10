using UnityEngine;
using UnityEngine.SceneManagement;
public class gameOverUI : MonoBehaviour
{
    
    public void OnRetry()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OnQuitToMenu()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene("Main Menu"); // This is subject to change 
    }
}
