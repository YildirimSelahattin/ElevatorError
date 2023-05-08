using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    // Start is called before the first frame update
    public DataList data;
    public TextAsset JSONText;
    public static GameDataManager Instance;
    public int currentLevel;
    public List<int> isBuyedList;
    public List<int> avaliableCharacterIndexes;
    public int playSound = 1;
    public AudioClip dingSound;
    public AudioClip[] hummingSounds;
    public AudioClip UISound;
    public int randomSpawn;
    public int playedBefore = 0;
    void Awake()
    {

        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
            LoadData();
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void LoadData()
    {
        playedBefore = PlayerPrefs.GetInt("PlayedBefore",0);
        data = JsonUtility.FromJson<DataList>(JSONText.text);
        currentLevel = PlayerPrefs.GetInt("CurrentLevel",1);
        randomSpawn = PlayerPrefs.GetInt("RandomSpawn",0);
        for(int i = 0; i < 8; i++)
        {
            if(i == 0 || i == 1)
            {
                isBuyedList.Add(PlayerPrefs.GetInt("IsBuyed", 1));
            }
            else
            {
                int value = PlayerPrefs.GetInt("IsBuyed", 1);
                isBuyedList.Add(value);
                if(value == 1)
                {
                    avaliableCharacterIndexes.Add(i);
                }
            }
        }
    }
    public void SaveData()
    {
       PlayerPrefs.SetInt("CurrentLevel", currentLevel);
       PlayerPrefs.SetInt("RandomSpawn", randomSpawn);
       PlayerPrefs.GetInt("PlayedBefore", playedBefore);
    }
}
