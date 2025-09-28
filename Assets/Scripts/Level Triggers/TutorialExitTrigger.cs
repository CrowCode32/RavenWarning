using UnityEngine;

public class TutorialExitTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object entering the trigger is the player.
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered the tutorial exit trigger.");
            GameManager.instance.gameData.finishedTutorial = true;
            // Tell the GameManager to start the run.
            GameManager.instance.BeginRun();
        }
    }
}