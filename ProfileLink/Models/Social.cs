using System.Text.Json.Serialization;

namespace ProfileLink.Data;

public partial class Social
{
	public int SocialId { get; set; }

	public string Platform { get; set; } = null!;

	public string Username { get; set; } = null!;

	public string Url { get; set; } = null!;

	public int? UserId { get; set; }

	[JsonIgnore]
	public User? User { get; set; }
}
