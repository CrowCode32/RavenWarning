using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class storm : MonoBehaviour
{
    [SerializeField] Transform wall;
    [SerializeField] Transform startPos;
    [SerializeField] Transform endPos;
    [SerializeField] int speed;

    Vector3 spawnPos;
    public float exitDiff;

    // Update is called once per frame
    void Update()
    {
        //If player has exited graveyard
        enabled = true;
        spawnPos = startPos.position;
        transform.Translate(Vector3.right * speed * Time.deltaTime);

        exitDiff = endPos.position.x - transform.position.x;

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

    void swapScene(int playerProg)
    {
        spawnPos.x = startPos.position.x - exitDiff;
        wall.position = spawnPos;
    }
}
