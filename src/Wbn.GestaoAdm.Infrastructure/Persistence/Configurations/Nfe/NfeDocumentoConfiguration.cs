using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wbn.GestaoAdm.Domain.Modules.Nfe.Entities;
using Wbn.GestaoAdm.Infrastructure.Persistence.Configurations.Common;

namespace Wbn.GestaoAdm.Infrastructure.Persistence.Configurations.Nfe;

public sealed class NfeDocumentoConfiguration : BaseEntityConfiguration<NfeDocumento>
{
    protected override void ConfigureEntity(EntityTypeBuilder<NfeDocumento> builder)
    {
        builder.ToTable("nfeDocumentos");

        builder.Property(d => d.EmpresaId)
            .HasColumnName("empresaId")
            .HasColumnType("bigint unsigned")
            .IsRequired();

        builder.Property(d => d.ChaveAcesso)
            .HasColumnName("chaveAcesso")
            .HasMaxLength(44)
            .IsRequired();

        builder.Property(d => d.Nsu)
            .HasColumnName("nsu")
            .HasColumnType("bigint");

        builder.Property(d => d.TipoDocumento)
            .HasColumnName("tipoDocumento")
            .HasMaxLength(50)
            .HasConversion<string>();

        builder.Property(d => d.SchemaDocumento)
            .HasColumnName("schemaDocumento")
            .HasMaxLength(100);

        builder.Property(d => d.CnpjEmitente)
            .HasColumnName("cnpjEmitente")
            .HasMaxLength(14);

        builder.Property(d => d.NomeEmitente)
            .HasColumnName("nomeEmitente")
            .HasMaxLength(255);

        builder.Property(d => d.CnpjDestinatario)
            .HasColumnName("cnpjDestinatario")
            .HasMaxLength(14);

        builder.Property(d => d.NomeDestinatario)
            .HasColumnName("nomeDestinatario")
            .HasMaxLength(255);

        builder.Property(d => d.NumeroNota)
            .HasColumnName("numeroNota")
            .HasMaxLength(20);

        builder.Property(d => d.Serie)
            .HasColumnName("serie")
            .HasMaxLength(10);

        builder.Property(d => d.DataEmissao)
            .HasColumnName("dataEmissao")
            .HasColumnType("datetime");

        builder.Property(d => d.ValorTotal)
            .HasColumnName("valorTotal")
            .HasPrecision(15, 2);

        builder.Property(d => d.StatusManifestacao)
            .HasColumnName("statusManifestacao")
            .HasMaxLength(50)
            .HasConversion<string>();

        builder.Property(d => d.XmlResumo)
            .HasColumnName("xmlResumo")
            .HasColumnType("longtext");

        builder.Property(d => d.XmlCompleto)
            .HasColumnName("xmlCompleto")
            .HasColumnType("longtext");

        builder.Property(d => d.XmlEvento)
            .HasColumnName("xmlEvento")
            .HasColumnType("longtext");

        builder.Property(d => d.ProtocoloManifestacao)
            .HasColumnName("protocoloManifestacao")
            .HasMaxLength(100);

        builder.Property(d => d.RetornoSefaz)
            .HasColumnName("retornoSefaz")
            .HasColumnType("longtext");

        builder.Property(d => d.DataDownload)
            .HasColumnName("dataDownload")
            .HasColumnType("datetime");

        builder.Property(d => d.DataCadastro)
            .HasColumnName("dataCriacao")
            .HasColumnType("datetime")
            .IsRequired();

        builder.Property(d => d.DataAtualizacao)
            .HasColumnName("dataAtualizacao")
            .HasColumnType("datetime");

        builder.HasOne(d => d.Empresa)
            .WithMany(e => e.NfeDocumentos)
            .HasForeignKey(d => d.EmpresaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(d => d.ChaveAcesso)
            .IsUnique();

        builder.HasIndex(d => d.EmpresaId);
    }
}
