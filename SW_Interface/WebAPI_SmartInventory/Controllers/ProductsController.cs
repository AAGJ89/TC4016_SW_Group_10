using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using SmartInventory.Shared;
using WebAPI_SmartInventory.Services;

namespace WebAPI_SmartInventory.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ProductsService _productoService;

        public ProductsController(ProductsService productoService)
        {
            _productoService = productoService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var productos = await _productoService.GetAllAsync();

            var productosSerializado = productos.Select(p => new Products
            {
                StringId = p.Id.ToString(),  // Convertir a string
                Nombre = p.Nombre,
                Descripcion = p.Descripcion,
                Precio = p.Precio,
                Stock = p.Stock,
                FechaCreacion = p.FechaCreacion
            }).ToList();
        
            return Ok(productosSerializado);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            if (!ObjectId.TryParse(id, out _))
            {
                return BadRequest("Invalid ID format. ID must be a valid ObjectId.");
            }

            var producto = await _productoService.GetByIdAsync(id);
            if (producto == null) return NotFound();
            // Convertir el producto a una versión con StringId
            var productoSerializado = new Products
            {
                Id = producto.Id,
                StringId = producto.Id.ToString(), // Convertir Id a StringId
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion,
                Precio = producto.Precio,
                Stock = producto.Stock,
                FechaCreacion = producto.FechaCreacion
            };
            return Ok(productoSerializado);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Products producto)
        {
            // Validación de la descripción
            if (string.IsNullOrWhiteSpace(producto.Descripcion))
            {
                return BadRequest("La descripción no puede estar vacía.");
            }
            await _productoService.CreateAsync(producto);
            // Asignar StringId después de que MongoDB genere el Id
            producto.StringId = producto.Id.ToString();
            return CreatedAtAction(nameof(GetById), new { id = producto.Id }, producto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, Products producto)
        {
            if (!ObjectId.TryParse(id, out _))
            {
                return BadRequest("Invalid ID format. ID must be a valid ObjectId.");
            }

            var existingProducto = await _productoService.GetByIdAsync(id);
            if (existingProducto == null) return NotFound();

            producto.Id = existingProducto.Id;
            await _productoService.UpdateAsync(id, producto);
            return Ok(producto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (!ObjectId.TryParse(id, out _))
            {
                return BadRequest("Invalid ID format. ID must be a valid ObjectId.");
            }

            var producto = await _productoService.GetByIdAsync(id);
            if (producto == null) return NotFound();

            await _productoService.DeleteAsync(id);
            return NoContent();
        }
    }
}
