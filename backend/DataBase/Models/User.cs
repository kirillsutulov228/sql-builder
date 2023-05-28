using System.ComponentModel.DataAnnotations.Schema;

public class User
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int userId { get; set; }
    public string firstname { get; set; }
    public string lastname { get; set; }
    public DateOnly registrationDate { get; set; }
    public int age { get; set; }
    public string? hobby { get; set; }

    public bool Equals(User comparableUser)
    {
        if (comparableUser.userId != userId) return false;
        return true;
    }
}
