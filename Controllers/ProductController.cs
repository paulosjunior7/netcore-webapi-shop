using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;

[Route("v1/products")]
public class ProductController : ControllerBase
{
    [HttpGet]
    [Route("")]
    public async Task<ActionResult<List<Product>>> Get(
        [FromServices] DataContext context
    )
    {
        var product = await context
            .Products
            .Include(x => x.Category)
            .AsNoTracking()
            .ToListAsync();
        return Ok(product);
    }

    [HttpGet]
    [Route("{id:int}")]
    public async Task<ActionResult<Product>> GetById(
        int id,
        [FromServices] DataContext context
        )
    {
        var product = await context
        .Products
        .Include(x => x.Category)
        .AsNoTracking()
        .FirstOrDefaultAsync(x => x.Id == id);
        return Ok(product);
    }

    
    [HttpGet]
    [Route("categories/{id:int}")]
    public async Task<ActionResult<List<Product>>> GetByCategory(
        int id,
        [FromServices] DataContext context
        )
    {
        var products = await context
            .Products
            .Include(x => x.Category)
            .AsNoTracking()
            .Where(x => x.CategoryId == id)
            .ToListAsync();

        return Ok(products);
    }

    [HttpPost]
    [Route("")]
    public async Task<ActionResult<List<Product>>> Post(
        [FromBody] Product model,
        [FromServices] DataContext context)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            context.Products.Add(model);
            await context.SaveChangesAsync();
            return Ok(model);
        }
        catch
        {
            return BadRequest(new { messagem = "Não foi possível criar a Produto" });
        }
    }

    [HttpPut]
    [Route("{id:int}")]
    public async Task<ActionResult<List<Product>>> Put(
            int id,
            [FromBody] Product model,
            [FromServices] DataContext context
     )
    {
        if (id != model.Id)
            return NotFound(new { message = "Produto não encontrada" });

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            context.Entry<Product>(model).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            await context.SaveChangesAsync();
            return Ok(model);
        }
        catch (DbUpdateConcurrencyException)
        {
            return BadRequest(new { messagem = "Este registro já foi atualizado" });
        }
        catch
        {
            return BadRequest(new { messagem = "Não foi possível atualizar o produto" });
        }
    }

    [HttpDelete]
    [Route("{id:int}")]
    public async Task<ActionResult<List<Product>>> Delete(
        int id,
        [FromServices] DataContext context
    )
    {
        var product = await context.Products.FirstOrDefaultAsync(x => x.Id == id);
        if (product == null)
        {
            return NotFound(new { messagem = "Produto não encontrado" });
        }

        try
        {
            context.Products.Remove(product);
            await context.SaveChangesAsync();
            return Ok(new { messagem = "Produto removido com sucesso" });
        }
        catch
        {
            return BadRequest(new { messagem = "Não foi possível remover o produto" });
        }
    }
}