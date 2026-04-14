namespace BudgetCouple.Infrastructure.Persistence.Configurations;

using BudgetCouple.Domain.Notifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class NotificationPreferencesConfiguration : IEntityTypeConfiguration<NotificationPreferences>
{
    public void Configure(EntityTypeBuilder<NotificationPreferences> builder)
    {
        builder.ToTable("notification_preferences");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(x => x.EmailHabilitado)
            .HasColumnName("email_habilitado")
            .HasDefaultValue(false);

        builder.Property(x => x.EmailEndereco)
            .HasColumnName("email_endereco")
            .HasColumnType("varchar(255)");

        builder.Property(x => x.WebPushHabilitado)
            .HasColumnName("webpush_habilitado")
            .HasDefaultValue(false);

        builder.Property(x => x.TelegramHabilitado)
            .HasColumnName("telegram_habilitado")
            .HasDefaultValue(false);

        builder.Property(x => x.TelegramChatId)
            .HasColumnName("telegram_chat_id")
            .HasColumnType("varchar(255)");

        builder.Property(x => x.NotificarVencimentos1Dia)
            .HasColumnName("notificar_vencimentos_1_dia")
            .HasDefaultValue(true);

        builder.Property(x => x.NotificarVencimentosDia)
            .HasColumnName("notificar_vencimentos_dia")
            .HasDefaultValue(true);

        builder.Property(x => x.NotificarAlertasOrcamento)
            .HasColumnName("notificar_alertas_orcamento")
            .HasDefaultValue(true);

        builder.Property(x => x.NotificarFaturas)
            .HasColumnName("notificar_faturas")
            .HasDefaultValue(true);
    }
}
