using DG.Tweening;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using Cinemachine;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    public GameObject elevatorButtonPrefab;
    public List<GameObject> elevatorButtonsList;
    public int currentFloor;
    public int totalFloor;
    public GameObject elevatorButtonsParent;
    public bool shouldItMove = false;
    public Transform nextFloorBasePos;
    public Transform prevFloorTargetPos;
    public Transform currentFloorPos;
    public Transform startNextFloorFallingPos;
    public GameObject currentFloorObject;
    public GameObject nextFloorObject;
    public List<int> stopFloorsList;
    public float timeCounter;
    public float waitingTime;
    public bool shouldCount;
    public static GameManager Instance;
    public GameObject doorObject;
    public int currentScore;
    public TextMeshPro currentFloorText;
    public Material arrowShiningMat;

    public CinemachineVirtualCamera camera;
    private bool isShaken;
    private float Shaketime;
    public static bool loadLevelDirectly;

    public GameObject leftTargetObject;
    public GameObject rightTargetObject;
    public GameObject slider;
    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        if (loadLevelDirectly)
        {
            StartCoroutine(StartAfterDelay());
        }
        ScrollingBG.breake = 0;
        //CreateElevatorEnvironment();
    }

    // Update is called once per frame
    void Update()
    {
        if (shouldCount == true)
        {
            timeCounter += Time.deltaTime;
            slider.transform.localScale = new Vector3(1-timeCounter/waitingTime,1,1);
            if (timeCounter > waitingTime)
            {
                slider.SetActive(false);
                slider.transform.localScale = new Vector3(1, 1, 1);
                ReadToGoUp();
            }
        }
    }
    public void ReadToGoUp()
    {
        if(shouldCount == false) {
            return;
        }
        timeCounter = 0;
        shouldCount = false;
        doorObject.GetComponent<DoorManager>().leftDoor.transform.DOLocalMoveX(0, 1f);
        doorObject.GetComponent<DoorManager>().rightDoor.transform.DOLocalMoveX(0, 1f).OnComplete(() =>
        {
            if (ControlGameFinish() != true)
            {
                MoveOneFloorUp();
            }
        });
    }
    public IEnumerator StartAfterDelay()
    {
        yield return new WaitForSeconds(1);
        ResetFloorTexts();
        MoveOneFloorUp();
        for (int i = 0; i < GameDataManager.Instance.data.elevatorArray[GameDataManager.Instance.currentLevel - 1].peopleList.Count; i++)
        {
            stopFloorsList.Add(GameDataManager.Instance.data.elevatorArray[GameDataManager.Instance.currentLevel - 1].peopleList[i].whichFloor);
        }
        for (int i = 0; i < GameDataManager.Instance.data.elevatorArray[GameDataManager.Instance.currentLevel - 1].boardingPeopleList.Count; i++)
        {
            stopFloorsList.Add(GameDataManager.Instance.data.elevatorArray[GameDataManager.Instance.currentLevel - 1].boardingPeopleList[i].whichFloorToBoard);
            stopFloorsList.Add(GameDataManager.Instance.data.elevatorArray[GameDataManager.Instance.currentLevel - 1].boardingPeopleList[i].whichFloor);
        }

    }
    /*
     *     public void ShakeCamera(float intensity, float time)
        {
            CinemachineBasicMultiChannelPerlin shake = camera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            shake.m_AmplitudeGain = intensity;
            Shaketime = time;
        }*/
    public void StartGame(int levelIndex)
    {
        GridSpawner.Instance.StartGame(levelIndex);
       
        ResetFloorTexts();
        MoveOneFloorUp();
        for (int i = 0; i < GameDataManager.Instance.data.elevatorArray[GameDataManager.Instance.currentLevel - 1].peopleList.Count; i++)
        {
            stopFloorsList.Add(GameDataManager.Instance.data.elevatorArray[GameDataManager.Instance.currentLevel - 1].peopleList[i].whichFloor);
        }
        for (int i = 0; i < GameDataManager.Instance.data.elevatorArray[GameDataManager.Instance.currentLevel - 1].boardingPeopleList.Count; i++)
        {
            stopFloorsList.Add(GameDataManager.Instance.data.elevatorArray[GameDataManager.Instance.currentLevel - 1].boardingPeopleList[i].whichFloorToBoard);
            stopFloorsList.Add(GameDataManager.Instance.data.elevatorArray[GameDataManager.Instance.currentLevel - 1].boardingPeopleList[i].whichFloor);
        }
    }

    public void MoveOneFloorUp()
    {
        slider.SetActive(false);
        ScrollingBG.breake = 1;
        camera.transform.DOKill();
        LightUpWhoShouldAboard();
        camera.transform.DOMove(new Vector3(camera.transform.position.x, camera.transform.position.y - 1f, camera.transform.position.z + 1f), 1f);
        for (int i = 0; i < nextFloorObject.GetComponent<FloorManager>().gridIsEmptyList.Count; i++)
        {
            nextFloorObject.GetComponent<FloorManager>().gridIsEmptyList[i] = true;
        }
        ShineLoop(arrowShiningMat);
        SpawnFloorPeople(nextFloorObject);
        currentFloorObject.transform.DOMoveY(startNextFloorFallingPos.position.y, 1f).SetEase(Ease.Linear).OnComplete(() =>
        {
            //delete people on floor
            for (int i = nextFloorObject.GetComponent<FloorManager>().PeopleParent.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(nextFloorObject.transform.GetChild(i).gameObject);
            }

            nextFloorObject.SetActive(true);
            nextFloorObject.transform.DOMoveY(currentFloorPos.position.y, 1f).SetEase(Ease.Linear);
            currentFloorObject.transform.DOMoveY(prevFloorTargetPos.position.y, 1f).SetEase(Ease.Linear).OnComplete(() =>
            {
                GameObject temp = currentFloorObject;
                currentFloorObject = nextFloorObject;
                nextFloorObject = temp;
                ClearPeopleParent(nextFloorObject);
                nextFloorObject.transform.position = new Vector3(nextFloorObject.transform.position.x, nextFloorBasePos.position.y, nextFloorObject.transform.position.z);
                nextFloorObject.SetActive(false);
                currentFloor++;
                ResetFloorTexts();

                if (stopFloorsList.Contains(currentFloor))
                {
                    
                    slider.transform.localScale = new Vector3(1, 1, 1);
                    slider.SetActive(true);
                    if (GameDataManager.Instance.playSound == 1)
                    {
                        GameObject sound = new GameObject("sound");
                        sound.AddComponent<AudioSource>();
                        sound.GetComponent<AudioSource>().volume = 0.1f;
                        sound.GetComponent<AudioSource>().PlayOneShot(GameDataManager.Instance.dingSound);
                        Destroy(sound, GameDataManager.Instance.dingSound.length); // Creates new object, add to it audio source, play sound, destroy this object after playing is done
                    }
                    camera.transform.DOMove(
        new Vector3(camera.transform.position.x, camera.transform.position.y + 1f,
            camera.transform.position.z - 1f), 1f);
                    arrowShiningMat.DOKill();
                    arrowShiningMat.DOFade(0, 0.1F);
                    ScrollingBG.breake = 0;
                    //open the doors 
                    doorObject.GetComponent<DoorManager>().leftDoor.transform.DOLocalMoveX(15, 1f);
                    doorObject.GetComponent<DoorManager>().rightDoor.transform.DOLocalMoveX(-15, 1f).OnComplete(() =>
                    {
                        //tell people go in elevator

                        for (int x = currentFloorObject.GetComponent<FloorManager>().floorPeopleList.Count - 1; x >= 0; x--)
                        {
                            currentFloorObject.GetComponent<FloorManager>().floorPeopleList[x].GetComponent<PeopleManager>().GoInElevator();
                        }

                        if(GameDataManager.Instance.playedBefore == 0)
                        {
                            if(GameDataManager.Instance.currentLevel == 1)
                            {
                                UIManager.Instance.tutorialHand.transform.parent = GridSpawner.Instance.elevatorPeopleList[0].transform;
                                UIManager.Instance.tutorialHand.SetActive(true);
                                UIManager.Instance.tutorialHand.GetComponent<TutorialHandManager>().Level1Move();
                            }
                            else if (GameDataManager.Instance.currentLevel == 2)
                            {
                                UIManager.Instance.tutorialHand.transform.parent = GridSpawner.Instance.elevatorPeopleList[0].transform;
                                UIManager.Instance.tutorialHand.SetActive(true);
                                if (currentFloor == 1)
                                {
                                    UIManager.Instance.tutorialHand.GetComponent<TutorialHandManager>().Level2Move();
                                }
                                else if(currentFloor ==2)
                                {
                                    UIManager.Instance.tutorialHand.GetComponent<TutorialHandManager>().Level1Move();
                                }
                              
                                
                            }
                            
                        }
                        //start counting
                        shouldCount = true;
                    });
                }
                else
                {
                    MoveOneFloorUp();
                }
            });
        });
    }
    public void ResetFloorTexts()
    {
        //currentFloorObject.GetComponent<FloorManager>().floorText.text = currentFloor.ToString();
        //nextFloorObject.GetComponent<FloorManager>().floorText.text = (currentFloor + 1).ToString();
        currentFloorText.text = currentFloor.ToString();
    }
    public void ShineLoop(Material mat)
    {
        mat.DOFade(0.5F, 0.2F).OnComplete(() =>
        {
            mat.DOFade(0, 0.3f).OnComplete(() =>
            {
                ShineLoop(mat);
            });
        });
    }
    public void ClearPeopleParent(GameObject floorObject)
    {
        for (int i = 0; i < floorObject.GetComponent<FloorManager>().PeopleParent.transform.childCount; i++)
        {
            Destroy(floorObject.GetComponent<FloorManager>().PeopleParent.transform.GetChild(i).gameObject);
        }
    }
    public void CreateFloorGrids()
    {
        currentFloorObject.GetComponent<FloorManager>().CreateGrid();
        nextFloorObject.GetComponent<FloorManager>().CreateGrid();
    }
    public void SpawnFloorPeople(GameObject floor)
    {
        //clear first
        for (int i = nextFloorObject.GetComponent<FloorManager>().floorPeopleList.Count - 1; i >= 0; i--)
        {
            Destroy(nextFloorObject.GetComponent<FloorManager>().floorPeopleList[i].gameObject);
        }

        nextFloorObject.GetComponent<FloorManager>().floorPeopleList.Clear();
        List<BoardingPeople> boardingPeopleList = GameDataManager.Instance.data.elevatorArray[GameDataManager.Instance.currentLevel - 1].boardingPeopleList;
        for (int i = 0; i < boardingPeopleList.Count; i++)
        {
            if (boardingPeopleList[i].whichFloorToBoard == currentFloor + 1)
            {
                int characterIndex = 0;
                switch (boardingPeopleList[i].characterName)
                {
                    case "bear":
                        characterIndex = 0;
                        break;
                    case "foxAndMom":
                        characterIndex = 1;
                        break;
                    case "hoodie":
                        characterIndex = 2;
                        break;
                    case "mice":
                        characterIndex = 3;
                        break;
                    case "fox":
                        characterIndex = 4;
                        break;
                    case "bearAndMom":
                        characterIndex = 5;
                        break;
                    case "giraffe":
                        characterIndex = 6;
                        break;
                }

                if (GameDataManager.Instance.randomSpawn == 1)//not buyed yet
                {
                    if (characterIndex != 5)
                    {
                        characterIndex = GridSpawner.Instance.GetRandomCharacterIndex();
                    }
                }
                if (boardingPeopleList[i].positionIndexList.Count == 1)
                {
                    GameObject temp = Instantiate(GridSpawner.Instance.peopleArray[characterIndex], floor.GetComponent<FloorManager>().gridList[boardingPeopleList[i].positionIndexList[0]].transform);

                    //change child objects pos
                    temp.GetComponent<PeopleManager>().peopleObject.transform.localRotation = Quaternion.identity;

                    floor.GetComponent<FloorManager>().floorPeopleList.Add(temp);
                    People peopleData = new People();
                    peopleData.whichFloor = boardingPeopleList[i].whichFloor;
                    peopleData.characterName = boardingPeopleList[i].characterName;
                    peopleData.positionIndexList = boardingPeopleList[i].positionIndexList;
                    temp.GetComponent<PeopleManager>().peopleData = peopleData.CloneViaFakeSerialization();
                    temp.GetComponent<PeopleManager>().peopleIndex = 1;
                    temp.layer = LayerMask.NameToLayer("Default");
                    temp.GetComponent<PeopleManager>().floorText.text = peopleData.whichFloor.ToString();
                    for (int y = 0; y < peopleData.positionIndexList.Count; y++)
                    {
                        floor.GetComponent<FloorManager>().gridIsEmptyList[boardingPeopleList[i].positionIndexList[0]] = false;
                    }
                }
                if (boardingPeopleList[i].positionIndexList.Count == 2)
                {
                    GameObject temp = Instantiate(GridSpawner.Instance.peopleArray[characterIndex], floor.GetComponent<FloorManager>().gridList[boardingPeopleList[i].positionIndexList[0]].transform);
                    floor.GetComponent<FloorManager>().floorPeopleList.Add(temp);
                    temp.GetComponent<PeopleManager>().peopleObject.transform.localRotation = Quaternion.identity;
                    People peopleData = new People();
                    peopleData.whichFloor = boardingPeopleList[i].whichFloor;
                    peopleData.characterName = boardingPeopleList[i].characterName;
                    peopleData.positionIndexList = boardingPeopleList[i].positionIndexList;
                    temp.GetComponent<PeopleManager>().peopleData = peopleData;
                    temp.GetComponent<PeopleManager>().peopleIndex = 1;
                    temp.layer = LayerMask.NameToLayer("Default");
                    temp.GetComponent<PeopleManager>().floorText.text = peopleData.whichFloor.ToString();
                    Vector3 pos = temp.transform.localPosition;
                    temp.transform.localPosition = new Vector3(pos.x + GridSpawner.Instance.xSize / 2, pos.y, pos.z);
                    for (int y = 0; y < peopleData.positionIndexList.Count; y++)
                    {
                        floor.GetComponent<FloorManager>().gridIsEmptyList[boardingPeopleList[i].positionIndexList[0]] = false;
                    }

                }

            }
        }
    }
    public bool ControlGameFinish()
    {
        if (GameDataManager.Instance.data.elevatorArray[GameDataManager.Instance.currentLevel - 1].totalFloor == currentFloor)
        {
            if (GridSpawner.Instance.elevatorPeopleList.Count == 0) //if there is no people left
            {
                UIManager.Instance.winScreen.SetActive(true);
                if (GameDataManager.Instance.playedBefore == 0 && GameDataManager.Instance.currentLevel == 2)
                {
                    GameDataManager.Instance.playedBefore = 1;
                    UIManager.Instance.tutorialHand.SetActive(false);
                }
            }
            else
            {
                UIManager.Instance.failScreen.SetActive(true);
            }
            return true;
        }
        return false;
    }
    public bool IsPeopleLeftToGetDown()
    {
        foreach (GameObject people in GridSpawner.Instance.elevatorPeopleList)
        {
            if (people.GetComponent<PeopleManager>().peopleData.whichFloor == currentFloor)
            {
                return false;
            }
        }
        return true;
    }

    public void LightUpWhoShouldAboard()
    {
        int nextFloorIndex = currentFloor + 1;
        foreach(GameObject people in GridSpawner.Instance.elevatorPeopleList)
        {
            if(people.GetComponent<PeopleManager>().peopleData.whichFloor == nextFloorIndex)
            {
                people.GetComponent<PeopleManager>().LightUpTextBg();
            }
            else
            {
                people.GetComponent<PeopleManager>().floorBgInsideImage.DOColor(Color.white,0.2f);
            }
        }
    }
}
