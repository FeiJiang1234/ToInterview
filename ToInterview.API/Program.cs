using ToInterview.API.Multithreading;
using ToInterview.API.Services;
using ToInterview.API.Settings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 注册委托和事件相关服务
builder.Services.AddScoped<EventService>();
builder.Services.AddScoped<UserEventHandlers>();
builder.Services.AddScoped<MultithreadingExamples>();
builder.Services.AddScoped<ThreadSafeEventService>();
builder.Services.AddScoped<ThreadSafeUserEventHandlers>();

builder.Services.AddScoped<AsyncAwaitPrincipleDemo>();

// 配置数据库设置
var dbSettings = builder.Configuration.GetSection("DbSettings");
builder.Services.Configure<DbSettings>(dbSettings);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
