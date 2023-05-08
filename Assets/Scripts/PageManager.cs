using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PageManager : MonoBehaviour
{
    public Image mainImage;
    public TextMeshProUGUI priceText;
    public GameObject priceTag;
    public GameObject buyButton;
    public GameObject buyedImage;
    public GameObject comingSoonImage;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }
    public void OnBuyButtonClicked()
    {
        StoreUIManager.Instance.BuyAnimalOnThePage();
    }
}
