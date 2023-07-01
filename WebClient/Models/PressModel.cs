namespace WebClient.Models
{
    public enum Category
    {
        Book,
        Magazine,
        EBook
    }
    public class PressModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Category Category { get; set; }
    }
}
