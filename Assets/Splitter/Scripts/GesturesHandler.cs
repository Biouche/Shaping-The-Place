using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GesturesHandler : MonoBehaviour
{
    public Toggle rotationToggle;
    public Toggle cutToggle;
    public Text ErrorMessage;
    public float MoveSpeed = 0.08f;
    public GameObject Plane;

    private float dist;
    private Vector3 offset;
    private RaycastHit hit;
    private bool ElementTouched = false;

    private int TapCount;
    private readonly float MaxDubbleTapTime = 0.35f;
    private float NewTime;

    public static bool fusionSelection = false;
    public static bool deleteSelection = false;



    // Use this for initialization
    void Start()
    {
        cutToggle.onValueChanged.AddListener(OnToggleValueChanged);
        TapCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (fusionSelection)
        {
            SelectObjectFusion();
        }
        else if (deleteSelection)
        {
            SelectObjectDelete();
        }
        else if (rotationToggle.isOn)
        {
            RotateElementEvent();
            MoveElementEvent();
        }
    }

    private void RotateElementEvent()
    {
        if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);
            if (touchZero.phase == TouchPhase.Began)
            {
                Ray ray = Camera.main.ScreenPointToRay(touchZero.position);
                Physics.Raycast(ray, out hit);
                ElementTouched = true;
            }
            if (touchZero.phase == TouchPhase.Moved)
            {
                if (hit.collider.gameObject != null && hit.collider.gameObject != Plane && ElementTouched)
                {
                    Vector3 rotationVector = new Vector3(touchZero.deltaPosition.y, -touchZero.deltaPosition.x) * 0.5f;
                    hit.collider.transform.parent.Rotate(rotationVector, Space.World);
                }
            }

            if (touchZero.phase == TouchPhase.Ended || touchOne.phase == TouchPhase.Ended)
                ElementTouched = false;
        }
    }

    private void MoveElementEvent()
    {
        Vector3 v3;
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform.tag == "ObjectToCut")
                    {
                        dist = hit.transform.position.z - Camera.main.transform.position.z;
                        v3 = new Vector3(touch.position.x, touch.position.y, dist);
                        v3 = Camera.main.ScreenToWorldPoint(v3);
                        offset = hit.transform.parent.transform.position - v3;
                        ElementTouched = true;
                    }
                }
            }
            if (touch.phase == TouchPhase.Moved)
            {
                if (hit.collider.gameObject != null && hit.collider.gameObject != Plane && ElementTouched)
                {
                    v3 = new Vector3(Input.mousePosition.x, Input.mousePosition.y, dist);
                    v3 = Camera.main.ScreenToWorldPoint(v3);
                    hit.transform.parent.transform.position = v3 + offset;
                }
            }
            if (touch.phase == TouchPhase.Ended)
                ElementTouched = false;
        }
    }


    private void SelectAndAddToList(SortedList<string, Color> list)
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
                Physics.Raycast(ray, out hit);
                if (hit.collider.gameObject != null && hit.collider.gameObject != Plane)
                {
                    GameObject SelectedGameObject = hit.collider.gameObject;

                    if (list.ContainsKey(SelectedGameObject.name))
                    {
                        Debug.Log("Removing from List : " + SelectedGameObject.name);
                        Material mat = SelectedGameObject.GetComponentInChildren<MeshRenderer>().materials[0];
                        mat.color = list[SelectedGameObject.name];
                        list.Remove(SelectedGameObject.name);
                    }
                    else
                    {
                        
                        Material mat = SelectedGameObject.GetComponentInChildren<MeshRenderer>().materials[0];
                        Debug.Log("Adding to List : " + SelectedGameObject.name + " Color : " + mat.color);
                        list.Add(SelectedGameObject.name, mat.color);
                        //Change color
                        mat.color = Color.red;
                    }
                }
                TapCount = 0;
            }
        }
        //Reset timer
        if (Time.time > NewTime)
            TapCount = 0;
    }

    private void SelectObjectDelete()
    {
        SelectAndAddToList(InteractionsController.instance.GetDeleteList());
    }

    public static void SetDeleteSelection(bool value)
    {
        deleteSelection = value;
    }

    private void SelectObjectFusion()
    {
        SelectAndAddToList(InteractionsController.instance.GetFusionList());
    }

    public static void SetFusionSelection(bool value)
    {
        fusionSelection = value;
    }

    private void OnToggleValueChanged(bool isOn)
    {
        //Change colors
        ColorBlock cbCut = cutToggle.colors;
        ColorBlock cbRotate = rotationToggle.colors;
        if (isOn)
        {
            //Cut On
            cbCut.normalColor = Color.red;
            cbCut.highlightedColor = Color.red;
            //rotate Off
            cbRotate.normalColor = Color.white;
            cbRotate.highlightedColor = Color.red;
        }
        else
        {
            //Rotate On
            cbRotate.normalColor = Color.red;
            cbRotate.highlightedColor = Color.red;
            //Cut Off
            cbCut.normalColor = Color.white;
            cbCut.highlightedColor = Color.red;


        }
        cutToggle.colors = cbCut;
        rotationToggle.colors = cbRotate;
        //Empty error message
        ErrorMessage.text = "";
    }
}
