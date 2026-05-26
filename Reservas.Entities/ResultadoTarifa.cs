namespace Reservas.Entities
{
    public class ResultadoTarifa
    {
        public int TarifaConfigId { get; set; }
        public int SedeId { get; set; }
        public int? EspacioId { get; set; }
        public int Capacidad { get; set; }
        public string TipoTemporada { get; set; } = null!;
        public decimal? PrecioOrdinario { get; set; }
        public decimal? PrecioEspecial { get; set; }
        public bool EsVisitaDia { get; set; }
        public int? MinAcompanantesTarifaEspecial { get; set; }
        public decimal? PrecioAcompananteAdicional { get; set; }
    }
}