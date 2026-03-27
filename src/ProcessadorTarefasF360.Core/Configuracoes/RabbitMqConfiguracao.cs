namespace ProcessadorTarefasF360.Core.Configuracoes;

public class RabbitMqConfiguracao
{
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 5672;
    public string Usuario { get; set; } = "guest";
    public string Senha { get; set; } = "guest";
    public string Fila { get; set; } = "processador-tarefas";
}
