﻿namespace BackendServices.Models;

public class CartItem
{
    public string ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal Price { get; set; }
}