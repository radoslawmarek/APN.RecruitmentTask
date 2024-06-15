using System.Text;
using APN.RecruitmentTask.Api.Endpoints;
using APN.RecruitmentTask.Application;
using APN.RecruitmentTask.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var secretKey = builder.Configuration["Authentication:SecretKey"];
        if (string.IsNullOrEmpty(secretKey))
        {
            throw new InvalidOperationException("Secret key is missing");
        }
        
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey)),
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateActor = false
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.RegisterInfrastructure(builder.Configuration);
builder.Services.RegisterApplication();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.AddAuthenticationEndpoints();
app.AddBooksEndpoints();
app.AddOrdersEndpoints();

app.Run();
