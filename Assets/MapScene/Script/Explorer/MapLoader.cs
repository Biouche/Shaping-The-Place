using Mapbox.Unity.MeshGeneration.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class MapLoader
{
    public static void SaveMap()
    {
        string dataPath = Application.streamingAssetsPath;

        //Serialize map
        GameObject map = GameObject.Find("Map");
        MapSerializable mapSerializable = new MapSerializable(map);
        foreach (Transform tileTransform in map.transform)
        {
            if (tileTransform.gameObject != map)
            {
                //Tile
                GameObject tileGO = tileTransform.gameObject;
                MapTile tile = new MapTile(tileGO);
                foreach (Transform tileElementTransform in tileGO.transform)
                {
                    //Tile Element
                    if (tileElementTransform.gameObject != tileGO)
                    {
                        String text = tileElementTransform.GetComponentInChildren<TextMesh>() != null ? tileElementTransform.GetComponentInChildren<TextMesh>().text : null;
                        TileElement tileElement = new TileElement(tileElementTransform.gameObject, "MapBuilding", text);
                        tileElement.SetAlterable(false);
                        tile.AddElement(tileElement);
                    }
                }
                mapSerializable.AddTile(tile);
            }
        }
        int i = 1;
        SerializedScene serializedScene = new SerializedScene();
        serializedScene.SetMap(mapSerializable);

        //Serialize Additional Elements
        GameObject additionalElementContainer = GameObject.Find("AdditionalElements");
        foreach (Transform elementTransform in additionalElementContainer.transform)
        {
            if (elementTransform.gameObject != additionalElementContainer && elementTransform.GetComponentInChildren<MeshFilter>() != null && elementTransform.gameObject.activeInHierarchy)
            {
                TileElement tileElement;
                

                String text = elementTransform.GetComponentInChildren<TextMesh>() != null ? elementTransform.GetComponentInChildren<TextMesh>().text : null;
                //ElementMenuController ElementMenuController = GameObject.Find("ElementMenuCanvas").GetComponent<ElementMenuController>();
                //if (chidlrenGO.GetComponent<MeshRenderer>().material.color != null && chidlrenGO.Equals(ElementMenuController.GetSelectedElement()))
                //    tileElement = new TileElement(chidlrenGO, "AdditionalElement", ElementMenuController.GetSelectedElement().GetComponent<MeshRenderer>().material.color, text);

                //else
                tileElement = new TileElement(elementTransform.gameObject, "AdditionalElement", text);

                tileElement.SetAlterable(elementTransform.GetComponent<SelectableElementController>().IsAlterable());
                serializedScene.AddAdditionalElement(tileElement);
                ++i;
            }
        }

        BinaryFormatter bf = new BinaryFormatter();
        //Save a file
        String filePath = dataPath + "/map-" + DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss.fff") + ".bytes";
        //String filePath = "./Assets" + Scenes.GetParamForString("titleProject") + "/map-" + DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss.fff") + ".bytes";

        FileStream stream = new FileStream(filePath, FileMode.Create);
        bf.Serialize(stream, serializedScene);

        Debug.Log("Serialization Done.");
        stream.Close();

        //Save a BLOB
        //byte[] savedDatas;
        //using (MemoryStream ms = new MemoryStream())
        //{
        //    bf.Serialize(ms, serializedScene);
        //    savedDatas = ms.ToArray();
        //    ms.Close();
        //}
        //Save BDD and Update binary file 
        //DataService.UpdateProposalSaveFile(savedDatas);
        //Scenes.GetSelectedProposal().File = savedDatas;
    }


    public static void LoadMap(byte[] datas)
    {
        //Read binary file
        SerializedScene serializedScene;
        using (MemoryStream ms = new MemoryStream(datas))
        {
            BinaryFormatter bf = new BinaryFormatter();
            serializedScene = bf.Deserialize(ms) as SerializedScene;
            ms.Close();
        }

        MapSerializable map = serializedScene.GetMap();

        //Create map
        GameObject mapGo = map.GetGameObject();
        mapGo.AddComponent<MapController>();

        //Create tiles
        foreach (MapTile tile in map.GetTiles())
        {
            GameObject tileGo = tile.GetGameObject();
            tileGo.transform.parent = mapGo.transform;
            //Mesh renderer
            MeshRenderer meshRenderer = tileGo.AddComponent<MeshRenderer>();
            meshRenderer.material = Resources.Load(tile.GetMaterialName(), typeof(Material)) as Material;
            Texture2D texture = new Texture2D(0, 0, TextureFormat.RGB24, true);
            texture.LoadImage(tile.GetTextureData());
            texture.wrapMode = TextureWrapMode.Clamp;
            meshRenderer.material.mainTexture = texture;
            //Mesh filter
            MeshFilter meshFilter = tileGo.AddComponent<MeshFilter>();
            meshFilter.mesh = tile.GetMesh();
            //Mesh collider
            tileGo.AddComponent<BoxCollider>();

            //File.WriteAllBytes("Deserialized/" + tileGo.name, tile.GetTextureData());
            //Create tile elements
            foreach (TileElement tileElement in tile.GetElements())
            {
                GameObject tileElementGo = tileElement.GetGameObject();
                tileElementGo.SetActive(false);
                tileElementGo.transform.parent = tileGo.transform;
                meshRenderer = tileElementGo.AddComponent<MeshRenderer>();
                Material tileElementMaterial = Resources.Load<Material>("Materials/" + tileElement.GetMaterialName());
                meshRenderer.material = tileElementMaterial;

                meshRenderer.material.color = tileElement.GetMaterialColor();
                meshFilter = tileElementGo.AddComponent<MeshFilter>();
                meshFilter.mesh = tileElement.GetMesh();
                
                tileElementGo.SetActive(true);
                //Text mesh
                if (tileElement.GetText() != null)
                    CreateTextMesh(tileElementGo, tileElement.GetText());

                //Scale elements
                tileElementGo.transform.localScale = new Vector3(tileElementGo.transform.localScale.x, tileElementGo.transform.localScale.y, tileElementGo.transform.localScale.z);

                SelectableElementController selectableElementController = tileElementGo.AddComponent<SelectableElementController>();
                selectableElementController.SetAlterable(false);
                selectableElementController.SetBuilding(true);
            }
        }

        //Create additional elements
        GameObject additionalElementContainer = GameObject.Find("AdditionalElements");
        foreach (TileElement element in serializedScene.GetAdditionalElements())
        {
            GameObject tileElementGo = element.GetGameObject();
            tileElementGo.transform.SetParent(additionalElementContainer.transform, false); ;
            //Mesh renderer
            MeshRenderer meshRenderer = tileElementGo.AddComponent<MeshRenderer>();
            meshRenderer.material = Resources.Load(element.GetMaterialName(), typeof(Material)) as Material;
            meshRenderer.material.color = element.GetMaterialColor();
            //Mesh filter
            MeshFilter meshFilter = tileElementGo.AddComponent<MeshFilter>();
            meshFilter.mesh = element.GetMesh();
            //Controller
            SelectableElementController selectableElementController = tileElementGo.AddComponent<SelectableElementController>();
            selectableElementController.SetAlterable(element.IsAlterable());
            selectableElementController.SetBuilding(false);
            // Mesh Collider
            tileElementGo.AddComponent<BoxCollider>();
            //Text mesh
            if (element.GetText() != null)
                CreateTextMesh(tileElementGo, element.GetText());
        }

    }

    private static void CreateTextMesh(GameObject GO, String Text)
    {
        GameObject child = new GameObject(GO.name + "-label");
        child.tag = "Label";
        child.transform.SetParent(GO.transform, false);
        child.transform.position = new Vector3(GO.transform.position.x, GO.GetComponent<BoxCollider>().bounds.max.y, GO.transform.position.z);
        child.transform.localScale = new Vector3(child.transform.localScale.x, 1 / 5f, child.transform.localScale.z);

        child.AddComponent<TextMesh>();
        TextMesh textMesh = child.GetComponent<TextMesh>();
        textMesh.text = Text;
        textMesh.font = Resources.Load("Arial", typeof(Font)) as Font;
        textMesh.fontSize = 30;
        textMesh.fontStyle = FontStyle.Bold;
        textMesh.anchor = TextAnchor.MiddleCenter;
    }
}



