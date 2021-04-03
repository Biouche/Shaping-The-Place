using UnityEngine;
using Mapbox.Unity.Map;
using System.IO;
using System;
using Mapbox.Unity.Utilities;

public class MapController : MonoBehaviour
{
    public AbstractMap _map;

    private static GameObject selectedObject;
    //private const float DEFAULT_SCALE = 12.56f;
    public const float DEFAULT_SCALE = 2f;

    // Use this for initialization
    void Start()
    {
        // Create New proposal
        if (Scenes.IsNewProposal() && _map != null)
        {
            Debug.Log("Creating new proposal");
            String initialCoordinates = Scenes.GetSelectedProject().Coordinates;
            if (initialCoordinates != null)
                _map.Initialize(Conversions.StringToLatLon(initialCoordinates), (int)_map.Zoom);

            var visualizer = _map.MapVisualizer;
            visualizer.OnMapVisualizerStateChanged += (s) =>
            {
                if (this == null)
                    return;
                if (s == ModuleState.Finished)
                {
                    InitBuildings();
                    InteractionsCanvasController.instance.OnClickSave();
                    Scenes.SetIsNewProposal(false);
                }
            };
        }
        //Load existing proposal
        else if (_map != null)
        {
            //for test
            var visualizer = _map.MapVisualizer;
            visualizer.OnMapVisualizerStateChanged += (s) =>
            {
                if (this == null)
                    return;
                if (s == ModuleState.Finished)
                {
                    InitBuildings();
                }
            };
            //Destroy(_map.gameObject);
            //MapLoader.LoadMap(Scenes.GetSelectedProposal().File);
        }
    }

    void InitBuildings()
    {
        //Add components to buildings
        foreach (Transform Tile in _map.transform)
        {
            Tile.gameObject.AddComponent<BoxCollider>();
            Tile.gameObject.tag = "Tile";
            foreach (Transform TileElement in Tile.transform)
            {
                if (TileElement.gameObject.GetComponent<SelectableElementController>() == null)
                {
                    //TileElement.gameObject.SetActive(false);
                    //Add interaction script
                    /*SelectableElementController controller = TileElement.gameObject.AddComponent<SelectableElementController>();
                    controller.SetBuilding(true);*/
                    //Add name above building if exists script
                    /*if (Array.Exists(nomsBuildings, building => building == objectContainer.gameObject.name))
                        InitBuildingName(objectContainer.gameObject.name, objectContainer.gameObject);*/

                    //Update scale
                    /*BoxCollider BC = TileElement.gameObject.GetComponent<BoxCollider>();
                    if (BC != null && BC.bounds.size.y <= DEFAULT_SCALE)
                        TileElement.gameObject.transform.localScale = new Vector3(TileElement.gameObject.transform.localScale.x, TileElement.gameObject.transform.localScale.y * DEFAULT_SCALE, TileElement.gameObject.transform.localScale.z);
                    TileElement.gameObject.SetActive(true);
                    TileElement.tag = "Building";*/
                }
            }
        }
    }

    public void InitBuildingName(string objectName, GameObject go)
    {
        string path = "./Assets/Resources/ModelDatabase/";

        StreamReader reader = new StreamReader(path + objectName + ".txt");
        addText(go, reader.ReadLine());
        reader.Close();
    }

    public static GameObject GetSelectedObject()
    {
        return selectedObject;
    }

    public static void SetSelectedObject(GameObject go)
    {
        selectedObject = go;
    }

    public static void addText(GameObject go, string title)
    {
        GameObject child = new GameObject(go.name + " label");
        child.tag = "Label";
        child.transform.parent = go.transform;
        child.transform.position = go.GetComponent<BoxCollider>().bounds.center;
        child.transform.position = new Vector3(child.transform.position.x, go.GetComponent<BoxCollider>().bounds.max.y, child.transform.position.z);
        child.transform.localScale = new Vector3(child.transform.localScale.x, 1 / DEFAULT_SCALE, child.transform.localScale.z);


        child.AddComponent<TextMesh>();
        TextMesh textMesh = child.GetComponent<TextMesh>();
        textMesh.text = title;
        textMesh.font = Resources.Load("norwester", typeof(Font)) as Font;
        textMesh.fontSize = 20;
        textMesh.anchor = TextAnchor.MiddleCenter;
    }

    public void OnClickExit()
    {
        print("EXIT");
        Application.Quit();
    }


    public void SetSatMap()
    {
        _map.ImageLayer.SetLayerSource(ImagerySourceType.MapboxSatellite);
    }

    public void SetMap()
    {
        _map.ImageLayer.SetLayerSource(ImagerySourceType.MapboxStreets);
    }
}
