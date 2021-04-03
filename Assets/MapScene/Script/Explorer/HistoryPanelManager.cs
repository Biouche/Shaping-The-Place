using UnityEngine;
using UnityEngine.UI;


public class HistoryPanelManager : MonoBehaviour
{
    public GameObject grid;
    public static HistoryPanelManager instance;
    private CameraController camControl;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        camControl = Camera.main.GetComponent<CameraController>();
        Refresh();
    }

    public void Refresh()
    {
        //Empty grid
        foreach (Transform child in grid.transform)
        {
            if (child != grid.transform)
                Destroy(child.gameObject);
        }
        
        //Fill grid
        GameObject[] arrayObj = GameObject.FindGameObjectsWithTag("AdditionalElements");
        foreach (GameObject obj in arrayObj)
        {
            //Create GO
            GameObject textContainer = new GameObject();
            GameObject textGo = new GameObject("Text");
            //Create background 
            Image img = textContainer.AddComponent<Image>();
            img.sprite = Resources.Load<Sprite>("UISprite");
            Button btn = textContainer.AddComponent<Button>();
            btn.transition = Selectable.Transition.None;

            //Add colors
            if (SelectedElementController.instance.GetSelectedElement() != null && obj.transform.parent.gameObject == SelectedElementController.instance.GetSelectedElement().GetContainer())
                img.color = new Color32(86, 125, 140, 255);
            else
                img.color = Color.white;

            btn.onClick.AddListener(delegate { TaskOnClick(obj); });

            //Create text
            Text txt = textGo.AddComponent<Text>();
            txt.text = obj.name.Split('-')[0];
            txt.fontSize = 40;
            txt.fontStyle = FontStyle.Bold;
            txt.color = Color.black;
            txt.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
            txt.alignment = TextAnchor.MiddleCenter;
            txt.transform.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 850);
            //Set parents
            textGo.transform.SetParent(textContainer.transform, false);
            textContainer.transform.SetParent(grid.transform, false);
        }
    }


    void TaskOnClick(GameObject obj)
    {
        if (obj != null)
        {
            // No object selected, select a new one
            if (SelectedElementController.instance.GetSelectedElement() == null)
            {
                SelectedElementController.instance.SetSelectedElement(obj.transform.parent.gameObject);
                //Move camera
                camControl.moveCameraTo(obj);
            }
            // An object is already selected, select a new one
            else if (SelectedElementController.instance.GetSelectedElement().GetContainerName() != "Container-" + obj.name)
            {
                GameObject go = GameObject.Find(obj.name);
                SelectedElementController.instance.UnSelectElement();
                SelectedElementController.instance.SetSelectedElement(go.transform.parent.gameObject);
                //Move camera
                camControl.moveCameraTo(go);
            }
            // Unselect
            else if (SelectedElementController.instance.GetSelectedElement().GetContainer() == obj.transform.parent.gameObject)
                SelectedElementController.instance.UnSelectElement();
        }
    }



}
