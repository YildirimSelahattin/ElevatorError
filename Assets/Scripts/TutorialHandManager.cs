using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialHandManager : MonoBehaviour
{
    Vector3 originalPos;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnEnable()
    {
        originalPos = transform.localPosition;

    }

    public void Level1Move()
    {
        transform.DOKill();
        transform.DOLocalMoveX(0,0.1f);
        transform.DOLocalMoveZ(-2, 1.5f).OnComplete(() =>
        {

            transform.DOLocalMoveZ(-5, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
            {
                Level1Move();
            });

        });
    }
    public void Level2Move()
    {
        transform.DOKill();
        transform.DOLocalMoveX(-1, 1).OnComplete(() =>
        {
            transform.DOLocalMoveX(1, 0.5f).OnComplete(() =>
            {
                Level2Move();
            });
        });
    }
    private void OnDisable()
    {
        transform.DOKill();
    }
}
