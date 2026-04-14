namespace BudgetCouple.Infrastructure.Persistence.Configurations;

using BudgetCouple.Domain.Accounting.Lancamentos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class LancamentoAnexoConfiguration : IEntityTypeConfiguration<LancamentoAnexo>
{
    public void Configure(EntityTypeBuilder<LancamentoAnexo> builder)
    {
        builder.ToTable("lancamento_anexos");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(x => x.LancamentoId)
            .HasColumnName("lancamento_id")
            .IsRequired();

        builder.Property(x => x.NomeArquivo)
            .HasColumnName("nome_arquivo")
            .HasColumnType("varchar(255)")
            .IsRequired();

        builder.Property(x => x.ContentType)
            .HasColumnName("content_type")
            .HasColumnType("varchar(100)")
            .IsRequired();

        builder.Property(x => x.TamanhoBytes)
            .HasColumnName("tamanho_bytes")
            .IsRequired();

        builder.Property(x => x.CaminhoStorage)
            .HasColumnName("caminho_storage")
            .HasColumnType("varchar(500)")
            .IsRequired();

        builder.Property(x => x.EnviadoEm)
            .HasColumnName("enviado_em")
            .HasColumnType("timestamp with time zone")
            .ValueGeneratedOnAdd();

        // Foreign key
        builder.HasOne<Lancamento>()
            .WithMany(l => l.Anexos_EF)
            .HasForeignKey(x => x.LancamentoId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(x => x.LancamentoId).HasDatabaseName("ix_lanc_anexo_lancamento_id");
    }
}
