using InvernaderoInteligente.Data;
using InvernaderoInteligente.Data.Interfaces;
using InvernaderoInteligente.Data.Services;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile ("appsettings.json", optional: false, reloadOnChange: true);



builder.Services.AddAuthentication ("Bearer")
   .AddJwtBearer ("Bearer", options => {
     options.RequireHttpsMetadata = false;

     var JTW = builder.Configuration["Jwt:Secret"]!;

     SymmetricSecurityKey issuersigningkey = new SymmetricSecurityKey (Encoding.UTF8.GetBytes (JTW));

     options.TokenValidationParameters = new TokenValidationParameters {

       ValidateIssuer = false,
       ValidateAudience = false,
       ValidateLifetime = true,
       ValidateIssuerSigningKey = true,

       IssuerSigningKey = issuersigningkey,

       LifetimeValidator = (DateTime? _, DateTime? expires, SecurityToken _, TokenValidationParameters _) => {
         return expires.HasValue && expires > DateTime.UtcNow;
       }


     };
   });





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

builder.Services.AddScoped<ISensorService, SensorService>();

builder.Services.AddScoped<UsuarioService>();

builder.Services.AddScoped<InvernaderoService>();

builder.Services.AddScoped<SensorService>();

builder.Services.AddSingleton<AuthUsuarioService>();





var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection(); 
app.UseAuthentication ();
app.UseAuthorization();
app.MapControllers();

app.Run();
