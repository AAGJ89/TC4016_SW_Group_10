using MongoDB.Driver;
using Microsoft.Extensions.Options;
using WebAPI_SmartInventory.Data;
using MongoDB.Bson;
using SmartInventory.Shared;

namespace WebAPI_SmartInventory.Services
{
    public class UsuariosService
    {
        private readonly IMongoCollection<Usuarios> _usuarios;

        public UsuariosService(IOptions<MongoDBSettings> mongoDBSettings)
        {
            var client = new MongoClient(mongoDBSettings.Value.ConnectionString);
            var database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
            _usuarios = database.GetCollection<Usuarios>(mongoDBSettings.Value.Collections["Usuarios"]);
        }

        public async Task<List<Usuarios>> GetAllAsync()
        {
            try
            {
                var usuarios = await _usuarios.Find(usuario => true).ToListAsync();
                return usuarios ?? new List<Usuarios>(); // Devuelve una lista vacía si `usuarios` es null
            }
            catch (Exception ex)
            {
                // Registrar el error o lanzar una excepción personalizada
                Console.WriteLine($"Error al obtener usuarios: {ex.Message}");

                // Devolver una lista vacía en caso de error, o relanzar la excepción según la lógica de la aplicación
                return new List<Usuarios>();
            }
        }

        public async Task<Usuarios> GetByIdAsync(string id)
        {
            if (!ObjectId.TryParse(id, out ObjectId objectId))
            {
                return null; // Devuelve null si el ID no es válido.
            }

            // Ajuste: Usar filtro explícito basado en la cadena `Id`.
            var filter = Builders<Usuarios>.Filter.Eq(p => p.Id, objectId);
            return await _usuarios.Find(filter).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(Usuarios usuario)
        {
            await _usuarios.InsertOneAsync(usuario);
        }

        public async Task UpdateAsync(string id, Usuarios usuario)
        {
            if (!ObjectId.TryParse(id, out ObjectId objectId))
            {
                return; // Termina el método si el ID no es válido
            }

            // Asegurar que el `product.Id` esté asignado como `ObjectId`
            usuario.Id = objectId;

            // Crear el filtro basado en `ObjectId`
            var filter = Builders<Usuarios>.Filter.Eq(p => p.Id, objectId);

            // Reemplazar el documento en la colección
            await _usuarios.ReplaceOneAsync(filter, usuario);
        }

        public async Task DeleteAsync(string id)
        {
            if (!ObjectId.TryParse(id, out ObjectId objectId))
            {
                return; // Termina el método si el ID no es válido
            }

            // Crear un filtro explícito para buscar por `ObjectId`
            var filter = Builders<Usuarios>.Filter.Eq(p => p.Id, objectId);

            // Usar `DeleteOneAsync` con el filtro
            await _usuarios.DeleteOneAsync(filter);
        }
    }
}
