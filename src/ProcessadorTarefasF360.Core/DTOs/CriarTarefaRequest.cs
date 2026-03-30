using ProcessadorTarefasF360.Core.Enums;

namespace ProcessadorTarefasF360.Core.DTOs;

public class CriarTarefaRequest
{
    public TipoTarefa Tipo { get; set; }
    public string DadosJson { get; set; } = "{}";
}
