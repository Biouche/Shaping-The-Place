using UnityEngine;
using System.Collections;

public class CreateDB : MonoBehaviour
{

    void Start()
    {
        StartSync();
    }

    private void StartSync()
    {
        DataService.CreateDB();
    }
}
