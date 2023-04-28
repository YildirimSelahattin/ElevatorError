using UnityEngine;
using UnityEngine.UI;

public class StartScreenSize : MonoBehaviour
{
    public Canvas _Canvas;
    public Image startScreenImage;
    public Sprite ipadImage;

    void Start()
    {
        int _width = Display.main.systemWidth;

        Debug.Log("Width: " + _width);

        if (_width > 1450)
        {
            _Canvas.GetComponent<CanvasScaler>().matchWidthOrHeight = 1;
            _Canvas.GetComponent<CanvasScaler>().referenceResolution = new Vector2(1080,2200);
            startScreenImage.sprite = ipadImage;
        }
    }
}