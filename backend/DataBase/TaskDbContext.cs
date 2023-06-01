using backend.DataBase.Models;
using Microsoft.EntityFrameworkCore;

public class TaskDbContext : DbContext
{
    public DbSet<Task> Tasks { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Post> Posts { get; set; }
    public string DbPath { get; }
    public TaskDbContext()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = System.IO.Path.Join(path, "TaskDB.db");
    }
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite(@"Data Source=DataBase\TaskDB.db");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Task>().ToTable("Tasks");
        modelBuilder.Entity<User>().ToTable("Users");
        modelBuilder.Entity<Post>().ToTable("Posts");
    }
}
