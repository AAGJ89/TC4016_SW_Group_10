using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using SmartInventory.Shared;
using WebAPI_SmartInventory.Services;

namespace WebAPI_SmartInventory.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly UsuariosService _usuarioService;

        public UsuariosController(UsuariosService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var usuarios = await _usuarioService.GetAllAsync();

            // Serializar cada usuario, convirtiendo Id a StringId
            var usuariosSerializado = usuarios.Select(u => new Usuarios
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                Activo = u.Activo,
                FechaCreacion = u.FechaCreacion,
                StringId = u.Id.ToString() // Convertir Id a StringId
            }).ToList();

            return Ok(usuariosSerializado);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            if (!ObjectId.TryParse(id, out _))
            {
                return BadRequest("Invalid ID format. ID must be a valid ObjectId.");
            }

            var usuario = await _usuarioService.GetByIdAsync(id);
            if (usuario == null) return NotFound();

            // Convertir el usuario a una versión con StringId
            var usuarioSerializado = new Usuarios
            {
                Id = usuario.Id,
                StringId = usuario.Id.ToString(), // Convertir Id a StringId
                Username = usuario.Username,
                Email = usuario.Email,
                Activo = usuario.Activo,
                FechaCreacion = usuario.FechaCreacion
            };

            return Ok(usuarioSerializado);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Usuarios usuario)
        {
            // Validación del Username y Email
            if (string.IsNullOrWhiteSpace(usuario.Username))
            {
                return BadRequest("El campo Username no puede estar vacío.");
            }
            if (string.IsNullOrWhiteSpace(usuario.Email))
            {
                return BadRequest("El campo Email no puede estar vacío.");
            }
            // Validación de formato de Email
            var emailRegex = new System.Text.RegularExpressions.Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            if (!emailRegex.IsMatch(usuario.Email))
            {
                return BadRequest("El campo Email debe contener una dirección de correo válida.");
            }
            await _usuarioService.CreateAsync(usuario);

            // Asignar StringId después de que MongoDB genere el Id
            usuario.StringId = usuario.Id.ToString();

            return CreatedAtAction(nameof(GetById), new { id = usuario.Id }, usuario);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, Usuarios usuario)
        {
            if (!ObjectId.TryParse(id, out _))
            {
                return BadRequest("Invalid ID format. ID must be a valid ObjectId.");
            }

            var existingProducto = await _usuarioService.GetByIdAsync(id);
            if (existingProducto == null) return NotFound();

            usuario.Id = existingProducto.Id;
            await _usuarioService.UpdateAsync(id, usuario);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (!ObjectId.TryParse(id, out _))
            {
                return BadRequest("Invalid ID format. ID must be a valid ObjectId.");
            }

            var usuario = await _usuarioService.GetByIdAsync(id);
            if (usuario == null) return NotFound();

            await _usuarioService.DeleteAsync(id);
            return NoContent();
        }
    }
}
