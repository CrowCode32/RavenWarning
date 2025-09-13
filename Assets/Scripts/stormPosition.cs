using UnityEngine;

public class stormPosition : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        GameManager.instance.stormSpawnPoint = GameObject.FindWithTag("Storm Start").transform;
        GameManager.instance.stormEndPoint = GameObject.FindWithTag("Storm End").transform;
    }
}
