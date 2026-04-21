using CBTClinic.Domain.Entities;

public class Article
{
    public int Id { get; set; }

    public string Title { get; set; }

    public string FilePath { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int AdminId { get; set; }
    public Admin Admin { get; set; }
}