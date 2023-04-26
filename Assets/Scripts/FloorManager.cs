using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FloorManager : MonoBehaviour
{
    public TextMeshPro floorText;
    public GameObject PeopleParent;
    public GameObject gridParent;
    public List<bool> gridIsEmptyList;
    public List<GameObject> gridList;
    public List<GameObject> floorPeopleList;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void CreateGrid()
    {
        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < GridSpawner.Instance.gridWidth; x++)
            {
                GameObject currGrid = Instantiate(GridSpawner.Instance.gridPrefab, gridParent.transform);
                currGrid.transform.localPosition = new Vector3(x * GridSpawner.Instance.xSize, 0, -y * GridSpawner.Instance.ySize);
                currGrid.GetComponent<GridManager>().index = y * 3 + x;
                currGrid.GetComponent<GridManager>().isEmpty = true;
                gridIsEmptyList.Add(true);
                gridList.Add(currGrid);
            }
        }
    }
}
