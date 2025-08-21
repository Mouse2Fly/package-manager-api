using Microsoft.EntityFrameworkCore;
using PackageModels;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<PackageDB>(opt => opt.UseInMemoryDatabase("PackageDB"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        builder => 
        {
            builder.WithOrigins("http://localhost:5173")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowAnyOrigin();
        });
});

var app = builder.Build();

app.UseCors();

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

app.MapGet("/package/{id}", async (int id, PackageDB db) =>
    await db.Packages.FindAsync(id)
        is Package packageBox
            ? Results.Ok(packageBox)
            : Results.NotFound("Not Found"));

app.MapPost("/package", async (PackagePartial packageData, PackageDB db) =>
{
    bool confirmed = false;
    int trackingNumber = 0;

    if (string.IsNullOrWhiteSpace(packageData.SenderAdress) ||
        string.IsNullOrWhiteSpace(packageData.SenderName) ||
        string.IsNullOrWhiteSpace(packageData.SenderPhone) ||
        string.IsNullOrWhiteSpace(packageData.RecipientAdress) ||
        string.IsNullOrWhiteSpace(packageData.RecipientName) ||
        string.IsNullOrWhiteSpace(packageData.RecipientPhone))
    {
        return Results.BadRequest("All fields must be filled.");
    }

    while (!confirmed)
    {
        Random random = new Random();
        trackingNumber = random.Next(100000, 999999);
        if (await db.Packages.FindAsync(trackingNumber) is null)
        {
            confirmed = true;
        }
    }

    Package packageBox = new Package
    {
        TrackingNumber = trackingNumber,
        SenderAdress = packageData.SenderAdress,
        SenderName = packageData.SenderName,
        SenderPhone = packageData.SenderPhone,
        RecipientAdress = packageData.RecipientAdress,
        RecipientName = packageData.RecipientName,
        RecipientPhone = packageData.RecipientPhone,
        CurrentStatus = new[] { "Created", DateTime.Now.ToString("o") },
        CreationDate = DateTime.Now,
        StatusHistory = new[] { "Created", DateTime.Now.ToString("o") }
    };

    db.Packages.Add(packageBox);
    await db.SaveChangesAsync();

    return Results.Created($"/package/{packageBox.TrackingNumber}", packageBox);
});

app.MapPut("/package/status/{id}", async (int id, PackageStatus inputStatus, PackageDB db) =>
{
    var existingPpackage = await db.Packages.FindAsync(id);

    if (existingPpackage is null) return Results.NotFound();

    var Statuses = new[] { "Created", "Sent", "Returned", "Accepted", "Canceled" };
    //Legal actions:
    //Created -> Sent, Canceled / 1, 4
    //Sent -> Accepted, Returned, Canceled / 2, 3, 4
    //Returned -> Sent, Canceled / 1, 4
    //Accepted -> no further actions
    //Canceled -> no further actions

    //step 1: Check if the new status is valid
    for(var i = 0; Statuses.Length > i; i++)
    {
        if (Statuses[i] == inputStatus.NewStatus)
        {
            Console.WriteLine("Current Status index: " + i);
            //check if the new status is legal
            switch (existingPpackage.CurrentStatus[0])
            {
                case "Created":
                    if (i == 0 || i == 2 || i == 3) return Results.BadRequest("Invalid status change from Created");
                    break;
                case "Sent":
                    if (i == 0 || i == 1) return Results.BadRequest("Invalid status change from Sent");
                    break;
                case "Returned":
                    if (i == 0 || i == 2 || i == 3) return Results.BadRequest("Invalid status change from Returned");
                    break;
                case "Accepted":
                    return Results.BadRequest("No further actions allowed for Accepted or Canceled status");
                case "Canceled":
                    return Results.BadRequest("No further actions allowed for Accepted or Canceled status");
            }
            break;
        }
    }
    //step 2: Prepeare the new current status
    var prepedStatus = new string[] { inputStatus.NewStatus, DateTime.Now.ToString("o") };
    existingPpackage.CurrentStatus = prepedStatus;
    //step 3: Prepare the new status history
    var prepedHistory = existingPpackage.StatusHistory
        .Concat(new[] { inputStatus.NewStatus, DateTime.Now.ToString("o") })
        .ToArray();
    existingPpackage.StatusHistory = prepedHistory;

    await db.SaveChangesAsync();

    return Results.Ok(existingPpackage);
});

void LogException(Exception error)
{
    Console.WriteLine("An error occurred:");
    Console.WriteLine(error.GetBaseException().ToString());
}

app.Run();
