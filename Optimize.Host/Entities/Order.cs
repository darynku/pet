namespace Optimize.Host.Entities;

public class Order
{
    public int Id { get; set; }
    public string Name { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime ProcessedAt { get; set; }
}