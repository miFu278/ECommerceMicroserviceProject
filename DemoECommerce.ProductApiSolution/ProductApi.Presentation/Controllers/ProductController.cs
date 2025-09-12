using eCommerce.SharedLibrary.Responses;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Application.DTOs;
using ProductApi.Application.DTOs.Conversions;
using ProductApi.Application.Interfaces;

namespace ProductApi.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController(IProduct productInterface) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProducts()
        {
            // Get all products from repo
            var products = await productInterface.GetAllAsync();
            if (!products.Any())
                return NotFound("No product detected in database");

            // convert from entity to DTO and return
            var (_, list) = ProductConversion.FromEntity(null!, products);
            return list!.Any() ? Ok(list) : NotFound("No product found");
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductDTO>> GetProduct(int id)
        {
            // Get single product from repo
            var product = await productInterface.FindByIdAsync(id);
            if (product == null) return NotFound($"Product with {id} not found");

            // convert from entity to DTO and return
            var (_product, _) = ProductConversion.FromEntity(product, null!);
            return _product is not null ? Ok(product) : NotFound($"Product with {id} not found");
        }

        [HttpPost]
        public async Task<ActionResult<Response>> CreateProdcut(ProductDTO product)
        {
            // check model state is all data annotations are passed
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // convert to entity
            var getEntity = ProductConversion.ToEntity(product);
            var response = await productInterface.CreateAsync(getEntity);
            return response.Flag is true ? Ok(response) : BadRequest(response);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Response>> UpdateProduct(int id, ProductDTO product)
        {
            // check model state is all data annotations are passed
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // convert to entity
            var existingProduct = await productInterface.FindByIdAsync(id);
            if (existingProduct == null)
                return NotFound($"Product with {id} not found");
            var getEntity = ProductConversion.ToEntity(product);
            var response = await productInterface.UpdateAsync(getEntity);
            return response.Flag is true ? Ok(response) : BadRequest(response);



        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Response>> DeleteProduct(int id)
        {
            // convert to entity
            var existingProduct = await productInterface.FindByIdAsync(id);
            if (existingProduct == null)
                return NotFound($"Product with {id} not found");
            var response = await productInterface.DeleteAsync(existingProduct);
            return response.Flag is true ? Ok(response) : BadRequest(response);
        }

    }
}
