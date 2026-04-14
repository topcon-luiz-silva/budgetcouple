namespace BudgetCouple.Infrastructure.Persistence.Configurations;

using BudgetCouple.Domain.Accounting.Lancamentos;
using BudgetCouple.Domain.Accounting.Lancamentos.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class LancamentoConfiguration : IEntityTypeConfiguration<Lancamento>
{
    public void Configure(EntityTypeBuilder<Lancamento> builder)
    {
        builder.ToTable("lancamentos");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(x => x.Tipo)
            .HasColumnName("tipo")
            .HasColumnType("varchar(20)")
            .HasConversion<string>()
            .IsRequired();

        builder.Property(x => x.Natureza)
            .HasColumnName("natureza")
            .HasColumnType("varchar(20)")
            .HasConversion<string>()
            .IsRequired();

        builder.Property(x => x.Valor)
            .HasColumnName("valor")
            .HasColumnType("numeric(18,2)");

        builder.Property(x => x.Data)
            .HasColumnName("data")
            .HasColumnType("date");

        builder.Property(x => x.ContaId)
            .HasColumnName("conta_id");

        builder.Property(x => x.CartaoId)
            .HasColumnName("cartao_id");

        builder.Property(x => x.CategoriaId)
            .HasColumnName("categoria_id");

        builder.Property(x => x.SubcategoriaId)
            .HasColumnName("subcategoria_id");

        builder.Property(x => x.Descricao)
            .HasColumnName("descricao")
            .HasColumnType("varchar(255)");

        builder.Property(x => x.Tags)
            .HasColumnName("tags")
            .HasColumnType("text[]")
            .HasDefaultValue(new List<string>());

        builder.Property(x => x.StatusPagamento)
            .HasColumnName("status_pagamento")
            .HasColumnType("varchar(12)")
            .HasConversion<string>()
            .IsRequired();

        builder.Property(x => x.DataPagamento)
            .HasColumnName("data_pagamento")
            .HasColumnType("date");

        builder.Property(x => x.IsParcelada)
            .HasColumnName("is_parcelada")
            .HasDefaultValue(false);

        builder.Property(x => x.IsRecorrente)
            .HasColumnName("is_recorrente")
            .HasDefaultValue(false);

        builder.Property(x => x.RecorrenciaId)
            .HasColumnName("recorrencia_id");

        builder.Property(x => x.Anexos)
            .HasColumnName("anexos")
            .HasColumnType("text[]")
            .HasDefaultValue(new List<string>());

        builder.Property(x => x.Classificacao)
            .HasColumnName("classificacao")
            .HasColumnType("varchar(20)")
            .HasConversion<string>()
            .IsRequired();

        builder.Property(x => x.FaturaPaga)
            .HasColumnName("fatura_paga")
            .HasDefaultValue(false);

        builder.Property(x => x.FaturaPagaEm)
            .HasColumnName("fatura_paga_em")
            .HasColumnType("timestamp with time zone");

        builder.Property(x => x.CriadoEm)
            .HasColumnName("criado_em")
            .HasColumnType("timestamp with time zone")
            .ValueGeneratedOnAdd();

        builder.Property(x => x.AtualizadoEm)
            .HasColumnName("atualizado_em")
            .HasColumnType("timestamp with time zone")
            .ValueGeneratedOnAddOrUpdate();

        // DadosParcelamento as owned type
        builder.OwnsOne(x => x.DadosParcelamento, dp =>
        {
            dp.Property(d => d.ParcelaAtual)
                .HasColumnName("parcela_atual");

            dp.Property(d => d.TotalParcelas)
                .HasColumnName("total_parcelas");

            dp.Property(d => d.LancamentoPaiId)
                .HasColumnName("lancamento_pai_id");
        });

        // Foreign key constraint for origem
        builder.ToTable(t => t.HasCheckConstraint("chk_origem",
            "(conta_id IS NOT NULL AND cartao_id IS NULL) OR " +
            "(conta_id IS NULL AND cartao_id IS NOT NULL) OR " +
            "(conta_id IS NULL AND cartao_id IS NULL AND natureza = 'PREVISTA')"));

        // Indexes
        builder.HasIndex(x => x.Data).HasDatabaseName("ix_lanc_data");
        builder.HasIndex(x => new { x.CartaoId, x.Data }).HasDatabaseName("ix_lanc_cartao_data");
        builder.HasIndex(x => new { x.ContaId, x.Data }).HasDatabaseName("ix_lanc_conta_data");
        builder.HasIndex(x => new { x.CategoriaId, x.Data }).HasDatabaseName("ix_lanc_cat_data");
    }
}
