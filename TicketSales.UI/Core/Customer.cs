using System.Text.Json.Serialization;

namespace TicketSales.UI.Core;

public class Customer
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string MiddleName { get; set; }
    public string Address { get; set; }
    public string Phone { get; set; }
    [JsonIgnore]
    Dictionary<int, List<Ticket>> Tickets = new Dictionary<int, List<Ticket>>();
}