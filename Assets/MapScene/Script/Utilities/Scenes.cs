using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scenes : MonoBehaviour
{
    public static Scenes instance;
    private static Dictionary<string, GameObject> goParameters = new Dictionary<string, GameObject>();
    private static Dictionary<string, string> stringParameters = new Dictionary<string, string>();
    private static User connectedUser;
    private static Project selectedProject;
    private static Proposal selectedProposal;

    private static bool isNewProposal = false;

    public static Vector3 PreviousPos;
    public static Vector3 PreviousScale;

    public static string LoadProposal = "load_proposal";
    public static string LoadProject = "load_project";
    public static string CreateProject = "new_project";
    public static string LandingPage = "premiere_page";
    public static string CreateUser = "detail_user";
    public static string Login = "connection";
    public static string Explorer = "Explorer";
    public static string EditObject = "EditObject";

    private static string currentScene = LandingPage;
    private static string lastScene = LandingPage;


    private void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public static void CloseScene(string sceneName)
    {
        SceneManager.UnloadSceneAsync(sceneName);
    }

    public static void Load(string sceneName)
    {
        foreach (GameObject go in Scenes.GetGOParameters().Values)
        {
            go.transform.parent = GameObject.Find("Dictionary").transform;
        }
        lastScene = currentScene;
        currentScene = sceneName;
        Debug.Log("Loading Scene : " + sceneName);
        SceneManager.LoadScene(sceneName);
    }

    public static void LoadAdditive(string sceneName)
    {
        foreach (GameObject go in Scenes.GetGOParameters().Values)
            go.transform.parent = GameObject.Find("Dictionary").transform;
        lastScene = currentScene;
        currentScene = sceneName;
        Debug.Log("Loading Scene : " + sceneName);
        SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
    }

    public static void SetActiveScene(string sceneName)
    {
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
    }

    //GO params
    public static Dictionary<string, GameObject> GetGOParameters()
    {
        return goParameters;
    }

    public static GameObject GetGOParam(string paramKey)
    {
        if (goParameters == null)
            return null;
        return goParameters[paramKey];
    }

    public static void AddGOParam(string paramKey, GameObject paramValue)
    {
        if (goParameters == null)
            Scenes.goParameters = new Dictionary<string, GameObject>();
        Scenes.goParameters.Add(paramKey, paramValue);
    }

    //String params

    public static string GetStringParam(string paramKey)
    {
        if (stringParameters == null)
            return null;
        return stringParameters[paramKey];
    }

    public static void AddStringParam(string paramKey, string paramValue)
    {
        if (stringParameters == null)
            Scenes.stringParameters = new Dictionary<string, string>();
        Scenes.stringParameters.Add(paramKey, paramValue);
    }

    //Connected User
    public static void SetConnectedUser(User user)
    {
        Scenes.connectedUser = user;
    }

    public static User GetConnectedUser()
    {
        return Scenes.connectedUser;
    }

    //Selected project
    public static void SetSelectedProject(Project project)
    {
        Scenes.selectedProject = project;
    }

    public static Project GetSelectedProject()
    {
        return Scenes.selectedProject;
    }

    //Selected Proposal
    public static void SetSelectedProposal(Proposal proposal)
    {
        Scenes.selectedProposal = proposal;
    }

    public static Proposal GetSelectedProposal()
    {
        return Scenes.selectedProposal;
    }

    public static bool IsNewProposal()
    {
        return Scenes.isNewProposal;
    }
    public static void SetIsNewProposal(bool value)
    {
        Scenes.isNewProposal = value;
    }

    //Last scene
    public static string GetLastScene()
    {
        return lastScene;
    }
}