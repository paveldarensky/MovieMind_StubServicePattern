using ServiceStub.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession();

// Читаем флаг из конфигурации
bool useStub = builder.Configuration.GetValue<bool>("UseStubLogin");
// Регистрируем нужную реализацию ILoginService
if (useStub)
    builder.Services.AddScoped<ILoginService, StubLoginService>();
else
    builder.Services.AddScoped<ILoginService, RealLoginService>();

// Читаем флаг из конфигурации
bool useStubMovieService = builder.Configuration.GetValue<bool>("UseStubRecommendation");
// Регистрируем нужную реализацию IMovieRecommendationService
if (useStubMovieService)
{
    builder.Services.AddScoped<IMovieRecommendationService, StubMovieRecommendationService>();
}
else
{
    builder.Services.AddScoped<IMovieRecommendationService, RealMovieRecommendationService>();
}


var app = builder.Build();

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

app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Login}/{id?}");

app.MapControllerRoute(
    name: "moviemindservice",
    pattern: "{controller=MovieMindService}/{action=Index}/{id?}");

app.Run();
