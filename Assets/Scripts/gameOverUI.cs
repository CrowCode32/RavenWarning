using UnityEngine;
using UnityEngine.SceneManagement;
public class gameOverUI : MonoBehaviour
{
    
    public void OnRetry()
    {
        //GameManager.instance.stateUnpause();

        //GameManager.instance.deathDataReset();
        //GameManager.instance.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);

        GameManager.instance.retryCurrentLevel();
    }

    public void OnQuitToMenu()
    {
        //GameManager.instance.stateUnpause();

        //GameManager.instance.deathDataReset();

        //GameManager.instance.loadingScene("MainMenu");

        GameManager.instance.quitToMainMenu();
    }
}
