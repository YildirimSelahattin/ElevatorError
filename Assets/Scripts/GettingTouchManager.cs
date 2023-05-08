using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GettingTouchManager : MonoBehaviour
{
    RaycastHit hit;
    Ray ray;
    public GameObject touchedPeople;
    [SerializeField] LayerMask touchablePeopleLayer;
    public Vector3 touchStartPos;
    public Vector3 curTouchPosition;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            curTouchPosition = Input.GetTouch(0).position;
            ray = Camera.main.ScreenPointToRay(curTouchPosition);

            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)                // This is actions when finger/cursor hit screen
            {
                if (Physics.Raycast(ray.origin, ray.direction, out hit, Mathf.Infinity, touchablePeopleLayer)) // if it hit to a machine object
                {
                    Debug.Log(hit.collider.gameObject);
                    touchedPeople = hit.collider.gameObject;
                    touchStartPos = curTouchPosition;// this is the start pos of swipe 
                }
            }
            else if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
            {

                if (touchedPeople != null)
                {
                    if (Mathf.Abs(curTouchPosition.y - touchStartPos.y) > 75)
                    {
                        if (touchStartPos.y > curTouchPosition.y)
                        {
                            Debug.Log("down");
                            touchedPeople.GetComponent<PeopleManager>().MoveForward();
                        }
                        else
                        {
                            Debug.Log("up");
                            touchedPeople.GetComponent<PeopleManager>().MoveBackward();
                        }
                        touchedPeople = null;
                    }
                    else if (Mathf.Abs(curTouchPosition.x - touchStartPos.x) > 75)
                    {
                        if (touchStartPos.x> curTouchPosition.x)
                        {
                            Debug.Log("left");
                            touchedPeople.GetComponent<PeopleManager>().MoveLeft();
                        }
                        else
                        {
                            Debug.Log("right");
                            touchedPeople.GetComponent<PeopleManager>().MoveRight();
                        }
                        touchedPeople = null;
                    }
                }


            }
            else if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                if (touchedPeople != null)
                {

                }
                touchedPeople = null;
                touchStartPos = Vector3.zero;
            }
        }
    }
}
