using UnityEngine;

public class caveLord : MonoBehaviour
{
    bool isTriggered;

    void Update()
    {
        if (isTriggered && Input.GetButtonDown("Interact"))
        {
            GameManager.instance.lordInformed("Cave");
            GameManager.instance.InformLordOfCave();
            Debug.Log("Informed");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isTriggered = true;
            Debug.Log("Triggered");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isTriggered = false;
        }
    }

}
