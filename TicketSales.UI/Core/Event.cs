namespace TicketSales.UI.Core;

public class Event
{
    public string Name { get; set; }
    public string Description { get; set; }
    // public List<string> Participants { get; set; } = [];
    public DateOnly Date { get; set; }
    public TimeOnly Time { get; set; }
    public int VenueId { get; set; }
    public List<Ticket> Tickets { get; set; } = [];
}