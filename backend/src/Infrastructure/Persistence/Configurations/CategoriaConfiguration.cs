namespace BudgetCouple.Infrastructure.Persistence.Configurations;

using BudgetCouple.Domain.Accounting.Categorias;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class CategoriaConfiguration : IEntityTypeConfiguration<Categoria>
{
    public void Configure(EntityTypeBuilder<Categoria> builder)
    {
        builder.ToTable("categorias");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(x => x.Nome)
            .HasColumnName("nome")
            .HasColumnType("varchar(100)")
            .IsRequired();

        builder.Property(x => x.Tipo)
            .HasColumnName("tipo")
            .HasColumnType("varchar(20)")
            .HasConversion<string>()
            .IsRequired();

        builder.Property(x => x.Icone)
            .HasColumnName("icone")
            .HasColumnType("varchar(50)");

        builder.Property(x => x.Cor)
            .HasColumnName("cor")
            .HasColumnType("varchar(7)");

        builder.Property(x => x.Sistema)
            .HasColumnName("sistema")
            .HasDefaultValue(false);

        builder.Property(x => x.Ativa)
            .HasColumnName("ativa")
            .HasDefaultValue(true);

        builder.Property(x => x.CriadoEm)
            .HasColumnName("criado_em")
            .HasColumnType("timestamp with time zone")
            .ValueGeneratedOnAdd();

        builder.Property(x => x.AtualizadoEm)
            .HasColumnName("atualizado_em")
            .HasColumnType("timestamp with time zone")
            .ValueGeneratedOnAddOrUpdate();

        // Subcategorias as owned type
        builder.OwnsMany(x => x.Subcategorias, sb =>
        {
            sb.ToTable("subcategorias");
            sb.HasKey("Id");
            sb.Property(s => s.Id).HasColumnName("id").ValueGeneratedNever();
            sb.Property(s => s.Nome).HasColumnName("nome").HasColumnType("varchar(100)").IsRequired();
            sb.Property(s => s.Ativa).HasColumnName("ativa").HasDefaultValue(true);
            sb.Property(s => s.CriadoEm).HasColumnName("criado_em").HasColumnType("timestamp with time zone").ValueGeneratedOnAdd();
            sb.Property(s => s.AtualizadoEm).HasColumnName("atualizado_em").HasColumnType("timestamp with time zone").ValueGeneratedOnAddOrUpdate();
            sb.WithOwner().HasForeignKey("CategoriaId");
            sb.HasKey("Id");
        });
    }
}
