using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wbn.GestaoAdm.Domain.Modules.Recebimentos.Entities;
using Wbn.GestaoAdm.Infrastructure.Persistence.Configurations.Common;

namespace Wbn.GestaoAdm.Infrastructure.Persistence.Configurations.Recebimentos;

public sealed class RecebimentoHistoricoConfiguration : BaseEntityConfiguration<RecebimentoHistorico>
{
    protected override void ConfigureEntity(EntityTypeBuilder<RecebimentoHistorico> builder)
    {
        builder.ToTable("recebimentoHistoricos");

        builder.Property(recebimentoHistorico => recebimentoHistorico.RecebimentoId)
            .HasColumnName("recebimentoId")
            .HasColumnType("bigint unsigned")
            .IsRequired();

        builder.Property(recebimentoHistorico => recebimentoHistorico.UsuarioId)
            .HasColumnName("usuarioId")
            .HasColumnType("bigint unsigned");

        builder.Property(recebimentoHistorico => recebimentoHistorico.Acao)
            .HasColumnName("acao")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(recebimentoHistorico => recebimentoHistorico.Descricao)
            .HasColumnName("descricao")
            .HasColumnType("text")
            .IsRequired();

        builder.Property(recebimentoHistorico => recebimentoHistorico.DataCadastro)
            .HasColumnName("dataCadastro")
            .HasColumnType("datetime")
            .IsRequired();

        builder.HasOne(recebimentoHistorico => recebimentoHistorico.Recebimento)
            .WithMany(recebimento => recebimento.Historicos)
            .HasForeignKey(recebimentoHistorico => recebimentoHistorico.RecebimentoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(recebimentoHistorico => recebimentoHistorico.Usuario)
            .WithMany(usuario => usuario.Historicos)
            .HasForeignKey(recebimentoHistorico => recebimentoHistorico.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
