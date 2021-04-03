using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapCanvasController : MonoBehaviour
{
    //Rotation sliders
    public Slider slideX;
    public Slider slideY;
    public Slider slideZ;

    //Scale buttons
    /*public Button ScaleXUpButton;
    public Button ScaleXDownButton;
    public Button ScaleYUpButton;
    public Button ScaleYDownButton;
    public Button ScaleZUpButton;
    public Button ScaleZDownButton;*/


    public GameObject SplitSelectionPanel;
    public GameObject UIContainer;
    public GameObject OpenUI;

    private float previousValueX;
    private float previousValueY;
    private float previousValueZ;

    public static MapCanvasController instance;

    // Use this for initialization
    void Start()
    {
        instance = this;
        //Rotation slider init
        this.previousValueX = 0;
        this.previousValueY = 0;
        this.previousValueZ = 0;
    }

    public void Slide(int value)
    {
        if (SelectedElementController.instance.GetSelectedElement() != null)
        {
            Vector3 rotationVector = Vector3.zero;
            float delta;
            //ROTATE X
            if (value == 0)
            {
                delta = slideX.value - this.previousValueX;
                rotationVector = Vector3.right * delta * 360;
                this.previousValueX = slideX.value;
            }
            // ROTATE Y
            else if (value == 1)
            {
                delta = slideY.value - this.previousValueY;
                rotationVector = Vector3.up * delta * 360;
                this.previousValueY = slideY.value;
            }
            // ROTATE Z
            else if (value == 2)
            {
                delta = slideZ.value - this.previousValueZ;
                rotationVector = Vector3.forward * delta * 360;
                this.previousValueZ = slideZ.value;
            }

            SelectedElementController.instance.GetSelectedElement().GetContainer().transform.Rotate(rotationVector, Space.World);
        }
    }

    public void SetVisible(bool value)
    {
        if (!value)
            this.UIContainer.SetActive(value);

        this.OpenUI.SetActive(value);
        //Reset sliders values
        if (!value)
        {
            this.previousValueX = this.slideX.value = 0;
            this.previousValueY = this.slideY.value = 0;
            this.previousValueZ = this.slideZ.value = 0;
        }
    }

    public void OnClickCancelPositionning()
    {
        GameObject.Find("ElementPositioningPanel").SetActive(false);
        GesturesController.instance.ResetObjectPositionValid();
    }

    public void OnClickEditMultiple()
    {
        Scenes.GetGOParameters().Clear();
        SplitSelectionPanel.SetActive(true);
    }

    public void OnClickConfirmEditMultiple()
    {
        SplitSelectionPanel.SetActive(false);
        if (GesturesController.instance.SplitSelectionList.Count > 0)
        {
            foreach (String goName in GesturesController.instance.SplitSelectionList.Keys)
            {
                Scenes.PreviousPos = GameObject.Find(goName).transform.position;
                Scenes.PreviousScale = GameObject.Find(goName).transform.localScale;
                Scenes.AddGOParam(goName, GameObject.Find(goName));
            }
            Scenes.LoadAdditive(Scenes.EditObject);
        }
    }

    public void OnClickCancelEditMultiple()
    {
        SplitSelectionPanel.SetActive(false);
        GesturesController.instance.SplitSelectionList.Clear();
    }

    public void StickToTheFloorEvent()
    {
        SelectedElementController.instance.GetSelectedElement().StickToTheFloor();
    }
    
    public void OnClickCloseUIEvent()
    {
        this.UIContainer.SetActive(false);
        this.OpenUI.SetActive(true);
    }

    public void OnClickOpenUIEvent()
    {
        this.UIContainer.SetActive(true);
        this.OpenUI.SetActive(false);
    }
}

