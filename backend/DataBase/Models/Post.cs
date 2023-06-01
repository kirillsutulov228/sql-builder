namespace backend.DataBase.Models
{
    public class Post
    {
        public int Id { get; set; }
        public User Author { get; set; }
        public string Title { get; set; }

        public string Text { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
