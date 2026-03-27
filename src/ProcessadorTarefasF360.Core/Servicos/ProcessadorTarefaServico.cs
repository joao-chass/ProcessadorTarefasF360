using Microsoft.Extensions.Options;
using ProcessadorTarefasF360.Core.Configuracoes;
using ProcessadorTarefasF360.Core.Enums;
using ProcessadorTarefasF360.Core.Interfaces;

namespace ProcessadorTarefasF360.Core.Servicos;

public class ProcessadorTarefaServico : IProcessadorTarefaServico
{
    private readonly IRepositorioTarefa _repositorio;
    private readonly IMensageriaTarefa _mensageria;
    private readonly ProcessamentoTarefaConfiguracao _config;

    public ProcessadorTarefaServico(
        IRepositorioTarefa repositorio,
        IMensageriaTarefa mensageria,
        IOptions<ProcessamentoTarefaConfiguracao> config)
    {
        _repositorio = repositorio;
        _mensageria = mensageria;
        _config = config.Value;
    }

    public async Task ProcessarAsync(string tarefaId)
    {
        var conseguiuIniciar = await _repositorio.TentarMarcarComoEmProcessamentoAsync(tarefaId);

        if (!conseguiuIniciar)
        {
            return;
        }

        var tarefa = await _repositorio.ObterPorIdAsync(tarefaId);

        if (tarefa is null)
        {
            return;
        }

        try
        {
            if (tarefa.Descricao.Contains("erro", StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception("Falha simulada no processamento.");
            }

            await Task.Delay(TimeSpan.FromSeconds(_config.TempoSimuladoEmSegundos));

            tarefa.Status = StatusTarefa.Concluida;
            tarefa.MensagemErro = null;

            await _repositorio.AtualizarAsync(tarefa);
        }
        catch (Exception ex)
        {
            tarefa.Tentativas++;

            if (tarefa.Tentativas >= tarefa.MaxTentativas)
            {
                tarefa.Status = StatusTarefa.Erro;
                tarefa.MensagemErro = ex.Message;
                await _repositorio.AtualizarAsync(tarefa);
                return;
            }

            tarefa.Status = StatusTarefa.Pendente;
            tarefa.MensagemErro = ex.Message;

            await _repositorio.AtualizarAsync(tarefa);

            _mensageria.PublicarTarefa(tarefa.Id);
        }
    }
}
