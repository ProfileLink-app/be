using ProfileLink.Data;
using System.ComponentModel.DataAnnotations;

namespace ProfileLinkTest.DTOs;

public class UserDTO
{
	public int UserId { get; set; }

	[Required]
	public string? Username { get; set; }

	[Required]
	public string? FirstName { get; set; }

	[Required]
	public string? LastName { get; set; }

	public string? Bio { get; set; }

	public string? Theme { get; set; }

	public ICollection<Social>? Socials { get; set; }

	public ICollection<Link>? Links { get; set; }
}
