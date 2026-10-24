namespace RegisterCustomerService.Proxy.AuthProxy
{
    public interface IAuthProxyService
    {
        public Task<bool> CreateUserAsync(int customerId, string email, string displayName, string Password);
    }
}
