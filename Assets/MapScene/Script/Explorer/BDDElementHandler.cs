using UnityEngine;
using UnityEngine.EventSystems;
using UnityExtension;
using System.IO;
using System;
using UnityEngine.UI;
using System.Collections;

public class BDDElementHandler : MonoBehaviour, IPointerDownHandler
{
    // Use this for initialization
    private string path;
    private GameObject ElementPositioningPanel;
    private const float MIN_MAX_SIZE = 20;

    public void SetElementPositioningPanel(GameObject ElementPositioningPanel)
    {
        this.ElementPositioningPanel = ElementPositioningPanel;
    }

    public void SetPath(string path)
    {
        this.path = path;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        ElementPositioningPanel.SetActive(true);
        //Wait for event
        StartCoroutine(DoStuff());
    }

    public IEnumerator DoStuff()
    {
        while (ElementPositioningPanel.activeInHierarchy )
        {
            //Get position
            if (GesturesController.instance.IsObjectPositionValid())
            {
                GesturesController.instance.ResetObjectPositionValid();
                Vector3 pos = GesturesController.instance.GetPosition();
                //Instantiate
                InstantiateGo(pos);
            }

            yield return new WaitForSeconds(0.001f);
        }


    }

    private void InstantiateGo(Vector3 position)
    {
        //Instantiate the a game object from the name of the UI element clicked 
        GameObject GO = new GameObject(this.name + "-" + DateTime.Now.ToString("hhmmssffff"));

        //Add material
        MeshRenderer mr = GO.AddComponent<MeshRenderer>();
        mr.material = Resources.Load<Material>("Materials/BenchMat");
        //Load mesh from file
        MeshFilter mf = GO.AddComponent<MeshFilter>();
        FileStream lStream = new FileStream(path, FileMode.Open);
        OBJData lOBJData = OBJLoader.LoadOBJ(lStream);
        lStream.Close();
        mf.mesh.LoadOBJ(lOBJData);
        mf.mesh.RecalculateNormals();

        //Box collider
        BoxCollider collider = GO.AddComponent<BoxCollider>();

        //Create container for rotation
        GameObject container = new GameObject("Container-" + GO.name);
        container.transform.position = collider.bounds.center;
        container.transform.SetParent(GameObject.Find("AdditionalElements").transform, true);
        //Add interactions script
        SelectableElementController selectableElement = container.AddComponent<SelectableElementController>();
        //Check if is alterable
        GameObject alterableElements = GameObject.Find("Alterable");
        if (this.transform.IsChildOf(alterableElements.transform))
            selectableElement.SetAlterable(true);
        else
            selectableElement.SetAlterable(false);

        //Set parent
        GO.transform.SetParent(container.transform, false);
        GO.transform.localPosition = Vector3.zero;
        GO.tag = "AdditionalElements";

        //Normalize scale to MIN_MAX_SIZE Unity units...
        if (collider.bounds.size.x > MIN_MAX_SIZE)
            while (collider.bounds.size.magnitude >= MIN_MAX_SIZE)
                container.transform.localScale -= new Vector3(0.01f, 0.01f, 0.01f);
        else
            while (collider.bounds.size.magnitude <= MIN_MAX_SIZE)
                container.transform.localScale += new Vector3(0.01f, 0.01f, 0.01f);

        container.transform.position = position;

        //Stick to floor
        Bounds bounds = collider.bounds;
        float colliderMinY = bounds.min.y;
        Vector3 newPosition = container.transform.position;
        if (colliderMinY < 0)
            newPosition += new Vector3(0, -colliderMinY, 0);
        else
            newPosition -= new Vector3(0, colliderMinY, 0);
        container.transform.position = newPosition;

        //Update UI
        HistoryPanelManager.instance.Refresh();
    }
}