[Serializable]
public class SerializedScene
{
    [SerializeField]
    private MapSerializable map;

    [SerializeField]
    private List<TileElement> additionalElements = new List<TileElement>();

    public MapSerializable GetMap()
    {
        return this.map;
    }

    public void SetMap(MapSerializable map)
    {
        this.map = map;
    }

    public List<TileElement> GetAdditionalElements()
    {
        return this.additionalElements;
    }

    public void AddAdditionalElement(TileElement element)
    {
        this.additionalElements.Add(element);
    }
}

[Serializable]
public class MapSerializable
{
    [SerializeField]
    private GameObjectSerializable mapGO;

    [SerializeField]
    private List<MapTile> tiles = new List<MapTile>();

    public MapSerializable(GameObject mapGO)
    {
        this.mapGO = new GameObjectSerializable(mapGO, mapGO.name);
    }

    public GameObject GetGameObject()
    {
        return mapGO.GetGameObject();
    }
    public void AddTile(MapTile tile)
    {
        this.tiles.Add(tile);
    }
    public List<MapTile> GetTiles()
    {
        return this.tiles;
    }
}

[Serializable]
public class MapTile
{
    [SerializeField]
    private GameObjectSerializable tileGO;

    [SerializeField]
    private List<TileElement> tileElements = new List<TileElement>();

