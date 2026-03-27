namespace ProcessadorTarefasF360.Core.Configuracoes;

public class MongoDbConfiguracao
{
    public string ConnectionString { get; set; } = string.Empty;
    public string Database { get; set; } = string.Empty;
    public string Collection { get; set; } = "tarefas";
}
