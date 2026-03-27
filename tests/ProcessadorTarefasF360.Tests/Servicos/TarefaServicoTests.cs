using Moq;
using ProcessadorTarefasF360.Core.DTOs;
using ProcessadorTarefasF360.Core.Entidades;
using ProcessadorTarefasF360.Core.Enums;
using ProcessadorTarefasF360.Core.Interfaces;
using ProcessadorTarefasF360.Core.Servicos;

namespace ProcessadorTarefasF360.Tests.Servicos;

public class TarefaServicoTests
{
    private readonly Mock<IRepositorioTarefa> _repositorioMock;
    private readonly Mock<IMensageriaTarefa> _mensageriaMock;
    private readonly TarefaServico _servico;

    public TarefaServicoTests()
    {
        _repositorioMock = new Mock<IRepositorioTarefa>();
        _mensageriaMock = new Mock<IMensageriaTarefa>();

        _servico = new TarefaServico(_repositorioMock.Object, _mensageriaMock.Object);
    }

    [Fact]
    public async Task CriarTarefaAsync_DeveCriarTarefaEPublicarNaFila()
    {
        var request = new CriarTarefaRequest
        {
            Descricao = "Processar arquivo"
        };

        _repositorioMock
            .Setup(x => x.CriarAsync(It.IsAny<TarefaProcessamento>()))
            .ReturnsAsync((TarefaProcessamento tarefa) =>
            {
                tarefa.Id = "123";
                return tarefa;
            });

        var resultado = await _servico.CriarTarefaAsync(request);

        Assert.NotNull(resultado);
        Assert.Equal("123", resultado.Id);
        Assert.Equal("Processar arquivo", resultado.Descricao);
        Assert.Equal(StatusTarefa.Pendente, resultado.Status);
        Assert.Equal(0, resultado.Tentativas);
        Assert.Equal(3, resultado.MaxTentativas);

        _repositorioMock.Verify(x => x.CriarAsync(It.IsAny<TarefaProcessamento>()), Times.Once);
        _mensageriaMock.Verify(x => x.PublicarTarefa("123"), Times.Once);
    }

    [Fact]
    public async Task ObterPorIdAsync_QuandoTarefaExistir_DeveRetornarResposta()
    {
        var tarefa = new TarefaProcessamento
        {
            Id = "123",
            Descricao = "Tarefa teste",
            Status = StatusTarefa.Concluida,
            Tentativas = 1,
            MaxTentativas = 3
        };

        _repositorioMock
            .Setup(x => x.ObterPorIdAsync("123"))
            .ReturnsAsync(tarefa);

        var resultado = await _servico.ObterPorIdAsync("123");

        Assert.NotNull(resultado);
        Assert.Equal("123", resultado!.Id);
        Assert.Equal("Tarefa teste", resultado.Descricao);
        Assert.Equal(StatusTarefa.Concluida, resultado.Status);
    }

    [Fact]
    public async Task ObterPorIdAsync_QuandoTarefaNaoExistir_DeveRetornarNull()
    {
        _repositorioMock
            .Setup(x => x.ObterPorIdAsync("999"))
            .ReturnsAsync((TarefaProcessamento?)null);

        var resultado = await _servico.ObterPorIdAsync("999");

        Assert.Null(resultado);
    }

    [Fact]
    public async Task ObterTodasAsync_DeveRetornarListaMapeada()
    {
        var tarefas = new List<TarefaProcessamento>
        {
            new()
            {
                Id = "1",
                Descricao = "Tarefa 1",
                Status = StatusTarefa.Pendente,
                Tentativas = 0,
                MaxTentativas = 3
            },
            new()
            {
                Id = "2",
                Descricao = "Tarefa 2",
                Status = StatusTarefa.Concluida,
                Tentativas = 1,
                MaxTentativas = 3
            }
        };

        _repositorioMock
            .Setup(x => x.ObterTodasAsync())
            .ReturnsAsync(tarefas);

        var resultado = await _servico.ObterTodasAsync();

        Assert.NotNull(resultado);
        Assert.Equal(2, resultado.Count);
        Assert.Equal("1", resultado[0].Id);
        Assert.Equal("2", resultado[1].Id);
    }
}