namespace ProcessadorTarefasF360.Core.Interfaces;

public interface IProcessadorTarefaServico
{
    Task ProcessarAsync(string tarefaId);
}
