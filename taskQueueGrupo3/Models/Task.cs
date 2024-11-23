using System.ComponentModel.DataAnnotations;

namespace taskQueueGrupo3.Models
{
    public class Task
    {
        public int Id { get; set; }

		[Required]
		[StringLength(100)]
        public string Name { get; set; }

        public string Description { get; set; }

		[Required]
		[RegularExpression("^(Alta|Media|Baja)$", ErrorMessage = "La prioridad debe ser 'Alta', 'Media' o 'Baja'.")]
		public string Priority { get; set; } // "Alta", "Media", "Baja"

        public DateTime ExecutionDate { get; set; }

		[Required]
		[RegularExpression("^(Pendiente|En Proceso|Finalizada|Fallida)$", ErrorMessage = "El estado debe ser 'Pendiente', 'En Proceso', 'Finalizada' o 'Fallida'.")]
		public string Status { get; set; } // "Pendiente", "En Proceso", "Finalizada", "Fallida"

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }

}
