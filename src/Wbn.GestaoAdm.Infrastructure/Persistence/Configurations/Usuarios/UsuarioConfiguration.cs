using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wbn.GestaoAdm.Domain.Modules.Usuarios.Entities;
using Wbn.GestaoAdm.Infrastructure.Persistence.Configurations.Common;

namespace Wbn.GestaoAdm.Infrastructure.Persistence.Configurations.Usuarios;

public sealed class UsuarioConfiguration : BaseEntityConfiguration<Usuario>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("usuarios");

        builder.Property(usuario => usuario.PerfilId)
            .HasColumnName("perfilId")
            .HasColumnType("bigint unsigned")
            .IsRequired();

        builder.Property(usuario => usuario.Nome)
            .HasColumnName("nome")
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(usuario => usuario.Email)
            .HasColumnName("email")
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(usuario => usuario.Login)
            .HasColumnName("login")
            .HasMaxLength(80)
            .IsRequired();

        builder.Property(usuario => usuario.SenhaHash)
            .HasColumnName("senhaHash")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(usuario => usuario.Telefone)
            .HasColumnName("telefone")
            .HasMaxLength(20);

        builder.Property(usuario => usuario.Ativo)
            .HasColumnName("ativo")
            .HasColumnType("tinyint(1)")
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(usuario => usuario.UltimoLogin)
            .HasColumnName("ultimoLogin")
            .HasColumnType("datetime");

        builder.Property(usuario => usuario.DataCadastro)
            .HasColumnName("dataCadastro")
            .HasColumnType("datetime")
            .IsRequired();

        builder.Property(usuario => usuario.DataAtualizacao)
            .HasColumnName("dataAtualizacao")
            .HasColumnType("datetime");

        builder.HasOne(usuario => usuario.Perfil)
            .WithMany(perfil => perfil.Usuarios)
            .HasForeignKey(usuario => usuario.PerfilId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(usuario => usuario.Email)
            .IsUnique();

        builder.HasIndex(usuario => usuario.Login)
            .IsUnique();
    }
}
