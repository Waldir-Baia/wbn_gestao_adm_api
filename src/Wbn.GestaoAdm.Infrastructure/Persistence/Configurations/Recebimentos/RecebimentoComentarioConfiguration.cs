using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wbn.GestaoAdm.Domain.Modules.Recebimentos.Entities;
using Wbn.GestaoAdm.Infrastructure.Persistence.Configurations.Common;

namespace Wbn.GestaoAdm.Infrastructure.Persistence.Configurations.Recebimentos;

public sealed class RecebimentoComentarioConfiguration : BaseEntityConfiguration<RecebimentoComentario>
{
    protected override void ConfigureEntity(EntityTypeBuilder<RecebimentoComentario> builder)
    {
        builder.ToTable("recebimentoComentarios");

        builder.Property(recebimentoComentario => recebimentoComentario.RecebimentoId)
            .HasColumnName("recebimentoId")
            .HasColumnType("bigint unsigned")
            .IsRequired();

        builder.Property(recebimentoComentario => recebimentoComentario.UsuarioId)
            .HasColumnName("usuarioId")
            .HasColumnType("bigint unsigned")
            .IsRequired();

        builder.Property(recebimentoComentario => recebimentoComentario.Comentario)
            .HasColumnName("comentario")
            .HasColumnType("text")
            .IsRequired();

        builder.Property(recebimentoComentario => recebimentoComentario.VisivelParaEmpresa)
            .HasColumnName("visivelParaEmpresa")
            .HasColumnType("tinyint(1)")
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(recebimentoComentario => recebimentoComentario.DataCadastro)
            .HasColumnName("dataCadastro")
            .HasColumnType("datetime")
            .IsRequired();

        builder.Property(recebimentoComentario => recebimentoComentario.DataAtualizacao)
            .HasColumnName("dataAtualizacao")
            .HasColumnType("datetime");

        builder.HasOne(recebimentoComentario => recebimentoComentario.Recebimento)
            .WithMany(recebimento => recebimento.Comentarios)
            .HasForeignKey(recebimentoComentario => recebimentoComentario.RecebimentoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(recebimentoComentario => recebimentoComentario.Usuario)
            .WithMany(usuario => usuario.Comentarios)
            .HasForeignKey(recebimentoComentario => recebimentoComentario.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
