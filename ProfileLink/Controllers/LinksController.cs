using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProfileLink.Data;

namespace ProfileLinkTest.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LinksController : ControllerBase
{
	private readonly DataContext _context;

	public LinksController(DataContext context)
	{
		_context = context;
	}

	[HttpGet("{id}")]
	public async Task<IActionResult> GetLinkById(int id)
	{
		var link = await _context.Links
			.Where(l => l.LinkId == id)
			.FirstOrDefaultAsync();

		if (link == null)
		{
			return NotFound($"Social with id '{id}' doesn't exist.");
		}
		return Ok(link);
	}

	public record LinkData(string Title, string Url);

	[HttpPost]
	public async Task<IActionResult> CreateLink(int userId, [FromBody] LinkData data)
	{
		if (data.Title == null || data.Url == null)
		{
			return BadRequest();
		}

		var user = _context.Users.Where(u => u.UserId == userId).FirstOrDefault();
		if (user == null)
		{
			return NotFound();
		}
		else
		{
			var newLink = new Link
			{
				Title = data.Title,
				Url = data.Url,
				Active = true,
				User = user
			};

			_context.Links.Add(newLink);
			await _context.SaveChangesAsync();

			return Ok();
		}
	}

	[HttpPut("{id}")]
	public async Task<IActionResult> UpdateLink(int id, [FromBody] LinkData data)
	{
		if (data.Title == null || data.Url == null)
		{
			return BadRequest();
		}

		var linkItem = _context.Links.Where(l => l.LinkId == id).FirstOrDefault();
		if (linkItem == null)
		{
			return NotFound();
		}

		linkItem.Title = data.Title;
		linkItem.Url = data.Url;

		try
		{
			await _context.SaveChangesAsync();
			return NoContent();
		}
		catch (Exception)
		{
			return NotFound();
		}
	}

	[HttpPut("active/{id}")]
	[AllowAnonymous]
	public async Task<IActionResult> ToggleActive(int id)
	{
		var linkItem = _context.Links.Where(l => l.LinkId == id).FirstOrDefault();
		if (linkItem == null)
		{
			return NotFound();
		}

		linkItem.Active = !linkItem.Active;

		try
		{
			await _context.SaveChangesAsync();
			return NoContent();
		}
		catch (Exception)
		{
			return NotFound();
		}
	}

	[HttpDelete("{id}")]
	public async Task<IActionResult> DeleteLink(int id)
	{
		var link = await _context.Links
			.Where(l => l.LinkId == id)
			.FirstOrDefaultAsync();

		if (link == null)
		{
			return NotFound();
		}

		_context.Links.Remove(link);
		await _context.SaveChangesAsync();

		return NoContent();
	}
}
