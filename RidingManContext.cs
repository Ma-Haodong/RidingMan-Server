using Microsoft.EntityFrameworkCore;

public class User
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Password { get; set; }
}
public class RidingManContext : DbContext
{
    public DbSet<User> Users { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseMySql("server=localhost;uid=root;pwd=root;database=riding_man", ServerVersion.Parse("8.0.0-mysql"));
    }
}