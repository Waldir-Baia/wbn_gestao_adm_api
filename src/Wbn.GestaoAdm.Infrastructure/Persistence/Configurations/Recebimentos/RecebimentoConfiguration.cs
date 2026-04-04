using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wbn.GestaoAdm.Domain.Modules.Recebimentos.Entities;
using Wbn.GestaoAdm.Infrastructure.Persistence.Configurations.Common;

namespace Wbn.GestaoAdm.Infrastructure.Persistence.Configurations.Recebimentos;

public sealed class RecebimentoConfiguration : BaseEntityConfiguration<Recebimento>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Recebimento> builder)
    {
        builder.ToTable("recebimentos");

        builder.Property(recebimento => recebimento.EmpresaId)
            .HasColumnName("empresaId")
            .HasColumnType("bigint unsigned")
            .IsRequired();

        builder.Property(recebimento => recebimento.UsuarioEnvioId)
            .HasColumnName("usuarioEnvioId")
            .HasColumnType("bigint unsigned")
            .IsRequired();

        builder.Property(recebimento => recebimento.CodigoRecebimento)
            .HasColumnName("codigoRecebimento")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(recebimento => recebimento.StatusRecebimento)
            .HasColumnName("statusRecebimento")
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(recebimento => recebimento.Origem)
            .HasColumnName("origem")
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(recebimento => recebimento.Observacao)
            .HasColumnName("observacao")
            .HasColumnType("text");

        builder.Property(recebimento => recebimento.DataEnvio)
            .HasColumnName("dataEnvio")
            .HasColumnType("datetime")
            .IsRequired();

        builder.Property(recebimento => recebimento.DataRecebimento)
            .HasColumnName("dataRecebimento")
            .HasColumnType("datetime")
            .IsRequired();

        builder.Property(recebimento => recebimento.DataAtualizacao)
            .HasColumnName("dataAtualizacao")
            .HasColumnType("datetime")
            .IsRequired();

        builder.HasOne(recebimento => recebimento.Empresa)
            .WithMany(empresa => empresa.Recebimentos)
            .HasForeignKey(recebimento => recebimento.EmpresaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(recebimento => recebimento.UsuarioEnvio)
            .WithMany(usuario => usuario.RecebimentosEnviados)
            .HasForeignKey(recebimento => recebimento.UsuarioEnvioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(recebimento => recebimento.CodigoRecebimento)
            .IsUnique();
    }
}
