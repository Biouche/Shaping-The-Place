using UnityEngine;
using UnityEngine.UI;

namespace Splitter
{
    public class TrailHandler : MonoBehaviour
    {

        private TrailRenderer trailRenderer;
        public Toggle CutToggle;
        private Text errorMesage;

        // Use this for initialization
        void Start()
        {
            trailRenderer = this.GetComponentInChildren<TrailRenderer>();
            trailRenderer.enabled = false;
            errorMesage = GameObject.Find("ErrorMessage").GetComponent<Text>();

            //Move trailrenderer
            this.transform.position = new Vector3(0, 0, -GameObject.Find("ObjectContainer").GetComponentInChildren<BoxCollider>().bounds.max.z - 1f);
        }

        public TrailRenderer GetTrailRenderer()
        {
            return this.trailRenderer;
        }

        // Update is called once per frame
        void Update()
        {
            if (CutToggle.isOn && !(GesturesHandler.fusionSelection || GesturesHandler.deleteSelection))
                HandleTrail();
        }

        private void HandleTrail()
        {
            trailRenderer.enabled = false;
            if (((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved) ||
                    Input.GetMouseButton(0)))
            {
                trailRenderer.enabled = true;
                Plane objPlane = new Plane(Camera.main.transform.forward * -1, this.transform.position);
                Ray mRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                float rayDistance;
                if (objPlane.Raycast(mRay, out rayDistance))
                    this.transform.position = mRay.GetPoint(rayDistance);
            }
            else if (Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended))
            {
                // If more than two points & trail size is larger than the mesh && ! self intersect line
                if (TrailIsValid())
                {
                    Vector3[] positions = new Vector3[trailRenderer.positionCount];
                    trailRenderer.GetPositions(positions);
                    SceneManager.instance.HandleCut(positions);
                }
                trailRenderer.Clear();
            }
        }

        private bool TrailIsValid()
        {
            // Trail is too short
            if (trailRenderer.positionCount < 3)
                errorMesage.text = "Trail is too short";
            else if (IsSelfIntersect())
                errorMesage.text = "Trail intersects itself.";
            else
            {
                errorMesage.text = "";
                return true;
            }
            return false;
        }

        private bool IsSelfIntersect()
        {
            Vector3[] positions = new Vector3[trailRenderer.positionCount];
            trailRenderer.GetPositions(positions);
            for (int i = 0; i < positions.Length; ++i)
            {
                for (int j = 0; j < positions.Length; ++j)
                {
                    if (j != i)
                    {
                        float dx = positions[i].x - positions[j].x;
                        float dy = positions[i].y - positions[j].y;
                        if (Mathf.Abs(dx) < 0.008 && Mathf.Abs(dy) < 0.008 && Mathf.Abs(i - j) > 5)
                            return true;
                    }
                }
            }
            return false;
        }
    }
}