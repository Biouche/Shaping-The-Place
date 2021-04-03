
using UnityEngine;

namespace Splitter
{
    public class CameraController : MonoBehaviour
    {

        public float _mouseWheelZoomSpeed = 1f;
        public TrailHandler TrailHandler;
        // Use this for initialization
        void Start()
        {
        }

        public void ZoomIn()
        {
            if (transform.localPosition.z <= -1)
            {
                transform.localPosition += transform.forward * _mouseWheelZoomSpeed;
                TrailHandler.GetTrailRenderer().Clear();
                TrailHandler.transform.position = new Vector3(0, 0, Camera.main.transform.position.z + 1);
            }

        }

        public void ZoomOut()
        {
            transform.localPosition -= transform.forward * _mouseWheelZoomSpeed;
            TrailHandler.GetTrailRenderer().Clear();
            TrailHandler.transform.position = new Vector3(0, 0, Camera.main.transform.position.z + 1);
        }
    }
}