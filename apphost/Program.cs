var builder = DistributedApplication.CreateBuilder(args);

// FLASK_PORT is read by backend\config.py
var backend = builder.AddPythonApp("backend", "../backend", "forecast_api.py")
    .WithPip()
    .WithHttpEndpoint(env: "FLASK_PORT")
    .WithEnvironment("FLASK_HOST", "127.0.0.1")
    .WithHttpHealthCheck("/api/health");

// The UI reads the backend URL from services__backend__http__0
builder.AddProject<Projects.BwetForecasterWinForms>("frontend")
    .WithReference(backend)
    .WaitFor(backend);

builder.Build().Run();
