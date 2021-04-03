using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using Assets.MapScene.Script.Explorer;

public class ScaleButtonsScript : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public char Axis;
    public bool ScaleUp;

    private bool PointerDown = false;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (PointerDown && SelectedElementController.instance.GetSelectedElement() != null)
            UpdateScale();
    }


    public void OnPointerUp(PointerEventData eventData)
    {
        PointerDown = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        PointerDown = true;
    }


    private void UpdateScale()
    {
        Vector3 Vect;
        SelectedElement SelectedElement = SelectedElementController.instance.GetSelectedElement();

        Collider collider = SelectedElement.GetCollider();
        Bounds bounds = collider.bounds;

        switch (Axis)
        {
            case 'X':
                Vect = new Vector3(GetScaleValue(SelectedElement.GetScale().x), 0, 0);
                break;
            case 'Y':
                Vect = new Vector3(0, GetScaleValue(SelectedElement.GetScale().y), 0);
                break;
            case 'Z':
                Vect = new Vector3(0, 0, GetScaleValue(SelectedElement.GetScale().z));
                break;
            default:
                return;
        }

        if (!ScaleUp)
            Vect = -Vect;

        SelectedElement.AddScale(Vect);

        if (bounds.min.y < 0)
            SelectedElementController.instance.GetSelectedElement().StickToTheFloor();
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

        return scaleValue/10;
    }
}
