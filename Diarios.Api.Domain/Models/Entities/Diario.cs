using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diarios.Api.Domain.Models.Entities
{
    public class Diario
    {
        public int Id { get; set; }
        public string NmEdicao { get; set; } = String.Empty;
        public string Caminho { get; set; } = String.Empty;
        public int Ano { get; set; }
        public int Mes { get; set; }
        public int Dia { get; set; }
        public DateOnly Data { get; set; } = new(); // modificação futura
    }
}
