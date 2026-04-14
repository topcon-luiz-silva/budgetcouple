namespace BudgetCouple.Infrastructure.Persistence.Configurations;

using BudgetCouple.Domain.Accounting.Recorrencias;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class RecorrenciaConfiguration : IEntityTypeConfiguration<Recorrencia>
{
    public void Configure(EntityTypeBuilder<Recorrencia> builder)
    {
        builder.ToTable("recorrencias");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(x => x.Frequencia)
            .HasColumnName("frequencia")
            .HasColumnType("varchar(20)")
            .HasConversion<string>()
            .IsRequired();

        builder.Property(x => x.DataInicio)
            .HasColumnName("data_inicio")
            .HasColumnType("date");

        builder.Property(x => x.DataFim)
            .HasColumnName("data_fim")
            .HasColumnType("date");

        builder.Property(x => x.TemplateJson)
            .HasColumnName("template_json")
            .HasColumnType("text")
            .IsRequired();

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
