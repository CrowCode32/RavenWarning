using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class buttonFunctions : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void featherSelect()
    {
        GameManager.instance.OnFeatherDropdownChanged();

    }
    public void trinketSelect()
    {
        GameManager.instance.OnTrinketDropdownChanged();


    }

    public void startGame()
    {
        Debug.Log("Game starting...");
        GameManager.instance.gameStarted = true;
  
        GameManager.instance.stateUnpause();
        GameManager.instance.loadingScene("Graveyard");
        GameManager.instance.playerHUD.SetActive(true);
    }

    public void load()
    {
        //uhhh
    }

    public void options()
    {
        GameManager.instance.journalMenuIndex = 0;
        GameManager.instance.ToggleJournal();
        

    }

    public void credits()
    {
        GameManager.instance.loadingScene("Credits");
        if(GameManager.instance.activeMenu != null)
        {
            GameManager.instance.activeMenu.SetActive(false);
            GameManager.instance.activeMenu = null;
        }
        GameManager.instance.stateUnpause();
        GameManager.instance.playerHUD.SetActive(false);
    }

    public void next()
    {
        GameManager.instance.journalMenus[GameManager.instance.journalMenuIndex].SetActive(false);
        GameManager.instance.journalMenuIndex++;
        if (GameManager.instance.journalMenuIndex>2)
        {
            GameManager.instance.journalMenuIndex = 0;
        }
        GameManager.instance.journalMenus[GameManager.instance.journalMenuIndex].SetActive(true);
    }

    public void prev()
    {
        GameManager.instance.journalMenus[GameManager.instance.journalMenuIndex].SetActive(false);
        GameManager.instance.journalMenuIndex--;
        if (GameManager.instance.journalMenuIndex < 0)
        {
            GameManager.instance.journalMenuIndex = 2;
        }
        GameManager.instance.journalMenus[GameManager.instance.journalMenuIndex].SetActive(true);
    }

    public void close()
    {

        GameManager.instance.ToggleJournal();
        
    }


    public void exit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif

    }

    public void resume()
    {

        GameManager.instance.stateUnpause();
    }

    public void journal()
    {
        GameManager.instance.journalMenuIndex = 1;
        GameManager.instance.ToggleJournal();
    }

    
        
    public void mouseSensitivity()
    {
        GameManager.instance.mouseSensitivity = GameManager.instance.ApplySlider(GameManager.instance.mouseSensitivitySlider);
    }
    public void brightness()
    {
        float val = GameManager.instance.ApplySlider(GameManager.instance.brightnessSlider);

        Color c = GameManager.instance.brightnessImage.color;
        c.a = 1f - val;
        GameManager.instance.brightnessImage.color = c; 
    }

    public void resolution()
    {

    }

    public void mainMenu()
    {
        if(GameManager.instance.activeMenu != null)
        {
            GameManager.instance.activeMenu.SetActive(false);
            GameManager.instance.activeMenu = null;
            GameManager.instance.loseMenuUI.SetActive(false);
        }
        GameManager.instance.loadMainMenu();
        GameManager.instance.playerHUD.SetActive(false);
    }
}
