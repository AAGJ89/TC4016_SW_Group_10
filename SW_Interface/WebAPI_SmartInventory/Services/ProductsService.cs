using MongoDB.Driver;
using Microsoft.Extensions.Options;
using WebAPI_SmartInventory.Data;
using MongoDB.Bson;
using SmartInventory.Shared;

namespace WebAPI_SmartInventory.Services
{
    public class ProductsService
    {
        private readonly IMongoCollection<Products> _products;

        public ProductsService(IOptions<MongoDBSettings> mongoDBSettings)
        {
            var client = new MongoClient(mongoDBSettings.Value.ConnectionString);
            var database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
            _products = database.GetCollection<Products>(mongoDBSettings.Value.Collections["Products"]);
        }

        public async Task<List<Products>> GetAllAsync()
        {
            try
            {
                var products = await _products.Find(product => true).ToListAsync();
                return products ?? new List<Products>(); // Devuelve una lista vacía si `products` es null
            }
            catch (Exception ex)
            {
                // Registrar el error o manejarlo según la lógica de la aplicación
                Console.WriteLine($"Error al obtener productos: {ex.Message}");

                // Devolver una lista vacía en caso de error, o relanzar la excepción según la lógica de la aplicación
                return new List<Products>();
            }
        }

        public async Task<Products> GetByIdAsync(string id)
        {
            if (!ObjectId.TryParse(id, out ObjectId objectId))
            {
                return null; // Devuelve null si el ID no es válido.
            }

            // Ajuste: Usar filtro explícito basado en la cadena `Id`.
            var filter = Builders<Products>.Filter.Eq(p => p.Id, objectId);
            return await _products.Find(filter).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(Products product)
        {
            await _products.InsertOneAsync(product);
        }

        public async Task UpdateAsync(string id, Products product)
        {
            if (!ObjectId.TryParse(id, out ObjectId objectId))
            {
                return; // Termina el método si el ID no es válido
            }

            // Asegurar que el `product.Id` esté asignado como `ObjectId`
            product.Id = objectId;

            // Crear el filtro basado en `ObjectId`
            var filter = Builders<Products>.Filter.Eq(p => p.Id, objectId);

            // Reemplazar el documento en la colección
            await _products.ReplaceOneAsync(filter, product);
        }

        public async Task DeleteAsync(string id)
        {
            if (!ObjectId.TryParse(id, out ObjectId objectId))
            {
                return; // Termina el método si el ID no es válido
            }

            // Crear un filtro explícito para buscar por `ObjectId`
            var filter = Builders<Products>.Filter.Eq(p => p.Id, objectId);

            // Usar `DeleteOneAsync` con el filtro
            await _products.DeleteOneAsync(filter);
        }
    }
}
