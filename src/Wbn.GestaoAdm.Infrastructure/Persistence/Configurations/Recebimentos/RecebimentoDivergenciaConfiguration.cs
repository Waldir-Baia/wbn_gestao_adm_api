using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wbn.GestaoAdm.Domain.Modules.Recebimentos.Entities;
using Wbn.GestaoAdm.Infrastructure.Persistence.Configurations.Common;

namespace Wbn.GestaoAdm.Infrastructure.Persistence.Configurations.Recebimentos;

public sealed class RecebimentoDivergenciaConfiguration : BaseEntityConfiguration<RecebimentoDivergencia>
{
    protected override void ConfigureEntity(EntityTypeBuilder<RecebimentoDivergencia> builder)
    {
        builder.ToTable("recebimentoDivergencias");

        builder.Property(recebimentoDivergencia => recebimentoDivergencia.RecebimentoId)
            .HasColumnName("recebimentoId")
            .HasColumnType("bigint unsigned")
            .IsRequired();

        builder.Property(recebimentoDivergencia => recebimentoDivergencia.UsuarioId)
            .HasColumnName("usuarioId")
            .HasColumnType("bigint unsigned")
            .IsRequired();

        builder.Property(recebimentoDivergencia => recebimentoDivergencia.TipoDivergencia)
            .HasColumnName("tipoDivergencia")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(recebimentoDivergencia => recebimentoDivergencia.Descricao)
            .HasColumnName("descricao")
            .HasColumnType("text")
            .IsRequired();

        builder.Property(recebimentoDivergencia => recebimentoDivergencia.Resolvida)
            .HasColumnName("resolvida")
            .HasColumnType("tinyint(1)")
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(recebimentoDivergencia => recebimentoDivergencia.DataResolucao)
            .HasColumnName("dataResolucao")
            .HasColumnType("datetime");

        builder.Property(recebimentoDivergencia => recebimentoDivergencia.UsuarioResolucaoId)
            .HasColumnName("usuarioResolucaoId")
            .HasColumnType("bigint unsigned");

        builder.Property(recebimentoDivergencia => recebimentoDivergencia.ObservacaoResolucao)
            .HasColumnName("observacaoResolucao")
            .HasColumnType("text");

        builder.Property(recebimentoDivergencia => recebimentoDivergencia.DataCadastro)
            .HasColumnName("dataCadastro")
            .HasColumnType("datetime")
            .IsRequired();

        builder.Property(recebimentoDivergencia => recebimentoDivergencia.DataAtualizacao)
            .HasColumnName("dataAtualizacao")
            .HasColumnType("datetime");

        builder.HasOne(recebimentoDivergencia => recebimentoDivergencia.Recebimento)
            .WithMany(recebimento => recebimento.Divergencias)
            .HasForeignKey(recebimentoDivergencia => recebimentoDivergencia.RecebimentoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(recebimentoDivergencia => recebimentoDivergencia.Usuario)
            .WithMany(usuario => usuario.DivergenciasRegistradas)
            .HasForeignKey(recebimentoDivergencia => recebimentoDivergencia.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(recebimentoDivergencia => recebimentoDivergencia.UsuarioResolucao)
            .WithMany(usuario => usuario.DivergenciasResolvidas)
            .HasForeignKey(recebimentoDivergencia => recebimentoDivergencia.UsuarioResolucaoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
