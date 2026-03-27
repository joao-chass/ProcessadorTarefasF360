using ProcessadorTarefasF360.Core.Extensoes;
using ProcessadorTarefasF360.Worker.Consumidores;
using ProcessadorTarefasF360.Worker.Servicos;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AdicionarCore(builder.Configuration);
builder.Services.AddSingleton<TarefaConsumer>();
builder.Services.AddHostedService<WorkerProcessamento>();

var host = builder.Build();
host.Run();
