using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.IO;
using UnityEngine.SceneManagement;

public class LoadProjectHandler : MonoBehaviour, IPointerDownHandler
{
    private Project currentProject;

    public void SetProject(Project currentProject)
    {
        this.currentProject = currentProject;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        Scenes.SetSelectedProject(currentProject);
        Scenes.Load(Scenes.LoadProposal);
    }
}
