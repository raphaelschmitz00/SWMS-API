using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace SwmsApi.Clients
{
	[Route("[controller]")]
	[ApiController]
	public class ClientsController : ControllerBase
	{
		private readonly SwmsContext _swmsContext;


		public ClientsController(SwmsContext swmsContext)
		{
			_swmsContext = swmsContext;
		}


		[HttpGet]
		public async Task<ActionResult<IEnumerable<Client>>> Index()
		{
			return await _swmsContext.Clients.ToListAsync();
		}


		[HttpGet("{id}")]
		public async Task<ActionResult<Client>> Get(long id)
		{
			Client client = await _swmsContext.Clients.FindAsync(id);
			if (client == null) return NotFound();

			return client;
		}


		[HttpPost]
		public async Task<ActionResult<Client>> Create(Client client)
		{
			_swmsContext.Clients.Add(client);
			await _swmsContext.SaveChangesAsync();

			return CreatedAtAction(nameof(Get), new {id = client.Id}, client);
		}


		[HttpPut]
		public async Task<IActionResult> Put(Client client)
		{
			_swmsContext.Entry(client).State = EntityState.Modified;
			await _swmsContext.SaveChangesAsync();

			return NoContent();
		}


		[HttpDelete("delete/{id}")]
		public async Task<IActionResult> Delete(long id)
		{
			//_swmsContext.Entry(client).State = EntityState.Deleted;

			Client internalClient = await _swmsContext.Clients.FindAsync(id);
			if (internalClient == null) return NotFound();

			_swmsContext.Clients.Remove(internalClient);
			await _swmsContext.SaveChangesAsync();

			return NoContent();
		}


		[HttpDelete]
		public async Task<IActionResult> Delete(Client client)
		{
			Client internalClient = await _swmsContext.Clients.FindAsync(client.Id);
			if (internalClient == null) return NotFound();

			_swmsContext.Clients.Remove(internalClient);
			await _swmsContext.SaveChangesAsync();

			return NoContent();
		}


		[HttpDelete]
		public async Task<IActionResult> Deleteo(Client client)
		{
			_swmsContext.Entry(client).State = EntityState.Deleted;
			await _swmsContext.SaveChangesAsync();

			return NoContent();
		}
	}
}