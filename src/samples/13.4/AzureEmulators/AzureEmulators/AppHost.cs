/*
 * https://aspire.dev/integrations/cloud/azure/overview/
  */


var builder = DistributedApplication.CreateBuilder(args);


#pragma warning disable ASPIRECOSMOSDB001
    var cosmos = builder.AddAzureCosmosDB("cosmos-db1")
        .RunAsPreviewEmulator(emulator =>
        {            emulator.WithDataExplorer();
        });
    var dbCosmos = cosmos.AddCosmosDatabase("CosmosDatabase1");
    var container = dbCosmos.AddContainer("CosmosEntriesContainer1", "/id");

    var csm = builder
        .AddProject<Projects.CosmosClient>("cosmosClient1")
        .WithReference(container)
        .WaitFor(container)
        ;


    var sqlAzure = builder.AddAzureSqlServer("AzureSql1")
        //TODO: add interface    
        //.WithDbGate()
        //.WithAdminer()
        .RunAsContainer()
        ;


    var db = sqlAzure.AddDatabase("AzureSqlDatabase1");
    //TODO: add tables to the database and reference them from the client project

    var sqlProj = builder
        .AddProject<Projects.SqlAzureClient>("sqlClient1")
        .WithReference(db)
        .WaitFor(db)
        ;





    var postgres = builder.AddAzurePostgresFlexibleServer("postgres1")
            .RunAsContainer()
            //TODO: add interface    
            //.WithDbGate()
            //.WithAdminer()
            ;

    var postgresdb = postgres.AddDatabase("postgresdb1");
    //TODO: add tables to the database and reference them from the client project

    var postgresProj = builder
        .AddProject<Projects.PostgresClient>("postgresClient1")
        .WithReference(postgresdb)
        .WaitFor(postgresdb)
        ;


    var azureStorage = builder.AddAzureStorage("AzureStorage1")
          .RunAsEmulator()
         .AddBlobs("AzureStorageBlobs1")
         ;

    var azureStorageProj = builder
        .AddProject<Projects.AzureStorageClient>("azureStorageClient1")
        .WithReference(azureStorage)
        .WaitFor(azureStorage)
        ;


    var appConfig = builder.AddAzureAppConfiguration("azureAppConfig1")
        .RunAsEmulator()
        .ConfigureInfrastructure(appConfig =>
        {
            //var store=appConfig.GetProvisionableResources().OfType<AppConfigurationStore>().FirstOrDefault();
            //if (store == null) return;
            //store.DisableLocalAuth = true;

        })
        ;

    builder.AddProject<Projects.AppConfigurationClient>("azureAppConfigClient1")
        .WithReference(appConfig)
        .WithRoleAssignments(appConfig, Azure.Provisioning.AppConfiguration.AppConfigurationBuiltInRole.AppConfigurationDataOwner)
        .WaitFor(appConfig)
        ;


    var azureManagedRedis = builder.AddAzureManagedRedis("azureManagedRedis1")
          .RunAsContainer()
           .WithAccessKeyAuthentication()

    ;

    builder.AddProject<Projects.AzureManagedRedisClient>("azureManagedRedisClient1")
        .WithReference(azureManagedRedis)
        .WaitFor(azureManagedRedis)
        ;

    var foundry = builder.AddFoundry("foundry1")
        .RunAsFoundryLocal();

    var chat = foundry.AddDeployment("foundryChat", Aspire.Hosting.Foundry.FoundryModel.Local.Phi4);

    builder.AddProject<Projects.FoundryClient>("foundryClient1")
        .WithReference(chat)
        .WaitFor(chat);

    var serviceBus = builder.AddAzureServiceBus("azureServiceBus1")
         .RunAsEmulator();
    var queue = serviceBus.AddServiceBusQueue("azureServiceQueue1");
    var topic = serviceBus.AddServiceBusTopic("azureServiceTopic1");
    topic.AddServiceBusSubscription("azureServiceSubscriptionTopic1");

    builder.AddProject<Projects.ServiceBusClient>("ServiceBusClient1")
        .WithReference(serviceBus)
        .WaitFor(serviceBus)
        ;


var eventHubs = builder.AddAzureEventHubs("azureEventHubs1")
     .RunAsEmulator()
     ;
var hub= eventHubs.AddHub("azureEventHubsHub1");
hub.AddConsumerGroup("azureEventHubsHubConsumer");
builder.AddProject<Projects.EventHubsClient>("EventHubsClient1")
        .WithReference(eventHubs)
        .WaitFor(eventHubs)
        ;

builder.Build().Run();



