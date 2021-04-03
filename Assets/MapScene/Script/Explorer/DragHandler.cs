using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class DragHandler : MonoBehaviour
{

    public float DragSpeed = 20f;

  
    private void MoveObject(object sender, System.EventArgs e)
    {
        //Vector3 newVect = new Vector3(-translationGesture.DeltaPosition.x, -translationGesture.DeltaPosition.y)* DragSpeed;
        //this.transform.parent.localPosition += newVect;
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
