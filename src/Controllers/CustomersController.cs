using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CustomerApi.Services;
using CustomerApi.Domain;
using CustomerApi.VM;

namespace CustomerApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _service;

        public CustomersController(ICustomerService service) => _service = service;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCustomerVm vm)
        {
            var domain = new Customer { FirstName = vm.FirstName, LastName = vm.LastName, Email = vm.Email };
            var result = await _service.CreateAsync(domain);
            if (!result.IsSuccess) return BadRequest(new { message = result.Message });
            var createdVm = ToVm(result.Data!);
            return CreatedAtAction(nameof(Get), new { id = createdVm.Id }, createdVm);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCustomerVm vm)
        {
            if (id != vm.Id) return BadRequest(new { message = "Id mismatch." });
            var domain = new Customer { Id = vm.Id, FirstName = vm.FirstName, LastName = vm.LastName, Email = vm.Email };
            var result = await _service.UpdateAsync(domain);
            if (!result.IsSuccess) return BadRequest(new { message = result.Message });
            return Ok(ToVm(result.Data!));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            if (!result.IsSuccess) return BadRequest(new { message = result.Message });
            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _service.GetAsync(id);
            if (!result.IsSuccess) return NotFound(new { message = result.Message });
            return Ok(ToVm(result.Data!));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            if (!result.IsSuccess) return BadRequest(new { message = result.Message });
            var vms = result.Data!.Select(ToVm).ToList();
            return Ok(vms);
        }

        private static CustomerVm ToVm(Customer c) =>
            new CustomerVm
            {
                Id = c.Id,
                FullName = $"{c.FirstName} {c.LastName}".Trim(),
                Email = c.Email,
                CreatedAt = c.CreatedAt
            };
    }
}
