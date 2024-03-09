var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseRouting();

for (var index = 1; index < 6;index++)
{
    var localIndex = index;
    app.MapGet(localIndex.ToString(), () => $"Endpoint #{localIndex}");
}

app.Run();
