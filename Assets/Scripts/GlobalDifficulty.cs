using UnityEngine;

public enum DifficultyLevel { Easy, Normal, Hardcore }

public static class GlobalDifficulty
{
    public static DifficultyLevel Difficulty
    {
        get => (DifficultyLevel)PlayerPrefs.GetInt("GameDifficulty", 1);
        set 
        {
            PlayerPrefs.SetInt("GameDifficulty", (int)value);
            PlayerPrefs.Save();
        }
    }
}