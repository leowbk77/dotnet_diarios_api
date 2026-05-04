namespace Diarios.Api.Models
{
    public class DiarioModel
    {
        public int id { get; set; }
        public string nmEdicao { get; set; } = String.Empty;
        public string caminho { get; set; } = String.Empty;
        public int ano { get; set; }
        public int mes {  get; set; }
        public int dia { get; set; }
    }
}
