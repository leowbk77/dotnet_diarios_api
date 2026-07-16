using Diarios.Api.Domain.Contracts.Service;  
using Diarios.Api.Application.Util.CustomException;
using Diarios.Api.Domain.Models.Entities;
using Diarios.Api.Domain.Models.Requests;
using Diarios.Api.Domain.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using Serilog;

namespace Diarios.Api.Controllers
{
    [ApiController]
    [Route("api/diarios")]
    public class DiarioController : ControllerBase
    {
        private readonly IDiarioService _service;
        public DiarioController(IDiarioService service)
        {
            _service = service;
        }

        [HttpGet("{cidade}/{id}")]
        [EndpointDescription("Endpoint que busca diario a partir do id")]
        [ProducesResponseType(typeof(Diario), StatusCodes.Status200OK)]
        public IActionResult GetDiarioById(int id, string cidade)
        {
            try
            {
                return Ok(_service.GetDiarioById(id, cidade));
            }
            catch (DiarioCustomException ex)
            {
                return StatusCode(ex.httpStatusCode, ex.Message);
            }
        }

        [HttpGet("search")]
        [EndpointDescription("Endpoint responsável por realizar as buscas pelos diários na base de dados a partir dos parâmetros recebidos pela query.")]
        [ProducesResponseType(typeof(SearchResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> Search([FromQuery][Description("Parametros de busca e filtragem dos diários")] 
                                    SearchRequest query)
        {
            Log.Information($"GET Search: {query.Cidade}: lastId:{query.LastDocId ?? 0}");
            try
            {
                return Ok(await _service.SearchDiariosAsync(query));
            }
            catch (DiarioCustomException ex)
            {
                return StatusCode(ex.httpStatusCode, ex.Message);
            }
        }

        [HttpGet("get-latest")]
        [EndpointDescription("Endpoint responsável por buscar o último diário indexado")]
        [ProducesResponseType(typeof(Diario), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetLatestDiario([FromQuery] string from)
        {
            Log.Information($"GET GetLatestDiario: from: {from}");
            try
            {
                return Ok(await _service.SearchForLatestAsync(from));
            }
            catch (DiarioCustomException ex)
            {
                return StatusCode(ex.httpStatusCode, ex.Message);
            }
        }

    }
}
