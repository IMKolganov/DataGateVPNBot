using DataGateVPNBot.DataBase.ConfigurationModels.Seeds;
using DataGateVPNBot.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataGateVPNBot.DataBase.ConfigurationModels;

public class LocalizationTextConfiguration : IEntityTypeConfiguration<LocalizationText>
{
    public void Configure(EntityTypeBuilder<LocalizationText> entity)
    {
        entity.HasKey(e => e.Id);

        entity.Property(e => e.Key)
            .IsRequired()
            .HasMaxLength(255);

        entity.Property(e => e.Language)
            .IsRequired()
            .HasConversion<int>();

        entity.Property(e => e.Text)
            .IsRequired();
        
        entity.HasData(LocalizationTextSeedData.GetData());
    }
}