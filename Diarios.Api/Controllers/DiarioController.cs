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

        [HttpGet("{cidade}/search")]
        [EndpointDescription("Endpoint responsável por realizar as buscas pelos diários na base de dados a partir dos parâmetros recebidos pela query.")]
        public IActionResult Search([Description("Identificador da cidade a ser buscado")]
                                    string cidade,
                                    [FromQuery][Description("Parametros de busca e filtragem dos diários")] 
                                    SearchQueryModel query)
        {
            Log.Information("Teste de log no Search");
            try
            {
                return Ok(_service.SearchDiarios(query, cidade));
            }
            catch (DiarioCustomException ex)
            {
                return StatusCode(ex.httpStatusCode, ex.Message);
            }
        }

    }
}
