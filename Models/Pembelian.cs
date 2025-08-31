using System.Text.Json.Serialization;

public class Pembelian
{
    [JsonPropertyName("id")]
    public long ID { get; set; }

    [JsonPropertyName("no")]
    public string No { get; set; }

    [JsonPropertyName("tanggal")]
    public DateTime Tanggal { get; set; }

    [JsonPropertyName("vendor")]
    public long Vendor { get; set; }

    // Tambahkan properti baru untuk nama vendor
    [JsonPropertyName("vendorNama")]
    public string VendorNama { get; set; }

    [JsonPropertyName("totalHarga")]
    public decimal TotHarga { get; set; }
}