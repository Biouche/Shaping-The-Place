using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveSun : MonoBehaviour
{
    public Slider slider;
    // Use this for initialization
    void Start()
    {
        slider.value = 60f;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localEulerAngles = new Vector3(slider.value, 0, 0);
    }
}
