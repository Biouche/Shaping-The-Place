using UnityEngine;
using Mapbox.Unity.Map;
using UnityEngine.UI;
using System.Collections;

public class CameraController : MonoBehaviour
{
    public float _mouseWheelZoomSpeed = 100f;
    private Vector3 initialPosition;
    public float m_MinSize = 6.5f;
    public Slider slider_zoom;
    private float initialSliderZoom;
    private float m_DampTime = 0.05f;
    public Camera Cam;
    public static CameraController instance;

    private Vector3 m_MoveVelocity;
    private float m_ZoomSpeed;

	public float zoom_movement;

    private bool animating = false;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
		initialPosition = new Vector3(0f, slider_zoom.value, -50f);
        initialSliderZoom = slider_zoom.value;
        animating = false;

        if (Display.displays.Length > 1)
            Display.displays[1].Activate();
        if (Display.displays.Length > 2)
            Display.displays[2].Activate();
    }

    private void Update()
    {
		MoveCameraAxis ();
        //MouseWheelZoom();
    }

    public void MouseWheelZoom()
    {

        float size = slider_zoom.value;

        size -= Input.GetAxis("Mouse ScrollWheel") * _mouseWheelZoomSpeed;
        // Make sure the camera's size isn't below the minimum.
        size = Mathf.Max(size, m_MinSize);
        size = Mathf.Min(size, slider_zoom.maxValue);

        slider_zoom.value = size;

    }

	public void MoveCameraAxis()
	{
		Cam.transform.position = new Vector3 (Cam.transform.position.x, slider_zoom.value, Cam.transform.position.z);
	}

    public void initializeCamera()
    {
        if (!animating)
        {
            animating = true;
            slider_zoom.interactable = false;
            StartCoroutine(AnimCamera(initialPosition, initialSliderZoom));
        }

        //slider_zoom.value = initialSliderZoom;
    }

    public void moveCameraTo(GameObject Target)
    {
        Vector3 targetPos = Target.GetComponent<Collider>().bounds.center;

        /*Vector3 xyz = Target.GetComponent<Collider>().bounds.size;
        float distance = Mathf.Max(xyz.x, xyz.y, xyz.z);
        distance /= (2.0f * Mathf.Tan(0.5f * Cam.fieldOfView * Mathf.Deg2Rad));*/

        Vector3 objectSizes = Target.GetComponent<Collider>().bounds.max - Target.GetComponent<Collider>().bounds.min;
        float objectSize = Mathf.Max(objectSizes.x, objectSizes.y, objectSizes.z);
        float cameraView = 2.0f * Mathf.Tan(0.5f * Mathf.Deg2Rad * Cam.fieldOfView); // Visible height 1 meter in front
        float distance = 2.0f * objectSize / cameraView; // Combined wanted distance from the object
        distance += 0.5f * objectSize; // Estimated offset from the center to the outside of the object



        Vector3 newSpot = targetPos + (-Cam.transform.forward.normalized * distance);

        targetPos = newSpot;

        if (!animating)
        {
            animating = true;
            slider_zoom.interactable = false;
            StartCoroutine(AnimCamera(targetPos, targetPos.y));
        }

        //slider_zoom.value = initialSliderZoom;


    }

    public IEnumerator AnimCamera(Vector3 position, float zoom)
    {
        float Distance = Mathf.Abs((Cam.transform.position - position).magnitude);



        while (Distance > 1)
        {

            Distance = Mathf.Abs((Cam.transform.position - position).magnitude);

			Cam.transform.position = Vector3.SmoothDamp(new Vector3(Cam.transform.position.x, slider_zoom.value, Cam.transform.position.z), position, ref m_MoveVelocity, m_DampTime);

            slider_zoom.value = Mathf.SmoothDamp(slider_zoom.value, zoom, ref m_ZoomSpeed, m_DampTime);

            yield return new WaitForSeconds(0.01f);

        }

		GesturesController.instance.CalulateNewPan (slider_zoom.value);
        slider_zoom.interactable = true;
        animating = false;
    }

}
