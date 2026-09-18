namespace Diarios.Api.Domain.Models.Responses
{
    public class IndexStatusResponse
    {
        public int DiariosIndexados { get; set; }
        public string DataMaisRecenteIndexada { get; set; } = string.Empty;
        public string DataMaisAntigaIndexada { get; set; } = string.Empty;
    }
}
