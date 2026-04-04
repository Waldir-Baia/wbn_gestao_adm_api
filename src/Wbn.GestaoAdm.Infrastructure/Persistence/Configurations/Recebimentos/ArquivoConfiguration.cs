using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wbn.GestaoAdm.Domain.Modules.Recebimentos.Entities;
using Wbn.GestaoAdm.Infrastructure.Persistence.Configurations.Common;

namespace Wbn.GestaoAdm.Infrastructure.Persistence.Configurations.Recebimentos;

public sealed class ArquivoConfiguration : BaseEntityConfiguration<Arquivo>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Arquivo> builder)
    {
        builder.ToTable("arquivos");

        builder.Property(arquivo => arquivo.RecebimentoId)
            .HasColumnName("recebimentoId")
            .HasColumnType("bigint unsigned")
            .IsRequired();

        builder.Property(arquivo => arquivo.TipoDocumentoId)
            .HasColumnName("tipoDocumentoId")
            .HasColumnType("bigint unsigned")
            .IsRequired();

        builder.Property(arquivo => arquivo.NomeOriginal)
            .HasColumnName("nomeOriginal")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(arquivo => arquivo.NomeArquivo)
            .HasColumnName("nomeArquivo")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(arquivo => arquivo.CaminhoArquivo)
            .HasColumnName("caminhoArquivo")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(arquivo => arquivo.Extensao)
            .HasColumnName("extensao")
            .HasMaxLength(20);

        builder.Property(arquivo => arquivo.MimeType)
            .HasColumnName("mimeType")
            .HasMaxLength(100);

        builder.Property(arquivo => arquivo.TamanhoBytes)
            .HasColumnName("tamanhoBytes")
            .HasColumnType("bigint")
            .IsRequired();

        builder.Property(arquivo => arquivo.OrdemExibicao)
            .HasColumnName("ordemExibicao")
            .IsRequired();

        builder.Property(arquivo => arquivo.Ativo)
            .HasColumnName("ativo")
            .HasColumnType("tinyint(1)")
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(arquivo => arquivo.DataUpload)
            .HasColumnName("dataUpload")
            .HasColumnType("datetime")
            .IsRequired();

        builder.HasOne(arquivo => arquivo.Recebimento)
            .WithMany(recebimento => recebimento.Arquivos)
            .HasForeignKey(arquivo => arquivo.RecebimentoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(arquivo => arquivo.TipoDocumento)
            .WithMany(tipoDocumento => tipoDocumento.Arquivos)
            .HasForeignKey(arquivo => arquivo.TipoDocumentoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
