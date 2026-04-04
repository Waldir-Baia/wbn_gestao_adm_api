using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wbn.GestaoAdm.Domain.Modules.TiposDocumento.Entities;
using Wbn.GestaoAdm.Infrastructure.Persistence.Configurations.Common;

namespace Wbn.GestaoAdm.Infrastructure.Persistence.Configurations.TiposDocumento;

public sealed class TipoDocumentoConfiguration : BaseEntityConfiguration<TipoDocumento>
{
    protected override void ConfigureEntity(EntityTypeBuilder<TipoDocumento> builder)
    {
        builder.ToTable("tiposDocumento");

        builder.Property(tipoDocumento => tipoDocumento.Nome)
            .HasColumnName("nome")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(tipoDocumento => tipoDocumento.Descricao)
            .HasColumnName("descricao")
            .HasColumnType("text");

        builder.Property(tipoDocumento => tipoDocumento.Ativo)
            .HasColumnName("ativo")
            .HasColumnType("tinyint(1)")
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(tipoDocumento => tipoDocumento.DataCadastro)
            .HasColumnName("dataCadastro")
            .HasColumnType("datetime")
            .IsRequired();

        builder.HasIndex(tipoDocumento => tipoDocumento.Nome)
            .IsUnique();
    }
}
