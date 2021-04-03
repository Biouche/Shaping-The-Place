using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using Assets.MapScene.Script.Explorer;


public class MoveButtonsScript : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public char Axis;
    public bool MoveUp;

    private bool PointerDown = false;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (PointerDown && SelectedElementController.instance.GetSelectedElement() != null)
            UpdatePosition();
    }


    public void OnPointerUp(PointerEventData eventData)
    {
        PointerDown = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        PointerDown = true;
    }


    private void UpdatePosition()
    {
        Vector3 Vect;
        SelectedElement SelectedElement = SelectedElementController.instance.GetSelectedElement();

        Collider collider = SelectedElement.GetCollider();
        Bounds bounds = collider.bounds;

        switch (Axis)
        {
            case 'X':
                Vect = Vector3.right;
                break;
            case 'Y':
                Vect = Vector3.up;
                break;
            case 'Z':
                Vect = Vector3.forward;
                break;
            default:
                return;
        }

        if (!MoveUp)
            Vect = -Vect;

        SelectedElement.AddPosition(Vect);

        if (bounds.min.y < 0)
            SelectedElementController.instance.GetSelectedElement().StickToTheFloor();
    }
}
