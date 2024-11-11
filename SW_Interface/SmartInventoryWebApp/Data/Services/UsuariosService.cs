using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using SmartInventory.Shared;

namespace SmartInventoryWebApp.Data.Services
{
    public class UsuariosService
    {
        private readonly HttpClient _httpClient;
        private const string ApiUrl = "https://localhost:7032/api/usuarios";

        public UsuariosService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Usuarios>> GetAllAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<Usuarios>>(ApiUrl);
        }

        public async Task<Usuarios> GetByIdAsync(string id)
        {
            return await _httpClient.GetFromJsonAsync<Usuarios>($"{ApiUrl}/{id}");
        }

        public async Task<Usuarios> CreateAsync(Usuarios usuario)
        {
            usuario.StringId = Guid.NewGuid().ToString();
            var response = await _httpClient.PostAsJsonAsync(ApiUrl, usuario);

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                throw new Exception(errorMessage);
            }

            var createdUsuario = await response.Content.ReadFromJsonAsync<Usuarios>();
            return createdUsuario;
        }

        public async Task UpdateAsync(string id, Usuarios usuario)
        {
            await _httpClient.PutAsJsonAsync($"{ApiUrl}/{id}", usuario);
        }

        public async Task DeleteAsync(string id)
        {
            await _httpClient.DeleteAsync($"{ApiUrl}/{id}");
        }
    }
}
