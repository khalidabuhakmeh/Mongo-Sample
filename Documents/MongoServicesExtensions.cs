using MongoDB.Driver;

namespace Documents;

public static class MongoServicesExtensions
{
    public static void AddMongo(this IServiceCollection services)
    {
        services.AddScoped(_ =>
            new MongoClient("mongodb://root:example@localhost:27017/"));
        
        services.AddScoped(svc => {
            var client = svc.GetRequiredService<MongoClient>();
            var database = client.GetDatabase("hello-world");
            return database;
        });
        
        // register collections
        services.AddScoped<IMongoCollection<Person>>(svc =>
        {
            var db = svc.GetRequiredService<IMongoDatabase>();
            return db.GetCollection<Person>(Person.Collection);
        });
    }
}