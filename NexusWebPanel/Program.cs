using Microsoft.AspNetCore.SignalR;
using NexusWebPanel.Hubs;
using NexusWebPanel.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();
builder.Services.AddHttpClient();

// Register WebsocketListener as a singleton
builder.Services.AddSingleton<WebsocketListener>(builder => new(8080));

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<LogHub>("/logs");

WebsocketListener listener = app.Services.GetRequiredService<WebsocketListener>();
listener.Start();

IHubContext<LogHub> hubContext = app.Services.GetRequiredService<IHubContext<LogHub>>();

listener.MessageReceived += message =>
    hubContext.Clients.All.SendAsync("ReceiveMessage", message);

app.Run();
