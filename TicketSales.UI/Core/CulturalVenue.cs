namespace TicketSales.UI.Core;

public class CulturalVenue
{
    public string Name { get; set; }
    public CulturalType Type { get; set; }
    public string Address { get; set; }
    public List<Event> Events { get; set; } = [];
}