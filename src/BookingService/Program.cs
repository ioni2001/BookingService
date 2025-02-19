using AutoMapper;
using BookingService.Middlewares;
using BookingService.Models.Models.Configuration;
using BookingService.Models.Models.Dtos;
using BookingService.Repositories.Implementations;
using BookingService.Repositories.Interfaces;
using BookingService.Services.AsyncDataServices;
using BookingService.Services.Profiles;
using BookingService.Services.Services.Implementations;
using BookingService.Services.Services.Interfaces;
using KafkaFlow;
using KafkaFlow.Configuration;
using KafkaFlow.Serializer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddControllers();
services.AddLogging(builder =>
{
    builder.AddConsole();
});

services.AddEndpointsApiExplorer();
services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "BookingService", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer",
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
{
    new OpenApiSecurityScheme
    {
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer",
        },
    },
    Array.Empty<string>()
},
    });
});

services.AddScoped<IUserRepository, UserRepository>();
services.AddScoped<IBookingRepository, BookingRepository>();
services.AddScoped<IRoomRepository, RoomRepository>();
services.AddScoped<IUserService, UserService>();
services.AddScoped<IBookRoomService, BookRoomService>();
services.AddSingleton<IJwtService, JwtService>();
services.AddSingleton<IKafkaProducerService, KafkaProducerService>();

services.AddSingleton<IMessageBusClient, MessageBusClient>();

services.Configure<MongoDbSettings>(builder.Configuration.GetRequiredSection("MongoDbSettings"));

var mapperConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new MappingProfile());
});

IMapper mapper = mapperConfig.CreateMapper();
services.AddSingleton(mapper);

services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
            ClockSkew = TimeSpan.FromSeconds(0)
        };
    });

services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder
            .SetIsOriginAllowed(origin => true)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});


var kafkaSettings = builder.Configuration.GetRequiredSection("KafkaSettings").Get<KafkaSettings>();
var commentsProducerSettings = builder.Configuration.GetRequiredSection("BookingCreatedProducerSettings").Get<TopicSettings>();

services.AddKafka(kafka => kafka
    .UseMicrosoftLog()
    .AddCluster(cluster => cluster
    .WithBrokers(kafkaSettings?.BootstrapServers)
    .WithSchemaRegistry(schema =>
    {
        schema.Url = kafkaSettings?.SchemaRegistry;
        schema.BasicAuthCredentialsSource = Confluent.SchemaRegistry.AuthCredentialsSource.UserInfo;
        schema.BasicAuthUserInfo = $"{kafkaSettings?.SaslUserName}:{kafkaSettings?.SaslPassword}";
    })
    .WithSecurityInformation(information =>
    {
        information.SecurityProtocol = SecurityProtocol.SaslSsl;
        information.SaslMechanism = SaslMechanism.ScramSha256;
        information.SaslUsername = kafkaSettings?.SaslUserName;
        information.SaslPassword = kafkaSettings?.SaslPassword;
    })
    .AddProducer(
        "booking-created-producer",
        producer => producer
            .DefaultTopic(commentsProducerSettings?.Topic)
            .AddMiddlewares(m => m.AddSingleTypeSerializer<BookingCreatedMessage, NewtonsoftJsonSerializer>()))
    ));


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();

app.UseAuthorization();

app.UseMiddleware<ErrorHandlerMiddleware>();

app.MapControllers();

app.Run();