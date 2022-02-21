using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class SaveData
{
    public bool[] isActive;
    public int[] highScores;
    public int[] stars;
    public int count;
}

public class GameData : MonoBehaviour
{
    public static GameData gameData;
    public SaveData saveData;
    public GameObject startPanel;
    public GameObject levelPanel;
    public int count = 0;
    // Start is called before the first frame update
    void Awake()
    {
        if(gameData == null)
        {
            DontDestroyOnLoad(this.gameObject);
            gameData = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        Load();
    }

    void Start()
    {
        
    }

    public void Save()
    {
        // create a binary formatter which can read binary files
        BinaryFormatter formatter = new BinaryFormatter ();
        // create a route from program to the file
        FileStream file = File.Open(Application.persistentDataPath + "/player.dat", FileMode.Create);
        // copy a blank save data
        SaveData data = new SaveData();
        data = saveData;

        // actually save the file 
        formatter.Serialize(file, data);
        file.Close();

        Debug.Log(Application.persistentDataPath);
    }

    public void Load()
    {
        // Check if the saved data exits
        if(File.Exists(Application.persistentDataPath + "/player.dat"))
        {
            // create a new binary formatter
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream stream = File.Open(Application.persistentDataPath + "/player.dat", FileMode.Open);
            saveData = binaryFormatter.Deserialize(stream) as SaveData;

            stream.Close();

            Debug.Log(Application.persistentDataPath);

        }
    }


    private void OnApplicationQuit()
    {
        Save();
    }

    private void OnDisable()
    {
        Save();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
