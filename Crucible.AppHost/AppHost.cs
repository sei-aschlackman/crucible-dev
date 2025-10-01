var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent)
    .WithPgAdmin();

//var postgresPasswordExpression = postgres.Resource.PasswordParameter;

var playerDb = postgres.AddDatabase("playerDb", "player");

// var keycloakDb = postgres.AddDatabase("keycloakDb", "keycloak");

var keycloak = builder.AddKeycloak("keycloak", 8080)
    .WithRealmImport("./resources/crucible-realm.json");

var playerApi = builder.AddProject<Projects.Player_Api>("player-api", launchProfileName: "Player.Api")
    .WaitFor(postgres)
    .WaitFor(keycloak)
    .WithHttpHealthCheck("api/health/ready")
    .WithReference(playerDb, "PostgreSQL")
    .WithEnvironment("Database__Provider", "PostgreSQL")
    .WithEnvironment("Authorization__Authority", "http://localhost:8080/realms/crucible")
    .WithEnvironment("Authorization__AuthorizationUrl", "http://localhost:8080/realms/crucible/protocol/openid-connect/auth")
    .WithEnvironment("Authorization__TokenUrl", "http://localhost:8080/realms/crucible/protocol/openid-connect/token")
    .WithEnvironment("Authorization_ClientId", "player.api");

builder.Build().Run();
