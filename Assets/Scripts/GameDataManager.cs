using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    // Start is called before the first frame update
    public DataList data;
    public TextAsset JSONText;
    public static GameDataManager Instance;
    public int currentLevel;
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
    }
}
