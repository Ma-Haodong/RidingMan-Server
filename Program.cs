var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<RidingManContext>();
var app = builder.Build();
app.Urls.Add("http://*:5000");

string imagesPath = $"{Directory.GetCurrentDirectory()}/icons";
Directory.CreateDirectory(imagesPath);
app.AddHttpHandler();

app.Run();
