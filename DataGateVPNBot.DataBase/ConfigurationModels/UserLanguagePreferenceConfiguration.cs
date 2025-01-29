using DataGateVPNBot.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataGateVPNBot.DataBase.ConfigurationModels;

public class UserLanguagePreferenceConfiguration : IEntityTypeConfiguration<UserLanguagePreference>
{
    public void Configure(EntityTypeBuilder<UserLanguagePreference> entity)
    {
        entity.HasKey(e => e.Id);
        entity.Property(e => e.TelegramId).IsRequired();
        entity.Property(e => e.PreferredLanguage).IsRequired()
            .HasConversion<int>(); 
    }
}