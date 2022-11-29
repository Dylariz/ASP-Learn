using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using SimpleApi;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var optionsBuilder = new DbContextOptionsBuilder<PersonContext>();
DbContextOptions<PersonContext> options = optionsBuilder.UseSqlite("Data Source=StaticResourses/people.db").Options;

app.Run(async context =>
{
    var response = context.Response;
    var request = context.Request;
    var path = request.Path;
    
    // Regex.IsMatch(path, @"/api/users/(\d*)")
    // path == "/api/users"
    if (Regex.IsMatch(path, @"/api/users/?(\d*)"))
    {
        response.ContentType = "text/json";
        
        int id;
        switch (request.Method)
        {
            case "GET":
                if (int.TryParse(path.Value?.Split('/').Last(), out id))
                    await GetPerson(id, response);
                else
                    await GetUsers(response); 
                break;
            case "POST":
                await CreatePerson(request, response);
                break;
            case "PUT":
                await UpdatePerson(request, response);
                break;
            case "DELETE":
                if (int.TryParse(path.Value?.Split('/').Last(), out id))
                    await DeletePerson(id, response);
                break;
        }
    }
    else
    {
        response.ContentType = "text/html";
        await response.SendFileAsync("StaticResourses/index.html");
    }
});

app.Run();

async Task GetUsers(HttpResponse response)
{
    await using var db = new PersonContext(options);
    var users = await db.Users.ToListAsync();
    await response.WriteAsJsonAsync(users);
}

async Task GetPerson(int id, HttpResponse response)
{
    await using var db = new PersonContext(options);
    var person = await db.Users.FindAsync(id);
    if (person != null)
    {
        await response.WriteAsJsonAsync(person);
    }
    else
    {
        response.StatusCode = 404;
        await response.WriteAsJsonAsync(new { message = "Пользователь не найден"});
    }
    
}

async Task CreatePerson(HttpRequest request, HttpResponse response)
{
    try
    {
        await using var db = new PersonContext(options);
        Person? person = await request.ReadFromJsonAsync<Person>();
        if (person != null)
        {
            db.Users.Add(person);
            await db.SaveChangesAsync();
            response.StatusCode = 201;
            await response.WriteAsJsonAsync(person);
        }
        else
        {
            throw new Exception("Некоректные данные");
        }
    }
    catch (Exception e)
    {
        response.StatusCode = 400;
        await response.WriteAsJsonAsync(new { message = e.Message });
    }
}

async Task DeletePerson(int id, HttpResponse response)
{
    await using var db = new PersonContext(options);
    var person = await db.Users.FindAsync(id);
    if (person != null)
    {
        db.Users.Remove(person);
        await db.SaveChangesAsync();
        await response.WriteAsJsonAsync(person);
    }
    else
    {
        response.StatusCode = 404;
        await response.WriteAsJsonAsync(new { message = "Пользователь не найден" });
    }
}

async Task UpdatePerson(HttpRequest request, HttpResponse response)
{
    await using var db = new PersonContext(options);
    var updatePerson = await request.ReadFromJsonAsync<Person>();
    var person = await db.Users.FindAsync(updatePerson?.Id);
    if (person != null)
    {
        person.Name = updatePerson?.Name;
        person.Age = updatePerson?.Age;
        db.Users.Update(person);
        await db.SaveChangesAsync();
        await response.WriteAsJsonAsync(person);
    }
    else
    {
        response.StatusCode = 404;
        await response.WriteAsJsonAsync(new { message = "Пользователь не найден" });
    }
}