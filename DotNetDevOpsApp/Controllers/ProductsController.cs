using DotNetDevOpsApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotNetDevOpsApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private static readonly List<Product> Products = new()
    {
        new Product { Id = 1, Name = "Laptop", Price = 65000, Category = "Electronics" },
        new Product { Id = 2, Name = "Keyboard", Price = 2500, Category = "Accessories" },
        new Product { Id = 3, Name = "Monitor", Price = 15000, Category = "Electronics" }
    };

    [HttpGet]
    public IActionResult GetProducts()
    {
        return Ok(Products);
    }

    [HttpGet("{id}")]
    public IActionResult GetProduct(int id)
    {
        var product = Products.FirstOrDefault(x => x.Id == id);

        if (product == null)
            return NotFound(new { message = "Product not found" });

        return Ok(product);
    }

    [HttpPost]
    public IActionResult CreateProduct(Product product)
    {
        product.Id = Products.Count == 0 ? 1 : Products.Max(x => x.Id) + 1;
        Products.Add(product);

        return CreatedAtAction(
            nameof(GetProduct),
            new { id = product.Id },
            product);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateProduct(int id, Product product)
    {
        var existingProduct = Products.FirstOrDefault(x => x.Id == id);

        if (existingProduct == null)
            return NotFound(new { message = "Product not found" });

        existingProduct.Name = product.Name;
        existingProduct.Price = product.Price;
        existingProduct.Category = product.Category;

        return Ok(existingProduct);
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteProduct(int id)
    {
        var product = Products.FirstOrDefault(x => x.Id == id);

        if (product == null)
            return NotFound(new { message = "Product not found" });

        Products.Remove(product);

        return Ok(new { message = "Product deleted successfully" });
    }
}
