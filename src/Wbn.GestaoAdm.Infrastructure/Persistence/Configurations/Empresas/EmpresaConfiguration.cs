using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wbn.GestaoAdm.Domain.Modules.Empresas.Entities;
using Wbn.GestaoAdm.Infrastructure.Persistence.Configurations.Common;

namespace Wbn.GestaoAdm.Infrastructure.Persistence.Configurations.Empresas;

public sealed class EmpresaConfiguration : BaseEntityConfiguration<Empresa>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Empresa> builder)
    {
        builder.ToTable("empresas");

        builder.Property(empresa => empresa.NomeFantasia)
            .HasColumnName("nomeFantasia")
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(empresa => empresa.RazaoSocial)
            .HasColumnName("razaoSocial")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(empresa => empresa.Cnpj)
            .HasColumnName("cnpj")
            .HasMaxLength(14)
            .IsRequired();

        builder.Property(empresa => empresa.CodigoInterno)
            .HasColumnName("codigoInterno")
            .HasMaxLength(50);

        builder.Property(empresa => empresa.Email)
            .HasColumnName("email")
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(empresa => empresa.Telefone)
            .HasColumnName("telefone")
            .HasMaxLength(20);

        builder.Property(empresa => empresa.Ativo)
            .HasColumnName("ativo")
            .HasColumnType("tinyint(1)")
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(empresa => empresa.DataCadastro)
            .HasColumnName("dataCadastro")
            .HasColumnType("datetime")
            .IsRequired();

        builder.Property(empresa => empresa.DataAtualizacao)
            .HasColumnName("dataAtualizacao")
            .HasColumnType("datetime");

        builder.HasIndex(empresa => empresa.Cnpj)
            .IsUnique();

        builder.HasIndex(empresa => empresa.CodigoInterno)
            .IsUnique();
    }
}
