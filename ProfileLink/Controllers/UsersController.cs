using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProfileLinkTest.DTOs;
using Microsoft.AspNetCore.Authorization;
using ProfileLink.Data;
using System.Security.Cryptography;
using System.Text;

namespace ProfileLinkTest.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
	private readonly DataContext _context;
	private readonly IConfiguration _config;

	public UsersController(DataContext context, IConfiguration config)
	{
		_context = context;
		_config = config;
	}

	[HttpGet("{id}")]
	public async Task<ActionResult<UserDTO>> Get(int id)
	{
		var user = await _context.Users
			.Where(u => u.UserId == id)
			.Include(u => u.Socials)
			.Include(u => u.Links)
			.Select(x => userDTO(x))
			.FirstOrDefaultAsync();

		if (user == null)
		{
			return NotFound($"User with id '{id}' doesn't exist.");
		}

		return Ok(user);
	}

	[HttpGet("profile/{username}")]
	[AllowAnonymous]
	public async Task<ActionResult<UserDTO>> GetByUsername(string username)
	{
		var user = await _context.Users
			.Where(u => u.Username.ToLower() == username.ToLower())
			.Include(u => u.Socials)
			.Include(u => u.Links)
			.Select(x => userDTO(x))
			.FirstOrDefaultAsync();

		if (user == null)
		{
			return NotFound($"User with username '{username}' doesn't exist.");
		}

		return Ok(user);
	}

	public record RegistrationData(string? FirstName, string? LastName, string? Username, string? Password);

	[HttpPost("register")]
	[AllowAnonymous]
	public async Task<ActionResult<UserDTO>> Post([FromBody] RegistrationData _userData)
	{
		
		if (_userData != null &&
			_userData.Username != null &&
			_userData.Password != null &&
			_userData.FirstName != null &&
			_userData.LastName != null)
		{
			var hashedPassword = HashPasword(_userData.Password);
			var user = await GetUser(_userData.Username);

			if (user == null)
			{
				var newUser = new User
				{
					Username = _userData.Username!,
					Password = hashedPassword,
					FirstName = _userData.FirstName!,
					LastName = _userData.LastName!,
					Bio = ""!,
					Theme = "840AD7"
				};

				_context.Users.Add(newUser);
				await _context.SaveChangesAsync();

				return CreatedAtAction(
					nameof(GetByUsername),
					new { username = newUser.Username },
					userDTO(newUser));
			}
			else
			{
				return BadRequest();
			}
		}
		else
		{
			return BadRequest();
		}
	}

	public record UserData(string FirstName, string LastName, string Username, string Bio, string Theme);

	[HttpPut("{id}")]
	public async Task<IActionResult> UpdateUser(int id, [FromBody] UserData data)
	{
		if (data.FirstName == null || data.LastName == null || data.Username == null || data.Bio == null || data.Theme == null)
		{
			return BadRequest();
		}

		var user = _context.Users.Where(l => l.UserId == id).FirstOrDefault();
		if (user == null)
		{
			return NotFound();
		}

		user.FirstName = data.FirstName;
		user.LastName = data.LastName;
		user.Username = data.Username;
		user.Bio = data.Bio;
		user.Theme = data.Theme;

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
	public async Task<ActionResult> DeleteUser(int id)
	{
		var user = await _context.Users
			.Where(u => u.UserId == id)
			.FirstOrDefaultAsync();

		if (user == null)
		{
			return NotFound();
		}

		_context.Users.Remove(user);
		await _context.SaveChangesAsync();
		return NoContent();
	}

	private static UserDTO userDTO(User user) => new UserDTO
	{
		UserId = user.UserId,
		Username = user.Username,
		FirstName = user.FirstName,
		LastName = user.LastName,
		Bio = user.Bio,
		Theme = user.Theme,
		Socials = user.Socials,
		Links = user.Links
	};

	private async Task<User> GetUser(string username)
	{
		var user = await _context.Users
			.Where(u => u.Username.ToLower() == username.ToLower())
			.FirstOrDefaultAsync();

		return user!;
	}

	private static string HashPasword(string password)
	{
		SHA256 hash = SHA256.Create();
		var passwordBytes = Encoding.Default.GetBytes(password);
		var hashedPassword = hash.ComputeHash(passwordBytes);

		return Convert.ToBase64String(hashedPassword);
	}
}
