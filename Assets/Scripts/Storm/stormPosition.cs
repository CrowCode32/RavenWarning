using UnityEngine;
using System.Collections;

public class stormPosition : MonoBehaviour
{   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        if (GameObject.FindWithTag("Storm Start").transform != null && GameObject.FindWithTag("Storm End") != null)
        {
            GameManager.instance.stormSpawnPoint = GameObject.FindWithTag("Storm Start").transform;
            GameManager.instance.stormEndPoint = GameObject.FindWithTag("Storm End").transform;
            setStormSpawn();
        }
    }

    public void setStormSpawn()
    {
        if (GameManager.instance.inCave != true)
        {
            GameManager.instance.stormSpawnPoint.position = new Vector3(GameManager.instance.stormSpawnPoint.position.x,
                GameManager.instance.stormSpawnPoint.position.y, GameManager.instance.stormSpawnPoint.position.z);
        } else
        {
            GameManager.instance.stormSpawnPoint.position = new Vector3(GameManager.instance.stormSpawnPoint.position.x - GameManager.instance.stormOffset,
                GameManager.instance.stormSpawnPoint.position.y, GameManager.instance.stormSpawnPoint.position.z);
        }
    }
}
