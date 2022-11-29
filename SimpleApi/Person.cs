using Microsoft.EntityFrameworkCore;

namespace SimpleApi;

public class Person
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public int? Age { get; set; }
}

public sealed class PersonContext : DbContext
{
    public DbSet<Person> Users { get; set; } = null!;
    
    public PersonContext(DbContextOptions<PersonContext> options) : base(options)
    {
        Database.EnsureCreated();
    }
}