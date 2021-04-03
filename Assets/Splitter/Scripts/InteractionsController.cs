using Assets.Splitter.Scripts;
using Splitter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

public class InteractionsController : MonoBehaviour
{

    public GameObject objectContainer;
    public GameObject FusionPanel;
    public GameObject DeletePanel;

    SortedList<string, Color> fusionList = new SortedList<string, Color>();
    SortedList<string, Color> deleteList = new SortedList<string, Color>();

    public static InteractionsController instance;

    private void Start()
    {
        instance = this;
    }

    public SortedList<string, Color> GetDeleteList()
    {
        return this.deleteList;
    }

    public SortedList<string, Color> GetFusionList()
    {
        return this.fusionList;
    }

    public void ValidateModifications()
    {
        Scenes.GetGOParameters().Clear();

        foreach (Transform elementTransform in objectContainer.transform)
        {
            if (elementTransform.transform != objectContainer.transform)
                Scenes.AddGOParam(elementTransform.gameObject.GetComponentInChildren<BoxCollider>().gameObject.name, elementTransform.gameObject);
        }
        Scenes.CloseScene("EditObject");
    }

    public void CloseScene()
    {
        Scenes.GetGOParameters().Clear();
        Scenes.CloseScene("EditObject");
    }

    /** DELETE **/

    public void ClickDelete()
    {
        this.DeletePanel.SetActive(true);
        GesturesHandler.SetDeleteSelection(true);
    }

    public void ClickConfirmDelete()
    {
        foreach (string GOName in deleteList.Keys)
        {
            GameObject tempGo = GameObject.Find(GOName);
            Destroy(tempGo.transform.parent.gameObject);
        }
        ResetDeleteSelection();
    }

    public void ClickCancelDelete()
    {
        ResetDeleteSelection();
    }

    private void ResetDeleteSelection()
    {
        //Reset colors
        foreach (string GOName in deleteList.Keys)
        {
            GameObject tempGo = GameObject.Find(GOName);
            Material mat = tempGo.GetComponentInChildren<MeshRenderer>().materials[0];
            mat.color = deleteList[GOName];
        }

        //Clear list
        deleteList.Clear();
        GesturesHandler.SetDeleteSelection(false);
        this.DeletePanel.SetActive(false);
    }


    /** FUSION **/
    public void ClickFusion()
    {
        this.FusionPanel.SetActive(true);
        GesturesHandler.SetFusionSelection(true);
        //Wait for event
        StartCoroutine(FusionSelection());
    }

    public IEnumerator FusionSelection()
    {
        //Wait for object selection or cancel
        while (fusionList.Count != 2 && this.FusionPanel.activeInHierarchy)
            yield return new WaitForSeconds(0.001f);
        //Do merge
        if (fusionList.Count == 2)
        {
            GameObject GO1 = GameObject.Find(fusionList.Keys[0]);
            GameObject GO2 = GameObject.Find(fusionList.Keys[1]);
            Debug.Log("MERGING " + GO1.name + " AND " + GO2.name);
            if (GeomUtils.Intersects(GO1, GO2))
            {
                Debug.Log("INTERSECTS");
                string GO1Off = ObjectFileFormat.MeshToOff(GO1.GetComponent<MeshFilter>().mesh, GO1.transform);
                string GO2Off = ObjectFileFormat.MeshToOff(GO2.GetComponent<MeshFilter>().mesh, GO2.transform);

                System.IntPtr GO1Ptr = Marshal.StringToHGlobalAnsi(GO1Off);
                IntPtr GO2Ptr = Marshal.StringToHGlobalAnsi(GO2Off);
                String matrix1 = GeomUtils.GetTransformationMatrix(GO1.transform, 0f, 0f, 0f);
                String matrix2 = GeomUtils.GetTransformationMatrix(GO1.transform, 0f, 0f, 0f);
                IntPtr GO1TransformPtr = Marshal.StringToHGlobalAnsi(matrix1);
                IntPtr GO2TransformPtr = Marshal.StringToHGlobalAnsi(matrix2);

                IntPtr ptrResult = CGALPlugin.BooleanOperationClean(GO1Ptr, GO1TransformPtr, GO2Ptr, GO2TransformPtr, Marshal.StringToHGlobalAnsi("union"));
                string result = Marshal.PtrToStringAnsi(ptrResult);

                //exportOff
                /*StreamWriter SW = new StreamWriter(SceneManager.DataPath + "Fusion-" + DateTime.Now.ToString("hhmmss") + (Time.deltaTime * 1000) + ".off");
                SW.Write(result);
                SW.Close();*/
                
                // Open string stream on the result off string
                byte[] byteArray = Encoding.UTF8.GetBytes(result);
                MemoryStream stream = new MemoryStream(byteArray);

                //Convert off string to Mesh and add it to scene
                Mesh myMesh = ObjectFileFormat.OffToMesh(new StreamReader(stream));
                stream.Close();
                myMesh.RecalculateNormals();

                //Export obj file
                ObjExporter.MeshToFile(myMesh, "Fusion-" + DateTime.Now.ToString("hhmmssffff"), "BenchMat");

                //Add mesh to scene
                GeomUtils.AddMeshToScene("Fusion-" + DateTime.Now.ToString("hhmmssffff"), myMesh, Vector3.zero, objectContainer.transform);

                //Destroy previous previous meshes
                Destroy(GO1.transform.parent.gameObject);
                Destroy(GO2.transform.parent.gameObject);
            }
            //Not intersecting
            else
                SceneManager.instance.errorMesage.text = "Selected objects are not overlapped !";

            ResetFusionSelection();
        }
    }

    private void ResetFusionSelection()
    {
        Debug.Log("FUSION LIST RESET");
        //Reset colors
        foreach (String GOName in fusionList.Keys)
        {
            Debug.Log("Find " + GOName + " and reset color");
            GameObject tempGo = GameObject.Find(GOName);
            Material mat = tempGo.GetComponentInChildren<MeshRenderer>().materials[0];
            mat.color = fusionList[GOName];
        }

        //Clear list
        fusionList.Clear();
        GesturesHandler.SetFusionSelection(false);
        this.FusionPanel.SetActive(false);
    }

    public void ClickCancelFusion()
    {
        ResetFusionSelection();
    }
}
