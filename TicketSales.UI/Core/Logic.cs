using System.Text.Json;
using System.Text.Json.Serialization;

namespace TicketSales.UI.Core;

public class Logic
{
    private class SaveData
    {
        public List<CulturalVenue> CulturalVenues { get; set; } = [];
        public List<Customer> Customers { get; set; } = [];
    }
    private string FilePath;
    private SaveData _saveData = new();
    public List<CulturalVenue> CulturalVenues
    {
        get => _saveData.CulturalVenues;
    }

    public List<Customer> Customers
    {
        get => _saveData.Customers;
    }
    
    public delegate void LoadedEventHandler();
    public event LoadedEventHandler? OnLoaded;

    public Logic(string filePath, LoadedEventHandler? onLoaded = null)
    {
        FilePath = filePath;
        OnLoaded += onLoaded;
        // Load().ContinueWith(_ => OnLoaded?.Invoke());
    }

    
    public async Task Save()
    {
        await using var fs = new FileStream(FilePath, FileMode.Create, FileAccess.Write);
        await JsonSerializer.SerializeAsync(fs, _saveData, JsonSerializerOptions.Default);
    }
    public void Load()
    {
        if (!File.Exists(FilePath))
            return;
        using var fs = new FileStream(FilePath, FileMode.Open);
        var data = JsonSerializer.Deserialize<SaveData>(fs, JsonSerializerOptions.Default);
        if (data is null)
            return;
        _saveData = data;
        // CulturalVenues = await JsonSerializer.DeserializeAsync<List<CulturalVenue>>(data.CulturalVenues.ToString()) ?? new List<CulturalVenue>();
        // Customers = await JsonSerializer.DeserializeAsync<List<Customer>>(data.Customers.ToString()) ?? new List<Customer>();
    }
}