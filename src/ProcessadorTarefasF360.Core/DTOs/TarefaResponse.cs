using ProcessadorTarefasF360.Core.Enums;

namespace ProcessadorTarefasF360.Core.DTOs;

public class TarefaResponse
{
    public string Id { get; set; } = string.Empty;
    public TipoTarefa Tipo { get; set; }
    public string DadosJson { get; set; } = "{}";
    public StatusTarefa Status { get; set; }
    public int Tentativas { get; set; }
    public int MaxTentativas { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime? DataAtualizacao { get; set; }
    public string? MensagemErro { get; set; }
}
