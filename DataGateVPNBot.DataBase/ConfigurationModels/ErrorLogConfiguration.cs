using DataGateVPNBot.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataGateVPNBot.DataBase.ConfigurationModels;

public class ErrorLogConfiguration : IEntityTypeConfiguration<ErrorLog>
{
    public void Configure(EntityTypeBuilder<ErrorLog> entity)
    {
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Message).IsRequired().HasMaxLength(1000);
        entity.Property(e => e.StackTrace).HasMaxLength(4000);
        entity.Property(e => e.Timestamp).IsRequired();
        entity.Property(e => e.Source).HasMaxLength(255);
    }
}