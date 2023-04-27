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
    void Awake()
    {

        if(Instance == null)
        {
            Instance = this;
        }
        LoadData();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void LoadData()
    {
        data = JsonUtility.FromJson<DataList>(JSONText.text);
        for(int i = 0; i < 7; i++)
        {
            if(i == 0 || i == 1)
            {
                isBuyedList.Add(PlayerPrefs.GetInt("IsBuyed", 1));
            }
            else
            {
                isBuyedList.Add(PlayerPrefs.GetInt("IsBuyed", 0));
            }
        }
    }
}
