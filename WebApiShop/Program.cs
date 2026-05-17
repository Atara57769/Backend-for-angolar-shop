using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Repositories;
using Services;
using NLog.Web;
using WebApiShop;
using WebApiShop.Middleware;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAutoMapper(cfg => { }, AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<IUserService, UserServices>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IRatingService, RatingService>();
builder.Services.AddScoped<IRatingRepository, RatingRepository>();
builder.Services.AddScoped<IChatBotRepository, ChatBotRepository>();
builder.Services.AddScoped<IChatBotService, ChatBotService>();
builder.Services.AddDbContext<ApiDBContext>
    (options => options.UseSqlServer(builder.Configuration.GetConnectionString("Home")));
// Add services to the container.
builder.Host.UseNLog();
builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddCors(o => o.AddDefaultPolicy(
    p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

builder.Services.AddOpenApi();
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "My API V1");
    });
}
// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseErrorHandling();

//app.UseRating();

app.UseStaticFiles();

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("home.html");

app.Run();
