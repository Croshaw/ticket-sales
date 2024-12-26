namespace TicketSales.UI.Core;

public enum CulturalType
{
    Theater,
    Museum,
    Circus,
    Cinema
}

public static class CulturalTypeExtensions
{
    public static CulturalType ParseCulturalType(string culturalType)
    {
        return culturalType switch
        {
            "Театр" => CulturalType.Theater,
            "Музей" => CulturalType.Museum,
            "Цирк" => CulturalType.Circus,
            "Кинотеатр" => CulturalType.Cinema,
            _ => throw new ArgumentOutOfRangeException(nameof(culturalType), culturalType, null)
        };
    }
    
    public static string GetString(this CulturalType culturalType)
    {
        return culturalType switch
        {
            CulturalType.Theater => "Театр",
            CulturalType.Museum => "Музей",
            CulturalType.Circus => "Цирк",
            CulturalType.Cinema => "Кинотеатр",
            _ => throw new ArgumentOutOfRangeException(nameof(culturalType), culturalType, null)
        };
    }
}