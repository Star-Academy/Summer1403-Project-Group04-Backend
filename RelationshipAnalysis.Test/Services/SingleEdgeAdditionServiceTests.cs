// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.DependencyInjection;
// using RelationshipAnalysis.Context;
// using RelationshipAnalysis.Services.GraphServices;
//
// namespace RelationshipAnalysis.Test.Services;
//
// public class SingleEdgeAdditionServiceTests
// {
//     private readonly SingleEdgeAdditionService _sut;
//     private readonly IServiceProvider _serviceProvider;
//     public SingleEdgeAdditionServiceTests()
//     {
//         var serviceCollection = new ServiceCollection();
//     
//         var options = new DbContextOptionsBuilder<ApplicationDbContext>()
//             .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
//             .Options;
//     
//         serviceCollection.AddScoped(_ => new ApplicationDbContext(options));
//     
//         _serviceProvider = serviceCollection.BuildServiceProvider();
//     
//         _sut = new(_serviceProvider);
//     }
//     
// }
