using Cms0053Demo.Data;
using Cms0053Demo.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=cms0053demo.db"));

builder.Services.AddSingleton<DemoCertificateService>();
builder.Services.AddScoped<FileStorageService>();
builder.Services.AddScoped<X12BuilderService>();
builder.Services.AddScoped<X12ParserService>();
builder.Services.AddScoped<CdaSchemaValidator>();
builder.Services.AddScoped<CcdaTemplateValidator>();
builder.Services.AddScoped<SchematronEvaluator>();
builder.Services.AddScoped<LoincClassifier>();
builder.Services.AddScoped<XmlSigVerifier>();
builder.Services.AddScoped<ClaimMatchService>();
builder.Services.AddScoped<AuditHashService>();
builder.Services.AddScoped<ValidationPipelineService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
    var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
    SeedData.Initialize(db, env);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapStaticAssets();
app.MapRazorPages().WithStaticAssets();

app.Run();
