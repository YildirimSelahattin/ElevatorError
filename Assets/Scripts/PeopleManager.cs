using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;
using JetBrains.Annotations;
using TMPro;

public class PeopleManager : MonoBehaviour
{
    public int peopleIndex;
    public bool onElevator = true;
    float moveTime = 0.5f;
    public People peopleData;
    public TextMeshPro floorText;
    public List<GameObject> headPartList;
    public List<GameObject> eyePartList;
    public GameObject peopleObject;
    public GameObject happyEmoji;
    public GameObject angerEmoji;
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0;i< eyePartList.Count; i++)
        {
            StartCoroutine(BlinkEyes(eyePartList[i]));
        }
        for(int i = 0;i<headPartList.Count;i++)
        {
            StartCoroutine(HeadTiltLeftRight(headPartList[i]));
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void MoveLeft()
    {
        int gridIndex = peopleData.positionIndexList[0];
        int columnIndex = GetColumnIndex(gridIndex);
        if (columnIndex > 0)
        {
            if (GridSpawner.Instance.gridIsEmptyList[gridIndex - 1] == 0)//if it is an empty spot
            {
                //empty old pos
                foreach (int gridNumber in peopleData.positionIndexList)
                {
                    GridSpawner.Instance.gridIsEmptyList[gridNumber] = 0;
                }
                gameObject.layer = LayerMask.NameToLayer("Default");
                Vector3 originalLocalPos = transform.localPosition;
                transform.parent = GridSpawner.Instance.gridList[gridIndex - 1].transform;
                MoveShake(moveTime, 0, 4);
                transform.DOLocalMoveX(originalLocalPos.x, moveTime).SetEase(Ease.Linear).OnComplete(() =>
                {
                    gameObject.layer = LayerMask.NameToLayer("People");
                });
                for (int i = 0; i < peopleData.positionIndexList.Count; i++)
                {
                    peopleData.positionIndexList[i]--;
                    GridSpawner.Instance.gridIsEmptyList[peopleData.positionIndexList[i]] = 1;
                }
            }
            else
            {
                angerEmoji.SetActive(true);
                gameObject.layer = LayerMask.NameToLayer("Default");
                transform.DOShakePosition(1f, 0.1f, 3, 0).OnComplete(() =>
                {
                    gameObject.layer = LayerMask.NameToLayer("People");
                });
            }
        }
        else
        {
            angerEmoji.SetActive(true);
            gameObject.layer = LayerMask.NameToLayer("Default");
            transform.DOShakePosition(1f, 0.1f, 3, 0).OnComplete(() =>
            {
                gameObject.layer = LayerMask.NameToLayer("People");
            });
        }
    }
    public void MoveRight()
    {
        int gridIndex = peopleData.positionIndexList[peopleData.positionIndexList.Count - 1];
        Debug.Log("grid" + gridIndex);
        int columnIndex = GetColumnIndex(gridIndex);
        Debug.Log("yeap" + columnIndex);
        if (columnIndex < GridSpawner.Instance.gridWidth - 1)
        {
            Debug.Log("niye");
            if (GridSpawner.Instance.gridIsEmptyList[gridIndex + 1] == 0)//if it is an empty spot
            {
                //empty old pos
                foreach (int gridNumber in peopleData.positionIndexList)
                {
                    GridSpawner.Instance.gridIsEmptyList[gridNumber] = 0;
                }
              
                gameObject.layer = LayerMask.NameToLayer("Default");
                Vector3 originalLocalPos = transform.localPosition;
                transform.parent = GridSpawner.Instance.gridList[peopleData.positionIndexList[0] + 1].transform;
                MoveShake(moveTime, 0, 4);
                transform.DOLocalMoveX(originalLocalPos.x, moveTime).SetEase(Ease.Linear).OnComplete(() =>
                {
                    gameObject.layer = LayerMask.NameToLayer("People");
                });
                for (int i = 0; i < peopleData.positionIndexList.Count; i++)
                {
                    peopleData.positionIndexList[i]++;
                    GridSpawner.Instance.gridIsEmptyList[peopleData.positionIndexList[i]] = 1;
                }
            }
            else
            {
                angerEmoji.SetActive(true);
                gameObject.layer = LayerMask.NameToLayer("Default");
                transform.DOShakePosition(1f, 0.1f, 3, 0).OnComplete(() =>
                {
                    gameObject.layer = LayerMask.NameToLayer("People");
                });
            }
        }
        else
        {
            angerEmoji.SetActive(true);
            gameObject.layer = LayerMask.NameToLayer("Default");
            transform.DOShakePosition(1f, 0.1f, 3, 0).OnComplete(() =>
            {
                gameObject.layer = LayerMask.NameToLayer("People");
            });
        }
    }
    public void MoveForward()
    {
        int rowIndex = GetRowIndex(peopleData.positionIndexList[0]);
        int columnIndex = GetColumnIndex(peopleData.positionIndexList[0]);

        if (rowIndex < GridSpawner.Instance.gridHeight - 1)
        {
            if (IsRowEmpty(columnIndex, rowIndex, "down"))//if it is an empty spot
            {
                int targetIndex = peopleData.positionIndexList[0] + GridSpawner.Instance.gridWidth;
                //change parent
                foreach (int gridNumber in peopleData.positionIndexList)
                {
                    GridSpawner.Instance.gridIsEmptyList[gridNumber] = 0;
                }
                
                gameObject.layer = LayerMask.NameToLayer("Default");
                Vector3 originalLocalPos = transform.localPosition;
                transform.parent = GridSpawner.Instance.gridList[targetIndex].transform;
                MoveShake(moveTime, 0, 4);
                transform.DOLocalMoveZ(originalLocalPos.z, moveTime).SetEase(Ease.Linear).OnComplete(() =>
                {
                    gameObject.layer = LayerMask.NameToLayer("People");
                });
                for (int i = 0; i < peopleData.positionIndexList.Count; i++)
                {
                    peopleData.positionIndexList[i] += GridSpawner.Instance.gridWidth;
                    GridSpawner.Instance.gridIsEmptyList[peopleData.positionIndexList[i]] = 1;
                }
            }
            else
            {
                angerEmoji.SetActive(true);
                gameObject.layer = LayerMask.NameToLayer("Default");
                transform.DOShakePosition(1f, 0.1f, 3, 0).OnComplete(() =>
                {
                    gameObject.layer = LayerMask.NameToLayer("People");
                });
            }

        }
        else
        {

            if (GameManager.Instance.currentFloor == peopleData.whichFloor)// if it is the right floor for people 
            {
                LeaveElevator();
            }
            else
            {
                angerEmoji.SetActive(true);
                gameObject.layer = LayerMask.NameToLayer("Default");
                transform.DOShakePosition(1f, 0.1f, 3, 0).OnComplete(() =>
                {
                    gameObject.layer = LayerMask.NameToLayer("People");
                });
            }
        }
    }
    public void MoveBackward()
    {
        Debug.Log("geriGitlA");
        int rowIndex = GetRowIndex(peopleData.positionIndexList[0]);
        int columnIndex = GetColumnIndex(peopleData.positionIndexList[0]);
        if (rowIndex != 0)
        {
            if (IsRowEmpty(columnIndex, rowIndex, "up"))//if it is an empty spot
            {
                int targetIndex = peopleData.positionIndexList[0] - GridSpawner.Instance.gridWidth;

                //change parent
                foreach (int gridNumber in peopleData.positionIndexList)
                {
                    GridSpawner.Instance.gridIsEmptyList[gridNumber] = 0;
                }
                
                gameObject.layer = LayerMask.NameToLayer("Default");
                Vector3 originalLocalPos = transform.localPosition;
                transform.parent = GridSpawner.Instance.gridList[targetIndex].transform;
                MoveShake(moveTime, 0, 4);
                transform.DOLocalMoveZ(originalLocalPos.z, moveTime).SetEase(Ease.Linear).OnComplete(() =>
                {
                    gameObject.layer = LayerMask.NameToLayer("People");
                });
                for (int i = 0; i < peopleData.positionIndexList.Count; i++)
                {
                    peopleData.positionIndexList[i] -= GridSpawner.Instance.gridWidth;
                    GridSpawner.Instance.gridIsEmptyList[peopleData.positionIndexList[i]] = 1;
                }
            }
            else
            {
                gameObject.layer = LayerMask.NameToLayer("Default");
                angerEmoji.SetActive(true);
                transform.DOShakePosition(1f, 0.1f, 3, 0).OnComplete(() =>
                {
                    gameObject.layer = LayerMask.NameToLayer("People");
                });
            }
        }
        else
        {
            angerEmoji.SetActive(true);
            gameObject.layer = LayerMask.NameToLayer("Default");
            transform.DOShakePosition(1f, 0.1f, 3, 0).OnComplete(() =>
            {
                gameObject.layer = LayerMask.NameToLayer("People");
            });
        }
    }

    public int GetColumnIndex(int gridIndex)
    {
        int rowIndex = GetRowIndex(gridIndex);

        return gridIndex - rowIndex * GridSpawner.Instance.gridWidth;
    }
    public int GetRowIndex(int gridIndex)
    {
        return gridIndex / GridSpawner.Instance.gridWidth;
    }

    public bool IsRowEmpty(int firstColumnIndex, int currentRowIndex, string keyString)
    {
        if (keyString == "up")
        {
            currentRowIndex -= 1;
        }
        if (keyString == "down")
        {
            currentRowIndex += 1;
        }
        for (int i = 0; i < peopleData.positionIndexList.Count; i++)
        {
            if (GridSpawner.Instance.gridIsEmptyList[currentRowIndex * GridSpawner.Instance.gridWidth + firstColumnIndex] == 1)
            {
                return false;
            }
            firstColumnIndex++;
        }
        return true;
    }

    public void MoveShake(float moveTime, int shakeCount, int shakeAmount)
    {
        Debug.Log("saas");
        float originalLocalY = transform.localPosition.y;
        float originalLocalX = transform.localPosition.x;
        transform.DOLocalRotate(new Vector3(0, 0, 0.5f), moveTime / (shakeAmount * 2)).OnComplete(() =>
        {
            shakeCount++;
            transform.DOLocalRotate(new Vector3(0, 0, -0.5f), moveTime / (shakeAmount * 2)).OnComplete(() =>
            {
                if (shakeCount > shakeAmount)
                {
                    transform.DOLocalRotate(Vector3.zero, 1f).OnComplete(() =>
                    {
                        return;
                    });
                }
                else
                {
                    shakeCount++;

                    Debug.Log("shakeCount" + shakeCount);
                    MoveShake(moveTime, shakeCount, 4);
                }
            });

        });
    }
    public void LeaveElevator()
    {
        if (isFloorEmpty())//if floor is empty
        {
            happyEmoji.SetActive(true);
            transform.parent = GameManager.Instance.currentFloorObject.GetComponent<FloorManager>().PeopleParent.transform;
            gameObject.layer = LayerMask.NameToLayer("Default");
            GameManager.Instance.currentScore++;
            UIManager.Instance.infoText.text = GameManager.Instance.currentScore.ToString();
            foreach (int gridNumber in peopleData.positionIndexList)
            {
                GridSpawner.Instance.gridIsEmptyList[gridNumber] = 0;
            }
            transform.DOLocalMoveZ(transform.localPosition.z - 5, 3f);
            MoveShake(3, 0, 12);
            GridSpawner.Instance.elevatorPeopleList.Remove(gameObject);
        }
    }
    public void GoInElevator()
    {

        int rowIndex = GridSpawner.Instance.gridHeight;
        int columnIndex =peopleData.positionIndexList[0];
        bool canGoIn = false;

        if (LookForColumn(rowIndex, columnIndex) == true){
            canGoIn = true;
        }
        else if (LookForRightLeft(rowIndex,columnIndex))//now look for lateral and 
        {
            LookForColumn(rowIndex, columnIndex);
            canGoIn = true;
        }
        if(canGoIn = true)
        {
            //now go in floor people 
            happyEmoji.SetActive(true);
            transform.DOKill();
            GameManager.Instance.currentFloorObject.GetComponent<FloorManager>().floorPeopleList.Remove(gameObject);
            int wantedIndex = GridSpawner.Instance.gridWidth * (GridSpawner.Instance.gridHeight - 1) + peopleData.positionIndexList[0];
            Debug.Log("youCanGo");
            Vector3 originalLocalPos = transform.localPosition;
            transform.parent = GridSpawner.Instance.gridList[wantedIndex].transform;
            MoveShake(moveTime, 0, 4);
            transform.DOLocalMoveX(0, moveTime).SetEase(Ease.Linear);
            GridSpawner.Instance.elevatorPeopleList.Add(gameObject);
            transform.DOLocalMoveZ(originalLocalPos.z, moveTime).SetEase(Ease.Linear).OnComplete(() =>
            {
                gameObject.layer = LayerMask.NameToLayer("People");
                peopleObject.transform.DOLocalRotate(new Vector3(0, 180, 0), 0.5f);
            });
            for (int i = 0; i < peopleData.positionIndexList.Count; i++)
            {
                peopleData.positionIndexList[i] = wantedIndex + i;
                GridSpawner.Instance.gridIsEmptyList[peopleData.positionIndexList[i]] = 1;
            }
        }
    }

    public IEnumerator BlinkEyes(GameObject eye)
    {
        
        yield return new WaitForSeconds(Random.Range(1,3));
        eye.GetComponent<Animator>().SetBool("blink", true);
        yield return new WaitForSeconds(0.5f);
        eye.GetComponent<Animator>().SetBool("blink", false);
        StartCoroutine(BlinkEyes(eye)); 
    }

    

    public IEnumerator HeadTiltLeftRight(GameObject head)
    {
        yield return new WaitForSeconds(Random.Range(5,10));
        head.GetComponent<Animator>().enabled = true;
        yield return new WaitForSeconds(3);
        head.GetComponent<Animator>().enabled = false;
        StartCoroutine(HeadTiltLeftRight(head));
    }
    public bool isFloorEmpty()
    {
        for (int i = 0; i < peopleData.positionIndexList.Count; i++)
        {
            int columnIndex = GetColumnIndex(peopleData.positionIndexList[i]);
            if (GameManager.Instance.currentFloorObject.GetComponent<FloorManager>().gridIsEmptyList[columnIndex] == false)
            {
                return false;
            }
        }
        return true;
    }

    public bool LookForColumn(int rowIndex,int columnIndex)
    {
        //FÝRST LOOK FOR ROW OPENÝNGS
        for (int tempRowIndex = rowIndex; tempRowIndex > 0; tempRowIndex--)
        {
           
            if (IsRowEmpty(columnIndex, tempRowIndex, "up"))
            {
                for (int i = tempRowIndex; i < GridSpawner.Instance.gridHeight; i++)
                {
                    Debug.Log((i) * GridSpawner.Instance.gridWidth + columnIndex+"heay" + tempRowIndex);
                    GameObject people = GridSpawner.Instance.gridList[(i) * GridSpawner.Instance.gridWidth + columnIndex].transform.GetChild(0).gameObject;
                    Debug.Log(people.name);
                    people.GetComponent<PeopleManager>().MoveBackward();
                }
                return true;
            }
        }
        return false;
    }
    public bool LookForRightLeft(int rowIndex,int columnIndex)
    {
        //FÝRST LOOK FOR LEFT
        
        for ( int tempRowIndex = rowIndex; rowIndex > 0; rowIndex--)
        {
            columnIndex = peopleData.positionIndexList[0];
            if (columnIndex > 0 ) // may go left
            {
                int currentIndex = tempRowIndex * GridSpawner.Instance.gridWidth+ columnIndex;

                if (GridSpawner.Instance.gridIsEmptyList[tempRowIndex*GridSpawner.Instance.gridWidth + columnIndex+1] == 0)
                {
                    GridSpawner.Instance.gridList[currentIndex].GetComponent<PeopleManager>().MoveLeft();
                    return true;
                }
            }
            columnIndex = peopleData.positionIndexList[peopleData.positionIndexList.Count-1];
            if (columnIndex < GridSpawner.Instance.gridWidth)
            {
                int currentIndex = tempRowIndex * GridSpawner.Instance.gridWidth + columnIndex;

                if (GridSpawner.Instance.gridIsEmptyList[tempRowIndex * GridSpawner.Instance.gridWidth + columnIndex+1] == 0)
                {
                    GridSpawner.Instance.gridList[currentIndex].GetComponent<PeopleManager>().MoveLeft();
                    return true;
                }
            }
        }
        return false;
    }
}
