using UnityEngine;

public class dialogueTrigger : MonoBehaviour
{
    [SerializeField] TextAsset dialogueFile;
    [SerializeField] GameObject dialogueBox;
    bool isTriggered;
    bool reset;

    // Update is called once per frame
    void Update()
    {

        if (isTriggered && Input.GetButtonDown("Interact"))
        {
            dialogueBox.SetActive(true);
            dialogue.instance.setDialogue(dialogueFile);
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
        }
    }
}
