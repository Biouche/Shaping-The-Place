using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GesturesController : MonoBehaviour
{
    //Double tap
    int TapCount;
    private readonly float MaxDubbleTapTime = 0.35f;
    float NewTime;

    //Raycast hit
    public float length = 1000f;
    private Plane MapRaycastPlane;

    //Camera zoom
    public float perspectiveZoomSpeed = 0.005f;        // The rate of change of the field of view in perspective mode.
    public Slider zoom_Slider;
    public float m_MinSize = 6.5f;

    //Camera move
    public float _panSpeed = 1f;

    //Selected Element
    private bool SelectedElementTouched = false;
    private float ScaleSpeed = 0.01f;
    private float dist;
    private Vector3 offset;

    //Object positionning
    private static bool ObjectPositionValid = false;
    private Vector3 position;
    public GameObject ElementPositioningPanel;
    public GameObject ElementPositioningPanelErrorText;

    //Split selection
    public GameObject SplitSelectionPanel;
    public GameObject SplitSelectionPanelErrorText;
    public Dictionary<string, Color> SplitSelectionList = new Dictionary<string, Color>();

    //Instance
    public static GesturesController instance;

    private void Awake()
    {
        instance = this;
        TapCount = 0;
        MapRaycastPlane = new Plane(Vector3.up, GameObject.Find("Map").transform.position);
        CalulateNewPan(zoom_Slider.value);
    }

    void Update()
    {
        if (ElementPositioningPanel.activeInHierarchy)
        {
            DetectObjectPositionning();
            CameraZoom();
            CameraMove();
        }
        else if (SplitSelectionPanel.activeInHierarchy)
        {
            SelectAndAddToSplitList();
            CameraZoom();
            CameraMove();
        }
        else
        {
            //Double tap detection
            DetectDoubleTap();

            //Camera gestures
            if (!SelectedElementTouched)
            {
                CameraZoom();
                CameraMove();
                RotateCameraAround();
            }
            //Selected element gestures
            if (SelectedElementController.instance.GetSelectedElement() != null)
            {
                ScaleSelectedElementEvent();
                MoveSelectedElementEvent();
            }
        }
    }


    public bool IsObjectPositionValid()
    {
        return ObjectPositionValid;
    }

    public void ResetObjectPositionValid()
    {
        ObjectPositionValid = false;
        ElementPositioningPanelErrorText.SetActive(false);
    }

    public Vector3 GetPosition()
    {
        return this.position;
    }

    public void DetectObjectPositionning()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Ended)
                TapCount += 1;

            if (TapCount == 1)
                NewTime = Time.time + MaxDubbleTapTime;

            else if (TapCount == 2 && Time.time <= NewTime)
            {
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    ElementPositioningPanelErrorText.SetActive(false);
                    //Position is valid
                    if (hit.collider.gameObject != null && hit.collider.gameObject.GetComponent<SelectableElementController>() == null && ElementPositioningPanel.activeInHierarchy)
                    {
                        Debug.Log(hit.collider.gameObject.name);
                        //Set position
                        this.position = hit.point;
                        ObjectPositionValid = true;
                    }
                    //Position is not valid
                    else
                    {
                        ObjectPositionValid = false;
                        ElementPositioningPanelErrorText.SetActive(true);
                    }
                }
                TapCount = 0;
            }
        }
        //Reset timer
        if (Time.time > NewTime)
        {
            TapCount = 0;
        }
    }

    public void SelectAndAddToSplitList()
    {
        //Double Tap
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Ended)
                TapCount += 1;

            if (TapCount == 1)
                NewTime = Time.time + MaxDubbleTapTime;
            else if (TapCount == 2 && Time.time <= NewTime)
            {
                GameObject hitGO = CheckRaycast(touch.position);
                if (hitGO != null)
                {
                    SelectableElementController SEC = hitGO.GetComponentInParent<SelectableElementController>();
                    if (SEC != null && SEC.IsAlterable())
                    {
                        if (SplitSelectionList.ContainsKey(hitGO.name))
                        {
                            hitGO.GetComponent<MeshRenderer>().material.color = SplitSelectionList[hitGO.name];
                            SplitSelectionList.Remove(hitGO.name);
                        }
                        else
                        {
                            SplitSelectionPanelErrorText.SetActive(false);
                            SplitSelectionList.Add(hitGO.name, hitGO.GetComponent<MeshRenderer>().material.color);
                            hitGO.GetComponent<MeshRenderer>().material.color = Color.red;
                        }
                    }
                    else
                        SplitSelectionPanelErrorText.SetActive(true);
                }
                TapCount = 0;
            }
        }

        //Reset timer
        if (Time.time > NewTime)
        {
            TapCount = 0;
        }
    }

    public void CameraMove()
    {
        if (Input.touchCount == 1 /*&& !IsPointerOverUIObject()*/)
        {
            Touch touch = Input.GetTouch(0);
            // !EventSystem.current.IsPointerOverGameObject(touch.fingerId) checks if touching UI
            if (touch.phase == TouchPhase.Moved)
            {
                Vector3 transVect = new Vector3(-touch.deltaPosition.x * _panSpeed * 2f, 0f, -touch.deltaPosition.y * _panSpeed * 2f);
                Camera.main.transform.Translate(transVect, Space.Self);
            }
        }
    }

    private readonly float MIN_CAMERA_ANGLE = 5f;
    private readonly float MAX_CAMERA_ANGLE = 80f;

    public void RotateCameraAround()
    {
        if (Input.touchCount == 3 && !IsPointerOverUIObject())
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Moved)
            {
                //Rotate around
                Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                    Camera.main.transform.RotateAround(hit.point, Vector3.up, touch.deltaPosition.x * _panSpeed * 1.5f);
                //TILT
                float rotationX = Camera.main.transform.eulerAngles.x;
                //GESTURE GOES UP, ANGLE GOES DOWN
                if (touch.deltaPosition.y > 0)
                    rotationX -= touch.deltaPosition.y * _panSpeed * 1.5f;
                //GESTURE GOES DOWN, ANGLE GOES UP
                else if (touch.deltaPosition.y < 0)
                    rotationX += -touch.deltaPosition.y * _panSpeed * 1.5f;

                rotationX = Mathf.Clamp(rotationX, MIN_CAMERA_ANGLE, MAX_CAMERA_ANGLE);
                Camera.main.transform.eulerAngles = new Vector3(rotationX, Camera.main.transform.eulerAngles.y, Camera.main.transform.eulerAngles.z);
            }
        }
    }

    public void CameraZoom()
    {
        float size = zoom_Slider.value;
        // If there are two touches on the device...
        if (Input.touchCount == 2)
        {
            // Store both touches.
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            // Find the position in the previous frame of each touch.
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            // Find the magnitude of the vector (the distance) between the touches in each frame.
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            // Find the difference in the distances between each frame.
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;


            // Otherwise change the field of view based on the change in distance between the touches.
            size += deltaMagnitudeDiff * _panSpeed;

            // Clamp the field of view to make sure it's between 0 and 180.
            //camera.fieldOfView = Mathf.Clamp(size, 0.1f, 179.9f);

            size = Mathf.Max(size, m_MinSize);
            size = Mathf.Min(size, zoom_Slider.maxValue);

            zoom_Slider.value = size;


            CalulateNewPan(size);
        }
    }

    public void CalulateNewPan(float amount)
    {
        _panSpeed = amount / zoom_Slider.maxValue;
    }

    public void DetectDoubleTap()
    {
        //Double Tap
        if (Input.touchCount == 1 && !IsPointerOverUIObject())
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Ended)
                TapCount += 1;

            if (TapCount == 1)
                NewTime = Time.time + MaxDubbleTapTime;

            else if (TapCount == 2 && Time.time <= NewTime)
            {
                GameObject hitGO = CheckRaycast(touch.position);
                if (hitGO != null)
                    ToggleSelectedElement(hitGO);
                TapCount = 0;
            }
        }
        //Double click
        /*else if (Input.GetMouseButtonDown(0))
        {
            TapCount += 1;
            if (TapCount == 1)
                NewTime = Time.time + MaxDubbleTapTime;
            else if (TapCount == 2 && Time.time <= NewTime)
            {
                GameObject hitGO = CheckRaycast(Input.mousePosition);
                if (hitGO != null && hitGO.tag == "AdditionalElements")
                    ToggleSelectedElement(hitGO);

                TapCount = 0;
            }
        }*/
        //Reset timer
        if (Time.time > NewTime)
        {
            TapCount = 0;
        }
    }

    //Move
    public void MoveSelectedElementEvent()
    {
        if (Input.touchCount == 1 /*&& !IsPointerOverUIObject()*/)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (CheckTouchedGO(hit.collider.gameObject))
                    {
                        this.SelectedElementTouched = true;

                        Ray ray2 = Camera.main.ScreenPointToRay(touch.position);
                        MapRaycastPlane.Raycast(ray2, out dist);
                        offset = hit.transform.position - ray2.GetPoint(dist);
                        offset = new Vector3(offset.x, 0, offset.z);
                    }
                }
            }
            if (touch.phase == TouchPhase.Moved)
            {
                if (this.SelectedElementTouched == true)
                {
                    Ray ray = Camera.main.ScreenPointToRay(touch.position);
                    MapRaycastPlane.Raycast(ray, out dist);
                    Vector3 v3Pos = ray.GetPoint(dist);
                    v3Pos = new Vector3(v3Pos.x, SelectedElementController.instance.GetSelectedElement().GetCurrentPosition().y, v3Pos.z);
                    SelectedElementController.instance.GetSelectedElement().SetCurrentPosition(v3Pos + offset);
                    //Reset Tap Count (object selection/unselection)
                    TapCount = 0;
                }
            }
            if (touch.phase == TouchPhase.Ended)
                this.SelectedElementTouched = false;
        }
    }

    private float GetScaleValue(float scale)
    {
        float scaleValue = 0f;
        if (scale >= 10)
            scaleValue = 1f;
        else if (scale > 5 && scale < 10)
            scaleValue = 0.5f;
        else if (scale > 1 && scale < 5)
            scaleValue = 0.1f;
        else if (scale > 0.5 && scale < 1)
            scaleValue = 0.05f;
        else if (scale > 0.1 && scale < 0.5)
            scaleValue = 0.01f;
        else if (scale > 0.05 && scale < 0.1)
            scaleValue = 0.005f;
        else
            scaleValue = 0.001f;

        return scaleValue;
    }

    //Scale
    public void ScaleSelectedElementEvent()
    {
        if (Input.touchCount == 2 && !IsPointerOverUIObject())
        {
            // Store both touches.
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            if (touchZero.phase == TouchPhase.Began)
            {
                Ray ray = Camera.main.ScreenPointToRay(touchZero.position);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (CheckTouchedGO(hit.collider.gameObject))
                        this.SelectedElementTouched = true;
                }
            }

            if (this.SelectedElementTouched == true)
            {
                //ScaleSpeed = GetScaleValue(ElementMenuController.GetSelectedElement().GetComponentInChildren<BoxCollider>().size.magnitude);
                // Find the position in the previous frame of each touch.
                Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                // Find the magnitude of the vector (the distance) between the touches in each frame.
                float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

                // Find the difference in the distances between each frame.
                float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
                if (SelectedElementController.instance.GetSelectedElement().GetScale().x < 0.01)
                    SelectedElementController.instance.GetSelectedElement().AddScale(new Vector3(0.01f, 0.01f, 0.01f));
                else
                    SelectedElementController.instance.GetSelectedElement().AddScale(new Vector3(-deltaMagnitudeDiff, -deltaMagnitudeDiff, -deltaMagnitudeDiff) * ScaleSpeed);
            }

            if (touchZero.phase == TouchPhase.Ended || touchOne.phase == TouchPhase.Ended)
                this.SelectedElementTouched = false;
        }
    }

    private void ToggleSelectedElement(GameObject go)
    {
        SelectableElementController SEC;
        GameObject selectedElement;
        //Additional element
        /*if (go.GetComponent<SelectableElementController>() == null)
        {*/
        // Get script
        SEC = go.GetComponentInParent<SelectableElementController>();
        // Get Container
        selectedElement = go.transform.parent.gameObject;
        //}
        //Building
        /*else
        {
            SEC = go.GetComponent<SelectableElementController>();
            selectedElement = go;
        }*/

        if (SEC != null)
        {
            if (SelectedElementController.instance.GetSelectedElement() == null)
                SelectedElementController.instance.SetSelectedElement(selectedElement);
            else
            {
                SelectedElementController.instance.UnSelectElement();
                this.SelectedElementTouched = false;
            }
        }
        else
            SelectedElementController.instance.UnSelectElement();
    }

    private GameObject CheckRaycast(Vector3 position)
    {
        Ray ray = Camera.main.ScreenPointToRay(position);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            return hit.collider.gameObject;
        }
        return null;
    }

    private bool CheckTouchedGO(GameObject GO)
    {
        if (GO == SelectedElementController.instance.GetSelectedElement().GetContainer() || GO.transform.parent.gameObject == SelectedElementController.instance.GetSelectedElement().GetContainer())
            return true;
        return false;
    }

    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current)
        {
            position = new Vector2(Input.touches[0].position.x, Input.touches[0].position.y)
        };
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
}
