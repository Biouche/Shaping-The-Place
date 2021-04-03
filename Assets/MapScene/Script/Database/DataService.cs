using SQLite4Unity3d;
using UnityEngine;
#if !UNITY_EDITOR
using System.Collections;
using System.IO;
#endif
using System.Collections.Generic;
using System.Linq;
using System;

public class DataService : MonoBehaviour
{

    private static SQLiteConnection _connection;

    public static DataService instance;

    private const string DatabaseName = "database.db";

    void Awake()
    {
        instance = this;
        Init();
        CreateDB();
    }

    private void Init()
    {
        var dbPath = Application.streamingAssetsPath + '/' + DatabaseName;
        _connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
        Debug.Log("DB PATH: " + dbPath);
    }

    public static void CreateDB()
    {
        string createrProjectStr = @"CREATE TABLE IF NOT EXISTS Project(
                            id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                            creation_date TEXT NOT NULL,
                            name TEXT NOT NULL,
                            coordinates TEXT NOT NULL,
                            description TEXT
                        )";
        _connection.Execute(createrProjectStr);

        string createProposalsStr = @"CREATE TABLE IF NOT EXISTS Proposal(
                            id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                            date TEXT NOT NULL,
                            file BLOB,
                            id_user INTEGER NOT NULL,
                            id_project INTEGER NOT NULL,
                            FOREIGN KEY(id_project) REFERENCES Project(id),
                            FOREIGN KEY(id_user) REFERENCES User(id)
                        );";
        _connection.Execute(createProposalsStr);

        string createUserStr = @"CREATE TABLE IF NOT EXISTS User(
                            id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                            username TEXT NOT NULL UNIQUE,
                            password TEXT NOT NULL,
                            email TEXT NOT NULL UNIQUE,
                            firstname TEXT NOT NULL,
                            lastname TEXT NOT NULL,
                            city TEXT,
                            type INTEGER
                        );";
        _connection.Execute(createUserStr);

        string createProjectUserAssociationStr = @"CREATE TABLE IF NOT EXISTS UserProjectAssociation(id_user INTEGER NOT NULL,
                                    id_project INTEGER NOT NULL,
                                    FOREIGN KEY(id_user) REFERENCES User(id),
                                    FOREIGN KEY(id_project) REFERENCES Project(id)
                                ); ";
        _connection.Execute(createProjectUserAssociationStr);
    }

    //User
    public static string CreateUser(string Username, string FirstName, string LastName, string Email, string Password, string City)
    {
        // Check email not exist
        TableQuery<User> tableQuery = _connection.Table<User>().Where(x => x.Username == Username);
        if (tableQuery.Count() != 0)
            return "Nom d'utilisateur déjà utilisé";
        tableQuery = _connection.Table<User>().Where(x => x.Email == Email);
        if (tableQuery.Count() != 0)
            return "Adresse email déjà utilisée";
        // Check username not exist
        User user = new User
        {
            Username = Username,
            Firstname = FirstName,
            Lastname = LastName,
            Email = Email,
            Password = Password,
            City = City
        };
        _connection.Insert(user);
        return user.Id.ToString();
    }

    public static User GetUserById(int Id)
    {
        TableQuery<User> tableQuery = _connection.Table<User>().Where(x => x.Id == Id);
        if (tableQuery.Count() == 0)
            return null;
        return tableQuery.First();
    }

    public static User CheckUserInfo(string Username, string password)
    {
        TableQuery<User> tableQuery = _connection.Table<User>().Where(x => x.Username == Username);
        if (tableQuery.Count() == 0)
            return null;
        return tableQuery.First(); ;
    }

    //Projects
    public static List<Project> GetAllProjects()
    {
        IEnumerable<Project> projects = _connection.Table<Project>();
        return projects.ToList<Project>();
    }

    public static List<Project> GetAllUserProjects(int IdUser)
    {
        IEnumerable<UserProjectAssociation> userProjectAssociation = _connection.Table<UserProjectAssociation>().Where(x => x.IdUser == IdUser);

        List<Project> associatedProjects = new List<Project>();

        foreach (UserProjectAssociation association in userProjectAssociation)
        {
            Project project = _connection.Table<Project>().Where(x => x.Id == association.IdProject).FirstOrDefault();
            associatedProjects.Add(project);
        }

        return associatedProjects;
    }

    public static Project CreateProject(string CreationDate, string Name, string coordinates, string description = "")
    {
        Project project = new Project
        {
            Coordinates = coordinates,
            CreationDate = CreationDate,
            Description = description,
            Name = Name
        };

        _connection.Insert(project);

        return project;
    }

    // PROPOSALS 
    public static List<Proposal> GetAllUserProposals(int IdUser, int IdProject)
    {
        IEnumerable<Proposal> userProposals = _connection.Table<Proposal>().Where(x => x.IdUser == IdUser).Where(x => x.IdProject == IdProject);
        return userProposals.ToList();
    }

    public static Proposal CreateProposal(string date, byte[] file, int idUser, int idProject)
    {
        Proposal proposal = new Proposal
        {
            date = date,
            File = file,
            IdUser = idUser,
            IdProject = idProject
        };
        _connection.Insert(proposal);

        return proposal;
    }

    public static void UpdateProposalSaveFile(byte[] datas)
    {
        string date = Convert.ToDateTime(DateTime.Now).ToString("dd/MM/yyyy HH:mm:ss");
        object[] args = new object[3];

        string query = "UPDATE " + "Proposal" + " set file = ?, date = ? WHERE id = ?";
        try
        {
            args[0] = datas;
            args[1] = date;
            args[2] = Scenes.GetSelectedProposal().Id;
            SQLiteCommand cmd = _connection.CreateCommand(query, args);
            Debug.Log(cmd.CommandText);
            cmd.ExecuteNonQuery();
        }
        catch (Exception exception)
        {
            Debug.LogError(exception.Message);
        }

    }
}
