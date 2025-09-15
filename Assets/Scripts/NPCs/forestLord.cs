using UnityEngine;

public class forestLord : MonoBehaviour
{
    bool isTriggered;

    void Update()
    {
        if (isTriggered && Input.GetButtonDown("Interact")) {
            GameManager.instance.InformLordOfForest();
            GameManager.instance.lordInformed("Forest");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isTriggered = true;
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
