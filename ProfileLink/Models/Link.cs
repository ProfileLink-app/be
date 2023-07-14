using System.Text.Json.Serialization;

namespace ProfileLink.Data;

public partial class Link
{
	public int LinkId { get; set; }

	public string Title { get; set; } = null!;

	public string Url { get; set; } = null!;

	public bool Active { get; set; }

	public int? UserId { get; set; }

	[JsonIgnore]
	public User? User { get; set; }
}
