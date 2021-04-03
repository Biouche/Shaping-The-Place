using SQLite4Unity3d;

public class User
{
    [PrimaryKey, AutoIncrement, NotNull, Column("id")]
    public int Id { get; set; }
    [NotNull, Column("username"), Unique]
    public string Username { get; set; }
    [NotNull, Column("password")]
    public string Password { get; set; }
    [NotNull, Column("email"), Unique]
    public string Email { get; set; }
    [NotNull, Column("firstname")]
    public string Firstname { get; set; }
    [NotNull, Column("lastname")]
    public string Lastname { get; set; }
    [NotNull, Column("city")]
    public string City { get; set; }
    [Column("type")]
    public string Type { get; set; }

    public override string ToString()
    {
        return string.Format("[Person: Id={0}, Username={1},  Password={2}, Email={3}, Firstname={4}, Lastname={5}, City={6}]", Id, Username, Password, Email, Firstname, Lastname, City);
    }
}
