namespace taskQueueGrupo3.Models
{
    public class Task
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Priority { get; set; } // "Alta", "Media", "Baja"
        public DateTime ExecutionDate { get; set; }
        public string Status { get; set; } // "Pendiente", "En Proceso", "Finalizada", "Fallida"
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

}