    [SerializeField]
    private MeshSerializable mesh;

    [SerializeField]
    private String materialName;

    [SerializeField]
    private byte[] textureData;

    public MapTile(GameObject tileGO)
    {
        this.tileGO = new GameObjectSerializable(tileGO, tileGO.name);
        this.mesh = new MeshSerializable(tileGO.GetComponent<MeshFilter>().mesh);
        this.materialName = "TerrainMaterial";
        Texture2D texture = (Texture2D)tileGO.GetComponent<MeshRenderer>().material.mainTexture;
        this.textureData = texture.EncodeToPNG();
        //File.WriteAllBytes("Serialized/" + "tile" + index + ".png", texture.EncodeToPNG());
    }

    public Mesh GetMesh()
    {
        return this.mesh.GetMesh();
    }

    public String GetMaterialName()
    {
        return this.materialName;
    }
    public void AddElement(TileElement element)
    {
        this.tileElements.Add(element);
    }

    public List<TileElement> GetElements()
    {
        return this.tileElements;
    }

    public GameObject GetGameObject()
    {
        return this.tileGO.GetGameObject();
    }

    public byte[] GetTextureData()
    {
        return this.textureData;
    }
}

[Serializable]
public class TileElement
{
    [SerializeField]
    private GameObjectSerializable tileElementGo;

    [SerializeField]
    private MeshSerializable mesh;

    [SerializeField]
    private String materialName;

    [SerializeField]
    private float materialColorR;
    [SerializeField]
    private float materialColorG;
    [SerializeField]
    private float materialColorB;

    [SerializeField]
    private bool isAlterable;

    [SerializeField]
    private String type;

    [SerializeField]
    private String text;

    public TileElement(GameObject go, String type, Color color, String text)
    {
        this.type = type;
        this.text = text;
        this.tileElementGo = new GameObjectSerializable(go, go.name);
        this.mesh = new MeshSerializable(go.GetComponent<MeshFilter>().mesh);
        if (type.Equals("Road"))
            this.materialName = "RoadsDefaultMaterial";
        else if (type.Equals("MapBuilding"))
            this.materialName = "BuildingsDefaultMat";
        else
            this.materialName = "TransparentGreyMaterial";

        this.materialColorR = color.r;
        this.materialColorG = color.g;
        this.materialColorB = color.b;

    }

    public TileElement(GameObject go, String type, String text)
    {
        this.type = type;
        this.text = text;
        this.tileElementGo = new GameObjectSerializable(go, go.name);
        if (type== "AdditionalElement")
        {
            this.mesh = new MeshSerializable(go.GetComponentInChildren<MeshFilter>().mesh);
            if (go.GetComponent<MeshRenderer>().material.color != null)
            {
                this.materialColorR = go.GetComponentInChildren<MeshRenderer>().material.color.r;
                this.materialColorG = go.GetComponentInChildren<MeshRenderer>().material.color.g;
                this.materialColorB = go.GetComponentInChildren<MeshRenderer>().material.color.b;
            }
            this.materialName = "TransparentGreyMaterial";
        }
          
        else
        {
            this.mesh = new MeshSerializable(go.GetComponent<MeshFilter>().mesh);
            if (go.GetComponent<MeshRenderer>().material.color != null)
            {
                this.materialColorR = go.GetComponent<MeshRenderer>().material.color.r;
                this.materialColorG = go.GetComponent<MeshRenderer>().material.color.g;
                this.materialColorB = go.GetComponent<MeshRenderer>().material.color.b;
            }
            this.materialName = "BuildingsDefaultMat";
        }
    }

    public GameObject GetGameObject()
    {
        return this.tileElementGo.GetGameObject();
    }

    public Mesh GetMesh()
    {
        return this.mesh.GetMesh();
    }

    public String GetMaterialName()
    {
        return this.materialName;
    }

    public Color GetMaterialColor()
    {
        return new Color(materialColorR, materialColorG, materialColorB);
    }

    public bool IsAlterable()
    {
        return this.isAlterable;
    }

    public void SetAlterable(bool value)
    {
        this.isAlterable = value;
    }

    public String GetText()
    {
        return this.text;
    }
}

[Serializable]
public class GameObjectSerializable
{
    [SerializeField]
    private string name;
    [SerializeField]
    private float posX;
    [SerializeField]
    private float posY;
    [SerializeField]
    private float posZ;
    [SerializeField]
    private float rotX;
    [SerializeField]
    private float rotY;
    [SerializeField]
    private float rotZ;
    [SerializeField]
    private float rotW;
    [SerializeField]
    private float scaleX;
    [SerializeField]
    private float scaleY;
    [SerializeField]
    private float scaleZ;

