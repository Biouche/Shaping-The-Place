using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ActiveSceneEvent : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        SceneManager.activeSceneChanged += ChangedActiveScene;
    }

    private void ChangedActiveScene(Scene current, Scene next)
    {
        if (current.name == Scenes.Explorer && next.name == Scenes.EditObject)
        {
            foreach (GameObject go in current.GetRootGameObjects())
                if (go.name == "Scene")
                    go.SetActive(false);
        }
        else if (current.name == Scenes.EditObject && next.name == Scenes.Explorer)
        {
            foreach (GameObject go in next.GetRootGameObjects())
                if (go.name == "Scene")
                {
                    //Activate Scene GO
                    go.SetActive(true);
                    //CameraController.instance.initializeCamera();

                    Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
                    RaycastHit hit;
                    Physics.Raycast(ray, out hit);

                    //Load from Splitter scene
                    if (Scenes.GetGOParameters().Count > 0)
                    {
                        foreach (GameObject DictionnaryGO in Scenes.GetGOParameters().Values)
                        {
                            //Set parent
                            DictionnaryGO.transform.SetParent(GameObject.Find("AdditionalElements").transform, false);
                            //Postition
                            DictionnaryGO.transform.position = Scenes.PreviousPos;
                            //Scale
                            DictionnaryGO.transform.localScale = Scenes.PreviousScale;
                            // stick to floor
                            BoxCollider collider = DictionnaryGO.GetComponentInChildren<BoxCollider>();
                            Bounds bounds = collider.bounds;
                            Vector3 position = DictionnaryGO.transform.position;
                            float colliderMinY = bounds.min.y;
                            if (colliderMinY < 0)
                                position += new Vector3(0, -colliderMinY, 0);
                            else
                                position -= new Vector3(0, colliderMinY, 0);
                            DictionnaryGO.transform.position = position;

                            //Add tag to children
                            collider.gameObject.tag = "AdditionalElements";

                            //Add components
                            SelectableElementController selectableElement = DictionnaryGO.AddComponent<SelectableElementController>();
                            selectableElement.SetAlterable(true);
                        }
                        Scenes.GetGOParameters().Clear();
                        HistoryPanelManager.instance.Refresh();
                    }
                    break;
                }

        }
    }
}


