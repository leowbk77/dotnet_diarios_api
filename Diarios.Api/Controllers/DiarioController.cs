using Diarios.Api.Models;
using Diarios.Api.Service.Interface;
using Diarios.Api.Util.CustomException;
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
        [ProducesResponseType(typeof(ResponseModel), StatusCodes.Status200OK)]
        public IActionResult Search([FromQuery][Description("Parametros de busca e filtragem dos diários")] 
                                    SearchQueryModel query)
        {
            Log.Information($"Search: {query.cidade}: lastId:{query.lastId}");
            try
            {
                return Ok(_service.SearchDiarios(query));
            }
            catch (DiarioCustomException ex)
            {
                return StatusCode(ex.httpStatusCode, ex.Message);
            }
        }

    }
}
