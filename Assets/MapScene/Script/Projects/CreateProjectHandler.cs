using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CreateProjectHandler : MonoBehaviour
{

    public InputField description;
    public InputField country;
    public InputField projectName;
    public InputField place;
    public InputField coordinates;

    public Text error;


    public void CreateProject()
    {
        string date = Convert.ToDateTime(DateTime.Now).ToString("dd/MM/yyyy");
        DataService.CreateProject(date, projectName.text, coordinates.text, description.text);
        Scenes.Load(Scenes.LoadProject);
    }

    public void ValidateProjectFile()
    {
        string path = "./Assets/Data/Projects/" + projectName.text + ".txt";
        string pathFolder = "./Assets/Data/Projects/" + projectName.text;
        string infoProject;

        if (File.Exists(path))
            error.text = "An error occured, please change your project name";
        else
        {
            Directory.CreateDirectory(pathFolder);

            infoProject = projectName.text + "\r\n" + country.text + "\r\n" + description.text + "\r\n" + place.text + "\r\n" + coordinates.text;

            StreamWriter sw = new StreamWriter(path);
            sw.WriteLine(infoProject);
            sw.Close();

            Scenes.Load(Scenes.LoadProject);
        }

    }
}

