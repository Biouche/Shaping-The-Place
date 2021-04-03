using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ColorPicking : MonoBehaviour, IPointerDownHandler, IDragHandler
{

    public Image colorSpaceImage;

    private Sprite colorSpaceSprite;

    private Texture2D colorSpace;

    public string Title = "Color Picker";
    public Vector2 startPos = new Vector2(20, 20);

    public GameObject receiver;
    public string colorSetFunctionName = "OnSetNewColor";
    public string colorGetFunctionName = "OnGetColor";
    public bool useExternalDrawer = false;
    public int drawOrder = 0;

    private Color TempColor;
    private Color SelectedColor;

    enum ESTATE
    {
        Hidden,
        Showed,
        Showing,
        Hidding
    };

    public void NotifyColor(Color color)
    {
        SetColor(color);
        SelectedColor = color;
    }

    void Start()
    {
        colorSpaceSprite = colorSpaceImage.sprite;
    }

    void Update()
    {
    }

    public void OnDrag(PointerEventData data)
    {
        Vector2 localCursor;

        var rect1 = GetComponent<RectTransform>();
        var pos1 = Display.RelativeMouseAt(data.position);

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rect1, pos1,
            GameObject.Find("CanvasCamera").GetComponent<Camera>(), out localCursor))
            return;

        int xpos = (int)(localCursor.x);
        int ypos = (int)(localCursor.y);

        if (xpos < 0) xpos = xpos + (int)rect1.rect.width / 2;
        else xpos += (int)rect1.rect.width / 2;

        ypos = (int)rect1.rect.height - (ypos + (int)rect1.rect.height / 2);

        float coeffX = colorSpaceSprite.rect.width / colorSpaceImage.rectTransform.rect.width;
        float coeffY = colorSpaceSprite.rect.height / colorSpaceImage.rectTransform.rect.height;

        /* print("colorSpaceSprite : " + colorSpaceSprite.rect);
         print("colorSpaceImage : " + colorSpaceImage.rectTransform.rect);
         print("coeffX : " + coeffX);
         print("coeffY : " + coeffY);*/
        colorSpace = colorSpaceSprite.texture;

        Vector2 mousePos = new Vector2(xpos, ypos);

        Color res = colorSpace.GetPixel((int)(coeffX * mousePos.x), colorSpace.height - (int)(coeffY * mousePos.y) - 1);
        SetColor(res);
        if (Input.GetMouseButton(0))
            ApplyColor();
    }

    public void OnPointerDown(PointerEventData dat)
    {
        Vector2 localCursor;

        var rect1 = GetComponent<RectTransform>();
        var pos1 = Display.RelativeMouseAt(dat.position);

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rect1, new Vector2(pos1.x, pos1.y),
            GameObject.Find("CanvasCamera").GetComponent<Camera>(), out localCursor))
            return;

        int xpos = (int)(localCursor.x);
        int ypos = (int)(localCursor.y);

        if (xpos < 0) xpos = xpos + (int)rect1.rect.width / 2;
        else xpos += (int)rect1.rect.width / 2;

        ypos = (int)rect1.rect.height - (ypos + (int)rect1.rect.height / 2);

        float coeffX = colorSpaceSprite.rect.width / colorSpaceImage.rectTransform.rect.width;
        float coeffY = colorSpaceSprite.rect.height / colorSpaceImage.rectTransform.rect.height;

        colorSpace = colorSpaceSprite.texture;

        Vector2 mousePos = new Vector2(xpos, ypos);

        Color res = colorSpace.GetPixel((int)(coeffX * mousePos.x), colorSpace.height - (int)(coeffY * mousePos.y) - 1);
        SetColor(res);
        if (Input.GetMouseButton(0))
            ApplyColor();
    }



    public void SetColor(Color color)
    {
        TempColor = color;
    }

    public Color GetColor()
    {
        return TempColor;
    }

    public void SetTitle(string title, Color textColor)
    {
        this.Title = title;
    }

    public void ApplyColor()
    {
        SelectedColor = TempColor;
        if (receiver)
            receiver.SendMessage(colorSetFunctionName, SelectedColor, SendMessageOptions.DontRequireReceiver);
    }
}
