using UnityEngine;

public class dialogueTrigger : MonoBehaviour
{
    [SerializeField] TextAsset diaInput;
    GameObject dialogueBox = GameManager.instance.dialogueBox;
    bool isTriggered;

    dialogue instance;

    void Start()
    {
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
            dialogueBox.SetActive(true);
            instance.startDialogue(diaInput);
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
