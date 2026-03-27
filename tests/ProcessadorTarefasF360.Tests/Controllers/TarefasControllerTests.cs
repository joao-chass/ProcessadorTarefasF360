using Microsoft.AspNetCore.Mvc;
using Moq;
using ProcessadorTarefasF360.Api.Controllers;
using ProcessadorTarefasF360.Core.DTOs;
using ProcessadorTarefasF360.Core.Enums;
using ProcessadorTarefasF360.Core.Interfaces;

namespace ProcessadorTarefasF360.Tests.Controllers;

public class TarefasControllerTests
{
    private readonly Mock<ITarefaServico> _tarefaServicoMock;
    private readonly TarefasController _controller;

    public TarefasControllerTests()
    {
        _tarefaServicoMock = new Mock<ITarefaServico>();
        _controller = new TarefasController(_tarefaServicoMock.Object);
    }

    [Fact]
    public async Task Criar_QuandoDescricaoForValida_DeveRetornarCreatedAtAction()
    {
        var request = new CriarTarefaRequest
        {
            Descricao = "Nova tarefa"
        };

        var response = new TarefaResponse
        {
            Id = "123",
            Descricao = "Nova tarefa",
            Status = StatusTarefa.Pendente,
            Tentativas = 0,
            MaxTentativas = 3
        };

        _tarefaServicoMock
            .Setup(x => x.CriarTarefaAsync(request))
            .ReturnsAsync(response);

        var resultado = await _controller.Criar(request);

        var createdResult = Assert.IsType<CreatedAtActionResult>(resultado);
        Assert.Equal(nameof(TarefasController.ObterPorId), createdResult.ActionName);
        Assert.Equal("123", createdResult.RouteValues!["id"]);

        var valor = Assert.IsType<TarefaResponse>(createdResult.Value);
        Assert.Equal("123", valor.Id);
    }

    [Fact]
    public async Task Criar_QuandoDescricaoForInvalida_DeveRetornarBadRequest()
    {
        var request = new CriarTarefaRequest
        {
            Descricao = ""
        };

        var resultado = await _controller.Criar(request);

        Assert.IsType<BadRequestObjectResult>(resultado);
    }

    [Fact]
    public async Task ObterPorId_QuandoTarefaExistir_DeveRetornarOk()
    {
        var response = new TarefaResponse
        {
            Id = "123",
            Descricao = "Tarefa encontrada",
            Status = StatusTarefa.Concluida,
            Tentativas = 1,
            MaxTentativas = 3
        };

        _tarefaServicoMock
            .Setup(x => x.ObterPorIdAsync("123"))
            .ReturnsAsync(response);

        var resultado = await _controller.ObterPorId("123");

        var okResult = Assert.IsType<OkObjectResult>(resultado);
        var valor = Assert.IsType<TarefaResponse>(okResult.Value);

        Assert.Equal("123", valor.Id);
    }

    [Fact]
    public async Task ObterPorId_QuandoTarefaNaoExistir_DeveRetornarNotFound()
    {
        _tarefaServicoMock
            .Setup(x => x.ObterPorIdAsync("999"))
            .ReturnsAsync((TarefaResponse?)null);

        var resultado = await _controller.ObterPorId("999");

        Assert.IsType<NotFoundObjectResult>(resultado);
    }

    [Fact]
    public async Task ObterTodas_DeveRetornarOkComLista()
    {
        var lista = new List<TarefaResponse>
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

        _tarefaServicoMock
            .Setup(x => x.ObterTodasAsync())
            .ReturnsAsync(lista);

        var resultado = await _controller.ObterTodas();

        var okResult = Assert.IsType<OkObjectResult>(resultado);
        var valor = Assert.IsType<List<TarefaResponse>>(okResult.Value);

        Assert.Equal(2, valor.Count);
    }
}