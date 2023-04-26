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
        levelArrayIndex--;
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
                case "fox":
                    characterIndex = 0;
                    break;
                case "mice":
                    characterIndex = 1;
                    break;
                case "hoodie":
                    characterIndex = 2;
                    break;
                case "bear":
                    characterIndex = 3;
                    break;
                case "giraffe":
                    characterIndex = 4;
                    break;
                case "foxAndMom":
                    characterIndex = 5;
                    break;
                case "bearAndMom":
                    characterIndex = 6;
                    break;
            }

            if (peopleData.positionIndexList.Count == 1)
            {
                GameObject temp = Instantiate(peopleArray[characterIndex], gridList[peopleData.positionIndexList[0]].transform);
                temp.GetComponent<PeopleManager>().peopleData = peopleData;
                temp.GetComponent<PeopleManager>().peopleIndex= 1;
                temp.GetComponent<PeopleManager>().onElevator= false;
                temp.name = temp.name + peopleCounter.ToString();
                temp.GetComponent<PeopleManager>().floorText.text = peopleData.whichFloor.ToString();
                elevatorPeopleList.Add(temp);
                for (int i = 0; i < peopleData.positionIndexList.Count; i++)
                {
                    gridIsEmptyList[peopleData.positionIndexList[i]] = 1;
                    gridList[i].GetComponent<GridManager>().isEmpty = false;
                }
            }
            if (peopleData.positionIndexList.Count == 2)
            {
                GameObject temp = Instantiate(peopleArray[characterIndex], gridList[peopleData.positionIndexList[0]].transform);
                temp.GetComponent<PeopleManager>().peopleData = peopleData;
                temp.GetComponent<PeopleManager>().peopleIndex = 1;
                temp.GetComponent<PeopleManager>().floorText.text = peopleData.whichFloor.ToString();
                Vector3 pos = temp.transform.localPosition;
                temp.transform.localPosition = new Vector3(pos.x + xSize / 2, pos.y, pos.z);
                elevatorPeopleList.Add(temp);
                for (int i = 0; i < peopleData.positionIndexList.Count; i++)
                {
                    gridIsEmptyList[peopleData.positionIndexList[i]] =1;
                    gridList[i].GetComponent<GridManager>().isEmpty = false;
                }
            }
            
            
        }
    }
}
