using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BackendServices.Models;

public class VendorComment
{
    // [BsonId]
    // [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string Comment { get; set; }
    public int Rank { get; set; }  // Rank between 1 and 5
    public string UserId { get; set; }  // ID of the user who made the comment
    public DateTime Timestamp { get; set; }  // When the comment was made
}