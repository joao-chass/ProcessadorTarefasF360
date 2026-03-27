using ProcessadorTarefasF360.Core.DTOs;
using ProcessadorTarefasF360.Core.Entidades;
using ProcessadorTarefasF360.Core.Interfaces;

namespace ProcessadorTarefasF360.Core.Servicos;

public class TarefaServico : ITarefaServico
{
    private readonly IRepositorioTarefa _repositorio;
    private readonly IMensageriaTarefa _mensageria;

    public TarefaServico(IRepositorioTarefa repositorio, IMensageriaTarefa mensageria)
    {
        _repositorio = repositorio;
        _mensageria = mensageria;
    }

    public async Task<TarefaResponse> CriarTarefaAsync(CriarTarefaRequest request)
    {
        var tarefa = new TarefaProcessamento
        {
            Descricao = request.Descricao.Trim(),
            Tentativas = 0,
            MaxTentativas = 3
        };

        await _repositorio.CriarAsync(tarefa);

        _mensageria.PublicarTarefa(tarefa.Id);

        return Mapear(tarefa);
    }

    public async Task<TarefaResponse?> ObterPorIdAsync(string id)
    {
        var tarefa = await _repositorio.ObterPorIdAsync(id);

        return tarefa is null ? null : Mapear(tarefa);
    }

    private static TarefaResponse Mapear(TarefaProcessamento tarefa)
    {
        return new TarefaResponse
        {
            Id = tarefa.Id,
            Descricao = tarefa.Descricao,
            Status = tarefa.Status,
            Tentativas = tarefa.Tentativas,
            MaxTentativas = tarefa.MaxTentativas,
            DataCriacao = tarefa.DataCriacao,
            DataAtualizacao = tarefa.DataAtualizacao,
            MensagemErro = tarefa.MensagemErro
        };
    }
}
