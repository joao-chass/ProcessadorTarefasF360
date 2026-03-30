using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using ProcessadorTarefasF360.Core.Enums;

namespace ProcessadorTarefasF360.Core.Entidades;

public class TarefaProcessamento
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    public TipoTarefa Tipo { get; set; }
    public string DadosJson { get; set; } = "{}";

    public StatusTarefa Status { get; set; } = StatusTarefa.Pendente;

    public int Tentativas { get; set; }

    public int MaxTentativas { get; set; } = 3;

    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

    public DateTime? DataAtualizacao { get; set; }

    public string? MensagemErro { get; set; }
}
