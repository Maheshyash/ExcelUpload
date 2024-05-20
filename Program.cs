using System.Data;
using System.Data.SqlClient;
using ExcelReportUpload.IRepositories;
using ExcelReportUpload.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models; 

var builder = WebApplication.CreateBuilder(args);


//Add JWTToken Configuration
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,//Validate the server and generate the token
        ValidateLifetime = true,
        ValidateAudience = true,// It will validate the audience during token validation
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidAudience = builder.Configuration["JWT:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]))
    };
});

//End of JSTToken Configuration
// BEGIN OF ADDING CORS POLICY FOR SPECIFICORIGIN
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowSpecificOrigin",
//        builder =>
//        {
//            builder.WithOrigins("http://example.com") // Replace with the client domain
//                   .AllowAnyHeader()
//                   .AllowAnyMethod();
//        });
//});
// END OF ADDING CORS POLICY FOR SPECIFICORIGIN
// BEGIN OF ADDING CORS POLICY FOR ALL LINKS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});
// END OF ADDING CORS POLICY FOR ALL LINKS
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//Adding Token inisde AddSwaggerGen
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition(name: JwtBearerDefaults.AuthenticationScheme, securityScheme: new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Please Provide Token",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = JwtBearerDefaults.AuthenticationScheme
            }
        }, new string[]{}
        }
        
    });
});
//End of Adding Token inisde AddSwaggerGen
var configuration = builder.Configuration;
var connectionString = configuration.GetConnectionString("LocalhostConnectionString");

// Register IDbConnection
builder.Services.AddScoped<IDbConnection>(_ => new SqlConnection(connectionString));


builder.Services.AddScoped<IExcelUploadRepository, ExcelUploadRepository>();
builder.Services.AddScoped<ILoginRepository, LoginRepository>();
var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
//BEGIN OF ADDING CORS POLOCIY
//app.UseCors("AllowSpecificOrigin");
app.UseCors("AllowAll");
//END OF ADDING CORS POLOCIY
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

