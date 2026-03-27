using ProcessadorTarefasF360.Core.Entidades;

namespace ProcessadorTarefasF360.Core.Interfaces;

public interface IRepositorioTarefa
{
    Task<TarefaProcessamento> CriarAsync(TarefaProcessamento tarefa);
    Task<TarefaProcessamento?> ObterPorIdAsync(string id);
    Task AtualizarAsync(TarefaProcessamento tarefa);
    Task<bool> TentarMarcarComoEmProcessamentoAsync(string id);
}
