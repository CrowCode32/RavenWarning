using UnityEngine;

public class caveCheckpoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Cave Checkpoint Reached");
            GameManager.instance.caveReached = true;
            GameManager.instance.updateProgUI();
            //GameManager.instance.LoadScene("Cave");
        }
        
    }
}
