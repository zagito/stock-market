var builder = DistributedApplication.CreateBuilder(args);

var orderdb =  builder.AddPostgres("order-db")
    .WithDataVolume()
    .WithPgAdmin();

var portfolidb = builder.AddPostgres("portfoli-db")
    .WithDataVolume()
    .WithPgAdmin();

var rabbitMq = builder.AddRabbitMQ("stock-mq")
    .WithManagementPlugin();

builder.AddProject<Projects.Order_API>("order-api")
    .WithReference(orderdb)
    .WithReference(rabbitMq);

builder.AddProject<Projects.Portfolio_API>("portfolio-api")
    .WithReference(portfolidb)
    .WithReference(rabbitMq);

builder.AddProject<Projects.Price_Grpc>("price-grpc")
    .WithReference(rabbitMq);

builder.Build().Run();
