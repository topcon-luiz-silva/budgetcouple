namespace BudgetCouple.Infrastructure.Persistence.Configurations;

using BudgetCouple.Domain.Notifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class NotificationHistoryConfiguration : IEntityTypeConfiguration<NotificationHistory>
{
    public void Configure(EntityTypeBuilder<NotificationHistory> builder)
    {
        builder.ToTable("notification_history");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(x => x.Canal)
            .HasColumnName("canal")
            .HasConversion<string>();

        builder.Property(x => x.Tipo)
            .HasColumnName("tipo")
            .HasConversion<string>();

        builder.Property(x => x.Titulo)
            .HasColumnName("titulo")
            .HasColumnType("varchar(255)");

        builder.Property(x => x.Corpo)
            .HasColumnName("corpo")
            .HasColumnType("text");

        builder.Property(x => x.Status)
            .HasColumnName("status")
            .HasConversion<string>();

        builder.Property(x => x.Erro)
            .HasColumnName("erro")
            .HasColumnType("text");

        builder.Property(x => x.EnviadoEm)
            .HasColumnName("enviado_em")
            .HasColumnType("timestamp with time zone");
    }
}
