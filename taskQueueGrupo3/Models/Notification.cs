namespace taskQueueGrupo3.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public string RecipientEmail { get; set; }
        public string Message { get; set; }
        public DateTime SentDate { get; set; }
        public bool IsSuccess { get; set; }
    }
}
