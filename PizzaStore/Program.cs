using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using PizzaStore.Models;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("Pizzas") ?? "Data Source=Pizzas.db";

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSqlite<PizzaContext>(connectionString);
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "PizzaStore API",
        Description = "Making the Pizzas you love",
        Version = "v1"
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "PizzaStore API V1"); });

app.MapGet("/", () => Results.Redirect("/swagger"));

app.MapGet("/pizzas", async (PizzaContext db) => await db.Pizzas.ToListAsync());
app.MapGet("/pizzas/{id:int}",
    async (PizzaContext db, int id) =>
        await db.Pizzas.FindAsync(id) is Pizza pizza ? Results.Ok(pizza) : Results.NotFound());

app.MapPost("/pizzas", async (PizzaContext db, Pizza pizza) =>
{
    await db.Pizzas.AddAsync(pizza);
    await db.SaveChangesAsync();
    return Results.Created($"pizza/{pizza.Id}", pizza);
});

app.MapPut("/pizzas/{id:int}", async (PizzaContext db, int id, Pizza updatePizza) =>
{
    if (id != updatePizza.Id)
        return Results.BadRequest();
    
    Pizza? pizza = await db.Pizzas.FindAsync(id);
    
    if (pizza is null)
        return Results.NotFound();

    pizza.Name = updatePizza.Name;
    pizza.Description = updatePizza.Description;
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/pizzas/{id:int}", async (PizzaContext db, int id) =>
{
    Pizza? pizza = await db.Pizzas.FindAsync(id);
    if (pizza is null)
        return Results.NotFound();
    db.Pizzas.Remove(pizza);
    await db.SaveChangesAsync();
    
    db.Database.ExecuteSqlRaw($"UPDATE Pizzas SET Id = Id - 1 WHERE Id > {id}");
    db.Database.ExecuteSqlRaw("UPDATE sqlite_sequence SET seq = seq - 1 WHERE name = 'Pizzas'");
    
    return Results.Ok();
});

app.Run();