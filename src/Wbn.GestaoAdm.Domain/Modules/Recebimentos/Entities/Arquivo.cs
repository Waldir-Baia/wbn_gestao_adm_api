using Wbn.GestaoAdm.Domain.Common.Entities;
using Wbn.GestaoAdm.Domain.Modules.TiposDocumento.Entities;

namespace Wbn.GestaoAdm.Domain.Modules.Recebimentos.Entities;

public sealed class Arquivo : BaseEntity
{
    private Arquivo()
    {
    }

    public Arquivo(
        ulong recebimentoId,
        ulong tipoDocumentoId,
        string nomeOriginal,
        string nomeArquivo,
        string caminhoArquivo,
        string? extensao,
        string? mimeType,
        long tamanhoBytes,
        int ordemExibicao,
        bool ativo = true)
    {
        RecebimentoId = recebimentoId;
        TipoDocumentoId = tipoDocumentoId;
        NomeOriginal = NormalizeRequired(nomeOriginal);
        NomeArquivo = NormalizeRequired(nomeArquivo);
        CaminhoArquivo = NormalizeRequired(caminhoArquivo);
        Extensao = NormalizeOptional(extensao);
        MimeType = NormalizeOptional(mimeType);
        TamanhoBytes = tamanhoBytes;
        OrdemExibicao = ordemExibicao;
        Ativo = ativo;
        DataUpload = DateTime.UtcNow;
    }

    public void Inativar()
    {
        if (!Ativo)
        {
            return;
        }

        Ativo = false;
    }

    public ulong RecebimentoId { get; private set; }
    public ulong TipoDocumentoId { get; private set; }
    public string NomeOriginal { get; private set; } = string.Empty;
    public string NomeArquivo { get; private set; } = string.Empty;
    public string CaminhoArquivo { get; private set; } = string.Empty;
    public string? Extensao { get; private set; }
    public string? MimeType { get; private set; }
    public long TamanhoBytes { get; private set; }
    public int OrdemExibicao { get; private set; }
    public bool Ativo { get; private set; }
    public DateTime DataUpload { get; private set; }

    public Recebimento Recebimento { get; private set; } = null!;
    public TipoDocumento TipoDocumento { get; private set; } = null!;
    public NotaFiscal? NotaFiscal { get; private set; }
    public Boleto? Boleto { get; private set; }

    public override bool Validate()
    {
        ClearErrors();

        if (RecebimentoId == 0)
        {
            AddError("O recebimento do arquivo e obrigatorio.");
        }

        if (TipoDocumentoId == 0)
        {
            AddError("O tipo de documento do arquivo e obrigatorio.");
        }

        if (string.IsNullOrWhiteSpace(NomeOriginal))
        {
            AddError("O nome original do arquivo e obrigatorio.");
        }

        if (string.IsNullOrWhiteSpace(CaminhoArquivo))
        {
            AddError("O caminho do arquivo e obrigatorio.");
        }

        if (TamanhoBytes < 0)
        {
            AddError("O tamanho do arquivo nao pode ser negativo.");
        }

        return !HasErrors;
    }

    private static string NormalizeRequired(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        return value.Trim();
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
