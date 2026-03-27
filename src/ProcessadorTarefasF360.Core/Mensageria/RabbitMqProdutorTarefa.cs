using System.Text;
using Microsoft.Extensions.Options;
using ProcessadorTarefasF360.Core.Configuracoes;
using ProcessadorTarefasF360.Core.Interfaces;
using RabbitMQ.Client;

namespace ProcessadorTarefasF360.Core.Mensageria;

public class RabbitMqProdutorTarefa : IMensageriaTarefa
{
    private readonly RabbitMqConfiguracao _config;

    public RabbitMqProdutorTarefa(IOptions<RabbitMqConfiguracao> config)
    {
        _config = config.Value;
    }

    public void PublicarTarefa(string tarefaId)
    {
        var factory = new ConnectionFactory
        {
            HostName = _config.Host,
            Port = _config.Port,
            UserName = _config.Usuario,
            Password = _config.Senha
        };

        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.QueueDeclare(
            queue: _config.Fila,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var properties = channel.CreateBasicProperties();
        properties.Persistent = true;

        var body = Encoding.UTF8.GetBytes(tarefaId);

        channel.BasicPublish(
            exchange: string.Empty,
            routingKey: _config.Fila,
            basicProperties: properties,
            body: body);
    }
}
