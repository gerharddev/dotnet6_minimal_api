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

app.MapPut("/users/{id}", async (int id, User userUpdate, UserDbContext context) =>
{
    var user = await context.Users.FindAsync(id);

    if (user is null) return Results.NotFound();

    user.Name = userUpdate.Name;
    user.Email = userUpdate.Email;
    await context.SaveChangesAsync();
    return Results.NoContent();
})
.WithName("UpdateUser")
.ProducesValidationProblem()
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status404NotFound);

app.MapDelete("/users/{id}", async (int id, UserDbContext context) =>
{
    if (await context.Users.FindAsync(id) is User user)
    {
        context.Users.Remove(user);
        await context.SaveChangesAsync();
        return Results.Ok(user);
    }

    return Results.NotFound();
})
.WithName("DeleteUser")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status404NotFound);

app.Run();


