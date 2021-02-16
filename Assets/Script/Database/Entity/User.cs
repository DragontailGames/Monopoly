using MongoDB.Bson;

public class User
{
    public ObjectId Id { get; set; }

    public string email { get; set; }

    public string username { get; set; }

    public string password { get; set; }

    public int coins { get; set; }
}