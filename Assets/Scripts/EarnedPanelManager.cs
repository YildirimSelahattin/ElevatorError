using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EarnedPanelManager : MonoBehaviour
{
    // Start is called before the first frame update
    public Image mainImage;
    public TextMeshProUGUI nameText;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnContinueButtonClicked()
    {
        StoreUIManager.Instance.pageList[StoreUIManager.Instance.currentPageIndex].GetComponent<PageManager>().buyedImage.gameObject.SetActive(true);
        gameObject.SetActive(false);

    }
}
