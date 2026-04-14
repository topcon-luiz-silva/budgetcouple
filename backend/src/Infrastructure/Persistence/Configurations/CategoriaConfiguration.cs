namespace BudgetCouple.Infrastructure.Persistence.Configurations;

using BudgetCouple.Domain.Accounting;
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

        // Seed data: Despesa categories (13)
        SeedCategories(builder);
    }

    private static void SeedCategories(EntityTypeBuilder<Categoria> builder)
    {
        // DESPESA categories (13)
        builder.HasData(
            Categoria.CreateForSeed(Guid.Parse("550e8400-e29b-41d4-a716-446655440001"), "Moradia", TipoCategoria.DESPESA, "home", "#DC2626", false),
            Categoria.CreateForSeed(Guid.Parse("550e8400-e29b-41d4-a716-446655440002"), "Alimentação", TipoCategoria.DESPESA, "utensils", "#F97316", false),
            Categoria.CreateForSeed(Guid.Parse("550e8400-e29b-41d4-a716-446655440003"), "Transporte", TipoCategoria.DESPESA, "car", "#EAB308", false),
            Categoria.CreateForSeed(Guid.Parse("550e8400-e29b-41d4-a716-446655440004"), "Saúde", TipoCategoria.DESPESA, "heart-pulse", "#EC4899", false),
            Categoria.CreateForSeed(Guid.Parse("550e8400-e29b-41d4-a716-446655440005"), "Educação", TipoCategoria.DESPESA, "graduation-cap", "#3B82F6", false),
            Categoria.CreateForSeed(Guid.Parse("550e8400-e29b-41d4-a716-446655440006"), "Lazer", TipoCategoria.DESPESA, "film", "#8B5CF6", false),
            Categoria.CreateForSeed(Guid.Parse("550e8400-e29b-41d4-a716-446655440007"), "Assinaturas", TipoCategoria.DESPESA, "repeat", "#06B6D4", false),
            Categoria.CreateForSeed(Guid.Parse("550e8400-e29b-41d4-a716-446655440008"), "Vestuário", TipoCategoria.DESPESA, "shirt", "#D946EF", false),
            Categoria.CreateForSeed(Guid.Parse("550e8400-e29b-41d4-a716-446655440009"), "Pets", TipoCategoria.DESPESA, "paw-print", "#14B8A6", false),
            Categoria.CreateForSeed(Guid.Parse("550e8400-e29b-41d4-a716-44665544000a"), "Presentes", TipoCategoria.DESPESA, "gift", "#F43F5E", false),
            Categoria.CreateForSeed(Guid.Parse("550e8400-e29b-41d4-a716-44665544000b"), "Impostos e Taxas", TipoCategoria.DESPESA, "receipt", "#64748B", false),
            Categoria.CreateForSeed(Guid.Parse("550e8400-e29b-41d4-a716-44665544000c"), "Serviços Financeiros", TipoCategoria.DESPESA, "landmark", "#6366F1", false),
            Categoria.CreateForSeed(Guid.Parse("550e8400-e29b-41d4-a716-44665544000d"), "Outros", TipoCategoria.DESPESA, "more-horizontal", "#78716C", false),

            // RECEITA categories (6)
            Categoria.CreateForSeed(Guid.Parse("550e8400-e29b-41d4-a716-44665544000e"), "Salário", TipoCategoria.RECEITA, "wallet", "#10B981", false),
            Categoria.CreateForSeed(Guid.Parse("550e8400-e29b-41d4-a716-44665544000f"), "Bonificação", TipoCategoria.RECEITA, "award", "#84CC16", false),
            Categoria.CreateForSeed(Guid.Parse("550e8400-e29b-41d4-a716-446655440010"), "Rendimentos", TipoCategoria.RECEITA, "trending-up", "#06B6D4", false),
            Categoria.CreateForSeed(Guid.Parse("550e8400-e29b-41d4-a716-446655440011"), "Freelance", TipoCategoria.RECEITA, "briefcase", "#8B5CF6", false),
            Categoria.CreateForSeed(Guid.Parse("550e8400-e29b-41d4-a716-446655440012"), "Reembolso", TipoCategoria.RECEITA, "undo-2", "#14B8A6", false),
            Categoria.CreateForSeed(Guid.Parse("550e8400-e29b-41d4-a716-446655440013"), "Outros", TipoCategoria.RECEITA, "more-horizontal", "#78716C", false),

            // System category: Credit Card Payment (1)
            Categoria.CreateForSeed(Guid.Parse("550e8400-e29b-41d4-a716-446655440014"), "Pagamento de Fatura de Cartão", TipoCategoria.DESPESA, "credit-card", "#0891B2", true)
        );
    }
}
