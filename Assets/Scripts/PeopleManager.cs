using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;
using JetBrains.Annotations;
using TMPro;
using UnityEngine.UI;

public class PeopleManager : MonoBehaviour
{
    public int peopleIndex;
    public bool onElevator = true;
    float moveTimeHorizontal = 0.25f;
    float moveTimeVertical = 0.18f;
    public People peopleData;
    public TextMeshPro floorText;
    public List<GameObject> headPartList;
    public List<GameObject> eyePartList;
    public GameObject peopleObject;
    public GameObject happyEmoji;
    public GameObject angerEmoji;
    public bool needGoInHelp = false;
    public SpriteRenderer floorBgInsideImage;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < eyePartList.Count; i++)
        {
            StartCoroutine(BlinkEyes(eyePartList[i]));
        }
        for (int i = 0; i < headPartList.Count; i++)
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
                MoveShake(moveTimeHorizontal, 0, 1, "stopAtEnd");
                if(GameDataManager.Instance.currentLevel == 2)
                {
                    UIManager.Instance.tutorialHand.GetComponent<TutorialHandManager>().Level1Move();
                }
                transform.DOLocalMoveX(originalLocalPos.x, moveTimeHorizontal).SetEase(Ease.Linear).OnComplete(() =>
                {
                    gameObject.layer = LayerMask.NameToLayer("People");
                });
                for (int i = 0; i < peopleData.positionIndexList.Count; i++)
                {
                    peopleData.positionIndexList[i]--;
                    GridSpawner.Instance.gridIsEmptyList[peopleData.positionIndexList[i]] = 1;
                }

                for (int x = GameManager.Instance.currentFloorObject.GetComponent<FloorManager>().floorPeopleList.Count - 1; x >= 0; x--)
                {
                    if(GameManager.Instance.currentFloorObject.GetComponent<FloorManager>().floorPeopleList[x].GetComponent<PeopleManager>().needGoInHelp == true)
                    {
                        GameManager.Instance.currentFloorObject.GetComponent<FloorManager>().floorPeopleList[x].GetComponent<PeopleManager>().GoInElevator();
                    }
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
        int columnIndex = GetColumnIndex(gridIndex);
        if (columnIndex < GridSpawner.Instance.gridWidth - 1)
        {
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
                if (GameDataManager.Instance.currentLevel == 2)
                {
                    UIManager.Instance.tutorialHand.GetComponent<TutorialHandManager>().Level1Move();
                }
                MoveShake(moveTimeHorizontal, 0, 1, "stopAtEnd");
                transform.DOLocalMoveX(originalLocalPos.x, moveTimeHorizontal).SetEase(Ease.Linear).OnComplete(() =>
                {
                    gameObject.layer = LayerMask.NameToLayer("People");
                });
                for (int i = 0; i < peopleData.positionIndexList.Count; i++)
                {
                    peopleData.positionIndexList[i]++;
                    GridSpawner.Instance.gridIsEmptyList[peopleData.positionIndexList[i]] = 1;
                }
                for (int x = GameManager.Instance.currentFloorObject.GetComponent<FloorManager>().floorPeopleList.Count - 1; x >= 0; x--)
                {
                    if (GameManager.Instance.currentFloorObject.GetComponent<FloorManager>().floorPeopleList[x].GetComponent<PeopleManager>().needGoInHelp == true)
                    {
                        GameManager.Instance.currentFloorObject.GetComponent<FloorManager>().floorPeopleList[x].GetComponent<PeopleManager>().GoInElevator();
                    }
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
                if (GameDataManager.Instance.currentLevel == 2)
                {
                    UIManager.Instance.tutorialHand.SetActive(false);
                }
                int targetIndex = peopleData.positionIndexList[0] + GridSpawner.Instance.gridWidth;
                //change parent
                foreach (int gridNumber in peopleData.positionIndexList)
                {
                    GridSpawner.Instance.gridIsEmptyList[gridNumber] = 0;
                }
                gameObject.layer = LayerMask.NameToLayer("Default");
                Vector3 originalLocalPos = transform.localPosition;
                transform.parent = GridSpawner.Instance.gridList[targetIndex].transform;
                float tempMoveTime;
                if (IsLeavingPathOpen(GetRowIndex(peopleData.positionIndexList[0]), GetColumnIndex(peopleData.positionIndexList[0])) && GameManager.Instance.currentFloor == peopleData.whichFloor)
                {
                    tempMoveTime = moveTimeVertical ;
                    MoveShake(tempMoveTime, 0, 1, "nonStop");
                }
                else
                {
                    tempMoveTime = moveTimeVertical;
                    MoveShake(moveTimeVertical, 0, 1, "stopAtEnd");
                }
                for (int i = 0; i < peopleData.positionIndexList.Count; i++)
                {
                    peopleData.positionIndexList[i] += GridSpawner.Instance.gridWidth;
                }
                transform.DOLocalMoveZ(originalLocalPos.z, tempMoveTime).SetEase(Ease.Linear).OnComplete(() =>
                {
                    if (peopleData.whichFloor == GameManager.Instance.currentFloor)//if it is the right floor for this people 
                    {
                        if (IsLeavingPathOpen(GetRowIndex(peopleData.positionIndexList[0]), GetColumnIndex(peopleData.positionIndexList[0])))
                        {
                            MoveForward();
                        }
                        else
                        {
                            for (int i = 0; i < peopleData.positionIndexList.Count; i++)
                            {
                                GridSpawner.Instance.gridIsEmptyList[peopleData.positionIndexList[i]] = 1;

                            }
                            gameObject.layer = LayerMask.NameToLayer("People");
                        }
                    }
                    else
                    {
                        for (int i = 0; i < peopleData.positionIndexList.Count; i++)
                        {
                            GridSpawner.Instance.gridIsEmptyList[peopleData.positionIndexList[i]] = 1;
                        }
                        gameObject.layer = LayerMask.NameToLayer("People");
                    }
                });
               
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
        else if (peopleData.whichFloor == GameManager.Instance.currentFloor && isFloorEmpty())
        {
            LeaveElevator(0.6f);
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
    public void MoveBackward()
    {
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
                MoveShake(moveTimeVertical, 0, 1, "stopAtEnd");
                transform.DOLocalMoveZ(originalLocalPos.z, moveTimeVertical).SetEase(Ease.Linear).OnComplete(() =>
                {
                    gameObject.layer = LayerMask.NameToLayer("People");
                });
                for (int i = 0; i < peopleData.positionIndexList.Count; i++)
                {
                    peopleData.positionIndexList[i] -= GridSpawner.Instance.gridWidth;
                    GridSpawner.Instance.gridIsEmptyList[peopleData.positionIndexList[i]] = 1;
                }
                for (int x = GameManager.Instance.currentFloorObject.GetComponent<FloorManager>().floorPeopleList.Count - 1; x >= 0; x--)
                {
                    if (GameManager.Instance.currentFloorObject.GetComponent<FloorManager>().floorPeopleList[x].GetComponent<PeopleManager>().needGoInHelp == true)
                    {
                        GameManager.Instance.currentFloorObject.GetComponent<FloorManager>().floorPeopleList[x].GetComponent<PeopleManager>().GoInElevator();
                    }
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

    public bool IsRowEmptyForGoIn(int firstColumnIndex, int currentRowIndex, string keyString, int len)
    {
        if (keyString == "up")
        {
            currentRowIndex -= 1;
        }
        if (keyString == "down")
        {
            currentRowIndex += 1;
        }
        Debug.Log("rowEmpty");
        for (int i = 0; i < len; i++)
        {
            if (GridSpawner.Instance.gridIsEmptyList[currentRowIndex * GridSpawner.Instance.gridWidth + firstColumnIndex] == 1)
            {
                return false;
            }
            firstColumnIndex++;
        }
        return true;
    }

    public void MoveShake(float moveTime, int shakeCount, int shakeAmount, string type)
    {
        float originalLocalY = transform.localPosition.y;
        float originalLocalX = transform.localPosition.x;
        transform.DOLocalRotate(new Vector3(0, 0, 2f), moveTime / (shakeAmount * 3)).OnComplete(() =>
        {
            shakeCount++;
            transform.DOLocalRotate(new Vector3(0, 0, -2f), moveTime / (shakeAmount * 3)).OnComplete(() =>
            {
                if (shakeCount >= shakeAmount)
                {
                    transform.DOLocalRotate(Vector3.zero, moveTime / (shakeAmount * 3)).OnComplete(() =>
                    {
                        return;
                    });
                }
                if (type == "stopAtEnd")
                {
                    transform.DOLocalRotate(Vector3.zero, moveTime / (shakeAmount * 3)).OnComplete(() =>
                    {
                        if (shakeCount >= shakeAmount)
                        {
                            return;
                        }
                        else
                        {
                            MoveShake(moveTime, shakeCount, shakeAmount, "stopAtEnd");
                        }
                    });
                }
                else if (type == "nonStop")
                {
                    if (shakeCount >= shakeAmount)
                    {
                        return;
                    }
                    else
                    {
                        MoveShake(moveTime, shakeCount, shakeAmount, "nonStop");
                    }
                }


            });

        });
    }
    public void LeaveElevator(float time)
    {
        if (GameManager.Instance.shouldCount == false)
        {
            gameObject.layer = LayerMask.NameToLayer("People");
            return;
        }
        if (isFloorEmpty() && GameManager.Instance.shouldCount == true)//if floor is empty
        {
            happyEmoji.SetActive(true);
            floorText.gameObject.transform.parent.gameObject.SetActive(false);
            transform.parent = GameManager.Instance.currentFloorObject.GetComponent<FloorManager>().PeopleParent.transform;
            gameObject.layer = LayerMask.NameToLayer("Default");
            GameManager.Instance.currentScore++;
            UIManager.Instance.infoText.text = GameManager.Instance.currentScore.ToString();
            foreach (int gridNumber in peopleData.positionIndexList)
            {
                GridSpawner.Instance.gridIsEmptyList[gridNumber] = 0;
            }
            
            GridSpawner.Instance.elevatorPeopleList.Remove(gameObject);
            if (GameDataManager.Instance.currentLevel == 2  && GameManager.Instance.currentFloor == 1)
            {
                UIManager.Instance.tutorialHand.transform.parent = GridSpawner.Instance.elevatorPeopleList[0].transform;
                UIManager.Instance.tutorialHand.GetComponent<TutorialHandManager>().Level1Move();
            }
            
            if (GameDataManager.Instance.playSound == 1)
            {
                GameObject sound = new GameObject("sound");
                sound.AddComponent<AudioSource>();
                sound.GetComponent<AudioSource>().volume = 1;
                sound.GetComponent<AudioSource>().PlayOneShot(GameDataManager.Instance.hummingSounds[Random.Range(0, 2)]);
                Destroy(sound, GameDataManager.Instance.hummingSounds[Random.Range(0, 2)].length); // Creates new object, add to it audio source, play sound, destroy this object after playing is done
            }
            floorBgInsideImage.DOKill();
            transform.DOLocalMoveZ(transform.localPosition.z - 3, time).OnComplete(() =>
            {
               
                transform.DOLocalJump(transform.localPosition,0.6f,1,0.8f).OnComplete(() =>
                {
                    int randomValue = Random.Range(0, 2);
                    if (randomValue == 0)//left
                    {

                        transform.DOLocalRotate(new Vector3(transform.localPosition.x, 90, transform.localPosition.z), 0.3f).OnComplete(() =>
                        {


                            transform.DOMoveX(GameManager.Instance.leftTargetObject.transform.position.x, 1f).OnComplete(() =>
                            {
                                if (GameManager.Instance.IsPeopleLeftToGetDown() == true)
                                {
                                    GameManager.Instance.ReadToGoUp();
                                }
                            });
                        });

                    }
                    if (randomValue == 1)//right
                    {
                        transform.DOLocalRotate(new Vector3(transform.localPosition.x, -90, transform.localPosition.z), 0.3f).OnComplete(() =>
                        {
                            transform.DOMoveX(GameManager.Instance.rightTargetObject.transform.position.x, 1f).OnComplete(() =>
                            {
                                if (GameManager.Instance.IsPeopleLeftToGetDown() == true)
                                {
                                    GameManager.Instance.ReadToGoUp();
                                }
                            });
                        });
                    }
                });
                MoveShake(time, 0, 2, "nonStop");
            });
        }
    }
    public void GoInElevator()
    {

        int rowIndex = GridSpawner.Instance.gridHeight;
        int columnIndex = peopleData.positionIndexList[0];
        bool canGoIn = false;
        
        if (LookForColumn(rowIndex, columnIndex) == true)
        {
            canGoIn = true;
        }
      
        if (canGoIn == true)
        {
            //now go in floor people 
            happyEmoji.SetActive(true);
            transform.DOKill();
            GameManager.Instance.currentFloorObject.GetComponent<FloorManager>().floorPeopleList.Remove(gameObject);
            int wantedIndex = GridSpawner.Instance.gridWidth * (GridSpawner.Instance.gridHeight - 1) + peopleData.positionIndexList[0];
            GameManager.Instance.currentFloorObject.GetComponent<FloorManager>().gridIsEmptyList[peopleData.positionIndexList[0]] = true;

            Vector3 originalLocalPos = transform.localPosition;
            transform.parent = GridSpawner.Instance.gridList[wantedIndex].transform;
            MoveShake(moveTimeVertical, 0, 1, "stopAtEnd");
            transform.DOLocalMoveX(0, moveTimeVertical).SetEase(Ease.Linear);
            GridSpawner.Instance.elevatorPeopleList.Add(gameObject);
            GameManager.Instance.stopFloorsList.Add(peopleData.whichFloor);
            transform.DOLocalMoveZ(originalLocalPos.z, moveTimeVertical).SetEase(Ease.Linear).OnComplete(() =>
            {
                gameObject.layer = LayerMask.NameToLayer("People");
                peopleObject.transform.DOLocalRotate(new Vector3(0, 180, 0), 0.5f);
            });
            for (int i = 0; i < peopleData.positionIndexList.Count; i++)
            {
                peopleData.positionIndexList[i] = wantedIndex + i;
                GridSpawner.Instance.gridIsEmptyList[peopleData.positionIndexList[i]] = 1;
            }
                if (GameManager.Instance.IsPeopleLeftToGetDown() == true && GameManager.Instance.currentFloorObject.GetComponent<FloorManager>().floorPeopleList.Count == 0)
                {
                    GameManager.Instance.ReadToGoUp();
                }
        }
        else
        {
            needGoInHelp = true;
        }
    }

    public IEnumerator BlinkEyes(GameObject eye)
    {

        yield return new WaitForSeconds(Random.Range(1, 3));
        eye.GetComponent<Animator>().SetBool("blink", true);
        yield return new WaitForSeconds(0.5f);
        eye.GetComponent<Animator>().SetBool("blink", false);
        StartCoroutine(BlinkEyes(eye));
    }



    public IEnumerator HeadTiltLeftRight(GameObject head)
    {
        yield return new WaitForSeconds(Random.Range(5, 10));
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

    public bool LookForColumn(int rowIndex, int columnIndex)
    {
        List<GameObject> moveObjectList = new List<GameObject>();
        //FÝRST LOOK FOR ROW OPENÝNGS
        for (int tempRowIndex = rowIndex - 1; tempRowIndex > 0; tempRowIndex--)
        {

            int len ;
            if (GridSpawner.Instance.gridIsEmptyList[tempRowIndex * GridSpawner.Instance.gridWidth + columnIndex] == 1)
            {
                if (GridSpawner.Instance.gridList[tempRowIndex * GridSpawner.Instance.gridWidth + columnIndex].transform.childCount == 0)
                {
                    len = 2;
                    columnIndex--;
                }
                else
                {
                    len = 1;
                }
                GameObject people = GridSpawner.Instance.gridList[(tempRowIndex) * GridSpawner.Instance.gridWidth + columnIndex].transform.GetChild(0).gameObject;
                moveObjectList.Add(people);
                if (IsRowEmptyForGoIn(columnIndex, tempRowIndex, "up", len))
                {
                    for (int i = moveObjectList.Count - 1; i >= 0; i--)
                    {
                        moveObjectList[i].GetComponent<PeopleManager>().MoveBackward();
                    }
                    return true;
                }
            }
            else
            {
                return true;
            }
            
        }
        return false;
    }
    public bool LookForRightLeft(int rowIndex, int columnIndex)
    {
        //FÝRST LOOK FOR LEFT
        int  prevObjectLenght = peopleData.positionIndexList.Count;
        for (int tempRowIndex = rowIndex-1; rowIndex > 0; rowIndex--)
        {
            if (GridSpawner.Instance.gridList[tempRowIndex * GridSpawner.Instance.gridWidth + columnIndex].transform.childCount == 0)
            {
                columnIndex--;
            }
            if (columnIndex > 0) // may go left
            {
                int currentIndex = tempRowIndex * GridSpawner.Instance.gridWidth + columnIndex;

                if (GridSpawner.Instance.gridIsEmptyList[tempRowIndex * GridSpawner.Instance.gridWidth + columnIndex - 1] == 0)
                {
                    for(int i = 0; i< prevObjectLenght; i++)
                    {
                        GridSpawner.Instance.gridList[currentIndex].transform.GetChild(0).gameObject.GetComponent<PeopleManager>().MoveLeft();
                    }
                    return true;
                }
            }
            columnIndex = columnIndex+1;
            if (columnIndex < GridSpawner.Instance.gridWidth-1)
            {
                int currentIndex = tempRowIndex * GridSpawner.Instance.gridWidth + columnIndex;
                if (GridSpawner.Instance.gridIsEmptyList[tempRowIndex * GridSpawner.Instance.gridWidth + columnIndex + 1] == 0)
                {
                    for (int i = 0; i < prevObjectLenght; i++)
                    {
                        GridSpawner.Instance.gridList[currentIndex].transform.GetChild(0).gameObject.GetComponent<PeopleManager>().MoveRight();
                    }
                    return true;
                }
            }
        }
        return false;
    }

    public bool IsLeavingPathOpen(int rowIndex, int columnIndex)
    {
        for (int rowCounter = rowIndex + 1; rowCounter < GridSpawner.Instance.gridHeight; rowCounter++)
        {
            for (int columnCounter = columnIndex; columnCounter < peopleData.positionIndexList.Count; columnCounter++)
            {
                if (GridSpawner.Instance.gridIsEmptyList[(rowCounter * GridSpawner.Instance.gridWidth) + columnCounter] == 1)
                {
                    return false;
                }
            }
        }
        return true;
    }
    public void WalkLeftInFloor()
    {

    }

    public void WalkRightInFloor()
    {

    }

    public void LightUpTextBg()
    {
        floorBgInsideImage.DOColor(Color.green, 0.5f).OnComplete(() =>
        {
            floorBgInsideImage.DOColor(Color.white, 0.5f).OnComplete(() =>
            {
                LightUpTextBg();
            });
        });
        
    }
}
