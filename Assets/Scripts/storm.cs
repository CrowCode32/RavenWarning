using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class storm : MonoBehaviour
{
    [SerializeField] int speed;

    Vector3 startPos; // = normal spawn position
    Vector3 endPos;   // last possible end point
    Vector3 spawnPos; // = normal spawn position + offset
    float exitDiff = 0;


    private void Start()
    {
        //Game manager instantiates
        startPos = GameManager.instance.stormSpawnPoint.position;
        endPos = GameManager.instance.stormEndPoint.position;
        spawnStorm();
    }
    
    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);

        //Determining how far the storm is from the exit for offset in future levels
        exitDiff = endPos.x - transform.position.x;

        if(exitDiff <= 0)
        {
            enabled = false;
        }
        
        // If player changed level
        // swapScene();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //Player dies,  lose activated
        } else if (collision.CompareTag("NPC"))
        {
            //Destroy NPC
        } else if (collision.CompareTag("Enemy"))
        {
            //Kill enemy
        }
    }

    void spawnStorm()
    {
        //if(player is on second level or higher){
        // spawnPos.x = startPos.transform.position.x - exitDiff;
        //} else {
        spawnPos = startPos;
        //}

        transform.position = spawnPos;
    }

    void swapScene(int playerProg)
    {
        spawnPos.x = startPos.x = exitDiff;
        GameManager.instance.stormSpawnPoint.position = spawnPos;
    }
}
