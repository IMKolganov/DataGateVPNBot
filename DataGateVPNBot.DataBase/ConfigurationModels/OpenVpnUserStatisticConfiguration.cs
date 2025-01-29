using DataGateVPNBot.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataGateVPNBot.DataBase.ConfigurationModels;

public class OpenVpnUserStatisticConfiguration : IEntityTypeConfiguration<OpenVpnUserStatistic>
{
    public void Configure(EntityTypeBuilder<OpenVpnUserStatistic> entity)
    {
        entity.HasKey(e => e.Id);
        entity.Property(e => e.SessionId)
            .IsRequired();
        entity.Property(e => e.CommonName)
            .HasMaxLength(255)
            .IsRequired();
        entity.Property(e => e.RealAddress)
            .HasMaxLength(255)
            .IsRequired();
        entity.Property(e => e.BytesReceived)
            .IsRequired();
        entity.Property(e => e.BytesSent)
            .IsRequired();
        entity.Property(e => e.ConnectedSince)
            .IsRequired();
        entity.Property(e => e.LastUpdated)
            .IsRequired();
    }
}