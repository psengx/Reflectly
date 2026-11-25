using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence;

public class ApplicationContext : DbContext
{
    public DbSet<TemplateEntity> Templates { get; set; }
    public DbSet<TemplateFieldEntity> TemplateFields { get; set; }
    public DbSet<EntryEntity> Entries { get; set; }
    
    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }
    public ApplicationContext(): base() { }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql();
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<TemplateFieldEntity>()
            .Property(u => u.FieldType)
            .HasConversion<string>();

        modelBuilder.Entity<EntryEntity>()
            .Property(e => e.Answers)
            .HasColumnType("jsonb");
        
        modelBuilder.Entity<EntryEntity>()
            .Property(e => e.FieldDefinitionSnapshot)
            .HasColumnType("jsonb");

        modelBuilder.Entity<TemplateFieldEntity>()
            .Property(e => e.Settings)
            .HasColumnType("jsonb");
    }
}