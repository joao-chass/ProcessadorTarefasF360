using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ProcessadorTarefasF360.Core.Configuracoes;
using ProcessadorTarefasF360.Core.Entidades;
using ProcessadorTarefasF360.Core.Enums;
using ProcessadorTarefasF360.Core.Interfaces;

namespace ProcessadorTarefasF360.Core.Repositorios;

public class TarefaRepositorioMongo : IRepositorioTarefa
{
    private readonly IMongoCollection<TarefaProcessamento> _collection;

    public TarefaRepositorioMongo(IOptions<MongoDbConfiguracao> config)
    {
        var client = new MongoClient(config.Value.ConnectionString);
        var database = client.GetDatabase(config.Value.Database);
        _collection = database.GetCollection<TarefaProcessamento>(config.Value.Collection);
    }

    public async Task<TarefaProcessamento> CriarAsync(TarefaProcessamento tarefa)
    {
        await _collection.InsertOneAsync(tarefa);
        return tarefa;
    }

    public async Task<TarefaProcessamento?> ObterPorIdAsync(string id)
    {
        return await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task<List<TarefaProcessamento>> ObterTodasAsync()
    {
        return await _collection
            .Find(_ => true)
            .SortByDescending(x => x.DataCriacao)
            .ToListAsync();
    }

    public async Task AtualizarAsync(TarefaProcessamento tarefa)
    {
        tarefa.DataAtualizacao = DateTime.UtcNow;
        await _collection.ReplaceOneAsync(x => x.Id == tarefa.Id, tarefa);
    }

    public async Task<bool> TentarMarcarComoEmProcessamentoAsync(string id)
    {
        var filtro = Builders<TarefaProcessamento>.Filter.And(
            Builders<TarefaProcessamento>.Filter.Eq(x => x.Id, id),
            Builders<TarefaProcessamento>.Filter.Eq(x => x.Status, StatusTarefa.Pendente));

        var atualizacao = Builders<TarefaProcessamento>.Update
            .Set(x => x.Status, StatusTarefa.EmProcessamento)
            .Set(x => x.DataAtualizacao, DateTime.UtcNow);

        var resultado = await _collection.UpdateOneAsync(filtro, atualizacao);

        return resultado.ModifiedCount > 0;
    }
}