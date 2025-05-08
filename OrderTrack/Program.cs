using Microsoft.EntityFrameworkCore;
using OrderTrack.Models.Domain;
using OrderTrack.Services.Implementations;
using OrderTrack.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(Program));


var EmployeeSpecificOrigins = "_employeeAppSpecificOrigins";


builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AllowAll",
                      policy =>
                      {
                          policy.AllowAnyOrigin()  // Permite cualquier origen
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                      });
});


var stringcConnection = builder.Configuration.GetConnectionString("OrderTrack");
builder.Services.AddDbContext<OrdertrackContext>(data => data.UseSqlServer(stringcConnection));

//Clases
builder.Services.AddHttpContextAccessor();

builder.Services.AddTransient<IBulkDataService, BulkDataService>();
builder.Services.AddTransient<ICargaDatosService, CargaDatosService>();
builder.Services.AddTransient<IDataProcessingService, DataProcessingService>();
builder.Services.AddTransient<IExcelService, ExcelService>();
builder.Services.AddTransient<IProductosService, ProductosService>();
builder.Services.AddTransient<IPedidoService, PedidoService>();
builder.Services.AddTransient<ITiendasService, TiendasService>();
builder.Services.AddTransient<IClienteService, ClienteService>();
builder.Services.AddTransient<IDatatableService, DatatableService>();





var app = builder.Build();

// Configure the HTTP request pipeline.

    app.UseSwagger();
    app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseAuthorization();
app.UseCors("AllowAll");

app.MapControllers();

app.Run();
