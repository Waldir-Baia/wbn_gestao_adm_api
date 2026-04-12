using Microsoft.EntityFrameworkCore;
using Wbn.GestaoAdm.Domain.Modules.Cfg.Entities;
using Wbn.GestaoAdm.Domain.Modules.Empresas.Entities;
using Wbn.GestaoAdm.Domain.Modules.Perfis.Entities;
using Wbn.GestaoAdm.Domain.Modules.Recebimentos.Entities;
using Wbn.GestaoAdm.Domain.Modules.TiposDocumento.Entities;
using Wbn.GestaoAdm.Domain.Modules.Usuarios.Entities;
using Wbn.GestaoAdm.Domain.Modules.UsuariosEmpresas.Entities;

namespace Wbn.GestaoAdm.Infrastructure.Persistence.Contexts;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<CfgConsulta> CfgConsultas => Set<CfgConsulta>();
    public DbSet<SubCfgCampo> SubCfgCampos => Set<SubCfgCampo>();
    public DbSet<Perfil> Perfis => Set<Perfil>();
    public DbSet<Empresa> Empresas => Set<Empresa>();
    public DbSet<TipoDocumento> TiposDocumento => Set<TipoDocumento>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<UsuarioEmpresa> UsuariosEmpresas => Set<UsuarioEmpresa>();
    public DbSet<Recebimento> Recebimentos => Set<Recebimento>();
    public DbSet<Arquivo> Arquivos => Set<Arquivo>();
    public DbSet<NotaFiscal> NotasFiscais => Set<NotaFiscal>();
    public DbSet<Boleto> Boletos => Set<Boleto>();
    public DbSet<NotaFiscalBoleto> NotasFiscaisBoletos => Set<NotaFiscalBoleto>();
    public DbSet<RecebimentoConferencia> RecebimentoConferencias => Set<RecebimentoConferencia>();
    public DbSet<RecebimentoDivergencia> RecebimentoDivergencias => Set<RecebimentoDivergencia>();
    public DbSet<RecebimentoHistorico> RecebimentoHistoricos => Set<RecebimentoHistorico>();
    public DbSet<RecebimentoComentario> RecebimentoComentarios => Set<RecebimentoComentario>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
