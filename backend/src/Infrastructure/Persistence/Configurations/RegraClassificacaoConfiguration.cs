namespace BudgetCouple.Infrastructure.Persistence.Configurations;

using BudgetCouple.Domain.Imports;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class RegraClassificacaoConfiguration : IEntityTypeConfiguration<RegraClassificacao>
{
    public void Configure(EntityTypeBuilder<RegraClassificacao> builder)
    {
        builder.ToTable("regras_classificacao");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(x => x.Padrao)
            .HasColumnName("padrao")
            .HasColumnType("varchar(200)")
            .IsRequired();

        builder.Property(x => x.TipoPadrao)
            .HasColumnName("tipo_padrao")
            .HasColumnType("varchar(10)")
            .HasConversion<string>();

        builder.Property(x => x.CategoriaId)
            .HasColumnName("categoria_id");

        builder.Property(x => x.SubcategoriaId)
            .HasColumnName("subcategoria_id");

        builder.Property(x => x.Prioridade)
            .HasColumnName("prioridade")
            .HasDefaultValue(100);

        builder.Property(x => x.CriadoEm)
            .HasColumnName("criado_em")
            .HasColumnType("timestamp with time zone")
            .ValueGeneratedOnAdd();
    }
}
