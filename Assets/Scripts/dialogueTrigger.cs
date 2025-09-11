using UnityEngine;

public class dialogueTrigger : MonoBehaviour
{
    [SerializeField] TextAsset diaInput;
    [SerializeField] GameObject dialogueBox;
    bool isTriggered;

    dialogue instance;

    void Start()
    {
        instance = dialogueBox.GetComponent<dialogue>();
    }

    // Update is called once per frame
    void Update()
    {

        if (isTriggered && Input.GetButtonDown("Interact"))
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
        }
    }
}
