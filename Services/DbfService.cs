using System.Data;
using System.Data.OleDb;

public class DbfService
{
    private readonly string _connectionString;

    public DbfService()
    {
        // Ganti dengan path folder tempat file DBF Anda berada
        string dbfFolderPath = @"C:\Users\LENOVO\Documents\Programming\python\GF API\ST_DB";
        _connectionString = $"Provider=VFPOLEDB.1;Data Source={dbfFolderPath};";
    }
    
    // Define the missing PembelianWithDetails class
    public class PembelianWithDetails
    {
        public Pembelian Pembelian { get; set; }
        public List<PembelianDetail> Details { get; set; }
    }

    public List<Pembelian> GetPembelian(int page, int limit, string sort, string filter)
    {
        var result = new List<Pembelian>();
        var fullData = new DataTable();

        using (var connection = new OleDbConnection(_connectionString))
        {
            try
            {
                connection.Open();

                // PERUBAHAN DI SINI: Query SQL ditambahkan JOIN
                string sqlQuery = @"
                    SELECT 
                        T1.ID, T1.NO, T1.TANGGAL, T1.VENDOR, T1.TOT_HARGA,
                        T2.NAMA AS VendorNama
                    FROM s_pembel.dbf AS T1
                    INNER JOIN s_vendor.g8a AS T2 ON T1.VENDOR = T2.ID";

                if (!string.IsNullOrWhiteSpace(filter))
                {
                    sqlQuery += $" WHERE T1.NO LIKE '%{filter}%' OR T2.NAMA LIKE '%{filter}%'";
                }
                
                using (var adapter = new OleDbDataAdapter(sqlQuery, connection))
                {
                    adapter.Fill(fullData);
                }
            }
            catch (Exception ex)
            {
                // Jika terjadi error, Anda bisa log atau melempar exception
                return null;
            }
        }

        var query = fullData.AsEnumerable();
        if (!string.IsNullOrWhiteSpace(sort))
        {
            query = query.OrderBy(row => row[sort]);
        }
        else
        {
            query = query.OrderBy(row => row["ID"]);
        }

        var pagedData = query.Skip((page - 1) * limit).Take(limit).ToList();

        foreach (DataRow row in pagedData)
        {
            // PERUBAHAN DI SINI: Ambil nilai VendorNama dari DataRow
            result.Add(new Pembelian
            {
                ID = Convert.ToInt64(row["ID"]),
                No = row["NO"].ToString().Trim(),
                Tanggal = Convert.ToDateTime(row["TANGGAL"]),
                Vendor = Convert.ToInt64(row["VENDOR"]),
                VendorNama = row["VendorNama"].ToString().Trim(),
                TotHarga = Convert.ToDecimal(row["TOT_HARGA"])
            });
        }
        return result;
    }


    public List<PembelianDetail> GetPembelianDetail(long id)
    {
        var result = new List<PembelianDetail>();

        using (var connection = new OleDbConnection(_connectionString))
        {
            connection.Open();
            string sqlQuery = $"SELECT PEMBEL_ID, STOCK_ID, JUMLAH, HARGA, CATATAN FROM s_pembed.g8a WHERE PEMBEL_ID = {id}";

            using (var command = new OleDbCommand(sqlQuery, connection))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    result.Add(new PembelianDetail
                    {
                        PembelID = Convert.ToInt64(reader["PEMBEL_ID"]),
                        StockID = Convert.ToInt64(reader["STOCK_ID"]),
                        Jumlah = Convert.ToDecimal(reader["JUMLAH"]),
                        Harga = Convert.ToDecimal(reader["HARGA"]),
                        Catatan = reader["CATATAN"].ToString().Trim()
                    });
                }
            }
        }
        return result;
    }

    public PembelianWithDetails? GetPembelianWithDetails(long id)
    {
        Pembelian? pembelianUtama = null;
        var details = new List<PembelianDetail>();

        using (var connection = new OleDbConnection(_connectionString))
        {
            connection.Open();

            // 1. Ambil data pembelian utama
            string sqlUtama = $"SELECT ID, NO, TANGGAL, VENDOR, TOT_HARGA FROM s_pembel.dbf WHERE ID = {id}";
            using (var commandUtama = new OleDbCommand(sqlUtama, connection))
            using (var readerUtama = commandUtama.ExecuteReader())
            {
                if (readerUtama.Read())
                {
                    pembelianUtama = new Pembelian
                    {
                        ID = Convert.ToInt64(readerUtama["ID"]),
                        No = readerUtama["NO"].ToString().Trim(),
                        Tanggal = Convert.ToDateTime(readerUtama["TANGGAL"]),
                        Vendor = Convert.ToInt64(readerUtama["VENDOR"]),
                        TotHarga = Convert.ToDecimal(readerUtama["TOT_HARGA"])
                    };
                }
            }

            // Jika pembelian utama tidak ditemukan, kembalikan null
            if (pembelianUtama == null)
            {
                return null;
            }

            // 2. Ambil data detail pembelian
            string sqlDetail = $"SELECT PEMBEL_ID, STOCK_ID, JUMLAH, HARGA, CATATAN FROM s_pembed.g8a WHERE PEMBEL_ID = {id}";
            using (var commandDetail = new OleDbCommand(sqlDetail, connection))
            using (var readerDetail = commandDetail.ExecuteReader())
            {
                while (readerDetail.Read())
                {
                    details.Add(new PembelianDetail
                    {
                        PembelID = Convert.ToInt64(readerDetail["PEMBEL_ID"]),
                        StockID = Convert.ToInt64(readerDetail["STOCK_ID"]),
                        Jumlah = Convert.ToDecimal(readerDetail["JUMLAH"]),
                        Harga = Convert.ToDecimal(readerDetail["HARGA"]),
                        Catatan = readerDetail["CATATAN"].ToString().Trim()
                    });
                }
            }
        }

        return new PembelianWithDetails
        {
            Pembelian = pembelianUtama,
            Details = details
        };
    }
}