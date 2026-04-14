namespace BudgetCouple.Infrastructure.Persistence.Configurations;

using BudgetCouple.Domain.Accounting.Contas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ContaConfiguration : IEntityTypeConfiguration<Conta>
{
    public void Configure(EntityTypeBuilder<Conta> builder)
    {
        builder.ToTable("contas");

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

        builder.Property(x => x.SaldoInicial)
            .HasColumnName("saldo_inicial")
            .HasColumnType("numeric(18,2)");

        builder.Property(x => x.Descricao)
            .HasColumnName("descricao")
            .HasColumnType("varchar(255)");

        builder.Property(x => x.Icone)
            .HasColumnName("icone")
            .HasColumnType("varchar(50)");

        builder.Property(x => x.Cor)
            .HasColumnName("cor")
            .HasColumnType("varchar(7)");

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
