using SQLite4Unity3d;

public class Proposal
{
    [Column("id"), PrimaryKey, AutoIncrement, NotNull]
    public int Id { get; set; }

    [Column("date"), NotNull]
    public string date { get; set; }

    [Column("file")]
    public byte[] File { get; set; }

    [Column("id_user"), NotNull]
    public int IdUser { get; set; }

    [Column("id_project"), NotNull]
    public int IdProject { get; set; }
}