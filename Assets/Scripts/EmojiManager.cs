using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class EmojiManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
        GetComponent<SpriteRenderer>().DOFade(0, 2f).OnComplete(() =>
        {
            Color temp =GetComponent<SpriteRenderer>().color;
            temp.a = 1;
            GetComponent<SpriteRenderer>().color = temp;    
            gameObject.transform.parent.gameObject.SetActive(false);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
