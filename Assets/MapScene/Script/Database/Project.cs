using SQLite4Unity3d;

public class Project
{
    [Column("id"), PrimaryKey, AutoIncrement, NotNull]
    public int Id { get; set; }
    [Column("creation_date"), NotNull]
    public string CreationDate { get; set; }
    [Column("name"), NotNull]
    public string Name { get; set; }
    [Column("coordinates"), NotNull]
    public string Coordinates { get; set; }
    [Column("description")]
    public string Description { get; set; }
}