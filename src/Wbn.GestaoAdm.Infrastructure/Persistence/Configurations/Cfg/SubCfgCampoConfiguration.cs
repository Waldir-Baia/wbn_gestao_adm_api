using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wbn.GestaoAdm.Domain.Modules.Cfg.Entities;
using Wbn.GestaoAdm.Infrastructure.Persistence.Seed.Cfg;

namespace Wbn.GestaoAdm.Infrastructure.Persistence.Configurations.Cfg;

public sealed class SubCfgCampoConfiguration : IEntityTypeConfiguration<SubCfgCampo>
{
    public void Configure(EntityTypeBuilder<SubCfgCampo> builder)
    {
        builder.ToTable("subCFGWeb");
        builder.HasKey(subCfg => subCfg.Identificador);

        builder.Ignore(subCfg => subCfg.Id);
        builder.Ignore(subCfg => subCfg.Deletado);
        builder.Ignore(subCfg => subCfg.Errors);
        builder.Ignore(subCfg => subCfg.HasErrors);

        builder.Property(subCfg => subCfg.Identificador)
            .HasColumnName("identificador")
            .HasColumnType("varchar(200)");

        builder.Property(subCfg => subCfg.IdentificadorCfg)
            .HasColumnName("identificadorCfg")
            .HasColumnType("varchar(200)")
            .IsRequired();

        builder.Property(subCfg => subCfg.Campo)
            .HasColumnName("campo")
            .HasColumnType("varchar(100)")
            .IsRequired();

        builder.Property(subCfg => subCfg.Descricao)
            .HasColumnName("descricao")
            .HasColumnType("varchar(100)")
            .IsRequired();

        builder.Property(subCfg => subCfg.TipoDados)
            .HasColumnName("tipoDados")
            .HasColumnType("varchar(50)")
            .IsRequired();

        builder.Property(subCfg => subCfg.OrdemCampo)
            .HasColumnName("ordemCampo")
            .HasColumnType("int")
            .IsRequired();

        builder.Property(subCfg => subCfg.PermitirFiltro)
            .HasColumnName("permitirFiltro")
            .HasColumnType("tinyint(1)")
            .IsRequired();

        builder.Property(subCfg => subCfg.Visivel)
            .HasColumnName("visivel")
            .HasColumnType("tinyint(1)")
            .IsRequired();

        builder.Property(subCfg => subCfg.CampoBusca)
            .HasColumnName("campoBusca")
            .HasColumnType("varchar(1000)")
            .IsRequired();

        builder.Property(subCfg => subCfg.Mascara)
            .HasColumnName("mascara")
            .HasColumnType("varchar(100)");

        builder.Property(subCfg => subCfg.LarguraColuna)
            .HasColumnName("larguraColuna")
            .HasColumnType("int")
            .IsRequired();

        builder.HasData(SubCfgSeedData.GetAll());
    }
}
