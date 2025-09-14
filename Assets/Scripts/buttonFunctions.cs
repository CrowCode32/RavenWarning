using UnityEngine;
using UnityEngine.EventSystems;

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
        GameManager.instance.gameStarted = true;
        GameManager.instance.loadingScene("Forest");
        GameManager.instance.stateUnpause();
    }

    public void load()
    {
        //uhhh
    }

    public void options()
    {
        
        GameManager.instance.journalMenuUI.SetActive(true);
        GameManager.instance.journalMenus[0].SetActive(true);
        
    }

    public void credits()
    {
        //load credits scene
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

        GameManager.instance.journalMenuUI.SetActive(false);
        
    }


    public void exit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif

    }
}
