var builder = DistributedApplication.CreateBuilder(args);

var keycloak = builder.AddKeycloak("keycloak", 8080)
                      .WithArgs("--verbose")
                      .WithRealmImport(@"../keycloak")
                      .WithDataVolume();

var apiService = builder.AddProject<Projects.SetListr_ApiService>("apiservice")
                        .WithReference(keycloak)
                        .WaitFor(keycloak);

builder.AddProject<Projects.SetListr_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(keycloak)
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
