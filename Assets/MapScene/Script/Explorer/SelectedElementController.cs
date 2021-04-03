using Assets.MapScene.Script.Explorer;
using System;
using UnityEngine;
using UnityEngine.UI;

public class SelectedElementController : MonoBehaviour
{
    // ZOOM LEVEL = 16
    //private const float scaleFactor = 2.387f;
    // ZOOM LEVEL = 17
    private const float scaleFactor = 1.193f;

    
    public GameObject elementInfo;
    private SelectedElement SelectedElement;
    private Text[] elementInfoValues;

    public GameObject zone_comment;
    public InputField TitleInput;
    public InputField CommentInput;
    public Button cutButton;
    public Image ElementImage;

    public static SelectedElementController instance;

    void Start()
    {
        instance = this;
        elementInfoValues = GameObject.Find("ElementInfo-Values").GetComponentsInChildren<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateElementInfo();
    }

    void UpdateElementInfo()
    {
        if (SelectedElement != null)
        {
            //Nom
            String name = SelectedElement.GetDisplayName();
            if (name == "")
                name = "Model";

            elementInfoValues[0].text = name;

            Collider b = SelectedElement.GetCollider();
            float defScale = 2f;
            /*if (SelectedElement.GetComponent<BoxCollider>() == null)
            {
                b = SelectedElement.GetComponentInChildren<BoxCollider>();
                defScale = MapController.DEFAULT_SCALE;
            }
            else
            {
                b = SelectedElement.GetComponent<BoxCollider>();
                defScale = MapController.DEFAULT_SCALE;
            }*/
            
            //Largeur (X)
            elementInfoValues[3].text = Math.Round(b.bounds.size.x / scaleFactor, 2).ToString() + 'm';
            //Hauteur (Y)
            elementInfoValues[1].text = Math.Round(b.bounds.size.y / (scaleFactor * defScale), 2).ToString() + 'm';
            //Longeur (Z)
            elementInfoValues[2].text = Math.Round(b.bounds.size.z / scaleFactor, 2).ToString() + 'm';
        }
    }

    private void ChangeSelectedElementColor(Color color)
    {
        foreach (Material mat in SelectedElement.GetMaterials())
            mat.color = color;
        SelectedElement.SetInitialColor(color);
    }

    public void Texture(Material newMat)
    {
        if (newMat != null)
        {
            MeshRenderer mr = SelectedElement.GetMeshRenderer();
            /*if (mr == null)
                mr = SelectedElement.GetComponentInChildren<MeshRenderer>();*/
            Material[] newMats = new Material[mr.materials.Length];
            for (int i = 0; i < newMats.Length; ++i)
                newMats[i] = newMat;
            mr.materials = newMats;
        }
        else
            Debug.Log("Mat is null");
    }


    public void OnClickEditObject()
    {
        Scenes.PreviousPos = this.SelectedElement.GetCurrentPosition();
        Scenes.PreviousScale = this.SelectedElement.GetScale();
        Scenes.AddGOParam("editedObject", this.SelectedElement.GetContainer());
        UnSelectElement();
        Scenes.LoadAdditive(Scenes.EditObject);
    }

    public void SetSelectedElement(GameObject go)
    {
        //Change color of the element to red
        this.SelectedElement = new SelectedElement(go);
        foreach (Material mat in SelectedElement.GetMaterials())
            mat.color = Color.red;

        //Set image
        Texture2D image = RuntimePreviewGenerator.GenerateModelPreview(go.transform, 128, 128, false);
        ElementImage.sprite = Sprite.Create(image, new Rect(0, 0, image.width, image.height), new Vector2(0.5f, 0.5f));

        //Active UI
        InteractionsCanvasController.instance.RightPanel.SetActive(true);
        MapCanvasController.instance.SetVisible(true);
        //Check if is alterable
        cutButton.interactable = go.GetComponent<SelectableElementController>().IsAlterable() ? true : false;

        //Update UI
        HistoryPanelManager.instance.Refresh();
    }

    public SelectedElement GetSelectedElement()
    {
        return this.SelectedElement;
    }

    public void UnSelectElement()
    {
        if (SelectedElement != null)
        {
            //Change color
            foreach (Material mat in SelectedElement.GetMaterials())
                mat.color = SelectedElement.GetInitialColor();
            //Unselect element
            SelectedElement = null;

            //Hide UI
            MapCanvasController.instance.SetVisible(false);
            InteractionsCanvasController.instance.RightPanel.SetActive(false);

            //Update UI
            HistoryPanelManager.instance.Refresh();
        }
    }

    public void DeleteSelectedObject()
    {
        GameObject ObjectToDestroy = this.SelectedElement.GetContainer();
        this.UnSelectElement();
        ObjectToDestroy.SetActive(false);
        HistoryPanelManager.instance.Refresh();
    }
    
    public void BtnValidate()
    {
            this.SelectedElement.SetTextMesh(TitleInput.text);
    }

    public void Duplicate()
    {
        // DUPLICATE
        GameObject duplicate = Instantiate(SelectedElement.GetContainer());

        // SET PARENT AND POSITION
        duplicate.transform.SetParent(GameObject.Find("AdditionalElements").transform, false);
        BoxCollider mcDuplicate = duplicate.GetComponentInChildren<BoxCollider>();
        duplicate.transform.position += new Vector3(mcDuplicate.bounds.size.x + 2f, 0, 0);

        // SET NEW NAME
        string oldName = SelectedElement.GetDisplayName();
        duplicate.name = "Container-" + oldName + "?" + DateTime.Now.ToString("hhmmssfff") + (Time.deltaTime * 1000);

        // SET NEW MATERIAL
        MeshRenderer mrDuplicate = duplicate.GetComponentInChildren<MeshRenderer>();
        mrDuplicate.material = Resources.Load<Material>("Materials/BenchMat");

        // SET COLOR
        mrDuplicate.material.color = this.SelectedElement.GetColor();

        this.UnSelectElement();
    }
}