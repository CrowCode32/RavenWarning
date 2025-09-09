using UnityEngine;

[CreateAssetMenu]
public class trinket : ScriptableObject
{
    public GameObject model;
    [Range(1, 20)] public int trinketNum;
    public string trinketName;
    public string trinketDesc;
}
