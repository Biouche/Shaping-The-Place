using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityExtension;

public class BDDPanelManager : MonoBehaviour
{
    private string unalterableModelsPath;
    private string alterableModelsPath;
    public GameObject alterableGrid;
    public GameObject unalterableGrid;
    public Button bddButton;

    public GameObject ElementPositioningPanel;

    public static BDDPanelManager instance;

    private void Awake()
    {
        instance = this;

        unalterableModelsPath = Application.streamingAssetsPath + "/ModelDatabase/Unalterable/";
        alterableModelsPath = Application.streamingAssetsPath + "/ModelDatabase/Alterable/";
    }

    void Start()
    {
        //Load alterable objects
        DirectoryInfo info = new DirectoryInfo(alterableModelsPath);
        FileInfo[] fileInfos = info.GetFiles();

        foreach (FileInfo fileInfo in fileInfos)
            if (fileInfo.Extension == ".obj")
                CreateGridElement(alterableGrid, fileInfo.Name.Split('.')[0], fileInfo.FullName);

        //Load unalterable objects
        info = new DirectoryInfo(unalterableModelsPath);
        fileInfos = info.GetFiles();

        foreach (FileInfo fileInfo in fileInfos)
            if (fileInfo.Extension == ".obj")
                CreateGridElement(unalterableGrid, fileInfo.Name.Split('.')[0], fileInfo.FullName);
    }

    private void CreateGridElement(GameObject elementGrid, string elementName, string path)
    {
        //Create button gameobject
        GameObject go = new GameObject(elementName);
        go.transform.SetParent(elementGrid.transform, false);

        //Add interaction component
        BDDElementHandler bddElementHandler = go.AddComponent<BDDElementHandler>();
        bddElementHandler.SetPath(path);
        bddElementHandler.SetElementPositioningPanel(ElementPositioningPanel);

        //Add image
        Image img = go.AddComponent<Image>() as Image;
        img.type = Image.Type.Sliced;
        //Create temporary instance to get thumb image
        GameObject thumbTransform = new GameObject(elementName);
        //Material
        MeshRenderer mr = thumbTransform.AddComponent<MeshRenderer>();
        mr.material = Resources.Load<Material>("Materials/BenchMat");
        //Mesh
        MeshFilter mf = thumbTransform.AddComponent<MeshFilter>();
        FileStream lStream = new FileStream(path, FileMode.Open);
        OBJData lOBJData = OBJLoader.LoadOBJ(lStream);
        lStream.Close();
        mf.mesh.LoadOBJ(lOBJData);
        mf.mesh.RecalculateNormals();

        //Generate thumb
        Texture2D image = RuntimePreviewGenerator.GenerateModelPreview(thumbTransform.transform, 128, 128, false);
        img.sprite = Sprite.Create(image, new Rect(0, 0, image.width, image.height), new Vector2(0.5f, 0.5f));
        //Destroy temporary instance
        Destroy(thumbTransform);

        //Text under button
        GameObject txGO = new GameObject();
        Text tx = txGO.AddComponent<Text>() as Text;
        tx.text = elementName;
        tx.color = Color.black;
        tx.alignment = TextAnchor.UpperCenter;
        tx.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        tx.fontStyle = FontStyle.Bold;
        tx.fontSize = 14;
        //tx.resizeTextForBestFit = true;
        //tx.resizeTextMaxSize = 25;
        txGO.transform.position = new Vector3(0, -63, 0);
        txGO.transform.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 25);
        txGO.transform.SetParent(go.transform, false);
    }

    //public void OnClickShowBDD()
    //{
    //    this.gameObject.SetActive(true);
    //    this.bddButton.gameObject.SetActive(false);
    //}

    //public void OnClickCloseBDD()
    //{
    //    this.gameObject.SetActive(false);
    //    this.bddButton.gameObject.SetActive(true);
    //}
}

