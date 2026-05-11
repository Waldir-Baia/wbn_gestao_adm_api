using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Wbn.GestaoAdm.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate_Schema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "cfgWeb",
                columns: table => new
                {
                    identificador = table.Column<string>(type: "varchar(200)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    buscaIdentificador = table.Column<string>(type: "varchar(150)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    buscaDescricao = table.Column<string>(type: "varchar(150)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    buscarCodigoAlternativo = table.Column<string>(type: "varchar(150)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    descricao = table.Column<string>(type: "varchar(100)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    campoChavePrimaria = table.Column<string>(type: "varchar(150)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    sql = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    observacao = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cfgWeb", x => x.identificador);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "empresas",
                columns: table => new
                {
                    id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nomeFantasia = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    razaoSocial = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    cnpj = table.Column<string>(type: "varchar(14)", maxLength: 14, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    codigoInterno = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    email = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    telefone = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ativo = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    dataCadastro = table.Column<DateTime>(type: "datetime", nullable: false),
                    dataAtualizacao = table.Column<DateTime>(type: "datetime", nullable: true),
                    certificadoDigitalA1 = table.Column<byte[]>(type: "longblob", nullable: true),
                    certificadoDigitalSenha = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    certificadoDigitalValidade = table.Column<DateTime>(type: "datetime", nullable: true),
                    certificadoDigitalAtivo = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    nfeCodigoUf = table.Column<ushort>(type: "smallint unsigned", nullable: false, defaultValue: (ushort)0),
                    nfeUltimoNsu = table.Column<long>(type: "bigint", nullable: false, defaultValue: 0L),
                    nfeMaxNsu = table.Column<long>(type: "bigint", nullable: false, defaultValue: 0L),
                    nfeDataUltimaConsulta = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_empresas", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "perfis",
                columns: table => new
                {
                    id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nome = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    descricao = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ativo = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    dataCadastro = table.Column<DateTime>(type: "datetime", nullable: false),
                    dataAtualizacao = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_perfis", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tiposDocumento",
                columns: table => new
                {
                    id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nome = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    descricao = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ativo = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    dataCadastro = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tiposDocumento", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "subCFGWeb",
                columns: table => new
                {
                    identificador = table.Column<string>(type: "varchar(200)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    identificadorCfg = table.Column<string>(type: "varchar(200)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    campo = table.Column<string>(type: "varchar(100)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    descricao = table.Column<string>(type: "varchar(100)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    tipoDados = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ordemCampo = table.Column<int>(type: "int", nullable: false),
                    permitirFiltro = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    visivel = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    campoBusca = table.Column<string>(type: "varchar(1000)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    mascara = table.Column<string>(type: "varchar(100)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    larguraColuna = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subCFGWeb", x => x.identificador);
                    table.ForeignKey(
                        name: "FK_subCFGWeb_cfgWeb_identificadorCfg",
                        column: x => x.identificadorCfg,
                        principalTable: "cfgWeb",
                        principalColumn: "identificador",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "usuarios",
                columns: table => new
                {
                    id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    perfilId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    nome = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    email = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    login = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    senhaHash = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    telefone = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ativo = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    statusSenha = table.Column<sbyte>(type: "tinyint", nullable: false, defaultValue: (sbyte)0),
                    ultimoLogin = table.Column<DateTime>(type: "datetime", nullable: true),
                    dataCadastro = table.Column<DateTime>(type: "datetime", nullable: false),
                    dataAtualizacao = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuarios", x => x.id);
                    table.ForeignKey(
                        name: "FK_usuarios_perfis_perfilId",
                        column: x => x.perfilId,
                        principalTable: "perfis",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "recebimentos",
                columns: table => new
                {
                    id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    empresaId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    usuarioEnvioId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    codigoRecebimento = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    statusRecebimento = table.Column<int>(type: "int", nullable: false),
                    origem = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    observacao = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    dataEnvio = table.Column<DateTime>(type: "datetime", nullable: false),
                    dataRecebimento = table.Column<DateTime>(type: "datetime", nullable: false),
                    dataAtualizacao = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_recebimentos", x => x.id);
                    table.ForeignKey(
                        name: "FK_recebimentos_empresas_empresaId",
                        column: x => x.empresaId,
                        principalTable: "empresas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_recebimentos_usuarios_usuarioEnvioId",
                        column: x => x.usuarioEnvioId,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "usuarioEmpresa",
                columns: table => new
                {
                    id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    usuarioId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    empresaId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    ativo = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    dataCadastro = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuarioEmpresa", x => x.id);
                    table.ForeignKey(
                        name: "FK_usuarioEmpresa_empresas_empresaId",
                        column: x => x.empresaId,
                        principalTable: "empresas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_usuarioEmpresa_usuarios_usuarioId",
                        column: x => x.usuarioId,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "arquivos",
                columns: table => new
                {
                    id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    recebimentoId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    tipoDocumentoId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    nomeOriginal = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    nomeArquivo = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    caminhoArquivo = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    extensao = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    mimeType = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    tamanhoBytes = table.Column<long>(type: "bigint", nullable: false),
                    ordemExibicao = table.Column<int>(type: "int", nullable: false),
                    ativo = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    dataUpload = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_arquivos", x => x.id);
                    table.ForeignKey(
                        name: "FK_arquivos_recebimentos_recebimentoId",
                        column: x => x.recebimentoId,
                        principalTable: "recebimentos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_arquivos_tiposDocumento_tipoDocumentoId",
                        column: x => x.tipoDocumentoId,
                        principalTable: "tiposDocumento",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "recebimentoComentarios",
                columns: table => new
                {
                    id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    recebimentoId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    usuarioId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    comentario = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    visivelParaEmpresa = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    dataCadastro = table.Column<DateTime>(type: "datetime", nullable: false),
                    dataAtualizacao = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_recebimentoComentarios", x => x.id);
                    table.ForeignKey(
                        name: "FK_recebimentoComentarios_recebimentos_recebimentoId",
                        column: x => x.recebimentoId,
                        principalTable: "recebimentos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_recebimentoComentarios_usuarios_usuarioId",
                        column: x => x.usuarioId,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "recebimentoConferencias",
                columns: table => new
                {
                    id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    recebimentoId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    usuarioConferenciaId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    statusConferencia = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    notaEncontrada = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    boletoEncontrado = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    valorConfere = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    dataVencimentoConfere = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    documentoConfere = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    observacao = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    dataConferencia = table.Column<DateTime>(type: "datetime", nullable: false),
                    dataCadastro = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_recebimentoConferencias", x => x.id);
                    table.ForeignKey(
                        name: "FK_recebimentoConferencias_recebimentos_recebimentoId",
                        column: x => x.recebimentoId,
                        principalTable: "recebimentos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_recebimentoConferencias_usuarios_usuarioConferenciaId",
                        column: x => x.usuarioConferenciaId,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "recebimentoDivergencias",
                columns: table => new
                {
                    id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    recebimentoId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    usuarioId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    tipoDivergencia = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    descricao = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    statusDivergencia = table.Column<int>(type: "int", nullable: false),
                    resolvida = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    dataResolucao = table.Column<DateTime>(type: "datetime", nullable: true),
                    usuarioResolucaoId = table.Column<ulong>(type: "bigint unsigned", nullable: true),
                    observacaoResolucao = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    dataCadastro = table.Column<DateTime>(type: "datetime", nullable: false),
                    dataAtualizacao = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_recebimentoDivergencias", x => x.id);
                    table.ForeignKey(
                        name: "FK_recebimentoDivergencias_recebimentos_recebimentoId",
                        column: x => x.recebimentoId,
                        principalTable: "recebimentos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_recebimentoDivergencias_usuarios_usuarioId",
                        column: x => x.usuarioId,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_recebimentoDivergencias_usuarios_usuarioResolucaoId",
                        column: x => x.usuarioResolucaoId,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "recebimentoHistoricos",
                columns: table => new
                {
                    id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    recebimentoId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    usuarioId = table.Column<ulong>(type: "bigint unsigned", nullable: true),
                    acao = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    descricao = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    dataCadastro = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_recebimentoHistoricos", x => x.id);
                    table.ForeignKey(
                        name: "FK_recebimentoHistoricos_recebimentos_recebimentoId",
                        column: x => x.recebimentoId,
                        principalTable: "recebimentos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_recebimentoHistoricos_usuarios_usuarioId",
                        column: x => x.usuarioId,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "boletos",
                columns: table => new
                {
                    id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    recebimentoId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    arquivoId = table.Column<ulong>(type: "bigint unsigned", nullable: true),
                    codigoBarras = table.Column<string>(type: "varchar(60)", maxLength: 60, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    linhaDigitavel = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    valorBoleto = table.Column<decimal>(type: "decimal(15,2)", precision: 15, scale: 2, nullable: false),
                    dataVencimento = table.Column<DateTime>(type: "date", nullable: true),
                    dataEmissao = table.Column<DateTime>(type: "date", nullable: true),
                    beneficiario = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    documentoBeneficiario = table.Column<string>(type: "varchar(18)", maxLength: 18, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    observacao = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    dataCadastro = table.Column<DateTime>(type: "datetime", nullable: false),
                    dataAtualizacao = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_boletos", x => x.id);
                    table.ForeignKey(
                        name: "FK_boletos_arquivos_arquivoId",
                        column: x => x.arquivoId,
                        principalTable: "arquivos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_boletos_recebimentos_recebimentoId",
                        column: x => x.recebimentoId,
                        principalTable: "recebimentos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "notasFiscais",
                columns: table => new
                {
                    id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    recebimentoId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    arquivoId = table.Column<ulong>(type: "bigint unsigned", nullable: true),
                    numeroNota = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    serie = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    chaveAcesso = table.Column<string>(type: "varchar(60)", maxLength: 60, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    valorTotal = table.Column<decimal>(type: "decimal(15,2)", precision: 15, scale: 2, nullable: false),
                    dataEmissao = table.Column<DateTime>(type: "date", nullable: true),
                    dataEntrada = table.Column<DateTime>(type: "date", nullable: true),
                    cpfCnpjEmitente = table.Column<string>(type: "varchar(18)", maxLength: 18, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    nomeEmitente = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    observacao = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    dataCadastro = table.Column<DateTime>(type: "datetime", nullable: false),
                    dataAtualizacao = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notasFiscais", x => x.id);
                    table.ForeignKey(
                        name: "FK_notasFiscais_arquivos_arquivoId",
                        column: x => x.arquivoId,
                        principalTable: "arquivos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_notasFiscais_recebimentos_recebimentoId",
                        column: x => x.recebimentoId,
                        principalTable: "recebimentos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "notaFiscalBoleto",
                columns: table => new
                {
                    id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    notaFiscalId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    boletoId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    dataCadastro = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notaFiscalBoleto", x => x.id);
                    table.ForeignKey(
                        name: "FK_notaFiscalBoleto_boletos_boletoId",
                        column: x => x.boletoId,
                        principalTable: "boletos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_notaFiscalBoleto_notasFiscais_notaFiscalId",
                        column: x => x.notaFiscalId,
                        principalTable: "notasFiscais",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "nfeDocumentos",
                columns: table => new
                {
                    id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    empresaId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    chaveAcesso = table.Column<string>(type: "varchar(44)", maxLength: 44, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    nsu = table.Column<long>(type: "bigint", nullable: true),
                    tipoDocumento = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    schemaDocumento = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    cnpjEmitente = table.Column<string>(type: "varchar(14)", maxLength: 14, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    nomeEmitente = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    cnpjDestinatario = table.Column<string>(type: "varchar(14)", maxLength: 14, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    nomeDestinatario = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    numeroNota = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    serie = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    dataEmissao = table.Column<DateTime>(type: "datetime", nullable: true),
                    valorTotal = table.Column<decimal>(type: "decimal(15,2)", precision: 15, scale: 2, nullable: true),
                    statusManifestacao = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    xmlResumo = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    xmlCompleto = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    xmlEvento = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    protocoloManifestacao = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    retornoSefaz = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    dataDownload = table.Column<DateTime>(type: "datetime", nullable: true),
                    dataCriacao = table.Column<DateTime>(type: "datetime", nullable: false),
                    dataAtualizacao = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_nfeDocumentos", x => x.id);
                    table.ForeignKey(
                        name: "FK_nfeDocumentos_empresas_empresaId",
                        column: x => x.empresaId,
                        principalTable: "empresas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "nfeProdutos",
                columns: table => new
                {
                    id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nfeDocumentoId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    codigoProduto = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    nomeProduto = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    descricao = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ncm = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    cfop = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    unidade = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    quantidade = table.Column<decimal>(type: "decimal(15,4)", precision: 15, scale: 4, nullable: true),
                    valorUnitario = table.Column<decimal>(type: "decimal(15,4)", precision: 15, scale: 4, nullable: true),
                    valorTotal = table.Column<decimal>(type: "decimal(15,2)", precision: 15, scale: 2, nullable: true),
                    ean = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    dataCriacao = table.Column<DateTime>(type: "datetime", nullable: false),
                    dataAtualizacao = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_nfeProdutos", x => x.id);
                    table.ForeignKey(
                        name: "FK_nfeProdutos_nfeDocumentos_nfeDocumentoId",
                        column: x => x.nfeDocumentoId,
                        principalTable: "nfeDocumentos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "cfgWeb",
                columns: new[] { "identificador", "buscaDescricao", "buscaIdentificador", "buscarCodigoAlternativo", "campoChavePrimaria", "descricao", "observacao", "sql" },
                values: new object[,]
                {
                    { "conferenciaFila", "empresaNome", "codigoRecebimento", "id", "r.id", "Fila de conferência", "Fila operacional da conferência de recebimentos.", "SELECT r.id, r.codigoRecebimento, r.statusRecebimento, e.nomeFantasia AS empresaNome, u.nome AS usuarioEnvioNome, r.origem, r.dataEnvio, r.dataAtualizacao FROM recebimentos r INNER JOIN empresas e ON e.id = r.empresaId INNER JOIN usuarios u ON u.id = r.usuarioEnvioId WHERE r.statusRecebimento IN (1, 2) ${WHEREOUT} ${ORDERBY}" },
                    { "recebimentos", "empresaNome", "codigoRecebimento", "id", "r.id", "Lista de recebimentos", "Lista de recebimentos do sistema.", "SELECT r.id, r.codigoRecebimento, r.statusRecebimento, r.empresaId, e.nomeFantasia AS empresaNome, r.usuarioEnvioId, u.nome AS usuarioEnvioNome, r.dataEnvio, r.dataAtualizacao, nf.numeroNota, b.dataVencimento FROM recebimentos r INNER JOIN empresas e ON e.id = r.empresaId INNER JOIN usuarios u ON u.id = r.usuarioEnvioId LEFT JOIN notasFiscais nf ON nf.recebimentoId = r.id AND nf.id = (SELECT MIN(id) FROM notasFiscais WHERE recebimentoId = r.id) LEFT JOIN boletos b ON b.recebimentoId = r.id AND b.id = (SELECT MIN(id) FROM boletos WHERE recebimentoId = r.id) ${WHERE} ${ORDERBY}" }
                });

            migrationBuilder.InsertData(
                table: "subCFGWeb",
                columns: new[] { "identificador", "campo", "campoBusca", "descricao", "identificadorCfg", "larguraColuna", "mascara", "ordemCampo", "permitirFiltro", "tipoDados", "visivel" },
                values: new object[,]
                {
                    { "conferenciaFila_codigoRecebimento", "codigoRecebimento", "r.codigoRecebimento", "Código", "conferenciaFila", 180, null, 1, true, "string", true },
                    { "conferenciaFila_dataAtualizacao", "dataAtualizacao", "r.dataAtualizacao", "Data atualização", "conferenciaFila", 160, null, 7, true, "dateTime", true },
                    { "conferenciaFila_dataEnvio", "dataEnvio", "r.dataEnvio", "Data envio", "conferenciaFila", 160, null, 6, true, "dateTime", true },
                    { "conferenciaFila_empresaNome", "empresaNome", "e.nomeFantasia", "Empresa", "conferenciaFila", 220, null, 3, true, "string", true },
                    { "conferenciaFila_id", "id", "r.id", "Id", "conferenciaFila", 80, null, 0, true, "integer", true },
                    { "conferenciaFila_origem", "origem", "r.origem", "Origem", "conferenciaFila", 120, null, 5, true, "string", true },
                    { "conferenciaFila_statusRecebimento", "statusRecebimento", "r.statusRecebimento", "Status", "conferenciaFila", 120, null, 2, true, "integer", true },
                    { "conferenciaFila_usuarioEnvioNome", "usuarioEnvioNome", "u.nome", "Usuário envio", "conferenciaFila", 220, null, 4, true, "string", true },
                    { "recebimentos_id", "id", "r.id", "Id", "recebimentos", 80, null, 0, true, "integer", false },
                    { "recebimentos_codigoRecebimento", "codigoRecebimento", "r.codigoRecebimento", "Codigo", "recebimentos", 180, null, 1, true, "string", true },
                    { "recebimentos_statusRecebimento", "statusRecebimento", "r.statusRecebimento", "Status", "recebimentos", 130, null, 2, true, "integer", true },
                    { "recebimentos_empresaId", "empresaId", "r.empresaId", "Id empresa", "recebimentos", 80, null, 3, true, "integer", false },
                    { "recebimentos_empresaNome", "empresaNome", "e.nomeFantasia", "Empresa", "recebimentos", 220, null, 4, true, "string", true },
                    { "recebimentos_usuarioEnvioId", "usuarioEnvioId", "r.usuarioEnvioId", "Id usuario", "recebimentos", 80, null, 5, true, "integer", false },
                    { "recebimentos_usuarioEnvioNome", "usuarioEnvioNome", "u.nome", "Usuario envio", "recebimentos", 200, null, 6, true, "string", true },
                    { "recebimentos_dataEnvio", "dataEnvio", "r.dataEnvio", "Data envio", "recebimentos", 160, null, 7, true, "dateTime", true },
                    { "recebimentos_dataAtualizacao", "dataAtualizacao", "r.dataAtualizacao", "Data atualizacao", "recebimentos", 160, null, 8, false, "dateTime", true },
                    { "recebimentos_numeroNota", "numeroNota", "nf.numeroNota", "Nota fiscal", "recebimentos", 140, null, 9, true, "string", true },
                    { "recebimentos_dataVencimento", "dataVencimento", "b.dataVencimento", "Vencimento boleto", "recebimentos", 140, null, 10, true, "date", true }
                });

            migrationBuilder.CreateIndex(
                name: "IX_arquivos_recebimentoId",
                table: "arquivos",
                column: "recebimentoId");

            migrationBuilder.CreateIndex(
                name: "IX_arquivos_tipoDocumentoId",
                table: "arquivos",
                column: "tipoDocumentoId");

            migrationBuilder.CreateIndex(
                name: "IX_boletos_arquivoId",
                table: "boletos",
                column: "arquivoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_boletos_linhaDigitavel",
                table: "boletos",
                column: "linhaDigitavel");

            migrationBuilder.CreateIndex(
                name: "IX_boletos_recebimentoId",
                table: "boletos",
                column: "recebimentoId");

            migrationBuilder.CreateIndex(
                name: "IX_empresas_cnpj",
                table: "empresas",
                column: "cnpj",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_empresas_codigoInterno",
                table: "empresas",
                column: "codigoInterno",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_notaFiscalBoleto_boletoId",
                table: "notaFiscalBoleto",
                column: "boletoId");

            migrationBuilder.CreateIndex(
                name: "IX_notaFiscalBoleto_notaFiscalId_boletoId",
                table: "notaFiscalBoleto",
                columns: new[] { "notaFiscalId", "boletoId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_notasFiscais_arquivoId",
                table: "notasFiscais",
                column: "arquivoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_notasFiscais_chaveAcesso",
                table: "notasFiscais",
                column: "chaveAcesso",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_notasFiscais_recebimentoId",
                table: "notasFiscais",
                column: "recebimentoId");

            migrationBuilder.CreateIndex(
                name: "IX_perfis_nome",
                table: "perfis",
                column: "nome",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_recebimentoComentarios_recebimentoId",
                table: "recebimentoComentarios",
                column: "recebimentoId");

            migrationBuilder.CreateIndex(
                name: "IX_recebimentoComentarios_usuarioId",
                table: "recebimentoComentarios",
                column: "usuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_recebimentoConferencias_recebimentoId",
                table: "recebimentoConferencias",
                column: "recebimentoId");

            migrationBuilder.CreateIndex(
                name: "IX_recebimentoConferencias_usuarioConferenciaId",
                table: "recebimentoConferencias",
                column: "usuarioConferenciaId");

            migrationBuilder.CreateIndex(
                name: "IX_recebimentoDivergencias_recebimentoId",
                table: "recebimentoDivergencias",
                column: "recebimentoId");

            migrationBuilder.CreateIndex(
                name: "IX_recebimentoDivergencias_usuarioId",
                table: "recebimentoDivergencias",
                column: "usuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_recebimentoDivergencias_usuarioResolucaoId",
                table: "recebimentoDivergencias",
                column: "usuarioResolucaoId");

            migrationBuilder.CreateIndex(
                name: "IX_recebimentoHistoricos_recebimentoId",
                table: "recebimentoHistoricos",
                column: "recebimentoId");

            migrationBuilder.CreateIndex(
                name: "IX_recebimentoHistoricos_usuarioId",
                table: "recebimentoHistoricos",
                column: "usuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_recebimentos_codigoRecebimento",
                table: "recebimentos",
                column: "codigoRecebimento",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_recebimentos_empresaId",
                table: "recebimentos",
                column: "empresaId");

            migrationBuilder.CreateIndex(
                name: "IX_recebimentos_usuarioEnvioId",
                table: "recebimentos",
                column: "usuarioEnvioId");

            migrationBuilder.CreateIndex(
                name: "IX_subCFGWeb_identificadorCfg",
                table: "subCFGWeb",
                column: "identificadorCfg");

            migrationBuilder.CreateIndex(
                name: "IX_tiposDocumento_nome",
                table: "tiposDocumento",
                column: "nome",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_usuarioEmpresa_empresaId",
                table: "usuarioEmpresa",
                column: "empresaId");

            migrationBuilder.CreateIndex(
                name: "IX_usuarioEmpresa_usuarioId_empresaId",
                table: "usuarioEmpresa",
                columns: new[] { "usuarioId", "empresaId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_email",
                table: "usuarios",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_login",
                table: "usuarios",
                column: "login",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_perfilId",
                table: "usuarios",
                column: "perfilId");

            migrationBuilder.CreateIndex(
                name: "IX_nfeDocumentos_chaveAcesso",
                table: "nfeDocumentos",
                column: "chaveAcesso",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_nfeDocumentos_empresaId",
                table: "nfeDocumentos",
                column: "empresaId");

            migrationBuilder.CreateIndex(
                name: "IX_nfeProdutos_nfeDocumentoId",
                table: "nfeProdutos",
                column: "nfeDocumentoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "nfeProdutos");

            migrationBuilder.DropTable(
                name: "nfeDocumentos");

            migrationBuilder.DropTable(
                name: "notaFiscalBoleto");

            migrationBuilder.DropTable(
                name: "recebimentoComentarios");

            migrationBuilder.DropTable(
                name: "recebimentoConferencias");

            migrationBuilder.DropTable(
                name: "recebimentoDivergencias");

            migrationBuilder.DropTable(
                name: "recebimentoHistoricos");

            migrationBuilder.DropTable(
                name: "subCFGWeb");

            migrationBuilder.DropTable(
                name: "usuarioEmpresa");

            migrationBuilder.DropTable(
                name: "boletos");

            migrationBuilder.DropTable(
                name: "notasFiscais");

            migrationBuilder.DropTable(
                name: "cfgWeb");

            migrationBuilder.DropTable(
                name: "arquivos");

            migrationBuilder.DropTable(
                name: "recebimentos");

            migrationBuilder.DropTable(
                name: "tiposDocumento");

            migrationBuilder.DropTable(
                name: "empresas");

            migrationBuilder.DropTable(
                name: "usuarios");

            migrationBuilder.DropTable(
                name: "perfis");
        }
    }
}
