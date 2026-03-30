using Microsoft.AspNetCore.Mvc;
using ProcessadorTarefasF360.Core.DTOs;
using ProcessadorTarefasF360.Core.Enums;
using ProcessadorTarefasF360.Core.Interfaces;

namespace ProcessadorTarefasF360.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TarefasController : ControllerBase
{
    private readonly ITarefaServico _tarefaServico;

    public TarefasController(ITarefaServico tarefaServico)
    {
        _tarefaServico = tarefaServico;
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] CriarTarefaRequest request)
    {
        if (request is null)
        {
            return BadRequest(new { mensagem = "Request inválido." });
        }

        if (!Enum.IsDefined(typeof(TipoTarefa), request.Tipo))
        {
            return BadRequest(new { mensagem = "O tipo da tarefa é inválido." });
        }

        var resposta = await _tarefaServico.CriarTarefaAsync(request);

        return CreatedAtAction(nameof(ObterPorId), new { id = resposta.Id }, resposta);
    }

    [HttpGet]
    public async Task<IActionResult> ObterTodas()
    {
        var resposta = await _tarefaServico.ObterTodasAsync();
        return Ok(resposta);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObterPorId(string id)
    {
        var resposta = await _tarefaServico.ObterPorIdAsync(id);

        if (resposta is null)
        {
            return NotFound(new { mensagem = "Tarefa não encontrada." });
        }

        return Ok(resposta);
    }
}