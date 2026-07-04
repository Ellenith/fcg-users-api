using FCG.API.Domain.Identidade;
using Microsoft.EntityFrameworkCore;

namespace FCG.API.Infraestrutura.Persistencia;

public class FGCDbContext : DbContext
{
    public FGCDbContext(DbContextOptions<FGCDbContext> options)
        : base(options)
    {
    }

    // Tabela: Usuarios
    public DbSet<Usuario> Usuarios { get; set; }

    // Apenas o contexto de usuários permanece no microsserviço.

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ── Configuração da entidade Usuario ──
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("Usuarios");

            entity.HasKey(u => u.Id);

            entity.Property(u => u.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(200);

            entity.HasIndex(u => u.Email)
                .IsUnique();

            entity.Property(u => u.PasswordHash)
                .IsRequired();

            entity.Property(u => u.Role)
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(u => u.CreatedAt)
                .IsRequired();
        });

    }
}