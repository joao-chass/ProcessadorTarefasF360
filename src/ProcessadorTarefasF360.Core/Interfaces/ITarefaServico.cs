using ProcessadorTarefasF360.Core.DTOs;

namespace ProcessadorTarefasF360.Core.Interfaces;

public interface ITarefaServico
{
    Task<TarefaResponse> CriarTarefaAsync(CriarTarefaRequest request);
    Task<TarefaResponse?> ObterPorIdAsync(string id);
}
