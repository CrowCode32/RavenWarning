using UnityEngine;
using UnityEngine.UI;



[CreateAssetMenu]
public class statsmenu : ScriptableObject
{
    [SerializeField] Text timeStat;
    [SerializeField] Text enemiesKilledStat;
    [SerializeField] Text trinketsUnlockedStat;
    [SerializeField] Text feathersUnlockedStat;
    [SerializeField] Text deathsStat;
    [SerializeField] Text runsCompletedStat;
    [SerializeField] Text npcStat;
}
