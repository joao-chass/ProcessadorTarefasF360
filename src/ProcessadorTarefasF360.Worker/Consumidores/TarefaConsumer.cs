using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProcessadorTarefasF360.Core.Configuracoes;
using ProcessadorTarefasF360.Core.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ProcessadorTarefasF360.Worker.Consumidores;

public class TarefaConsumer : IDisposable
{
    private readonly RabbitMqConfiguracao _config;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TarefaConsumer> _logger;

    private IConnection? _connection;
    private IModel? _channel;
    private string? _consumerTag;

    public TarefaConsumer(
        IOptions<RabbitMqConfiguracao> config,
        IServiceProvider serviceProvider,
        ILogger<TarefaConsumer> logger)
    {
        _config = config.Value;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task IniciarAsync(CancellationToken cancellationToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _config.Host,
            Port = _config.Port,
            UserName = _config.Usuario,
            Password = _config.Senha,
            DispatchConsumersAsync = true
        };

        int tentativas = 0;

        while (tentativas < 10)
        {
            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                _channel.QueueDeclare(
                    queue: _config.Fila,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                _channel.BasicQos(0, 1, false);

                var consumer = new AsyncEventingBasicConsumer(_channel);

                consumer.Received += async (_, ea) =>
                {
                    var tarefaId = Encoding.UTF8.GetString(ea.Body.ToArray());

                    try
                    {
                        using var scope = _serviceProvider.CreateScope();
                        var processador = scope.ServiceProvider.GetRequiredService<IProcessadorTarefaServico>();

                        await processador.ProcessarAsync(tarefaId);

                        _channel.BasicAck(ea.DeliveryTag, false);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Erro ao processar tarefa {TarefaId}", tarefaId);
                        _channel.BasicNack(ea.DeliveryTag, false, true);
                    }
                };

                _consumerTag = _channel.BasicConsume(
                    queue: _config.Fila,
                    autoAck: false,
                    consumer: consumer);

                _logger.LogInformation("Conectado ao RabbitMQ com sucesso.");

                return;
            }
            catch
            {
                tentativas++;
                _logger.LogWarning("RabbitMQ ainda não disponível... tentativa {Tentativas}", tentativas);
                await Task.Delay(3000, cancellationToken);
            }
        }

        throw new Exception("Não foi possível conectar ao RabbitMQ.");
    }

    public Task PararAsync()
    {
        if (_channel is not null && !string.IsNullOrWhiteSpace(_consumerTag))
        {
            _channel.BasicCancel(_consumerTag);
        }

        _channel?.Close();
        _connection?.Close();

        _channel?.Dispose();
        _connection?.Dispose();

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }
}
