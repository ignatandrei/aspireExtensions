namespace DiagramDocusaurusGenerator;

public class DatabaseModelReact
{
    public string nameDB { get; set; } = "";

    public string partialDocumentFile => $"_markdown-{nameDB}-database.mdx";
    //File   File.Copy(file, Path.Combine(docsFolder, partialExampleFile), true);
    public string partialDocumentFileUser => $"_markdown-{nameDB}-database-user.mdx";

    public string NameReactPartialFileComponent => $"{nameDB}Database";
    public string NameReactPartialFileUserComponent => $"{nameDB}DatabaseUser";
    public string ReactPartialFileComponent => $"<{NameReactPartialFileComponent} />";
    public string ReactPartialFileUserComponent => $"<{NameReactPartialFileUserComponent} />";


}