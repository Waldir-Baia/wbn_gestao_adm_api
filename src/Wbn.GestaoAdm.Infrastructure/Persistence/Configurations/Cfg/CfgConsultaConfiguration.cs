using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wbn.GestaoAdm.Domain.Modules.Cfg.Entities;
using Wbn.GestaoAdm.Infrastructure.Persistence.Configurations.Common;
using Wbn.GestaoAdm.Infrastructure.Persistence.Seed.Cfg;

namespace Wbn.GestaoAdm.Infrastructure.Persistence.Configurations.Cfg;

public sealed class CfgConsultaConfiguration : IEntityTypeConfiguration<CfgConsulta>
{
    public void Configure(EntityTypeBuilder<CfgConsulta> builder)
    {
        builder.ToTable("cfgWeb");
        builder.HasKey(cfg => cfg.Identificador);

        builder.Ignore(cfg => cfg.Id);
        builder.Ignore(cfg => cfg.Deletado);
        builder.Ignore(cfg => cfg.Errors);
        builder.Ignore(cfg => cfg.HasErrors);

        builder.Property(cfg => cfg.Identificador)
            .HasColumnName("identificador")
            .HasColumnType("varchar(200)");

        builder.Property(cfg => cfg.BuscaIdentificador)
            .HasColumnName("buscaIdentificador")
            .HasColumnType("varchar(150)");

        builder.Property(cfg => cfg.BuscaDescricao)
            .HasColumnName("buscaDescricao")
            .HasColumnType("varchar(150)");

        builder.Property(cfg => cfg.BuscarCodigoAlternativo)
            .HasColumnName("buscarCodigoAlternativo")
            .HasColumnType("varchar(150)");

        builder.Property(cfg => cfg.Descricao)
            .HasColumnName("descricao")
            .HasColumnType("varchar(100)")
            .IsRequired();

        builder.Property(cfg => cfg.CampoChavePrimaria)
            .HasColumnName("campoChavePrimaria")
            .HasColumnType("varchar(150)")
            .IsRequired();

        builder.Property(cfg => cfg.Sql)
            .HasColumnName("sql")
            .HasColumnType("text")
            .IsRequired();

        builder.Property(cfg => cfg.Observacao)
            .HasColumnName("observacao")
            .HasColumnType("text");

        builder.HasMany(cfg => cfg.SubCfgCampos)
            .WithOne(subCfg => subCfg.CfgConsulta)
            .HasForeignKey(subCfg => subCfg.IdentificadorCfg)
            .HasPrincipalKey(cfg => cfg.Identificador)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasData(CfgSeedData.GetAll());
    }
}
