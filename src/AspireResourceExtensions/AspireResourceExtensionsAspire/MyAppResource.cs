using AspireResourceExtensionsAspire.Templates;

namespace AspireResourceExtensionsAspire;


internal record MyAppResource(Dictionary<string, MyResource> resources, List<MyRelationResource> relationResources)
{
    public string ExportToMermaind()
    {
        Mermaid m = new(this);
        return m.Render();
    }
    

    public static MyAppResource ConstructMermaid(DistributedApplication distributedApplication,IDistributedApplicationBuilder builder1)
    {
        Dictionary<string, MyResource> resources = [];
        List<MyRelationResource> relationResources = [];

        foreach (var res in builder1.Resources)
        {
            resources.Add(res.Name, new MyResource(res));
        }
        foreach (var item in builder1.Resources)
        {

            if (!item.TryGetAnnotationsOfType<ResourceRelationshipAnnotation>(out var resourceRelationshipAnnotations))
                continue;

            var myRes = resources[item.Name];
            foreach (var relation in resourceRelationshipAnnotations)
            {

                relationResources.Add(new MyRelationResource(myRes, resources[relation.Resource.Name], relation.Type));

            }

        }
        return new(resources,relationResources);
    }
    
}

class MyResource(IResource resource)
{
    public string Name => resource.Name;
}
record MyRelationResource(MyResource fromResource, MyResource toResource, string relationName)
{

}