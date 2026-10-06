namespace ResumeForge.Models;

public class AppUser
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string Role { get; set; } = "User";      // "Admin" or "User"
}

public class Resume
{
    public int Id { get; set; }
    public int UserId { get; set; }
    [Required, StringLength(80)] public string Title { get; set; }
    [Required, StringLength(80)] public string FullName { get; set; }
    [Required, EmailAddress] public string Email { get; set; }
    [StringLength(20)] public string Phone { get; set; }
    [StringLength(150)] public string Address { get; set; }
    [StringLength(600)] public string Summary { get; set; }
    [StringLength(400)] public string Skills { get; set; }        // comma separated
    [StringLength(1000)] public string Education { get; set; }    // one item per line
    [StringLength(1500)] public string Experience { get; set; }   // one item per line
    [StringLength(1500)] public string Projects { get; set; }     // one item per line
    public string Template { get; set; } = "Classic";             // Classic, Modern or Minimal
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

public class UserRow
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    public int Resumes { get; set; }
}

public class RegisterVm
{
    [Required, StringLength(80)] public string Name { get; set; }
    [Required, EmailAddress] public string Email { get; set; }
    [Required, MinLength(6, ErrorMessage = "Password needs at least 6 characters.")] public string Password { get; set; }
}

public class LoginVm
{
    [Required, EmailAddress] public string Email { get; set; }
    [Required] public string Password { get; set; }
}

public class AppDb : DbContext
{
    public AppDb(DbContextOptions<AppDb> options) : base(options) { }
    public DbSet<AppUser> Users { get; set; }
    public DbSet<Resume> Resumes { get; set; }

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<AppUser>().HasIndex(u => u.Email).IsUnique();
        mb.Entity<Resume>().HasOne<AppUser>().WithMany().HasForeignKey(r => r.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}
