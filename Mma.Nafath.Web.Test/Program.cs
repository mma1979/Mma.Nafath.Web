using Mma.Nafath.Web.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddNafathWebAuthentication(options =>
{
    options.BaseUrl = "https://api.example.sa/";
    options.CreateSessionEndpoint = "/sessions";
    options.FetchJwkEndpoint = "/jwk/{0}";
    options.BasicAuth = "Basic VThIMVUVZOWdCYlFqYzRVTZzWGtKd0NSTnI6YWlHWE02xaUY0Qw==";
    options.CertificatePath = "./certificates/cert.pfx";
    options.CertificatePassword = "P@ssw0rd";
    options.UseCertificate = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
