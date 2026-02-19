using Microsoft.AspNetCore.Mvc;
using ProductService.models;

namespace ProductService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : Controller
{
    private static readonly List<Product> products = new();
     
     [HttpGet]
     public ActionResult<List<Product>> GetAll() => Ok(products);

     [HttpGet("{id:int}")]
     public ActionResult<Product> GetById(int id)
    {
        var p = products.FirstOrDefault(x => x.Id == id);
        return p is null ? NotFound() : Ok (p);
    }

    [HttpPost]
     public ActionResult<Product> Create(Product product )
    {
        product.Id = products.Count == 0 ? 1 : products.Max(x => x.Id) + 1;
        products.Add(product);
        return CreatedAtAction(nameof(GetById) , new {id = product.Id},product);
    }
}