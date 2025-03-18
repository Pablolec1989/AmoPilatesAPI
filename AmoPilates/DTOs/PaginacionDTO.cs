namespace AmoPilates.DTOs
{
    public class PaginacionDTO
    {
        public int Pagina { get; set; } = 1;
        private int recordsPorPagina = 10;
        private readonly int cantiadMaxRecordsPorPagina = 50;
        public int RecordsPorPagina
        {
            get { return recordsPorPagina; }
            set
            {
                recordsPorPagina = (value > cantiadMaxRecordsPorPagina) ? cantiadMaxRecordsPorPagina : value;
            }
        }
    }
}
