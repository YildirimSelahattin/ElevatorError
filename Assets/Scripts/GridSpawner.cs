using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSpawner : MonoBehaviour
{
    // Start is called before the first frame update
    public List<GameObject> gridList = new List<GameObject>();
    public List<int> gridIsEmptyList = new List<int>();
    public GameObject gridPrefab;
    public GameObject[] peopleArray;
    public GameObject leftLimit;
    public GameObject rightLimit;
    public GameObject topLimit;
    public GameObject botLimit;
    public float xSize;
    public float ySize;
    public int xIndex;
    public int yIndex;
    public int gridWidth;
    public int gridHeight;
    public GameObject gridParent;
    public static GridSpawner Instance;
    public List<GameObject> elevatorPeopleList;
    public GameObject[] floors;


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


    public void StartGame(int levelArrayIndex)
    {
        float distanceBetweenX = Mathf.Abs(leftLimit.transform.position.x - rightLimit.transform.position.x);
        float distanceBetweenY = Mathf.Abs(topLimit.transform.position.z - botLimit.transform.position.z);
        xSize = (distanceBetweenX / GameDataManager.Instance.data.elevatorArray[levelArrayIndex].gridWidth);
        ySize = (distanceBetweenY / GameDataManager.Instance.data.elevatorArray[levelArrayIndex].gridHeight);
        gridHeight = GameDataManager.Instance.data.elevatorArray[levelArrayIndex].gridHeight;
        gridWidth = GameDataManager.Instance.data.elevatorArray[levelArrayIndex].gridWidth;
        CreateGrid(levelArrayIndex);
        GameManager.Instance.CreateFloorGrids();
    }

    public void CreateGrid(int levelIndex)
    {
        // for elevator grid
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                GameObject currGrid = Instantiate(gridPrefab, gridParent.transform);
                currGrid.transform.localPosition = new Vector3(x * xSize, 0, -y * ySize);
                currGrid.GetComponent<GridManager>().index = y * gridHeight + x;
                currGrid.GetComponent<GridManager>().isEmpty = true;
                gridIsEmptyList.Add(0);
                gridList.Add(currGrid);
            }
        }

        
        
        SpawnPeople(levelIndex);
    }


    public void SpawnPeople(int levelIndex)
    {
        for (int peopleCounter = 0; peopleCounter < GameDataManager.Instance.data.elevatorArray[levelIndex].peopleList.Count; peopleCounter++)
        {
            People peopleData = GameDataManager.Instance.data.elevatorArray[levelIndex].peopleList[peopleCounter];
            int characterIndex = 0;
            switch (peopleData.characterName)
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
            if (GameDataManager.Instance.isBuyedList[characterIndex] == 0)//not buyed yet
            {
                if(characterIndex != 5)
                {
                    characterIndex = 1;
                }
                else
                {
                    characterIndex =  GetRandomCharacterIndex();
                }
            }
            GameObject temp = Instantiate(peopleArray[characterIndex], gridList[peopleData.positionIndexList[0]].transform);
            temp.GetComponent<PeopleManager>().peopleData = peopleData;
            temp.GetComponent<PeopleManager>().onElevator = false;
            temp.GetComponent<PeopleManager>().floorText.text = peopleData.whichFloor.ToString();
            elevatorPeopleList.Add(temp);
            temp.name = temp.name + peopleCounter.ToString();

            for (int i = 0; i < peopleData.positionIndexList.Count; i++)
            {
                gridIsEmptyList[peopleData.positionIndexList[i]] = 1;
                gridList[i].GetComponent<GridManager>().isEmpty = false;
            }
            if (peopleData.positionIndexList.Count == 2)
            {
                Vector3 pos = temp.transform.localPosition;
                temp.transform.localPosition = new Vector3(pos.x + xSize / 2, pos.y, pos.z);
            }
            
            
        }
    }

    public int GetRandomCharacterIndex()
    {
        int startIndex = Random.Range(0,7);
        while (true)
        {
            if(startIndex==1 || startIndex == 5)// two charactered people
            {
                startIndex++;
                continue;
            }
            else
            {
                startIndex %= 7;
                if (GameDataManager.Instance.isBuyedList[startIndex] == 1)
                {
                    return startIndex;
                }
            }
        }
    }
}
