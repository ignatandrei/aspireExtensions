namespace WebAPIDocsExtensionAspire;

internal class AnnotationClients : IResourceAnnotation
{
    public string Text { get; set; } = string.Empty;
    public string[] Data { get; set; } = [];
    public string Url { get; set; } = string.Empty;
    public AnnotationClients(string text)
    {
        Text = text;
    }
}
