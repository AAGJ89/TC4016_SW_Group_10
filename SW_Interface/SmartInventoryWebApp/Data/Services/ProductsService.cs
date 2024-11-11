using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using SmartInventory.Shared;

namespace SmartInventoryWebApp.Data.Services
{
    public class ProductsService
    {
        private readonly HttpClient _httpClient;
        private const string ApiUrl = "https://localhost:7032/api/products";

        public ProductsService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Products>> GetAllAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<Products>>(ApiUrl);
        }

        public async Task<Products> GetByIdAsync(string id)
        {
            return await _httpClient.GetFromJsonAsync<Products>($"{ApiUrl}/{id}");
        }

        public async Task<Products> CreateAsync(Products product)
        {
            product.StringId = Guid.NewGuid().ToString();
            var response = await _httpClient.PostAsJsonAsync(ApiUrl, product);

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                throw new Exception(errorMessage); // Lanza una excepción con el mensaje de error
            }

            // Si la respuesta es exitosa, deserializa el contenido a un objeto Products
            var createdProduct = await response.Content.ReadFromJsonAsync<Products>();
            return createdProduct;
        }

        public async Task UpdateAsync(string id, Products product)
        {
            await _httpClient.PutAsJsonAsync($"{ApiUrl}/{id}", product);
        }

        public async Task DeleteAsync(string id)
        {
            await _httpClient.DeleteAsync($"{ApiUrl}/{id}");
        }
    }
}
