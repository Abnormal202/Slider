using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using System;

/// <summary>
/// This class handles storing achievement data and sending it to steam. Use <see cref="SetAchievementStat(string, int)"/> to set/update a particular achievement stat.
/// You can also use <see cref="GetAchievementData"/> to get all stored achievement data. This is used for saving achievement data to save profiles.
/// </summary>
public class AchievementManager : Singleton<AchievementManager>
{
    private Dictionary<string, int> achievementStats;

    private void Start()
    {
        InitializeSingleton();
        achievementStats = new Dictionary<string, int>();
    }

    /// <summary>
    /// Set a statistic with the given key. This key needs to match one setup in Steamworks (message Daniel or Travis to have them create a statistic!)
    /// </summary>
    /// <param name="statName"></param>
    /// <param name="value"></param>
    public static void SetAchievementStat(string statName, int value)
    {
        _instance.achievementStats[statName] = value;
        _instance.SendAchievementStatsToSteam();
    }

    /// <summary>
    /// Increment a statistic with the given key. This key needs to match one setup in Steamworks (message Daniel or Travis to have them create a statistic!)
    /// </summary>
    /// <param name="statName"></param>
    /// <param name="value"></param>
    public static void IncrementAchievementStat(string statName, int increment = 1)
    {
        if (_instance.achievementStats.ContainsKey(statName))
        {
            _instance.achievementStats[statName] += increment;
        }
        else
        {
            _instance.achievementStats[statName] = increment;
        }
        Debug.Log(_instance.achievementStats[statName]);
        _instance.SendAchievementStatsToSteam();
    }

    /// <returns>An array of all AchievementStatistics, or an empty array if they cannot be found</returns>
    public static AchievementStatistic[] GetAchievementData()
    {
        try
        {
            AchievementStatistic[] pairs = new AchievementStatistic[_instance.achievementStats.Count];
            int i = 0;
            foreach (string key in _instance.achievementStats.Keys)
            {
                pairs[i] = new AchievementStatistic(key, _instance.achievementStats[key]);
                i++;
            }
            return pairs;
        } catch (NullReferenceException)
        {
            Debug.LogWarning("[AchievementManager] Failed to load achievement stats. This could indicate that you are not " +
                "connected to Steam or that AchievementManager is not present in your scene.");
            return new AchievementStatistic[0];
        }
        
    }

    /// <summary>
    /// This replaces all achievement stats with the passed in key-value pair array.
    /// This is dangerous and should only really be used when loading achievement stats from a save profile.
    /// </summary>
    /// <param name="achievementStatistics"></param>
    public static void OverwriteAchievementData(AchievementStatistic[] achievementStatistics)
    {
        if (achievementStatistics != null)
        {
            foreach (AchievementStatistic statistic in achievementStatistics)
            {
                _instance.achievementStats[statistic.Key] = statistic.Value;
            }
            _instance.SendAchievementStatsToSteam();
        }
    }

    private void SendAchievementStatsToSteam()
    {
        foreach (string key in achievementStats.Keys)
        {
            SteamUserStats.SetStat(key, achievementStats[key]);
        }
    }
}

/// <summary>
/// Serializable struct that represents a key-value pair for a Steam achievement statistic.
/// </summary>
[System.Serializable]
public struct AchievementStatistic
{
    public string Key { get; private set; }
    public int Value { get; private set; }

    public AchievementStatistic(string key, int value)
    {
        Key = key;
        Value = value;
    }
}
