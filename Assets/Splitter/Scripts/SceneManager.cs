using Assets.Splitter.Scripts;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Splitter
{
    public class SceneManager : MonoBehaviour
    {
        public static SceneManager instance;
        public static string DataPath;
        public GameObject objectContainer;

        public Text errorMesage;


        private void Awake()
        {
            instance = this;
            DataPath = Application.streamingAssetsPath + "/ModelDatabase/Alterable/";
        }


        // Use this for initialization
        void Start()
        {
            Scenes.SetActiveScene(Scenes.EditObject);
            //Import from Explorer scene
            GameObject biggestGO = null;
            foreach (GameObject GO in Scenes.GetGOParameters().Values)
            {
                GameObject newGO = InitGO(GO);
                if (biggestGO != null && newGO.GetComponent<Collider>().bounds.max.z < biggestGO.GetComponent<Collider>().bounds.max.z)
                    continue;
                else
                    biggestGO = newGO;
            }
            
            ////FOR TESTS
            //MeshRenderer renderer = CuttedGameObject.GetComponent<MeshRenderer>();
            MeshFilter mf = biggestGO.GetComponent<MeshFilter>();
            BoxCollider collider = biggestGO.GetComponent<BoxCollider>();

            //Set GO position to center of the screen
            Vector3 center = collider.bounds.center;
            objectContainer.transform.position = new Vector3(biggestGO.transform.position.x - center.x, biggestGO.transform.position.y - center.y);

            // get eventual distance betewen z of bounding box and the back plane
            if (biggestGO.GetComponent<BoxCollider>().bounds.Intersects(GameObject.Find("Plane").GetComponent<BoxCollider>().bounds))
            {
                float dist = Vector3.Distance(new Vector3(0, 0, biggestGO.GetComponent<BoxCollider>().bounds.max.z), GameObject.Find("Plane").transform.position);
                objectContainer.transform.Translate(0, 0, -dist);
            }

            //Scale camera
            Vector3 xyz = mf.mesh.bounds.size;
            float distance = Mathf.Max(xyz.x, xyz.y, xyz.z);
            distance /= (2.0f * Mathf.Tan(0.5f * Camera.main.fieldOfView * Mathf.Deg2Rad));
            // Move camera in -z-direction; change '2.0f' to your needs
            Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, -distance * 2.0f);
        }

        public GameObject InitGO(GameObject GO)
        {
            //Create GO
            GameObject go = new GameObject(GO.GetComponentInChildren<BoxCollider>().gameObject.name);
            //Mesh Filter
            MeshFilter mf = go.AddComponent<MeshFilter>();
            mf.sharedMesh = GO.GetComponentInChildren<MeshFilter>().sharedMesh;
            //Mesh renderer
            MeshRenderer renderer = go.AddComponent<MeshRenderer>();
            renderer.sharedMaterial = Resources.Load("Materials/BenchMat", typeof(Material)) as Material;
            //Mesh Collider
            BoxCollider collider = go.AddComponent<BoxCollider>();
            //Create container
            GameObject container = new GameObject(GO.name);
            container.transform.position = collider.bounds.center;
            //Set parents
            container.transform.SetParent(objectContainer.transform, true);
            go.transform.SetParent(container.transform, true);
            //Destroy imported GO
            Destroy(GO);
            //Set tag
            go.tag = "ObjectToCut";
            return go;
        }

        public void HandleCut(Vector3[] positions)
        {
            //Generate cut meshes from points
            Vector2 B1;

            //LOOP
            Transform[] childs = this.objectContainer.GetComponentsInChildren<Transform>();
            foreach (Transform childTransform in childs)
            {
                //Check its not parent transform
                if (childTransform.GetComponent<BoxCollider>() == null || childTransform.gameObject == this.objectContainer)
                    continue;

                GameObject cuttedGameObject = childTransform.gameObject;

                //Generated cut meshes. Mesh is generated at world origin.
                Mesh cutMesh = GeomUtils.GenerateMeshFromPositions(positions, 1, out B1, cuttedGameObject);
                Mesh cutMesh2 = GeomUtils.GenerateMeshFromPositions(positions, 2, out B1, cuttedGameObject);

                //Generate cut GO
                GameObject cutMeshGO = AddMeshToScene("Cut Mesh", cutMesh, 1f);
                GameObject cutMeshGO2 = AddMeshToScene("Cut Mesh 2", cutMesh2, 1f);

                //Move cut mesh to previous position to check intersection
                cutMeshGO.transform.position = Vector3.zero;
                cutMeshGO.transform.position += new Vector3(B1.x, B1.y, 0);
                cutMeshGO2.transform.position = Vector3.zero;
                cutMeshGO2.transform.position += new Vector3(B1.x, B1.y, 0);

                //Check intersection between parts and the plane drawn
                if (GeomUtils.Intersects(cutMeshGO, cuttedGameObject) && GeomUtils.CheckTrailIsLongEnough(positions, cuttedGameObject))
                {
                    try
                    {
                        //Generate cutted mesh off file and matrix 
                        string cuttedMeshOff = ObjectFileFormat.MeshToOff(cuttedGameObject.GetComponent<MeshFilter>().mesh, cuttedGameObject.transform);
                        string cuttedMeshMatrix = GeomUtils.GetTransformationMatrix(cuttedGameObject.transform, 0f, /*-2f * cutMeshGO.transform.rotation.eulerAngles.y*/ 0f, 0f);

                        //Do cut with cut mesh 1
                        Mesh mesh1 = CutMesh(cutMesh, cutMeshGO, cuttedGameObject.name, cuttedMeshOff, cuttedMeshMatrix);
                        //Do cut with cut mesh 2
                        Mesh mesh2 = CutMesh(cutMesh2, cutMeshGO2, cuttedGameObject.name, cuttedMeshOff, cuttedMeshMatrix);

                        // Add result to scene

                        GameObject Result1 = GeomUtils.AddMeshToScene("Split-" + DateTime.Now.ToString("hhmmssffff") + ".1", mesh1, new Vector3(0, -0.5f, 0), objectContainer.transform);
                        GameObject Result2 = GeomUtils.AddMeshToScene("Split-" + DateTime.Now.ToString("hhmmssffff") + ".2", mesh2, new Vector3(0, 0.5f, 0), objectContainer.transform);

                        //Destroy cutted game object container
                        Destroy(GameObject.Find(cuttedGameObject.name).transform.parent.gameObject);
                    }
                    catch (FileNotFoundException e)
                    {
                        Debug.Log(e);
                        errorMesage.text = "Le fichier de maillage est introuvable";
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e);
                        errorMesage.text = "Trail is too short.";
                    }
                }
                //Destroy cut GO
                Destroy(cutMeshGO);
                Destroy(cutMeshGO2);
            }
        }

        private Mesh CutMesh(Mesh cutMesh, GameObject cutMeshGO, string cuttedGoName, string cuttedMeshOff, string cuttedMeshMatrix)
        {
            /* ********** handle cut mesh ********** */
            string cutMeshOff = ObjectFileFormat.MeshToOff(cutMesh, cutMeshGO.transform);
            IntPtr cutMeshPtr = Marshal.StringToHGlobalAnsi(cutMeshOff);
            IntPtr cutMeshTransformPtr = Marshal.StringToHGlobalAnsi(GeomUtils.GetTransformationMatrix(cutMeshGO.transform, 0f, 0f, 0f));

            //exportOff
            //ExportOff(cutMeshOff, "./Assets/" + cutMeshGO.name + ".off");

            /* ********** handle cutted mesh ********** */
            IntPtr cuttedMeshPtr = Marshal.StringToHGlobalAnsi(cuttedMeshOff);
            IntPtr cuttedMeshTransformPtr = Marshal.StringToHGlobalAnsi(cuttedMeshMatrix);

            //exportOff
            //ExportOff(cuttedMeshOff, "./Assets/" + cuttedGoName + ".off");

            /* ********** compute cut with CGAL********** */
            IntPtr ptrResult = CGALPlugin.BooleanOperationClean(cuttedMeshPtr, cuttedMeshTransformPtr, cutMeshPtr, cutMeshTransformPtr, Marshal.StringToHGlobalAnsi("difference"));

            //Convert IntPtr to string
            string result = Marshal.PtrToStringAnsi(ptrResult);

            //exportOff
            //ExportOff(result, DataPath + initialObjectName + parts + ".off");

            // Open string stream on the result off string
            byte[] byteArray = Encoding.UTF8.GetBytes(result);
            MemoryStream stream = new MemoryStream(byteArray);

            //Convert off string to Mesh and add it to scene
            Mesh myMesh = ObjectFileFormat.OffToMesh(new StreamReader(stream));
            stream.Close();
            myMesh.RecalculateNormals();

            //Export obj file
            ObjExporter.MeshToFile(myMesh, "Split-" + DateTime.Now.ToString("hhmmssffff"), "BenchMat");
#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif
            return myMesh;
        }

        private GameObject AddMeshToScene(string name, Mesh mesh, float scale)
        {
            GameObject go = new GameObject(name);
            go.transform.position = Vector3.zero;
            go.transform.localScale = new Vector3(1f, 1f, 1f) * scale;
            go.AddComponent<MeshRenderer>();
            MeshFilter mf = go.AddComponent<MeshFilter>();
            mf.mesh = mesh;
            go.AddComponent<BoxCollider>();
            return go;
        }

        private void ExportOff(string offFile, string path)
        {
            StreamWriter sw = new StreamWriter(path);
            sw.Write(offFile);
            sw.Close();
        }
    }
}