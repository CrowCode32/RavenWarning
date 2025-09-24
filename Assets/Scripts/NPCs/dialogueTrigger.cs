using UnityEngine;

public class dialogueTrigger : MonoBehaviour
{
    
    [SerializeField] TextAsset diaInput;
    [SerializeField] GameObject feather;
    GameObject dialogueBox = GameManager.instance.dialogueBox;
    GameData gameData = GameManager.instance.gameData;
    bool isTriggered;

    dialogue instance;

    void Start()
    {
        if (feather != null)
        {
            feather.SetActive(false);
        }

        if (GameManager.instance.dialogueBox != null)
        {
            dialogueBox = GameManager.instance.dialogueBox;
            instance = dialogueBox.GetComponent<dialogue>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isTriggered && Input.GetButtonDown("Interact") && instance.isRunning == false)
        {
            gameData.npcStat++;
            dialogueBox.SetActive(true);
            instance.startDialogue(diaInput);
        }

        if (feather != null && instance.isComplete)
        {
            bool hasFeather = false;
            if (GameManager.instance.feathersAquired.Count > 0)
            {
                for (int i = 0; i < GameManager.instance.feathersAquired.Count; i++)
                {
                    if (GameManager.instance.feathersAquired[i].name == feather.name)
                    {
                        hasFeather = true;
                    }
                }
            }

            if (!hasFeather)
            {
                feather.SetActive(true);
            }

        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isTriggered = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isTriggered = false;
            instance.isRunning = false;
            if (dialogueBox != null) { dialogueBox.SetActive(false); }
        }
    }
}
