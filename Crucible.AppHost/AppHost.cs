var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent)
    .WithPgAdmin();

// var keycloakDb = postgres.AddDatabase("keycloakDb", "keycloak");

var keycloak = builder.AddKeycloak("keycloak", 8080)
    .WithRealmImport("./resources/crucible-realm.json");

var playerDb = postgres.AddDatabase("playerDb", "player");

var playerApi = builder.AddProject<Projects.Player_Api>("player-api", launchProfileName: "Player.Api")
    .WaitFor(postgres)
    .WaitFor(keycloak)
    .WithHttpHealthCheck("api/health/ready")
    .WithReference(playerDb, "PostgreSQL")
    .WithEnvironment("Database__Provider", "PostgreSQL")
    .WithEnvironment("Authorization__Authority", "http://localhost:8080/realms/crucible")
    .WithEnvironment("Authorization__AuthorizationUrl", "http://localhost:8080/realms/crucible/protocol/openid-connect/auth")
    .WithEnvironment("Authorization__TokenUrl", "http://localhost:8080/realms/crucible/protocol/openid-connect/token")
    .WithEnvironment("Authorization__ClientId", "player.api");

var playerUiRoot = "/mnt/data/crucible/player/player.ui";

File.Copy("./resources/player.ui.json", $"{playerUiRoot}/src/assets/config/settings.env.json", overwrite: true);

IResourceBuilder<ExecutableResource>? playerUiNpmInstall = null;

if (!Directory.Exists($"{playerUiRoot}/node_modules"))
{
    playerUiNpmInstall = builder.AddExecutable(
    "player-ui-install",
    "npm",
    workingDirectory: playerUiRoot,
    "install");
}

var playerUi = builder.AddNpmApp("player-ui", playerUiRoot)
        .WithHttpEndpoint(port: 4301, env: "PORT", isProxied: false);

if (playerUiNpmInstall is not null)
{
    playerUi = playerUi.WaitForCompletion(playerUiNpmInstall);
}

var casterDb = postgres.AddDatabase("casterDb", "caster");

var casterApi = builder.AddProject<Projects.Caster_Api>("caster-api", launchProfileName: "Caster.Api")
    .WaitFor(postgres)
    .WaitFor(keycloak)
    .WithHttpHealthCheck("api/health/ready")
    .WithReference(casterDb, "PostgreSQL")
    .WithEnvironment("Database__Provider", "PostgreSQL")
    .WithEnvironment("Authorization__Authority", "http://localhost:8080/realms/crucible")
    .WithEnvironment("Authorization__AuthorizationUrl", "http://localhost:8080/realms/crucible/protocol/openid-connect/auth")
    .WithEnvironment("Authorization__TokenUrl", "http://localhost:8080/realms/crucible/protocol/openid-connect/token")
    .WithEnvironment("Authorization__ClientId", "caster.api");

var casterUiRoot = "/mnt/data/crucible/caster/caster.ui";

File.Copy("./resources/caster.ui.json", $"{casterUiRoot}/src/assets/config/settings.env.json", overwrite: true);

IResourceBuilder<ExecutableResource>? casterUiNpmInstall = null;

if (!Directory.Exists($"{casterUiRoot}/node_modules"))
{
    casterUiNpmInstall = builder.AddExecutable(
    "caster-ui-install",
    "npm",
    workingDirectory: casterUiRoot,
    "install");
}

var casterUi = builder.AddNpmApp("caster-ui", casterUiRoot)
    .WithHttpEndpoint(port: 4310, env: "PORT", isProxied: false);

if (casterUiNpmInstall is not null)
{
    casterUiNpmInstall = casterUi.WaitForCompletion(casterUiNpmInstall);
}

var topoDb = postgres.AddDatabase("topoDb", "topomojo");

var topoApi = builder.AddProject<Projects.TopoMojo_Api>("topomojo")
    .WaitFor(postgres)
    .WaitFor(keycloak)
    //.WithHttpHealthCheck("api/health/ready")
    .WithReference(topoDb, "PostgreSQL")
    .WithEnvironment("Database__ConnectionString", topoDb.Resource.ConnectionStringExpression)
    .WithEnvironment("Database__Provider", "PostgreSQL")
    .WithEnvironment("Oidc__Authority", "http://localhost:8080/realms/crucible")
    .WithEnvironment("Oidc__Audience", "topomojo")
    .WithEnvironment("OpenApi__Client__AuthorizationUrl", "http://localhost:8080/realms/crucible/protocol/openid-connect/auth")
    .WithEnvironment("OpenApi__Client__TokenUrl", "http://localhost:8080/realms/crucible/protocol/openid-connect/token")
    .WithEnvironment("OpenApi__Client__ClientId", "topomojo.api")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
    .WithEnvironment("ASPNETCORE_URLS", "http://localhost:5000")
    .WithEnvironment("Headers__Cors__Origins__0", "http://localhost:4201")
    .WithEnvironment("Headers__Cors__Methods__0", "*")
    .WithEnvironment("Headers__Cors__Headers__0", "*")
    .WithHttpEndpoint(name: "http", port: 5000, env: "PORT", isProxied: false)
    .WithUrlForEndpoint("http", url =>
    {
        url.Url = "/api";
    });

var topoUiRoot = "/mnt/data/crucible/topomojo/topomojo-ui/";

File.Copy("./resources/topomojo.ui.json", $"{topoUiRoot}/projects/topomojo-work/src/assets/settings.json", overwrite: true);

IResourceBuilder<ExecutableResource>? topoUiNpmInstall = null;

if (!Directory.Exists($"{topoUiRoot}/node_modules"))
{
    topoUiNpmInstall = builder.AddExecutable(
    "topo-ui-install",
    "npm",
    workingDirectory: topoUiRoot,
    "install");
}

var topoUi = builder.AddNpmApp("topomojo-ui", topoUiRoot, args: ["topomojo-work"])
    .WithHttpEndpoint(port: 4201, env: "PORT", isProxied: false);

if (topoUiNpmInstall is not null)
{
    topoUiNpmInstall = topoUi.WaitForCompletion(topoUiNpmInstall);
}

builder.Build().Run();
