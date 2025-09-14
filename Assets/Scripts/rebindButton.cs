using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class rebindButton : MonoBehaviour
{
    [SerializeField] private string actionName;
    [SerializeField] private TextMeshProUGUI buttonLabel;



    public void StartRebind()
    {
        StartCoroutine(WaitForKey());
    }

    private IEnumerator WaitForKey()
    {
        GameManager.instance.keyInput.SetActive(true);

        bool keyChosen = false;

        while(!keyChosen)
        {
            foreach(KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
            {
                if(Input.GetKeyDown(key))
                {
                    InputManager.instance.SetKey(actionName, key);
                    buttonLabel.text = key.ToString();
                    keyChosen = true;
                    GameManager.instance.keyInput.SetActive(false);
                    break;
                }
            }
            yield return null;
        }
    }

}
