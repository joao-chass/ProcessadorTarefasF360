using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProcessadorTarefasF360.Core.Configuracoes;
using ProcessadorTarefasF360.Core.Interfaces;
using ProcessadorTarefasF360.Core.Mensageria;
using ProcessadorTarefasF360.Core.Repositorios;
using ProcessadorTarefasF360.Core.Servicos;

namespace ProcessadorTarefasF360.Core.Extensoes;

public static class InjecaoDependenciaExtensao
{
    public static IServiceCollection AdicionarCore(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MongoDbConfiguracao>(configuration.GetSection("MongoDb"));
        services.Configure<RabbitMqConfiguracao>(configuration.GetSection("RabbitMq"));
        services.Configure<ProcessamentoTarefaConfiguracao>(configuration.GetSection("ProcessamentoTarefa"));

        services.AddScoped<IRepositorioTarefa, TarefaRepositorioMongo>();
        services.AddScoped<IMensageriaTarefa, RabbitMqProdutorTarefa>();
        services.AddScoped<ITarefaServico, TarefaServico>();
        services.AddScoped<IProcessadorTarefaServico, ProcessadorTarefaServico>();

        return services;
    }
}
