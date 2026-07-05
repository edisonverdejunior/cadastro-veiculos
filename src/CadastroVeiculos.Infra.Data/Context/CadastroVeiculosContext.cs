using CadastroVeiculo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CadastroVeiculos.Infra.Data.Context
{
    public class CadastroVeiculosContext(DbContextOptions<CadastroVeiculosContext> options) : DbContext(options)
    {
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Veiculo> Veiculos { get; set; }
        public DbSet<MfaRecoveryCode> MfaRecoveryCodes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nome).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Login).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Senha).IsRequired();
                entity.HasIndex(e => e.Login).IsUnique();

                entity.Property(e => e.MfaEnabled).IsRequired().HasDefaultValue(false);
                entity.Property(e => e.MfaSecret);
                entity.Property(e => e.MfaEnrolledAt);

                entity.HasMany(e => e.MfaRecoveryCodes)
                      .WithOne(c => c.Usuario!)
                      .HasForeignKey(c => c.UsuarioId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<MfaRecoveryCode>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CodeHash).IsRequired();
                entity.Property(e => e.UsedAt);
                entity.HasIndex(e => e.UsuarioId);
            });

            modelBuilder.Entity<Veiculo>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Descricao).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Marca).IsRequired();
                entity.Property(e => e.Modelo).IsRequired().HasMaxLength(30);
                entity.Property(e => e.Opcionais).HasMaxLength(500);
                entity.Property(e => e.Valor).HasPrecision(18, 2);
            });
        }
    }
}
