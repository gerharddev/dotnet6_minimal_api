using Microsoft.EntityFrameworkCore;
using MinimalApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<UserDbContext>(options => options.UseInMemoryDatabase("Users"));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapSwagger();
app.UseSwaggerUI();

app.MapGet("/users", async (UserDbContext context) =>
await context.Users.ToListAsync())
.WithName("GetAllUsers");

app.MapGet("/users/{id}", async (int id,UserDbContext context) =>
await context.Users.FindAsync(id)
    is User user
        ? Results.Ok(user)
        : Results.NotFound())
.WithName("GetUserById")
.Produces<User>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound);

app.MapPost("/users", async (User user, UserDbContext context) =>
{

    context.Users.Add(user);
    await context.SaveChangesAsync();

    return Results.Created($"/users/{user.Id}", user);
})
.WithName("PostUser")
.ProducesValidationProblem()
.Produces<User>(StatusCodes.Status201Created);

app.Run();


