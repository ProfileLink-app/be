﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProfileLink.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ProfileLinkTest.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthenticationController : ControllerBase
{
	public IConfiguration _config;
	private readonly DataContext _context;

	public AuthenticationController(IConfiguration config, DataContext context)
	{
		_config = config;
		_context = context;
	}

	public record AuthenticationData(string? Username, string Password);
	[HttpPost]
	[AllowAnonymous]
	public async Task<IActionResult> Post(AuthenticationData _userData)
	{
		if (_userData != null && _userData.Username != null && _userData.Password != null)
		{
			var user = await AuthenticateUser(_userData.Username, HashPasword(_userData.Password));

			if (user != null)
			{
				return Ok(GenerateToken(user));
			}
			else
			{
				return BadRequest("Invalid credentials");
			}
		}
		else
		{
			return BadRequest();
		}
	}

	public record usernameData(string username);
	[HttpPost("username")]
	[AllowAnonymous]
	public async Task<IActionResult> CheckUsernameValid([FromBody] usernameData data)
	{
		if (data.username != null)
		{
			var user = await GetUser(data.username);

			if (user == null)
			{
				Console.WriteLine(1);
				return Ok("Username available");
			}
			else
			{
				Console.WriteLine(2);
				return BadRequest("Username taken");
			}
		}
		else
		{
			return BadRequest();
		}
	}


	private async Task<User> GetUser(string username)
	{
		var user = await _context.Users
			.Where(u => u.Username.ToLower() == username.ToLower())
			.FirstOrDefaultAsync();

		return user!;
	}

	private async Task<User> AuthenticateUser(string username, string password)
	{
		var user = await _context.Users
			.Where(u => u.Username.ToLower() == username.ToLower() && u.Password == password)
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

	private string GenerateToken(User user)
	{
		var secretKey = new SymmetricSecurityKey(
					Encoding.ASCII.GetBytes(
						_config.GetValue<string>("Authentication:SecretKey")!));

		var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

		List<Claim> claims = new() {
					new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
					new Claim("UserName", user.Username.ToString()),
					new Claim("FirstName", user.FirstName.ToString()),
					new Claim("LastName", user.LastName.ToString())
				};

		var token = new JwtSecurityToken(
			_config.GetValue<string>("Authentication:Issuer"),
			_config.GetValue<string>("Authentication:Audience"),
			claims,
			DateTime.UtcNow,
			DateTime.UtcNow.AddMinutes(60),
			signingCredentials);

		return new JwtSecurityTokenHandler().WriteToken(token);
	}
}