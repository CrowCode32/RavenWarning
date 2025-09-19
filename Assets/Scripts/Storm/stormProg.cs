using UnityEngine;
using UnityEngine.SceneManagement;

public class stormProg : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Collide called");
        if (collision.CompareTag("Storm"))
        {
            Debug.Log("Storm collided");
            switch (SceneManager.GetActiveScene().name)
            {
                case "Graveyard":
                    GameManager.instance.stormGraveyard = true;
                    break;

                case "Cave":
                    GameManager.instance.stormCave = true;
                    break;

                case "Forest":
                    GameManager.instance.stormForest = true;
                    break;
                
                case "Kingdom":
                    GameManager.instance.stormKingdom = true;
                    break;
            }
            GameManager.instance.updateProgUI();
        }
    }
}
