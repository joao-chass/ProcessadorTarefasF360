using ProcessadorTarefasF360.Worker.Consumidores;

namespace ProcessadorTarefasF360.Worker.Servicos;

public class WorkerProcessamento : BackgroundService
{
    private readonly TarefaConsumer _consumer;
    private readonly ILogger<WorkerProcessamento> _logger;

    public WorkerProcessamento(TarefaConsumer consumer, ILogger<WorkerProcessamento> logger)
    {
        _consumer = consumer;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Worker de processamento iniciado.");

        await _consumer.IniciarAsync(stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Parando worker de processamento.");

        await _consumer.PararAsync();

        await base.StopAsync(cancellationToken);
    }
}
