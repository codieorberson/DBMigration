using DBMigration.Repositories;
using DBMigration.Repositories.Interfaces;
using DBMigration.Services;
using DBMigration.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);
string sso2Server = string.Empty;
int refreshtime = 0;
if (builder.Configuration["SSO2Env"] == "Dev")
{
    sso2Server = "SSO2-DEV.corp.vha.ad";
    refreshtime = 16;
}
else if (builder.Configuration["SSO2Env"] == "Test")
{
    sso2Server = "SSO2-Test.corp.vha.ad";
    refreshtime = 16;
}
else if (builder.Configuration["SSO2Env"] == "Stage")
{
    sso2Server = "SSO2-Stg.corp.vha.ad";
    refreshtime = 6;
}
else if (builder.Configuration["SSO2Env"] == "Prod")
{
    sso2Server = "SSO2-Prod.corp.vha.ad";
    refreshtime = 3;
}

builder.Configuration["RefreshTime"] = refreshtime.ToString();
builder.Configuration["ConnectionStrings:MetisConnection"] = string.Format(builder.Configuration.GetConnectionString("MetisConnection"), sso2Server);
builder.Configuration["ConnectionStrings:EWSDataStoreConnection"] = string.Format(builder.Configuration.GetConnectionString("EWSDataStoreConnection"), sso2Server);
builder.Configuration["ConnectionStrings:SDE2014Connection"] = string.Format(builder.Configuration.GetConnectionString("SDE2014Connection"), sso2Server);
builder.Configuration["ConnectionStrings:UHC_SMDConnection"] = string.Format(builder.Configuration.GetConnectionString("UHC_SMDConnection"), sso2Server);
builder.Configuration["ConnectionStrings:UHCCoreConnection"] = string.Format(builder.Configuration.GetConnectionString("UHCCoreConnection"), sso2Server);
builder.Configuration["ConnectionStrings:UHCNotificationConnection"] = string.Format(builder.Configuration.GetConnectionString("UHCNotificationConnection"), sso2Server);
builder.Configuration["ConnectionStrings:User_SyncConnection"] = string.Format(builder.Configuration.GetConnectionString("User_SyncConnection"), sso2Server);
builder.Configuration["ConnectionStrings:SSOLoggingConnection"] = string.Format(builder.Configuration.GetConnectionString("SSOLoggingConnection"), sso2Server);


// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IDBUsersRepository, DBUsersRepository>();
builder.Services.AddSingleton<IIceUserPermissionsRepository, IceUserPermissionsRepository>();
builder.Services.AddSingleton<IIceTablesRefreshedRepository, IceTablesRefreshedRepository>();
builder.Services.AddSingleton<IMetisRepository, MetisRepository>();
builder.Services.AddSingleton<IDBOSRepository, DBOSRepository>();
builder.Services.AddSingleton<IDBUsersService, DBUsersService>();
builder.Services.AddSingleton<IGenerateDocumentsService, GenerateDocumentsService>();
builder.Services.AddSingleton<IIceUserPermissionsService, IceUserPermissionsService>();
builder.Services.AddSingleton<IIceTablesRefreshedService, IceTablesRefreshedService>();
builder.Services.AddSingleton<IMetisDBOSService, MetisDBOSService>();
builder.Services.AddSingleton<IDBOSService, DBOSService>();
builder.Services.AddSingleton<IExcelService, ExcelService>();


var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
