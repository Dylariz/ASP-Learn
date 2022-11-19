using RazorPagesPizza.Models;

namespace RazorPagesPizza.Services;

public static class PizzaService
{
    private static List<Pizza> Pizzas { get; }
    private static int id = 3;

    static PizzaService()
    {
        Pizzas = new List<Pizza>
        {
            new Pizza { Id = 1, Name = "Classic Italian", Price = 500, Size = PizzaSize.Large, IsCheeseSides = false },
            new Pizza { Id = 2, Name = "Veggie", Price = 700, Size = PizzaSize.Small, IsCheeseSides = true }
        };
    }
    
    public static List<Pizza> GetAll() => Pizzas;
    public static Pizza? Get(int id) => Pizzas.FirstOrDefault(p => p.Id == id);
    
    public static void Add(Pizza pizza)
    {
        pizza.Id = id++;
        Pizzas.Add(pizza);
    }
    
    public static void Update(Pizza pizza)
    {
        var index = Pizzas.FindIndex(p => p.Id == pizza.Id);
        if (index == -1)
            return;
        Pizzas[index] = pizza;
    }
    
    public static void Delete(int id)
    {
        var pizza = Get(id);
        if (pizza is null)
            return;

        Pizzas.Remove(pizza);
    }
}