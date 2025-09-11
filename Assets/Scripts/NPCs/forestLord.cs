using UnityEngine;

public class forestLord : MonoBehaviour
{
    bool isTriggered;

    void Update()
    {
        if(isTriggered == true && Input.GetButtonDown("Interact"))
        {
            GameManager.instance.lordInformed("Forest");
        }

        isTriggered = false;
        //Destroy(gameObject);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isTriggered = true;
        }
    }
}
