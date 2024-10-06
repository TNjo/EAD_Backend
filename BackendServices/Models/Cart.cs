using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BackendServices.Models;
public class Cart
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString(); // This maps to MongoDB's _id field
        
    public string CustomerId { get; set; }

    public List<CartItem> Items { get; set; } = new List<CartItem>();

    public decimal TotalPrice => Items.Sum(item => item.Price * item.Quantity); // Calculate total price
}