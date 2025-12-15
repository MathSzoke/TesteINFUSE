using CreditoConstituido.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CreditoConstituido.Infrastructure.Data.Configurations;

public sealed class CreditoEntityConfiguration : IEntityTypeConfiguration<Credito>
{
    public void Configure(EntityTypeBuilder<Credito> builder)
    {
        builder.ToTable("credito");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");

        builder.Property(x => x.NumeroCredito)
            .HasColumnName("numero_credito")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.NumeroNfse)
            .HasColumnName("numero_nfse")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.DataConstituicao)
            .HasColumnName("data_constituicao")
            .HasColumnType("date")
            .IsRequired();

        builder.Property(x => x.ValorIssqn)
            .HasColumnName("valor_issqn")
            .HasPrecision(15, 2)
            .IsRequired();

        builder.Property(x => x.TipoCredito)
            .HasColumnName("tipo_credito")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.SimplesNacional)
            .HasColumnName("simples_nacional")
            .IsRequired();

        builder.Property(x => x.Aliquota)
            .HasColumnName("aliquota")
            .HasPrecision(5, 2)
            .IsRequired();

        builder.Property(x => x.ValorFaturado)
            .HasColumnName("valor_faturado")
            .HasPrecision(15, 2)
            .IsRequired();

        builder.Property(x => x.ValorDeducao)
            .HasColumnName("valor_deducao")
            .HasPrecision(15, 2)
            .IsRequired();

        builder.Property(x => x.BaseCalculo)
            .HasColumnName("base_calculo")
            .HasPrecision(15, 2)
            .IsRequired();

        builder.HasIndex(x => x.NumeroCredito).IsUnique();
        builder.HasIndex(x => x.NumeroNfse);
    }
}