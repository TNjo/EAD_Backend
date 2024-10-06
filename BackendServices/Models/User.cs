namespace BackendServices.Models;

// Models/User.cs
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public string Username { get; set; }
    public string Password { get; set; }
    
    public string Email { get; set; }

    public string Role { get; set; } // Administrator, Vendor, CSR
    public int Status { get; set; } = 1;  // 1 = Active, 0 = Deactivated
}


// public class User
// {
//     [BsonId]
//     [BsonRepresentation(BsonType.ObjectId)]
//     public string Id { get; set; }
//
//     public string Email { get; set; }
//     public string Password { get; set; }
//     public string Role { get; set; } = "User";  // Default role
//     public int Status { get; set; } = 1;  // 1 = Active, 0 = Deactivated
// }

