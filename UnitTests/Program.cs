using Microsoft.AspNetCore.Builder;
using Microsoft.DotNet.Scaffolding.Shared.ProjectModel;
using Microsoft.Extensions.DependencyInjection;
using NoticeBoard.Data;
using UnitTests.Controllers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<NoticeBoardContext>();

var app = builder.Build();


app.Run();
