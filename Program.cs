using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<PackageDB>(opt => opt.UseInMemoryDatabase("PackageDB"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
var app = builder.Build();

try
{
    app.MapGet("/package", async (PackageDB db) =>
    await db.Packages.ToListAsync());
}
catch (Exception ex)
{
    LogException(ex);
    return;
}

//app.MapGet("/package", async (PackageDB db) =>
//    await db.Packages.ToListAsync());

//app.MapGet("/todoitems/complete", async (PackageDB db) =>
//    await db.Todos.Where(t => t.IsComplete).ToListAsync());

app.MapGet("/package/{id}", async (int id, PackageDB db) =>
    await db.Packages.FindAsync(id)
        is PackageTest package
            ? Results.Ok(package)
            : Results.NotFound());

app.MapPost("/package", async (PackageTest package, PackageDB db) =>
{
    db.Packages.Add(package);
    await db.SaveChangesAsync();

    return Results.Created($"/package/{package.Id}", package);
});

app.MapPut("/package/{id}", async (int id, PackageTest inputPackage, PackageDB db) =>
{
    var package = await db.Packages.FindAsync(id);

    if (package is null) return Results.NotFound();

    package.Name = inputPackage.Name;
    //package.IsComplete = inputPackage.IsComplete;

    await db.SaveChangesAsync();

    return Results.NoContent();
});

//app.MapDelete("/package/{id}", async (int id, PackageDB db) =>
//{
//    if (await db.Todos.FindAsync(id) is PackageTest todo)
//    {
//        db.Todos.Remove(todo);
//        await db.SaveChangesAsync();
//        return Results.NoContent();
//    }

//    return Results.NotFound();
//});

void LogException(Exception error)
{
    Console.WriteLine("An error occurred:");
    Console.WriteLine(error.GetBaseException().ToString());
}

app.Run();