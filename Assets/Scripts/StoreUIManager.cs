using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreUIManager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject pagePrefab;
    public GameObject pageParent;
    public List<GameObject> pageList;
    public int currentPageIndex;
    public GameObject leftArrow;
    public GameObject rightArrow;
    public int[] buyPricesArray;
    public Sprite[] animalImagesArray;
    public Sprite[] earnedAnimalImagesArray;
    public GameObject[] botSideCirclesArray;
    public int gridWidth;
    public Sprite emptyCircle;
    public Sprite filledCircle;
    public GameObject rewardEarnedPanel;
    public string[] nameArray;
    public static StoreUIManager Instance;



    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            CreateLevelPanels();
            ControlRightLeftButton();
        }
        //gridParent.transform.DOLocalMoveY(gridWidth * currentPageIndex, 0.4f);
     

    }

    public void ControlRightLeftButton()
    {
        if (currentPageIndex == 0)
        {
            leftArrow.SetActive(false);
            rightArrow.SetActive(true);
        }
        else if (currentPageIndex == 7)
        {
            leftArrow.SetActive(true);
            rightArrow.SetActive(false);
        }
        else
        {
            leftArrow.SetActive(true);
            rightArrow.SetActive(true);
        }
    }
    public void CreateLevelPanels()
    {
        int gridCounter = 0;


        for(int pageCounter = 0; pageCounter < 7; pageCounter++)
        {
            GameObject page = Instantiate(pagePrefab, pageParent.transform);
            page.transform.localPosition = new Vector3(gridWidth * gridCounter,0 , 0);
            page.GetComponent<PageManager>().mainImage.sprite = animalImagesArray[pageCounter];
            page.GetComponent<PageManager>().mainImage.preserveAspect= true;

            if (GameDataManager.Instance.isBuyedList[pageCounter] == 1)//buyed
            {
                page.GetComponent<PageManager>().buyedImage.SetActive(true);
                page.GetComponent<PageManager>().buyButton.SetActive(false);
                page.GetComponent<PageManager>().priceTag.SetActive(false);
            }
            else//not buyed
            {
                page.GetComponent<PageManager>().buyedImage.SetActive(false);
                page.GetComponent<PageManager>().buyButton.SetActive(true);
                page.GetComponent<PageManager>().priceTag.SetActive(true);
                page.GetComponent<PageManager>().priceText.text = buyPricesArray[pageCounter].ToString();
            }
            pageList.Add(page);
            gridCounter++;
        }
    }

    public void SlideRight()
    {
        botSideCirclesArray[currentPageIndex].GetComponent<Image>().sprite = emptyCircle;
        currentPageIndex++;
        botSideCirclesArray[currentPageIndex].GetComponent<Image>().sprite = filledCircle;
        pageParent.transform.DOLocalMoveX(-gridWidth * currentPageIndex, 0.4f);
        ControlRightLeftButton();
        UIManager.Instance.PlayUISound();
    }
    public void Slideleft()
    {
        botSideCirclesArray[currentPageIndex].GetComponent<Image>().sprite = emptyCircle;
        currentPageIndex--;
        botSideCirclesArray[currentPageIndex].GetComponent<Image>().sprite = filledCircle;
        pageParent.transform.DOLocalMoveX(-gridWidth * currentPageIndex, 0.4f);
        ControlRightLeftButton();
        UIManager.Instance.PlayUISound();
    }

    public void BuyAnimalOnThePage()
    {
        GameDataManager.Instance.isBuyedList[currentPageIndex] = 1;
        rewardEarnedPanel.GetComponent<EarnedPanelManager>().nameText.text = nameArray[currentPageIndex];
        rewardEarnedPanel.GetComponent<EarnedPanelManager>().mainImage.sprite = earnedAnimalImagesArray[currentPageIndex];
        rewardEarnedPanel.SetActive(true);
        pageList[currentPageIndex].GetComponent<PageManager>().buyButton.gameObject.SetActive(false);
        pageList[currentPageIndex].GetComponent<PageManager>().buyedImage.gameObject.SetActive(true);
    }
}
