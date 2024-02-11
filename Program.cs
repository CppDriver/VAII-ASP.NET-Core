using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MultimediaLibrary;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<MultimediaLibrary.Data.DatabaseContext>();
builder.Services.AddHttpContextAccessor();
builder.Services.Configure<MultimediaLibrarySettings>(builder.Configuration.GetSection("MultimediaLibrarySettings"));
builder.Services.AddScoped<MultimediaLibrary.Services.AuthService.IAuthService, MultimediaLibrary.Services.AuthService.AuthService>();
builder.Services.AddScoped<MultimediaLibrary.Services.UploadService.IUploadService, MultimediaLibrary.Services.UploadService.UploadService>();
builder.Services.AddScoped<MultimediaLibrary.Services.ImageService.IMediaService, MultimediaLibrary.Services.ImageService.MediaService>();
builder.Services.AddScoped<MultimediaLibrary.Services.UserService.IUserService, MultimediaLibrary.Services.UserService.UserService>();
builder.Services.AddScoped<MultimediaLibrary.Services.GalleryService.IGalleryService, MultimediaLibrary.Services.GalleryService.GalleryService>();
builder.Services.AddScoped<MultimediaLibrary.Services.CommentService.ICommentService, MultimediaLibrary.Services.CommentService.CommentService>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "MultimediaLibrary",
            ValidAudience = "MultimediaLibrary",
            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes("temporary solution, needs 256 bits, it had only 144, but why it was working till now"))
        };
    });
builder.Services.AddDistributedMemoryCache();

var app = builder.Build();

app.UseAuthentication();

app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors(policy =>
    policy.WithOrigins("http://localhost:4200")
          .AllowAnyMethod()
          .AllowAnyHeader());
}

app.UseHttpsRedirection();


app.MapControllers();

app.Run();
