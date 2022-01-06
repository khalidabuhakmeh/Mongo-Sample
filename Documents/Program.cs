using System.Linq.Expressions;
using Documents;
using MongoDB.Bson;
using MongoDB.Driver;
using static Microsoft.AspNetCore.Http.Results;
using Branchy;

var builder = WebApplication.CreateBuilder(args);

// register client, database, and collections 
builder.Services.AddMongo();

var app = builder.Build();

app.Path("/", root =>
{
    root.MapGet(Root);
    root.Path("person", people =>
    {
        people.MapPost(CreatePerson);
        people.MapGet("{id}", GetPerson);
    });
});

app.Run();

// Endpoints
async Task<IResult> Root(IMongoCollection<Person> people)  {
    Expression<Func<Person, bool>> filter = p => true;

    var latest = await people
        .Find(filter)
        .SortByDescending(x => x.CreatedAt)
        .Limit(10)
        .ToListAsync();

    var total = await people.CountDocumentsAsync(filter);

    return Ok(new
    {
        results = latest ?? new(),
        total = total
    });
}

async Task<IResult> CreatePerson(IMongoCollection<Person> people, Person person)
{
    // avoid the ability to set any other properties
    // typically I'd use a different request type
    var newPerson = new Person {
        Name = person.Name,
        Age = person.Age
    };
    await people.InsertOneAsync(newPerson);
    return Created($"/person/{person.Id}", newPerson);
}

async Task<IResult> GetPerson(IMongoCollection<Person> people, string id)
{
    if (!ObjectId.TryParse(id, out _))
        return BadRequest();

    var person = await people.Find(p => p.Id == id).SingleAsync();

    return person is { }
        ? Ok(person)
        : NotFound();
}
