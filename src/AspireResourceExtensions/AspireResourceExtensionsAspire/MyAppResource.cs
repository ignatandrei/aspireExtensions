namespace AspireResourceExtensionsAspire;


record MyAppResource(Dictionary<string, MyResource> resources, Dictionary<string,MyRelationResource> relationResources)
{
    public string ExportToMermaid()
    {
        Mermaid m = new(this);
        return m.Render();
    }
    public string ExportToCSV()
    {
        CSV m = new(this);
        return m.Render();
    }
    public string[] MyResources()
    {
        return resources.Keys.ToArray();
    }
    public bool ExistResource(string resourceName)
    {
        return resources.ContainsKey(resourceName); 
    }
    public KeyValuePair<string, string>[]? DetailsResource(string idResource)
    {
        if (!ExistResource(idResource))
            return null;

        var resource = resources[idResource];
        var details = resource.Properties;
        return [.. details];

    }

    public static MyAppResource Construct(DistributedApplication distributedApplication,IDistributedApplicationBuilder builder)
    {
        string currentfolder = Environment.CurrentDirectory;
        Dictionary<string, MyResource> resources = [];
        Dictionary<string,MyRelationResource> relationResources = [];
        resources = builder.Resources.ToDictionary(item => item.Name, item => new MyResource(item));

        foreach (var item in builder.Resources)
        {            
            var myRes = resources[item.Name];

            if (item.TryGetAnnotationsOfType<ResourceRelationshipAnnotation>(out var resourceRelationshipAnnotations))
            {
                foreach (var relation in resourceRelationshipAnnotations)
                {
                    var key = $"{myRes.Name}->{relation.Resource.Name}";
                    if (!relationResources.ContainsKey(key))
                    {
                        relationResources.Add(key, new MyRelationResource(myRes, resources[relation.Resource.Name]));
                    }
                    relationResources[key].AddRelation(relation.Type);
                    //    relationResources.Add(new MyRelationResource(myRes, resources[relation.Resource.Name], relation.Type));

                }
            }
            if(item.TryGetAnnotationsOfType<ExecutableAnnotation>(out var executableAnnotations))
            {
                foreach(var exec in executableAnnotations)
                {
                    myRes.Properties.Add("ExecutablePath", exec.Command);
                    
                    myRes.Properties.Add("WorkingDir", Path.GetRelativePath(currentfolder, exec.WorkingDirectory));

                }
            }
            if(item.TryGetAnnotationsOfType<IProjectMetadata>(out var projectMetadataAnnotations))
            {
                foreach(var projectMetadata in projectMetadataAnnotations)
                {

                    var name = Path.GetRelativePath(currentfolder, projectMetadata.ProjectPath);
                    myRes.Properties.Add("ProjectFilePath", name);
                }
            }
            if(item.TryGetAnnotationsOfType<EndpointAnnotation>(out var endpointAnnotations))
            {
                foreach(var endpoint in endpointAnnotations)
                {
                    //TODO                    
                }
            }
            if(item.TryGetAnnotationsOfType<ResourceCommandAnnotation>(out var resourceCommandAnnotations))
            {

                foreach(var resourceCommand in resourceCommandAnnotations)
                {
                    myRes.Properties.Add("CMD_" + resourceCommand.Name, resourceCommand.DisplayName);
                }
            }
        }
        return new(resources,relationResources);
    }
    
}

class MyResource(IResource resource)
{
    public string Name => resource.Name;

    public string Type
    {
        get
        {
            if(field == null)
            {
                field = resource.GetType().Name;
            }
            return field;
        }
    }

    public Dictionary<string,string> Properties { get; init; } = [];

}
record MyRelationResource(MyResource fromResource, MyResource toResource)
{
    public HashSet<string> RelationTypes { get; init; } = [];

    public void AddRelation(string type)
    {
            RelationTypes.Add(type);
    }

}