using Microsoft.EntityFrameworkCore;

namespace ProfileLink.Data;

public class DataContext : DbContext
{

	public DataContext()
	{
	}

	public DataContext(DbContextOptions<DataContext> options)
		: base(options)
	{
	}

	public virtual DbSet<Link> Links { get; set; }

	public virtual DbSet<Social> Socials { get; set; }

	public virtual DbSet<User> Users { get; set; }

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
		=> optionsBuilder.UseSqlServer("Data Source=profile-link.database.windows.net;Initial Catalog=profile-link;User ID=dylan;Password=@Michael1216;Connect Timeout=30;Encrypt=True;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Link>(entity =>
		{
			entity.HasKey(e => e.LinkId).HasName("PK__Links__2D122135611CBFF0");

			entity.HasOne(d => d.User).WithMany(p => p.Links)
				.HasForeignKey(d => d.UserId)
				.HasConstraintName("FK__Links__UserId__6E01572D");
		});

		modelBuilder.Entity<Social>(entity =>
		{
			entity.HasKey(e => e.SocialId).HasName("PK__Socials__67CF711A91610A6B");

			entity.HasOne(d => d.User).WithMany(p => p.Socials)
				.HasForeignKey(d => d.UserId)
				.HasConstraintName("FK__Socials__UserId__6B24EA82");
		});

		modelBuilder.Entity<User>(entity =>
		{
			entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4C519190E4");
		});

	}

}










//    public DataContext(DbContextOptions<DataContext> options) : base(options)
//    {
//    }

//    public DbSet<User> Users { get; set; }
//    public DbSet<Social> Socials { get; set; }
//    public DbSet<Link> Links { get; set; }

//    protected override void OnModelCreating(ModelBuilder modelBuilder)
//    {
//		modelBuilder.Entity<Link>(entity =>
//		{
//			entity.HasKey(e => e.LinkId).HasName("PK__Links__2D122135611CBFF0");

//			entity.Property(e => e.Title).HasColumnType("text");
//			entity.Property(e => e.Url).HasColumnType("text");

//			entity.HasOne(d => d.User).WithMany(p => p.Links)
//				.HasForeignKey(d => d.UserId)
//				.HasConstraintName("FK__Links__UserId__6E01572D");
//		});

//		modelBuilder.Entity<Social>(entity =>
//		{
//			entity.HasKey(e => e.SocialId).HasName("PK__Socials__67CF711A91610A6B");

//			entity.Property(e => e.Platform).HasColumnType("text");
//			entity.Property(e => e.Url).HasColumnType("text");
//			entity.Property(e => e.Username).HasColumnType("text");

//			entity.HasOne(d => d.User).WithMany(p => p.Socials)
//				.HasForeignKey(d => d.UserId)
//				.HasConstraintName("FK__Socials__UserId__6B24EA82");
//		});

//		modelBuilder.Entity<User>(entity =>
//		{
//			entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4C519190E4");

//			entity.Property(e => e.Bio).HasColumnType("text");
//			entity.Property(e => e.FirstName).HasColumnType("text");
//			entity.Property(e => e.LastName).HasColumnType("text");
//			entity.Property(e => e.Password).HasColumnType("text");
//			entity.Property(e => e.Theme).HasColumnType("text");
//			entity.Property(e => e.Username).HasColumnType("text");
//		});
//	}
//}
