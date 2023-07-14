using System;
using System.Collections.Generic;

namespace ProfileLink.Data;

public partial class User
{
	public string Username { get; set; } = null!;

	public string Password { get; set; } = null!;

	public string FirstName { get; set; } = null!;

	public string LastName { get; set; } = null!;

	public string Bio { get; set; } = null!;

	public string Theme { get; set; } = null!;

	public int UserId { get; set; }

	public ICollection<Link> Links { get; set; } = new List<Link>();

	public ICollection<Social> Socials { get; set; } = new List<Social>();
}
