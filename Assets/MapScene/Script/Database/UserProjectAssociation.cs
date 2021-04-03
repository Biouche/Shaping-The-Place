using SQLite4Unity3d;

public class UserProjectAssociation
{
    [Column("id_user"), NotNull]
    public int IdUser { get; set; }
    [Column("id_project"), NotNull]
    public int IdProject { get; set; }
}