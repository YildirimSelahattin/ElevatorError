using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject startScreen;
    public GameObject inGameScreen;
    public GameObject winScreen;
    public GameObject failScreen;
    public GameObject getItScreen;
    public TextMeshProUGUI infoText;
    public static UIManager Instance;
    public GameObject storePanel;
    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnStartButtonClicked()
    {
        startScreen.SetActive(false);
        inGameScreen.SetActive(true);
        int levelIndex = GameDataManager.Instance.currentLevel-1;
        GameManager.Instance.StartGame(levelIndex);
    }
    public void PlayUISound()
    {

    }
    public void OnStoreButtonClicked()
    {
        storePanel.SetActive(true);
        startScreen.SetActive(false);
    }
    public void OnHomeButtonClicked()
    {
        startScreen.SetActive(true);
        storePanel.SetActive(false);
        inGameScreen.SetActive(false);
    }
    public void OnRestartButtonClicked()
    {
        failScreen.SetActive(false);

    }
}
