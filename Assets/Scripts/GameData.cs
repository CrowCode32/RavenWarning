using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;

/// <summary>
/// A container class to hold all data that needs to be saved and loaded.
/// This is a plain C# class, not a MonoBehaviour.
/// The [System.Serializable] attribute is required so Unity's JsonUtility can process it.
/// </summary>
[System.Serializable]
public class GameData
{
    public static GameData instance;
    
    // --- META PROGRESSION ---
    public int currency; // The resource players collect for permanent upgrades.
    public List<string> unlockedFeatherIDs; // A list of the unique feathers the player has unlocked.

    // --- RUN-SPECIFIC PROGRESS ---
    public bool lesserLordCaveInformed;
    public bool lesserLordForestInformed;
    public bool finishedTutorial;
    public float timeStat;
    public int killsStat;
    public int trinketStat;
    public int featherStat;
    public int deathStat;
    public int winStat;
    public int npcStat;

    private void Awake()
    {
        if(instance != null && instance!= this)
        {
            instance = this;
        }
    }
    
    /// <summary>
    /// The constructor sets the default values for a brand new game.
    /// This is what gets called for a first-time player.
    /// </summary>
    public GameData()
    {
        this.currency = 0;
        this.unlockedFeatherIDs = new List<string>();
        this.lesserLordCaveInformed = false;
        this.lesserLordForestInformed = false;
        this.finishedTutorial = false;
        this.timeStat = 0;
        this.killsStat = 0;
        this.trinketStat = 0; 
        this.featherStat = 0;   
        this.deathStat = 0;
        this.winStat = 0;
        this.npcStat = 0;
    }
}