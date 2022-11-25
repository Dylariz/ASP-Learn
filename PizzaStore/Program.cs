using Microsoft.OpenApi.Models;
using PizzaStore.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "PizzaStore API", Description = "Making the Pizzas you love", Version = "v1" });
});
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "PizzaStore API V1");
});

app.MapGet("/", () => "Hello World!");
app.MapGet("/pizzas", PizzaService.GetPizzas);
app.MapGet("/pizzas/{id}", PizzaService.GetPizza);
app.MapPost("/pizzas", PizzaService.CreatePizza);
app.MapPut("/pizzas", PizzaService.UpdatePizza);
app.MapDelete("/pizzas/{id}", PizzaService.RemovePizza);


app.Run();
