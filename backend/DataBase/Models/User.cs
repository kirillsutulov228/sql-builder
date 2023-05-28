using System.ComponentModel.DataAnnotations.Schema;

public class User
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int userId { get; set; }
    public string firstname { get; set; }
    public string lastname { get; set; }
    public DateOnly registrationDate { get; set; }

}
