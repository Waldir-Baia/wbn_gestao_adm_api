using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wbn.GestaoAdm.Domain.Modules.Perfis.Entities;
using Wbn.GestaoAdm.Infrastructure.Persistence.Configurations.Common;

namespace Wbn.GestaoAdm.Infrastructure.Persistence.Configurations.Perfis;

public sealed class PerfilConfiguration : BaseEntityConfiguration<Perfil>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Perfil> builder)
    {
        builder.ToTable("perfis");

        builder.Property(perfil => perfil.Nome)
            .HasColumnName("nome")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(perfil => perfil.Descricao)
            .HasColumnName("descricao")
            .HasColumnType("text");

        builder.Property(perfil => perfil.Ativo)
            .HasColumnName("ativo")
            .HasColumnType("tinyint(1)")
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(perfil => perfil.DataCadastro)
            .HasColumnName("dataCadastro")
            .HasColumnType("datetime")
            .IsRequired();

        builder.Property(perfil => perfil.DataAtualizacao)
            .HasColumnName("dataAtualizacao")
            .HasColumnType("datetime");

        builder.HasIndex(perfil => perfil.Nome)
            .IsUnique();
    }
}
