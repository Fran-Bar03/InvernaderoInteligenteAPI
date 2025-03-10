using InvernaderoInteligente.Data;
using InvernaderoInteligente.Data.Interfaces;
using InvernaderoInteligente.Data.Services;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Registra IMongoClient como Singleton
builder.Services.AddSingleton<IMongoClient>(sp =>
    new MongoClient("mongodb+srv://emilianomongeosuna:monge25@test.gqelt.mongodb.net/?retryWrites=true&w=majority&appName=Test")
);

builder.Services.Configure<ConfiguracionMongo>(builder.Configuration.GetSection("MongoSettings"));
builder.Services.AddSingleton((sp =>
{
    var config = sp.GetRequiredService<IOptions<ConfiguracionMongo>>().Value;
    return new MongoClient(config.Connection);
}));



// Registrar el servicio de invernaderos
builder.Services.AddScoped<IInvernaderoService, InvernaderoService>();

builder.Services.AddScoped<IUsuarioService, UsuarioService>();

builder.Services.AddScoped<UsuarioService>();

builder.Services.AddScoped<InvernaderoService>();






var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
