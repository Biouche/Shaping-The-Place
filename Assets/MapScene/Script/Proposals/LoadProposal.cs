using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.IO;
using Mapbox.Unity.Utilities;
using System;

public class LoadProposal : MonoBehaviour
{
    public GameObject proposalsGrid;
    public static LoadProposal instance;

    // Use this for initialization
    void Start()
    {
        //Load proposals from db
        LoadProposalsFromDB();
    }

    private void LoadProposalsFromDB()
    {
        List<Proposal> UserProposals = DataService.GetAllUserProposals(Scenes.GetConnectedUser().Id, Scenes.GetSelectedProject().Id);
        foreach (Proposal proposal in UserProposals)
        {
            CreateGridElement(proposal);
        }
    }

    private void CreateGridElement(Proposal proposal)
    {
        // image projet 
        GameObject go = new GameObject(proposal.date);
        Image img = go.AddComponent<Image>() as Image;
        img.type = Image.Type.Sliced;
        go.SetActive(true);
        go.transform.SetParent(proposalsGrid.transform);
        LoadProposalHandler handler = go.AddComponent<LoadProposalHandler>();
        handler.SetCurrentProposal(proposal);
        img.sprite = Resources.Load<Sprite>("ville");
        //titre projet
        GameObject title = new GameObject(proposal.date + "-Title");
        Text TitleProposal = title.AddComponent<Text>() as Text;
        title.transform.SetParent(go.transform);

        TitleProposal.text = proposal.date;
        TitleProposal.color = Color.black;
        TitleProposal.fontSize = 20;
        TitleProposal.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        TitleProposal.alignment = TextAnchor.MiddleCenter;
        TitleProposal.transform.position += new Vector3(0, -60f);
        title.SetActive(true);

    }

    public void NewProposal()
    {
        string date = Convert.ToDateTime(DateTime.Now).ToString("dd/MM/yyyy HH:mm:ss");
        Proposal newProposal = DataService.CreateProposal(date, null, Scenes.GetConnectedUser().Id, Scenes.GetSelectedProject().Id);
        Scenes.SetSelectedProposal(newProposal);
        Scenes.SetIsNewProposal(true);
        Scenes.Load(Scenes.Explorer);
    }


}
