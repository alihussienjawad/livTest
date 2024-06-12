var builder = DistributedApplication.CreateBuilder(args);

 
builder.AddProject<Projects.SiliconWeb>("siliconweb");
 

builder.AddProject<Projects.BackOffice>("backoffice");
 

builder.Build().Run();
