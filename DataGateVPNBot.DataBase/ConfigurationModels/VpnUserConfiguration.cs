using DataGateVPNBot.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataGateVPNBot.DataBase.ConfigurationModels;

public class VpnUserConfiguration : IEntityTypeConfiguration<VpnUser>
{
    public void Configure(EntityTypeBuilder<VpnUser> entity)
    {
        entity.HasKey(e => e.Id);
        entity.Property(e => e.TelegramId).IsRequired();
        entity.Property(e => e.Username).HasMaxLength(50);
        entity.HasIndex(e => e.Username)
            .IsUnique();
        entity.Property(e => e.PasswordHash).HasMaxLength(200);
        entity.Property(e => e.CreatedAt).IsRequired();
    }
}