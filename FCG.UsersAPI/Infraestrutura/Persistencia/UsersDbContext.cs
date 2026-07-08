// ============================================================
// UsersDbContext.cs — Contexto EF Core do UsersAPI
//
// Banco próprio do microsserviço — isolado dos demais.
// Arquivo: users.db (SQLite)
// ============================================================

using FCG.UsersAPI.Domain.Entidades;
using Microsoft.EntityFrameworkCore;

namespace FCG.UsersAPI.Infraestrutura.Persistencia;

public class UsersDbContext : DbContext
{
    public UsersDbContext(DbContextOptions<UsersDbContext> options)
        : base(options) { }

    public DbSet<Usuario> Usuarios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("Usuarios");
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Name).IsRequired().HasMaxLength(100);
            entity.Property(u => u.Email).IsRequired().HasMaxLength(200);
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.PasswordHash).IsRequired();
            entity.Property(u => u.Role).IsRequired().HasMaxLength(20);
            entity.Property(u => u.CreatedAt).IsRequired();
        });
    }
}