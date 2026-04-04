using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wbn.GestaoAdm.Domain.Modules.Recebimentos.Entities;
using Wbn.GestaoAdm.Infrastructure.Persistence.Configurations.Common;

namespace Wbn.GestaoAdm.Infrastructure.Persistence.Configurations.Recebimentos;

public sealed class NotaFiscalConfiguration : BaseEntityConfiguration<NotaFiscal>
{
    protected override void ConfigureEntity(EntityTypeBuilder<NotaFiscal> builder)
    {
        builder.ToTable("notasFiscais");

        builder.Property(notaFiscal => notaFiscal.RecebimentoId)
            .HasColumnName("recebimentoId")
            .HasColumnType("bigint unsigned")
            .IsRequired();

        builder.Property(notaFiscal => notaFiscal.ArquivoId)
            .HasColumnName("arquivoId")
            .HasColumnType("bigint unsigned");

        builder.Property(notaFiscal => notaFiscal.NumeroNota)
            .HasColumnName("numeroNota")
            .HasMaxLength(50);

        builder.Property(notaFiscal => notaFiscal.Serie)
            .HasColumnName("serie")
            .HasMaxLength(20);

        builder.Property(notaFiscal => notaFiscal.ChaveAcesso)
            .HasColumnName("chaveAcesso")
            .HasMaxLength(60);

        builder.Property(notaFiscal => notaFiscal.ValorTotal)
            .HasColumnName("valorTotal")
            .HasPrecision(15, 2)
            .IsRequired();

        builder.Property(notaFiscal => notaFiscal.DataEmissao)
            .HasColumnName("dataEmissao")
            .HasColumnType("date");

        builder.Property(notaFiscal => notaFiscal.DataEntrada)
            .HasColumnName("dataEntrada")
            .HasColumnType("date");

        builder.Property(notaFiscal => notaFiscal.CpfCnpjEmitente)
            .HasColumnName("cpfCnpjEmitente")
            .HasMaxLength(18);

        builder.Property(notaFiscal => notaFiscal.NomeEmitente)
            .HasColumnName("nomeEmitente")
            .HasMaxLength(150);

        builder.Property(notaFiscal => notaFiscal.Observacao)
            .HasColumnName("observacao")
            .HasColumnType("text");

        builder.Property(notaFiscal => notaFiscal.DataCadastro)
            .HasColumnName("dataCadastro")
            .HasColumnType("datetime")
            .IsRequired();

        builder.Property(notaFiscal => notaFiscal.DataAtualizacao)
            .HasColumnName("dataAtualizacao")
            .HasColumnType("datetime");

        builder.HasOne(notaFiscal => notaFiscal.Recebimento)
            .WithMany(recebimento => recebimento.NotasFiscais)
            .HasForeignKey(notaFiscal => notaFiscal.RecebimentoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(notaFiscal => notaFiscal.Arquivo)
            .WithMany()
            .HasForeignKey(notaFiscal => notaFiscal.ArquivoId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(notaFiscal => notaFiscal.ChaveAcesso)
            .IsUnique();
    }
}
