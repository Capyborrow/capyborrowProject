using Microsoft.EntityFrameworkCore;
using capyborrowProject.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//A DB is needed to be able to use the APIContext
//builder.Services.AddDbContext<APIContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("OurDB"))); 


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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
