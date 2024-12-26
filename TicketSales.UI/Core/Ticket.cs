namespace TicketSales.UI.Core;

public class Ticket
{
    public int EventId { get; set; }
    public decimal Price { get; set; }
    public Status Status { get; set; } = Status.Available;
}