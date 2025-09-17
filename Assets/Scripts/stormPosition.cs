using UnityEngine;

public class stormPosition : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        if (GameObject.FindWithTag("Storm Start").transform != null && GameObject.FindWithTag("Storm End") != null)
        {
            GameManager.instance.stormSpawnPoint = GameObject.FindWithTag("Storm Start").transform;
            GameManager.instance.stormEndPoint = GameObject.FindWithTag("Storm End").transform;
        }
        else
        {
            Debug.Log("Storm start or end = null");
        }
    }
}
