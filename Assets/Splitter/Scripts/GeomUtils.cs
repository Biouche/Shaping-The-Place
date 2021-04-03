using Splitter;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace Assets.Splitter.Scripts
{
    class GeomUtils
    {
        public static GameObject AddMeshToScene(string name, Mesh mesh, Vector3 position, Transform parent)
        {
            //Game Object
            GameObject go = new GameObject(name);
            //Material
            MeshRenderer mr = go.AddComponent<MeshRenderer>();
            mr.material = Resources.Load<Material>("Materials/BenchMat");
            //Mesh
            MeshFilter mf = go.AddComponent<MeshFilter>();
            mf.mesh = mesh;
            mf.mesh.RecalculateNormals();
            //Scale
            go.transform.localScale = new Vector3(1f, 1f, 1f);
            //Components
            BoxCollider collider = go.AddComponent<BoxCollider>();
            //Set position
            go.transform.position = position;
            //Create container for rotation
            GameObject container = new GameObject("Container-" + name);
            container.transform.position = collider.bounds.center;
            container.transform.SetParent(parent, true);
            //Parent
            go.transform.SetParent(container.transform, true);
            //Set tag
            go.transform.tag = "ObjectToCut";
            return go;
        }

        public static Mesh GenerateMeshFromPositions(Vector3[] positions, int orientation, out Vector2 bary, GameObject cuttedGameObject)
        {
            //Compute barycentric point coordinates 
            float xValue = 0f;
            float yValue = 0f;
            for (int i = 0; i < positions.Length; i++)
            {
                xValue += positions[i].x;
                yValue += positions[i].y;
            }

            // Use barycenter to move to center
            bary = new Vector2(xValue / positions.Length, yValue / positions.Length);

            Vector2 transVect = new Vector3(-bary.x, -bary.y, positions[0].z);

            List<Vector3> list = new List<Vector3>();
            int valueI = 0, lastHit = 0;
            bool first = false;
            for (valueI = 0; valueI < positions.Length; valueI++)
            {
                Ray ray = new Ray(positions[valueI], Camera.main.transform.forward);
                Debug.DrawRay(new Vector3(positions[valueI].x, positions[valueI].y, 0), Camera.main.transform.forward, Color.red, 10f);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.gameObject != null && hit.collider.gameObject == cuttedGameObject)
                    {
                        if (!first && valueI > 0)
                        {
                            list.Add(positions[valueI - 1]);
                            first = true;
                        }
                        list.Add(positions[valueI]);
                        lastHit = valueI;
                    }
                }

            }
            if (positions.Length > lastHit + 1)
                list.Add(positions[lastHit + 1]);

            //List<Vector3> list = new List<Vector3>();
            //for (int i = 0; i < positions.Length; i++)
            //    list.Add(positions[i]);

            int nbVertices = list.Count * 2;
            Vector3[] vertices = new Vector3[nbVertices]; //add 4 points for extrusion
            float cutMeshDepth = Vector3.Distance(Camera.main.transform.position, GameObject.Find("Plane").transform.position);
            int k = 0;
            for (int i = 0; i < list.Count; i++)
            {
                //Backward projection
                vertices[k] = new Vector3(list[i].x + transVect.x, list[i].y + transVect.y, list[i].z - cutMeshDepth);
                ++k;
                //Frontward projection
                vertices[k] = new Vector3(list[i].x + transVect.x, list[i].y + transVect.y, list[i].z + cutMeshDepth);
                ++k;
            }

            int nbTriangles = (nbVertices - 2) * 3;
            // Add 8 triangles for extrusion
            int[] triangles = new int[nbTriangles];

            int index = 0, value = 0;
            // Clockwise
            if (orientation == 1)
            {

                while (index < nbTriangles / 2)
                {
                    triangles[index] = value;
                    ++index;
                    triangles[index] = value + 2;
                    ++index;
                    triangles[index] = value + 1;
                    ++index;
                    value = value + 2;

                }
                value = 1;
                while (index < nbTriangles)
                {
                    triangles[index] = value;
                    ++index;
                    ++value;
                    triangles[index] = value;
                    ++index;
                    ++value;
                    triangles[index] = value;
                    ++index;
                }
            }
            //Anti Clockwise
            else if (orientation == 2)
            {
                while (index < nbTriangles / 2)
                {
                    triangles[index] = value;
                    ++index;
                    ++value;
                    triangles[index] = value;
                    ++index;
                    ++value;
                    triangles[index] = value;
                    ++index;

                }
                value = 1;
                while (index < nbTriangles)
                {
                    triangles[index] = value;
                    ++index;
                    triangles[index] = value + 2;
                    ++index;
                    triangles[index] = value + 1;
                    ++index;
                    value = value + 2;
                }
            }
            else
            {
                return null;
            }

            Mesh cutMesh = new Mesh
            {
                vertices = vertices,
                triangles = triangles
            };

            cutMesh.RecalculateBounds();
            cutMesh.RecalculateNormals();

            return cutMesh;
        }

        public static String GetQuaternion(Transform transform)
        {
            StringBuilder strB = new StringBuilder();
            float x = transform.rotation.x;
            float y = transform.rotation.y;
            float z = transform.rotation.z;
            float w = transform.rotation.w;

            strB.Append(x).AppendLine();
            strB.Append(y).AppendLine();
            strB.Append(z).AppendLine();
            strB.Append(w).AppendLine();

            return strB.ToString();
        }

        public static String GetTransformationMatrix(Transform transform, float compensationX, float compensationY, float compensationZ)
        {
            StringBuilder strB = new StringBuilder();
            Matrix4x4 inputMatrix = transform.localToWorldMatrix;

            Quaternion rotation = Quaternion.Euler(compensationX, compensationY, compensationZ);
            Matrix4x4 rotationMatrix = Matrix4x4.Rotate(rotation);

            Matrix4x4 transformationMatrix = inputMatrix * rotationMatrix;

            strB.Append(transformationMatrix.m00).AppendLine();
            strB.Append(transformationMatrix.m01).AppendLine();
            strB.Append(transformationMatrix.m02).AppendLine();
            strB.Append(transformationMatrix.m03).AppendLine();
            strB.Append(transformationMatrix.m10).AppendLine();
            strB.Append(transformationMatrix.m11).AppendLine();
            strB.Append(transformationMatrix.m12).AppendLine();
            strB.Append(transformationMatrix.m13).AppendLine();
            strB.Append(transformationMatrix.m20).AppendLine();
            strB.Append(transformationMatrix.m21).AppendLine();
            strB.Append(transformationMatrix.m22).AppendLine();
            strB.Append(transformationMatrix.m23).AppendLine();

            return strB.ToString();
        }

        public static bool Intersects(GameObject cutGO, GameObject go2)
        {
            if (cutGO.GetComponent<BoxCollider>().bounds.Intersects(go2.GetComponent<BoxCollider>().bounds))
            {
                string cutMeshOff = ObjectFileFormat.MeshToOff(cutGO.GetComponent<MeshFilter>().mesh, cutGO.transform);
                string cuttedMeshOff = ObjectFileFormat.MeshToOff(go2.GetComponent<MeshFilter>().mesh, go2.transform);
                int result = CGALPlugin.CheckIntersection(Marshal.StringToHGlobalAnsi(cutMeshOff), Marshal.StringToHGlobalAnsi(cuttedMeshOff));
                if (result == 1)
                    return true;
            }
            SceneManager.instance.errorMesage.text = "Trail do not intersects any object.";
            return false;
        }

        public static bool CheckTrailIsLongEnough(Vector3[] positions, GameObject cuttedGO)
        {
            Bounds cuttedGOBounds = cuttedGO.GetComponent<BoxCollider>().bounds;
            if (cuttedGOBounds.Contains(new Vector3(positions[0].x, positions[0].y, 0f)) || cuttedGOBounds.Contains(new Vector3(positions[positions.Length - 1].x, positions[positions.Length - 1].y, 0f)))
            {
                SceneManager.instance.errorMesage.text = "Trail is too short.";
                return false;
            }
                
            return true;
        }
    }
}
