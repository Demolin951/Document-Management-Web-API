using DocumentApi.Data;
using Microsoft.EntityFrameworkCore;
using DocumentApi.Services;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite("Data Source=documents.db"));
builder.Services.AddScoped<AccessService>();
builder.Services.AddScoped<VersionService>();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFIle = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFIle);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{

    app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/openapi/v1.json", "v1");
});
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();