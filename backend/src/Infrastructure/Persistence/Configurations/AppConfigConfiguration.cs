namespace BudgetCouple.Infrastructure.Persistence.Configurations;

using BudgetCouple.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class AppConfigConfiguration : IEntityTypeConfiguration<AppConfig>
{
    public void Configure(EntityTypeBuilder<AppConfig> builder)
    {
        builder.ToTable("app_config");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(x => x.PinHash)
            .HasColumnName("pin_hash")
            .HasColumnType("varchar(255)")
            .IsRequired();

        builder.Property(x => x.PinSetAt)
            .HasColumnName("pin_set_at")
            .HasColumnType("timestamp with time zone");

        builder.Property(x => x.FailedAttempts)
            .HasColumnName("failed_attempts")
            .HasDefaultValue(0);

        builder.Property(x => x.LockedUntil)
            .HasColumnName("locked_until")
            .HasColumnType("timestamp with time zone");

        builder.Property(x => x.Tema)
            .HasColumnName("tema")
            .HasColumnType("varchar(20)")
            .HasDefaultValue("light");

        builder.Property(x => x.EmailNotificacao)
            .HasColumnName("email_notificacao")
            .HasColumnType("varchar(255)");

        builder.Property(x => x.TelegramChatIds)
            .HasColumnName("telegram_chat_ids")
            .HasColumnType("text[]")
            .HasDefaultValue(new List<string>());
    }
}
