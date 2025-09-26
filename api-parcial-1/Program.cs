using api_parcial_1;
using api_parcial_1.Data;
using api_parcial_1.Dto;
using api_parcial_1.Mapper;
using api_parcial_1.Validation;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

// registro del contexto para hacer uso en los endpoints
builder.Services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("TestDb"));

// registro de validaciones
builder.Services.AddValidatorsFromAssemblyContaining<PersonValidation>();

// serilog
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .WriteTo.File("logs/log-app01-.log", rollingInterval: RollingInterval.Day)
    .WriteTo.Console());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// ░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░
//using var context = new AppDbContext(); // contexto de la base de datos

// operaciones con la entidad

// datos de prueba - sedeer inicial
//List<Person> persons = new List<Person>();
//persons.Add(new Person(44745058, "Esteban", "Gomez"));
//persons.Add(new Person(46924236, "Diego", "Gomez"));
//persons.Add(new Person(94867994, "Ramon", "Gomez"));
//persons.Add(new Person(29626101, "Lidia", "Salinas"));

//context.AddRange(persons);
//context.SaveChanges();

//context.Persons.Add(new Person
//{
//    Dni = 46924236,
//    FirstName = "Diego",
//    LastName = "Gomez"
//});

//context.SaveChanges();

//var personFirst = context.Persons.First();

// pruebas en consola
//var msg = $"Persona obtenida: {personFirst.FirstName} - {personFirst.LastName}";
//Console.WriteLine(msg);
//Console.WriteLine(personFirst.ToString());

// endpoints
//app.MapGet("api/persons/first", () =>
//{
//    return Results.Ok(PersonMapper.ToResponse(personFirst));
//});

//app.MapGet("api/persons", () =>
//{
//    var personsDto = persons.Select(p => PersonMapper.ToResponse(p)).ToList();
//    return Results.Ok(personsDto);
//});

// error al usar el contexto
//app.MapGet("api/persons/{dni:long}", (long dni) =>
//{

//    var person = context.Persons.FirstOrDefaultAsync(p => p.Dni == dni);

//    if (person == null)
//    {
//        return new
//        {
//            Message = "Ah ocurrido un error",
//            Details = $"Persona con DNI '{dni}' no existe"
//        };
//    }

//    return Results.Ok(PersonMapper.ToResponse(person));

//});

// ░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░

app.MapGet("/log", (ILogger<Program> logger) =>
{
    logger.LogInformation("LOG DE INFORMACION");
    logger.LogError("LOG DE ERROR");
    logger.LogWarning("LOG DE ADVERTENCIA");
    return Results.Ok("Hola Mundo");
});

app.MapPost("api/persons", async (
    AppDbContext context, 
    PersonDto requets, 
    IValidator<PersonDto> validator,
    ILogger<Program> logger
) =>
{

    // validacion
    var validationResult = await validator.ValidateAsync(requets);

    if (!validationResult.IsValid)
    {
        var errors = validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage });
        return Results.BadRequest(errors);
    }

    var savePerson = PersonMapper.ToEntity(requets);

    await context.AddAsync(savePerson);
    context.SaveChanges();

    // usar serilog
    //Console.WriteLine($"Persona creada: ${savePerson.ToString()}");
    logger.LogInformation("Persona creada: {persona}", savePerson.ToString());

    var response = PersonMapper.ToResponse(savePerson);

    return Results.Created(string.Empty, response);

});

app.MapGet("/api/persons", async (AppDbContext context) =>
{
    
    // trae la entidad y la mapea a dto de respuesta
    var persons = await context.Persons.Select(p => PersonMapper.ToResponse(p)).ToListAsync();

    // verifica que se incrementa el id
    //var personsEntity = await context.Persons.ToListAsync(); // trae la entidad directamente
    //foreach (var person in personsEntity)
    //{
    //    Console.WriteLine(person.ToString());
    //}

    return Results.Ok(persons);

});

app.MapGet("/api/persons/count", async (AppDbContext context) =>
{
    var cantidad = await context.Persons.CountAsync();
    return Results.Ok($"Cantidad: {cantidad}");
});

app.MapGet("/api/persons/{dni:long}", async (AppDbContext context, long dni) =>
{
    
    var person = await context.Persons.FirstOrDefaultAsync(p => p.Dni == dni);

    if (person == null)
    {
        return Results.NotFound(new 
        { 
            Message = "Ah ocurrido un error",
            Details = $"Persona con DNI '{dni}' no existe"
        });
    }

    return Results.Ok(PersonMapper.ToResponse(person));

});

app.MapDelete("/api/persons/{dni:long}", async (AppDbContext context, long dni) =>
{

    var personExisting = await context.Persons.FirstOrDefaultAsync(p => p.Dni == dni);

    if (personExisting == null)
    {
        return Results.NotFound(new
        {
            Message = "Ah ocurrido un error",
            Details = $"Persona con DNI '{dni}' no existe"
        });
    }

    context.Persons.Remove(personExisting);
    await context.SaveChangesAsync();

    return Results.Ok($"Persona con DNI '{dni}' eliminado correctamente");

});

// ░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░

app.Run();

