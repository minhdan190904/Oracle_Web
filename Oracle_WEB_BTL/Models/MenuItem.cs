namespace Oracle_WEB_BTL.Models
{
    public class MenuItem
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Link { get; set; }
        public string? Icon { get; set; }
        public List<MenuItem> SubItems { get; set; } = new List<MenuItem>();
        public bool IsAdmin { get; set; }
    }
}