    public GameObjectSerializable(GameObject gO, String name)
    {
        this.name = name;

        this.posX = gO.transform.position.x;
        this.posY = gO.transform.position.y;
        this.posZ = gO.transform.position.z;

        this.rotX = gO.transform.rotation.x;
        this.rotY = gO.transform.rotation.y;
        this.rotZ = gO.transform.rotation.z;
        this.rotW = gO.transform.rotation.w;

        this.scaleX = gO.transform.localScale.x;
        this.scaleY = gO.transform.localScale.y;
        this.scaleZ = gO.transform.localScale.z;
    }

    public GameObject GetGameObject()
    {
        GameObject gO = new GameObject(this.name);
        gO.transform.position = new Vector3(this.posX, this.posY, this.posZ);
        gO.transform.rotation = new Quaternion(rotX, rotY, rotZ, rotW);
        gO.transform.localScale = new Vector3(scaleX, scaleY, scaleZ);
        return gO;
    }

}

[Serializable]
public class MeshSerializable
{
    [SerializeField]
    public float[] vertices;
    [SerializeField]
    public int[] triangles;
    [SerializeField]
    public float[] uv;
    [SerializeField]
    public float[] uv2;
    [SerializeField]
    public float[] normals;
    [SerializeField]
    public Color[] colors;

    public MeshSerializable(Mesh m) // Constructor: takes a mesh and fills out SerializableMeshInfo data structure which basically mirrors Mesh object's parts.
    {
        vertices = new float[m.vertexCount * 3]; // initialize vertices array.
        for (int i = 0; i < m.vertexCount; i++) // Serialization: Vector3's values are stored sequentially.
        {
            vertices[i * 3] = m.vertices[i].x;
            vertices[i * 3 + 1] = m.vertices[i].y;
            vertices[i * 3 + 2] = m.vertices[i].z;
        }
        triangles = new int[m.triangles.Length]; // initialize triangles array
        for (int i = 0; i < m.triangles.Length; i++) // Mesh's triangles is an array that stores the indices, sequentially, of the vertices that form one face
        {
            triangles[i] = m.triangles[i];
        }
        uv = new float[m.uv.Length * 2]; // initialize uvs array
        for (int i = 0; i < m.uv.Length; i++) // uv's Vector2 values are serialized similarly to vertices' Vector3
        {
            uv[i * 2] = m.uv[i].x;
            uv[i * 2 + 1] = m.uv[i].y;
        }
        uv2 = new float[m.uv2.Length]; // uv2
        for (int i = 0; i < m.uv2.Length; i++)
        {
            uv[i * 2] = m.uv2[i].x;
            uv[i * 2 + 1] = m.uv2[i].y;
        }
        normals = new float[m.normals.Length * 3]; // normals are very important

        for (int i = 0; i < m.normals.Length; i++) // Serialization
        {
            normals[i * 3] = m.normals[i].x;
            normals[i * 3 + 1] = m.normals[i].y;
            normals[i * 3 + 2] = m.normals[i].z;
        }
        colors = new Color[m.colors.Length];
        for (int i = 0; i < m.colors.Length; i++)
        {
            colors[i] = m.colors[i];
        }
    }

    // GetMesh gets a Mesh object from currently set data in this SerializableMeshInfo object.
    // Sequential values are deserialized to Mesh original data types like Vector3 for vertices.
    public Mesh GetMesh()
    {
        Mesh m = new Mesh();
        List<Vector3> verticesList = new List<Vector3>();
        for (int i = 0; i < vertices.Length / 3; i++)
        {
            verticesList.Add(new Vector3(
                    vertices[i * 3], vertices[i * 3 + 1], vertices[i * 3 + 2]
                ));
        }
        m.SetVertices(verticesList);
        m.triangles = triangles;
        List<Vector2> uvList = new List<Vector2>();
        for (int i = 0; i < uv.Length / 2; i++)
        {
            uvList.Add(new Vector2(
                    uv[i * 2], uv[i * 2 + 1]
                ));
        }
        m.SetUVs(0, uvList);
        List<Vector2> uv2List = new List<Vector2>();
        for (int i = 0; i < uv2.Length / 2; i++)
        {
            uv2List.Add(new Vector2(
                    uv2[i * 2], uv2[i * 2 + 1]
                ));
        }
        m.SetUVs(1, uv2List);
        List<Vector3> normalsList = new List<Vector3>();
        for (int i = 0; i < normals.Length / 3; i++)
        {
            normalsList.Add(new Vector3(
                    normals[i * 3], normals[i * 3 + 1], normals[i * 3 + 2]
                ));
        }
        m.SetNormals(normalsList);
        m.colors = colors;

        return m;
    }
}