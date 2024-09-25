using MongoDB.Driver;
using NasaApodApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add all services
builder.Services.AddControllers()
    .AddNewtonsoftJson();

builder.Services.AddSingleton<IMongoClient, MongoClient>(sp =>
{
    var uri = builder.Configuration["MongoDb:ConnectionString"];
    return new MongoClient(uri);
});
builder.Services.AddScoped<IApodService, ApodService>(); 
builder.Services.AddHostedService<ApodDataFetcherService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
