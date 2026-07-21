
namespace Diarios.Api.Domain.Models
{
    public class QueryDefinition
    {
        public string Sql { get; set; } = String.Empty;
        public Dictionary<string, object> Parameters { get; set; } = new();
    }
}
