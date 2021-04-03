using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.IO;
using UnityEngine.SceneManagement;

public class LoadProposalHandler : MonoBehaviour, IPointerDownHandler
{
    private Proposal currentProposal;

    public void SetCurrentProposal(Proposal proposal)
    {
        this.currentProposal = proposal;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        Scenes.SetSelectedProposal(currentProposal);
        Scenes.SetIsNewProposal(false);
        Scenes.Load(Scenes.Explorer);
    }
}
