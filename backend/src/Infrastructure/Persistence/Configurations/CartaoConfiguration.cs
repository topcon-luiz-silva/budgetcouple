namespace BudgetCouple.Infrastructure.Persistence.Configurations;

using BudgetCouple.Domain.Accounting.Cartoes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class CartaoConfiguration : IEntityTypeConfiguration<Cartao>
{
    public void Configure(EntityTypeBuilder<Cartao> builder)
    {
        builder.ToTable("cartoes");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(x => x.Nome)
            .HasColumnName("nome")
            .HasColumnType("varchar(100)")
            .IsRequired();

        builder.Property(x => x.Bandeira)
            .HasColumnName("bandeira")
            .HasColumnType("varchar(20)")
            .IsRequired();

        builder.Property(x => x.DiaFechamento)
            .HasColumnName("dia_fechamento")
            .IsRequired();

        builder.Property(x => x.DiaVencimento)
            .HasColumnName("dia_vencimento")
            .IsRequired();

        builder.Property(x => x.Limite)
            .HasColumnName("limite")
            .HasColumnType("numeric(18,2)");

        builder.Property(x => x.ContaPagamentoId)
            .HasColumnName("conta_pagamento_id")
            .IsRequired();

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
