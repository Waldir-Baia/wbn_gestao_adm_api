using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wbn.GestaoAdm.Domain.Modules.Nfe.Entities;
using Wbn.GestaoAdm.Infrastructure.Persistence.Configurations.Common;

namespace Wbn.GestaoAdm.Infrastructure.Persistence.Configurations.Nfe;

public sealed class NfeProdutoConfiguration : BaseEntityConfiguration<NfeProduto>
{
    protected override void ConfigureEntity(EntityTypeBuilder<NfeProduto> builder)
    {
        builder.ToTable("nfeProdutos");

        builder.Property(p => p.NfeDocumentoId)
            .HasColumnName("nfeDocumentoId")
            .HasColumnType("bigint unsigned")
            .IsRequired();

        builder.Property(p => p.CodigoProduto)
            .HasColumnName("codigoProduto")
            .HasMaxLength(100);

        builder.Property(p => p.NomeProduto)
            .HasColumnName("nomeProduto")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(p => p.Descricao)
            .HasColumnName("descricao")
            .HasMaxLength(1000);

        builder.Property(p => p.Ncm)
            .HasColumnName("ncm")
            .HasMaxLength(20);

        builder.Property(p => p.Cfop)
            .HasColumnName("cfop")
            .HasMaxLength(10);

        builder.Property(p => p.Unidade)
            .HasColumnName("unidade")
            .HasMaxLength(20);

        builder.Property(p => p.Quantidade)
            .HasColumnName("quantidade")
            .HasPrecision(15, 4);

        builder.Property(p => p.ValorUnitario)
            .HasColumnName("valorUnitario")
            .HasPrecision(15, 4);

        builder.Property(p => p.ValorTotal)
            .HasColumnName("valorTotal")
            .HasPrecision(15, 2);

        builder.Property(p => p.Ean)
            .HasColumnName("ean")
            .HasMaxLength(50);

        builder.Property(p => p.DataCadastro)
            .HasColumnName("dataCriacao")
            .HasColumnType("datetime")
            .IsRequired();

        builder.Property(p => p.DataAtualizacao)
            .HasColumnName("dataAtualizacao")
            .HasColumnType("datetime");

        builder.HasOne(p => p.NfeDocumento)
            .WithMany(d => d.Produtos)
            .HasForeignKey(p => p.NfeDocumentoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(p => p.NfeDocumentoId);
    }
}
