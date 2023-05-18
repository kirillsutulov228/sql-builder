var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddScoped<ExampleService>();

var app = builder.Build();
app.UseRouting();
app.MapControllers();

app.Run();
