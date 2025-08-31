using System.Text.Json.Serialization;

public class PembelianDetail
{
    [JsonPropertyName("pembelianId")]
    public long PembelID { get; set; }

    [JsonPropertyName("stockId")]
    public long StockID { get; set; }

    [JsonPropertyName("jumlah")]
    public decimal Jumlah { get; set; }

    [JsonPropertyName("harga")]
    public decimal Harga { get; set; }

    [JsonPropertyName("catatan")]
    public string Catatan { get; set; }
}