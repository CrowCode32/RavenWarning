using UnityEngine;
using UnityEngine.UI;



[CreateAssetMenu]
public class trinket : ScriptableObject
{
    public GameObject model;
    [Range(0, 17)] public int trinketNum;
    public string trinketName;
    public string trinketDesc;
}
