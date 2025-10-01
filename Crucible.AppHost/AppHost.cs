var builder = DistributedApplication.CreateBuilder(args);

var pgPass = builder.AddParameter("pgPass");

var postgres = builder.AddPostgres("postgres", password: pgPass)
    .WithDataVolume()
    .WithPgAdmin();

var playerDb = postgres.AddDatabase("player");

builder.Build().Run();
