using APN.RecruitmentTask.Api.Endpoints;
using APN.RecruitmentTask.Application;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.RegisterApplication();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.AddBooksEndpoints();
app.AddOrdersEndpoints();

app.Run();
