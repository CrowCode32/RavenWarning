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

    public void start()
    {
        //load Graveyard scene
    }

    public void load()
    {
        //uhhh
    }

    public void options()
    {
        
        GameManager.instance.journalMenuUI.SetActive(true);
        GameManager.instance.SettingsMenuUI.SetActive(true);
        
    }

    public void credits()
    {
        //load credits scene
    }

    public void next()
    {

    }

    public void prev()
    {

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
