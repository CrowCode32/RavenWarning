using UnityEngine;

public class forestCheckpoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Forest Checkpoint Reached");
            GameManager.instance.inForest = true;
            GameManager.instance.loadingScene("Forest");
        }
    }
}
