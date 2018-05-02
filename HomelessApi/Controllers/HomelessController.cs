using Microsoft.AspNetCore.Mvc;
using HomelessApi.Models;
using HomelessApi.Repositories;
using System.Threading.Tasks;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Authorization;

namespace HomelessApi.Controllers
{
	[Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    public class HomelessController : Controller
    {
        HomelessRepository homelessRepository;

        public HomelessController(HomelessRepository homelessRepository)
        {
            this.homelessRepository = homelessRepository;
        }

        [HttpGet]
		[Authorize(AuthenticationSchemes = "Bearer")]
        public IQueryable<CustomPin> Get()
        {
            return homelessRepository.Query();
        }

        [HttpGet("{id}", Name = nameof(Get))]
        public async Task<IActionResult> Get(string id)
        {
            var customPin = await homelessRepository.GetOneAsync(id);

            if (customPin == null)
                return NotFound();

            return Ok(customPin);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CustomPin customPin)
        {
            if (customPin == null)
                return BadRequest();

            await homelessRepository.SaveAsync(customPin);
            return CreatedAtRoute(nameof(Get), new { id = customPin.Id }, customPin);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody] CustomPin customPin)
        {
            if (await homelessRepository.UpdateAsync(customPin) == null)
                return NotFound();

            return Ok(customPin);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await homelessRepository.RemoveAsync(id);
            if (result == null)
                return NotFound();

            return Ok(result);
        }
    }
}
