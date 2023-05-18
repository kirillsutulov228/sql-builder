var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddScoped<ExampleService>();
builder.Services.AddCors();

var app = builder.Build();
app.UseRouting();
app.MapControllers();

app.UseCors((builder) => {
  builder.AllowAnyOrigin();
  builder.AllowAnyMethod();
  builder.AllowAnyHeader();
});

app.Run();
