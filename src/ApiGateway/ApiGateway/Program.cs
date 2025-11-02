// مسیر: src/ApiGateway/ApiGateway/Program.cs
var builder = WebApplication.CreateBuilder(args);

// اضافه کردن سرویس‌های Reverse Proxy و خواندن پیکربندی از appsettings
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

// فعال کردن Middleware برای Reverse Proxy
app.MapReverseProxy();

app.Run();
