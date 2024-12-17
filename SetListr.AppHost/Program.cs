var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.SetListr_ApiService>("apiservice");

builder.AddProject<Projects.SetListr_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
