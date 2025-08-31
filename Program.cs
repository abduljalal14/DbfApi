var builder = WebApplication.CreateBuilder(args);

// Tambahkan layanan ke container Dependency Injection
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<DbfService>(); // Daftarkan DbfService sebagai Singleton

var app = builder.Build();

// Konfigurasi pipeline permintaan HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(); 
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();