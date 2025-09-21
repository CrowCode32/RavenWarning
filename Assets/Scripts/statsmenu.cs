using UnityEngine;
using TMPro;

public class statsMenu : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timeStat;
    [SerializeField] TextMeshProUGUI killsStat;
    [SerializeField] TextMeshProUGUI trinketStat;
    [SerializeField] TextMeshProUGUI featherStat;
    [SerializeField] TextMeshProUGUI deathStat;
    [SerializeField] TextMeshProUGUI winStat;
    [SerializeField] TextMeshProUGUI npcStat;

    private GameData gameData;

    private void Start()
    {
        gameData = GameManager.instance.gameData;
    }

    private void Update()
    {
        timeStat.text = FormatTime(gameData.timeStat);
        killsStat.text = gameData.killsStat.ToString();
        trinketStat.text = gameData.trinketStat.ToString() + "/18";
        featherStat.text = gameData.featherStat.ToString() + "/3";
        deathStat.text = gameData.deathStat.ToString();
        winStat.text = gameData.winStat.ToString();
        npcStat.text = gameData.npcStat.ToString();


    }


    string FormatTime(float seconds)
    {
        int mins = Mathf.FloorToInt(seconds / 60f);
        int secs = Mathf.FloorToInt(seconds % 60f);
        return $"{mins:00}:{secs:00}";
    }
}

