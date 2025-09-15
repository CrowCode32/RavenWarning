using UnityEngine;

public class kingdomCheckpoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Kingdom Checkpoint Reached");
            GameManager.instance.inKingdom = true;
            GameManager.instance.loadingScene("Kingdom");
        }
    }
}
