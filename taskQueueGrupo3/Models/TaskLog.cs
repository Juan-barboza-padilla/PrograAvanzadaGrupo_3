namespace taskQueueGrupo3.Models
{
    public class TaskLog
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public string LogMessage { get; set; }
        public DateTime LogDate { get; set; }
        public Task Task { get; set; }
    }

}
