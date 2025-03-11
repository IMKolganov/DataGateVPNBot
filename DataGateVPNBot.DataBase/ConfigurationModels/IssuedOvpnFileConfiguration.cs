using DataGateVPNBot.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataGateVPNBot.DataBase.ConfigurationModels;

public class IssuedOvpnFileConfiguration : IEntityTypeConfiguration<IssuedOvpnFile>
{
    public void Configure(EntityTypeBuilder<IssuedOvpnFile> entity)
    {
        entity.HasKey(e => e.Id);
            
        entity.Property(e => e.CertName)
            .IsRequired()
            .HasMaxLength(255);
            
        entity.Property(e => e.CertId)
            .HasMaxLength(255);
            
        entity.Property(e => e.FileName)
            .IsRequired()
            .HasMaxLength(255);

        entity.Property(e => e.FilePath)
            .IsRequired()
            .HasMaxLength(500);

        entity.Property(e => e.IssuedAt)
            .IsRequired();

        entity.Property(e => e.IssuedTo)
            .IsRequired()
            .HasMaxLength(255);

        entity.Property(e => e.KeyFilePath)
            .IsRequired()
            .HasMaxLength(500);

        entity.Property(e => e.ReqFilePath)
            .IsRequired()
            .HasMaxLength(500);

        entity.Property(e => e.CertFilePath)
            .IsRequired()
            .HasMaxLength(500);
            
        entity.Property(e => e.Message)
            .HasMaxLength(500);
    }
}