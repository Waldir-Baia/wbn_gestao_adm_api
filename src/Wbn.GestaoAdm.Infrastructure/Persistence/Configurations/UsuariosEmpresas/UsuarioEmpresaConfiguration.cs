using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wbn.GestaoAdm.Domain.Modules.UsuariosEmpresas.Entities;
using Wbn.GestaoAdm.Infrastructure.Persistence.Configurations.Common;

namespace Wbn.GestaoAdm.Infrastructure.Persistence.Configurations.UsuariosEmpresas;

public sealed class UsuarioEmpresaConfiguration : BaseEntityConfiguration<UsuarioEmpresa>
{
    protected override void ConfigureEntity(EntityTypeBuilder<UsuarioEmpresa> builder)
    {
        builder.ToTable("usuarioEmpresa");

        builder.Property(usuarioEmpresa => usuarioEmpresa.UsuarioId)
            .HasColumnName("usuarioId")
            .HasColumnType("bigint unsigned")
            .IsRequired();

        builder.Property(usuarioEmpresa => usuarioEmpresa.EmpresaId)
            .HasColumnName("empresaId")
            .HasColumnType("bigint unsigned")
            .IsRequired();

        builder.Property(usuarioEmpresa => usuarioEmpresa.Ativo)
            .HasColumnName("ativo")
            .HasColumnType("tinyint(1)")
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(usuarioEmpresa => usuarioEmpresa.DataCadastro)
            .HasColumnName("dataCadastro")
            .HasColumnType("datetime")
            .IsRequired();

        builder.HasOne(usuarioEmpresa => usuarioEmpresa.Usuario)
            .WithMany(usuario => usuario.UsuariosEmpresas)
            .HasForeignKey(usuarioEmpresa => usuarioEmpresa.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(usuarioEmpresa => usuarioEmpresa.Empresa)
            .WithMany(empresa => empresa.UsuariosEmpresas)
            .HasForeignKey(usuarioEmpresa => usuarioEmpresa.EmpresaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(usuarioEmpresa => new { usuarioEmpresa.UsuarioId, usuarioEmpresa.EmpresaId })
            .IsUnique();
    }
}
