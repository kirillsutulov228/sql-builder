using System.ComponentModel.DataAnnotations.Schema;

public class Task
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int taskId { get; set; }
    public int taskNum { get; set; }
    public string taskName { get; set; }
    public string taskDescription { get; set; }
    public string correctAnswer { get; set; }

}
