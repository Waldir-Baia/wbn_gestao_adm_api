using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wbn.GestaoAdm.Domain.Modules.Recebimentos.Entities;
using Wbn.GestaoAdm.Infrastructure.Persistence.Configurations.Common;

namespace Wbn.GestaoAdm.Infrastructure.Persistence.Configurations.Recebimentos;

public sealed class RecebimentoConferenciaConfiguration : BaseEntityConfiguration<RecebimentoConferencia>
{
    protected override void ConfigureEntity(EntityTypeBuilder<RecebimentoConferencia> builder)
    {
        builder.ToTable("recebimentoConferencias");

        builder.Property(recebimentoConferencia => recebimentoConferencia.RecebimentoId)
            .HasColumnName("recebimentoId")
            .HasColumnType("bigint unsigned")
            .IsRequired();

        builder.Property(recebimentoConferencia => recebimentoConferencia.UsuarioConferenciaId)
            .HasColumnName("usuarioConferenciaId")
            .HasColumnType("bigint unsigned")
            .IsRequired();

        builder.Property(recebimentoConferencia => recebimentoConferencia.StatusConferencia)
            .HasColumnName("statusConferencia")
            .HasMaxLength(30)
            .IsRequired();


        builder.Property(recebimentoConferencia => recebimentoConferencia.NotaEncontrada)
            .HasColumnName("notaEncontrada")
            .HasColumnType("tinyint(1)")
            .IsRequired();

        builder.Property(recebimentoConferencia => recebimentoConferencia.BoletoEncontrado)
            .HasColumnName("boletoEncontrado")
            .HasColumnType("tinyint(1)")
            .IsRequired();

        builder.Property(recebimentoConferencia => recebimentoConferencia.ValorConfere)
            .HasColumnName("valorConfere")
            .HasColumnType("tinyint(1)")
            .IsRequired();

        builder.Property(recebimentoConferencia => recebimentoConferencia.DataVencimentoConfere)
            .HasColumnName("dataVencimentoConfere")
            .HasColumnType("tinyint(1)")
            .IsRequired();

        builder.Property(recebimentoConferencia => recebimentoConferencia.DocumentoConfere)
            .HasColumnName("documentoConfere")
            .HasColumnType("tinyint(1)")
            .IsRequired();

        builder.Property(recebimentoConferencia => recebimentoConferencia.Observacao)
            .HasColumnName("observacao")
            .HasColumnType("text");

        builder.Property(recebimentoConferencia => recebimentoConferencia.DataConferencia)
            .HasColumnName("dataConferencia")
            .HasColumnType("datetime")
            .IsRequired();

        builder.Property(recebimentoConferencia => recebimentoConferencia.DataCadastro)
            .HasColumnName("dataCadastro")
            .HasColumnType("datetime")
            .IsRequired();

        builder.HasOne(recebimentoConferencia => recebimentoConferencia.Recebimento)
            .WithMany(recebimento => recebimento.Conferencias)
            .HasForeignKey(recebimentoConferencia => recebimentoConferencia.RecebimentoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(recebimentoConferencia => recebimentoConferencia.UsuarioConferencia)
            .WithMany(usuario => usuario.Conferencias)
            .HasForeignKey(recebimentoConferencia => recebimentoConferencia.UsuarioConferenciaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
