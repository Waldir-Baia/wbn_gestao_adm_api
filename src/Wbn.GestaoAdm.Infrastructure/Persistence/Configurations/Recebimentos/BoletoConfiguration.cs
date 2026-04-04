using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wbn.GestaoAdm.Domain.Modules.Recebimentos.Entities;
using Wbn.GestaoAdm.Infrastructure.Persistence.Configurations.Common;

namespace Wbn.GestaoAdm.Infrastructure.Persistence.Configurations.Recebimentos;

public sealed class BoletoConfiguration : BaseEntityConfiguration<Boleto>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Boleto> builder)
    {
        builder.ToTable("boletos");

        builder.Property(boleto => boleto.RecebimentoId)
            .HasColumnName("recebimentoId")
            .HasColumnType("bigint unsigned")
            .IsRequired();

        builder.Property(boleto => boleto.ArquivoId)
            .HasColumnName("arquivoId")
            .HasColumnType("bigint unsigned");

        builder.Property(boleto => boleto.CodigoBarras)
            .HasColumnName("codigoBarras")
            .HasMaxLength(60);

        builder.Property(boleto => boleto.LinhaDigitavel)
            .HasColumnName("linhaDigitavel")
            .HasMaxLength(80);

        builder.Property(boleto => boleto.ValorBoleto)
            .HasColumnName("valorBoleto")
            .HasPrecision(15, 2)
            .IsRequired();

        builder.Property(boleto => boleto.DataVencimento)
            .HasColumnName("dataVencimento")
            .HasColumnType("date");

        builder.Property(boleto => boleto.DataEmissao)
            .HasColumnName("dataEmissao")
            .HasColumnType("date");

        builder.Property(boleto => boleto.Beneficiario)
            .HasColumnName("beneficiario")
            .HasMaxLength(150);

        builder.Property(boleto => boleto.DocumentoBeneficiario)
            .HasColumnName("documentoBeneficiario")
            .HasMaxLength(18);

        builder.Property(boleto => boleto.Observacao)
            .HasColumnName("observacao")
            .HasColumnType("text");

        builder.Property(boleto => boleto.DataCadastro)
            .HasColumnName("dataCadastro")
            .HasColumnType("datetime")
            .IsRequired();

        builder.Property(boleto => boleto.DataAtualizacao)
            .HasColumnName("dataAtualizacao")
            .HasColumnType("datetime");

        builder.HasOne(boleto => boleto.Recebimento)
            .WithMany(recebimento => recebimento.Boletos)
            .HasForeignKey(boleto => boleto.RecebimentoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(boleto => boleto.Arquivo)
            .WithMany()
            .HasForeignKey(boleto => boleto.ArquivoId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(boleto => boleto.LinhaDigitavel);
    }
}
