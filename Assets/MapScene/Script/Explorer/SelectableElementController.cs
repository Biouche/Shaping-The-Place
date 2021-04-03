using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectableElementController : MonoBehaviour
{
    private bool isAlterable = false;
    private bool isBuilding = false;

    public bool IsAlterable()
    {
        return this.isAlterable;
    }

    public void SetAlterable(bool value)
    {
        this.isAlterable = value;
    }

    public bool IsBuilding()
    {
        return this.isBuilding;
    }
    public void SetBuilding(bool value)
    {
        this.isBuilding = value;
    }
}
