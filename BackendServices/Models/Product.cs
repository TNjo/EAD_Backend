﻿namespace BackendServices.Models;

// Models/Product.cs
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

// public class Product
// {
//     [BsonId]
//     [BsonRepresentation(BsonType.ObjectId)]
//     public string ProductId { get; set; }
//     
//     public string Name { get; set; }
//     
//     public string Description { get; set; }
//     
//     public decimal Price { get; set; }
//
//     public string VendorId { get; set; }
//
//     public bool IsActive { get; set; }
// }



public class Product
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }  // Auto-generated by MongoDB

    public string ProductName { get; set; }
    public decimal Price { get; set; }
    public int AvailableQuantity { get; set; }
    public string Category { get; set; }
    public string Description { get; set; }
    public string Image { get; set; }  // URL or path to image

    public int StockStatus { get; set; } = 2;  // 0 = Out of Stock, 1 = Low Stock, 2 = In Stock
    public bool ProductStatus { get; set; }  // 0 = Inactive, 1 = Active

    // public string VendorId { get; set; }  // Vendor who owns the product

    /* [BsonRepresentation(BsonType.ObjectId)]
     public string VendorId { get; set; }  // Linked to the Vendor who owns the product*/
    public string VendorEmail { get; set; }  // Store the Vendor's Email instead of VendorId
    public DateTime CreatedDate { get; set; }
}
