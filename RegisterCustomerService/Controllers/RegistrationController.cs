using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RegisterCustomerService.Data;
using RegisterCustomerService.Models;
using RegisterCustomerService.Proxy.AuthProxy;

namespace RegisterCustomerService.Controllers
{
    [ApiController]
    [Route("Registration")]
    public class RegistrationController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IAuthProxyService _authProxyService;

        public RegistrationController(AppDbContext db, IAuthProxyService authProxyService)
        {
            _db = db;
            _authProxyService = authProxyService;
        }

        [HttpPost("RegisterCustomer")]
        public async Task<IActionResult> RegisterCustomer([FromBody] CustomerRegistrationRequest request)
        {
            var existingCustomer = await _db.Customers.FirstOrDefaultAsync(c => c.Email == request.Email);
            if (existingCustomer != null) { 
                return Conflict(new { Message = "Customer already exists." });
            }
            var customer = new Customer
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                Address = request.Address
            };

            _db.Customers.Add(customer);
            await _db.SaveChangesAsync();
            await _authProxyService.CreateUserAsync(customer.Id, customer.Email, $"{customer.FirstName} {customer.LastName}",request.Password);
            return CreatedAtAction(null, new { id = customer.Id }, new { Message = "Customer registered successfully", customer.Id });
        }

        [HttpPut("UpdateCustomer/{id}")]
        [Authorize]
        public IActionResult UpdateCustomer(int id, [FromBody] CustomerRegistrationRequest request)
        {
            var customer = _db.Customers.Find(id);
            if (customer == null)
            {
                return NotFound(new { Message = "Customer not found" });
            }

            customer.FirstName = request.FirstName;
            customer.LastName = request.LastName;
            customer.Email = request.Email;
            customer.PhoneNumber = request.PhoneNumber;
            customer.Address = request.Address;

            _db.SaveChanges();

            return Ok(new { Message = "Customer updated successfully", customer.Id });
        }

        [HttpDelete("DeleteCustomer/{id}")]
        [Authorize]
        public IActionResult DeleteCustomer(int id)
        {
            var customer = _db.Customers.Find(id);
            if (customer == null)
            {
                return NotFound(new { Message = "Customer not found" });
            }

            _db.Customers.Remove(customer);
            _db.SaveChanges();

            return NoContent();
        }
    }
}
