using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[CreateAssetMenu]
public class HighScore : ScriptableObject
{
    [SerializeField] private SharedInt score;
    private int maxScore;

    public int MaxScore
    {
        get
        {
            LoadScore();
            CheckScore();
            return maxScore;
        }
        set => maxScore = value;
    }
    
    private void CheckScore()
    {
        if (score.Value > maxScore)
        {
            maxScore = score.Value;
            SaveScore();
        }
    }

    private void SaveScore()
    {
        var bf = new BinaryFormatter();
        var file = File.Create(Application.persistentDataPath + "/HighScore.dat");
        bf.Serialize(file, maxScore);
        file.Close();
    }

    private void LoadScore()
    {
        if (File.Exists(Application.persistentDataPath + "/HighScore.dat"))
        {
            var bf = new BinaryFormatter();
            var file = File.Open(Application.persistentDataPath + "/HighScore.dat", FileMode.Open);
            maxScore = (int) bf.Deserialize(file);
            file.Close();
        }
        else
        {
            maxScore = 0;
        }
    }
}
