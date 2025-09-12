using UnityEngine;

public class kingTrigger : MonoBehaviour
{
    [SerializeField] TextAsset KingInformed;
    [SerializeField] TextAsset KingUninformed;
    [SerializeField] TextAsset KingCaveInformed;
    [SerializeField] TextAsset KingForestInformed;
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
        if (isTriggered && Input.GetButtonDown("Interact") && instance.isRunning == false)
        {
            dialogueBox.SetActive(true);

            // Both informed, win
            if (GameManager.instance.lordInCaveInformed && GameManager.instance.lordInForestInformed)
            {
                instance.startDialogue(KingInformed);
            // Only forest lord informed
            } else if (GameManager.instance.lordInForestInformed)
            {
                instance.startDialogue(KingForestInformed);
            // Only cave lord informed
            } else if (GameManager.instance.lordInCaveInformed)
            {
                instance.startDialogue(KingCaveInformed);
            // Neither informed
            } else
            {
                instance.startDialogue(KingUninformed);
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
            dialogueBox.SetActive(false);
        }
    }
}
