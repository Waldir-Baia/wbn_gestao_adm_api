using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wbn.GestaoAdm.Domain.Modules.Recebimentos.Entities;
using Wbn.GestaoAdm.Infrastructure.Persistence.Configurations.Common;

namespace Wbn.GestaoAdm.Infrastructure.Persistence.Configurations.Recebimentos;

public sealed class NotaFiscalBoletoConfiguration : BaseEntityConfiguration<NotaFiscalBoleto>
{
    protected override void ConfigureEntity(EntityTypeBuilder<NotaFiscalBoleto> builder)
    {
        builder.ToTable("notaFiscalBoleto");

        builder.Property(notaFiscalBoleto => notaFiscalBoleto.NotaFiscalId)
            .HasColumnName("notaFiscalId")
            .HasColumnType("bigint unsigned")
            .IsRequired();

        builder.Property(notaFiscalBoleto => notaFiscalBoleto.BoletoId)
            .HasColumnName("boletoId")
            .HasColumnType("bigint unsigned")
            .IsRequired();

        builder.Property(notaFiscalBoleto => notaFiscalBoleto.DataCadastro)
            .HasColumnName("dataCadastro")
            .HasColumnType("datetime")
            .IsRequired();

        builder.HasOne(notaFiscalBoleto => notaFiscalBoleto.NotaFiscal)
            .WithMany(notaFiscal => notaFiscal.NotasFiscaisBoletos)
            .HasForeignKey(notaFiscalBoleto => notaFiscalBoleto.NotaFiscalId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(notaFiscalBoleto => notaFiscalBoleto.Boleto)
            .WithMany(boleto => boleto.NotasFiscaisBoletos)
            .HasForeignKey(notaFiscalBoleto => notaFiscalBoleto.BoletoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(notaFiscalBoleto => new { notaFiscalBoleto.NotaFiscalId, notaFiscalBoleto.BoletoId })
            .IsUnique();
    }
}
