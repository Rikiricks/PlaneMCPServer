using System.Text;
using System.Text.Json;

namespace MCPServerPlane.Services
{
    public class PlaneAPIService
    {
        private readonly IHttpClientFactory _httpClientFactory;      
        private readonly string _projectId;
       

        public PlaneAPIService(IHttpClientFactory httpClientFactory, string projectId)
        {
            _httpClientFactory = httpClientFactory;
            _projectId = projectId;
        }

        public async Task<string> GetProjectStatusesAsync()
        {
            var client = _httpClientFactory.CreateClient("PlaneClient");


            var response = await client.GetAsync($"projects/{_projectId}/states");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return content;
        }

        public async Task<string> GetAllWorkItemsAsync()
        {
            var client = _httpClientFactory.CreateClient("PlaneClient");

            var response = await client.GetAsync($"projects/{_projectId}/work-items");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return content;
        }

        /// <summary>
        /// Search for work items across the workspace
        /// </summary>
        public async Task<string> SearchWorkItemsAsync(string searchTerm)
        {
            var client = _httpClientFactory.CreateClient("PlaneClient");
            var url = $"work-items/search?search={Uri.EscapeDataString(searchTerm)}";
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return content;
        }

        public async Task<string> CreateWorkItemAsync(string name, string descriptionHtml, string stateId)
        {
            var httpClient = _httpClientFactory.CreateClient("PlaneClient");

            var requestBody = new
            {
                name = name,
                description_html = descriptionHtml,
                state = stateId
            };

            var jsonContent = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync($"projects/{_projectId}/work-items/", content);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            return responseContent;
        }
    }
}
