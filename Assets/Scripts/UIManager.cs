using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    public GameObject gameMusic;
    public GameObject tutorialHand;
    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        if (GameManager.loadLevelDirectly)
        {
            startScreen.SetActive(false);
            inGameScreen.SetActive(true);
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
        gameMusic.SetActive(true);
        GameManager.Instance.StartGame(GameDataManager.Instance.currentLevel - 1);
        PlayUISound();
    }
    public void PlayUISound()
    {
        if (GameDataManager.Instance.playSound == 1)
        {
            GameObject sound = new GameObject("sound");
            sound.AddComponent<AudioSource>();
            sound.GetComponent<AudioSource>().volume = 1f;
            sound.GetComponent<AudioSource>().PlayOneShot(GameDataManager.Instance.UISound);
            Destroy(sound, GameDataManager.Instance.UISound.length); // Creates new object, add to it audio source, play sound, destroy this object after playing is done
        }
    }
    public void OnStoreButtonClicked()
    {
        storePanel.SetActive(true);
        startScreen.SetActive(false);
        PlayUISound();
    }
    public void OnHomeButtonClicked()
    {
        GameManager.loadLevelDirectly = false;
        SceneManager.LoadScene(0);
    }
    public void OnRestartButtonClicked()
    {
        failScreen.SetActive(false);
        GameManager.loadLevelDirectly = true;
        PlayUISound();
        SceneManager.LoadScene(0);
    }
    public void OnNextLevelButtonClicked()
    {
        PlayUISound();
        GameDataManager.Instance.currentLevel++;
        if(GameDataManager.Instance.currentLevel== GameDataManager.Instance.data.elevatorArray.Length+1)
        {
            GameDataManager.Instance.currentLevel %= GameDataManager.Instance.data.elevatorArray.Length;
            GameDataManager.Instance.randomSpawn = 1;
        }
        GameManager.loadLevelDirectly = true;
        GameDataManager.Instance.SaveData();
        SceneManager.LoadScene(0);
    }
}
