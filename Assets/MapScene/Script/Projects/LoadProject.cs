using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.IO;

public class LoadProject : MonoBehaviour
{

    public const string ProjectPath = "./Assets/Data/Projects";
    public GameObject projectGrid;

    // Use this for initialization
    void Start()
    {
        //LoadProjectsFromFiles();
        //Reset selected project
        Scenes.SetSelectedProject(null);
        LoadProjectsFromDB();
    }

    public void LoadProjectsFromDB()
    {
        //Get existing projects from DB
        List<Project> allProjects = DataService.GetAllProjects();

        //Get projects current user participated to
        DataService.GetAllUserProjects(Scenes.GetConnectedUser().Id);

        //Create grid
        foreach (Project project in allProjects)
        {
            CreateGridElement(project);
        }
    }

    private void CreateGridElement(Project project)
    {
        // image projet 
        GameObject go = new GameObject(project.Name);
        Image img = go.AddComponent<Image>() as Image;
        LoadProjectHandler loadProjectHandler = go.AddComponent<LoadProjectHandler>();
        loadProjectHandler.SetProject(project);

        img.type = Image.Type.Sliced;

        go.SetActive(true);
        go.transform.SetParent(projectGrid.transform);

        // img.sprite = Sprite.Create(image, new Rect(0, 0, image.width, image.height), new Vector2(0.5f, 0.5f));
        img.sprite = Resources.Load<Sprite>("ville");

        //titre projet
        GameObject title = new GameObject(project.Name + "-Title");
        Text TitleProject = title.AddComponent<Text>() as Text;
        title.transform.SetParent(go.transform);
        TitleProject.text = project.Name;
        TitleProject.color = Color.black;
        TitleProject.fontSize = 20;
        TitleProject.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
       
        ContentSizeFitter csf = title.AddComponent<ContentSizeFitter>();
        csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        csf.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        title.SetActive(true);
        title.transform.position += new Vector3(0, -LayoutUtility.GetPreferredHeight(TitleProject.rectTransform) - 50);
    }

    public void CreateProject()
    {
        Scenes.Load(Scenes.CreateProject);
    }

    public void LoadProjectsFromFiles()
    {
        //for each file.txt create a GridElement
        foreach (string FileName in System.IO.Directory.GetFiles(ProjectPath))
        {
            if (System.IO.Path.GetExtension(FileName) == ".txt")
            {
                CreateGridElementFiles(projectGrid, System.IO.Path.GetFileNameWithoutExtension(FileName), ProjectPath);
            }
        }
    }

    private void CreateGridElementFiles(GameObject elementGrid, string FileName, string path)
    {
        // image projet 
        GameObject go = new GameObject(FileName);
        Image img = go.AddComponent<Image>() as Image;
        go.AddComponent<LoadProjectHandler>();

        img.type = Image.Type.Sliced;

        Texture2D image = new Texture2D(100, 100);
        image.LoadImage(File.ReadAllBytes("./Assets/Resources/ville.jpg"));

        go.SetActive(true);
        go.transform.SetParent(elementGrid.transform);

        img.sprite = Sprite.Create(image, new Rect(0, 0, image.width, image.height), new Vector2(0.5f, 0.5f));


        //titre projet
        GameObject title = new GameObject(FileName);
        Text TitleProject = title.AddComponent<Text>() as Text;
        title.transform.SetParent(go.transform);
        string InputPath = "./Assets/Data/Projects/" + FileName + ".txt";

        TitleProject.color = Color.black;
        TitleProject.fontSize = 20;
        TitleProject.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        TitleProject.alignment = TextAnchor.MiddleCenter;
        TitleProject.transform.position += new Vector3(0, -70f);
        title.SetActive(true);


        if (File.Exists(InputPath))
        {
            StreamReader reader = new StreamReader(InputPath);
            TitleProject.text = reader.ReadLine();
            reader.Close();
        }
    }
}
