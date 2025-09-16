using UnityEngine;

public class kingTrigger : MonoBehaviour
{
    [SerializeField] TextAsset KingInformed;
    [SerializeField] TextAsset KingUninformed;
    [SerializeField] TextAsset KingCaveInformed;
    [SerializeField] TextAsset KingForestInformed;
    GameObject dialogueBox = GameManager.instance.dialogueBox;
    bool isTriggered;
    bool win;

    dialogue instance;

    void Start()
    {
        instance = dialogueBox.GetComponent<dialogue>();
        instance.isComplete = false; // Reset the completion flag before starting.
    }

    // Update is called once per frame
    void Update()
    {
        if (isTriggered && Input.GetButtonDown("Interact") && instance.isRunning == false)
        {
            GameManager.instance.inKing = true;
            GameManager.instance.updateProgUI();

            dialogueBox.SetActive(true);

            //instance.isComplete = false; // Reset the completion flag before starting.

            // Both informed, win
            if (GameManager.instance.lordInCaveInformed && GameManager.instance.lordInForestInformed)
            {
                win = true;
                instance.startDialogue(KingInformed);
                // Only forest lord informed
            }
            else if (GameManager.instance.lordInForestInformed)
            {
                win = false;
                instance.startDialogue(KingForestInformed);
                // Only cave lord informed
            }
            else if (GameManager.instance.lordInCaveInformed)
            {
                win = false;
                instance.startDialogue(KingCaveInformed);
                // Neither informed
            }
            else
            {
                win = false;
                instance.startDialogue(KingUninformed);
            }
        }

        if (instance.isComplete == true)
        {
            CheckWinCondition();
        }
    }

    public void CheckWinCondition()
    {
        Debug.Log("Win: " + win);
        if (win)
        {
            GameManager.instance.gameWon();
        }
        else
        {
            GameManager.instance.gameLost();
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
