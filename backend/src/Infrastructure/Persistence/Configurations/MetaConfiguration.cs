namespace BudgetCouple.Infrastructure.Persistence.Configurations;

using BudgetCouple.Domain.Budgeting.Metas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class MetaConfiguration : IEntityTypeConfiguration<Meta>
{
    public void Configure(EntityTypeBuilder<Meta> builder)
    {
        builder.ToTable("metas");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(x => x.Tipo)
            .HasColumnName("tipo")
            .HasColumnType("varchar(20)")
            .IsRequired();

        builder.Property(x => x.Nome)
            .HasColumnName("nome")
            .HasColumnType("varchar(100)")
            .IsRequired();

        builder.Property(x => x.ValorAlvo)
            .HasColumnName("valor_alvo")
            .HasColumnType("numeric(18,2)");

        builder.Property(x => x.ValorAtual)
            .HasColumnName("valor_atual")
            .HasColumnType("numeric(18,2)")
            .HasDefaultValue(0);

        builder.Property(x => x.DataLimite)
            .HasColumnName("data_limite")
            .HasColumnType("date");

        builder.Property(x => x.CategoriaId)
            .HasColumnName("categoria_id");

        builder.Property(x => x.Periodo)
            .HasColumnName("periodo")
            .HasColumnType("varchar(10)");

        builder.Property(x => x.PercentualAlerta)
            .HasColumnName("percentual_alerta")
            .HasDefaultValue(80);

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
    }
}
