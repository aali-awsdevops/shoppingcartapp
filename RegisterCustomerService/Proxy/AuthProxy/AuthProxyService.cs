namespace RegisterCustomerService.Proxy.AuthProxy
{
    public class AuthProxyService :IAuthProxyService
    {
        private readonly HttpClient _httpClient;
        public AuthProxyService(HttpClient httpClient) 
        { 
            _httpClient = httpClient;
        }

        public async Task<bool> CreateUserAsync(int customerId, string email, string displayName, string password)
        {
            // Implement the logic to call the AuthorizationService API to create a user
            // For example, you can use HttpClient to send a POST request to the AuthorizationService
            // and return true if the user was created successfully, or false otherwise.
            // Example:
            var response = await _httpClient.PostAsJsonAsync("Registration/CreateUser", new { CustomerId = customerId, Email = email, DisplayName = displayName , Password = password });
            response.EnsureSuccessStatusCode();
            return response.IsSuccessStatusCode;
        }
    }
}
