using System.Xml;

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

public class TableModelReact
{
    public TableModelReact(string nameTable, DatabaseModelReact databaseModelReact)
    {
        NameTable = nameTable;
        DatabaseModelReact = databaseModelReact;
    }
    public string nameDB => DatabaseModelReact.nameDB;
    public string partialDocumentFile => $"_markdown-{nameDB}-{NameTable}-database.mdx";
    //File   File.Copy(file, Path.Combine(docsFolder, partialExampleFile), true);
    public string partialDocumentFileUser => $"_markdown-{nameDB}-{NameTable}-database-user.mdx";

    public string NameReactPartialFileComponent => $"{nameDB}{NameTable}Database";
    public string NameReactPartialFileUserComponent => $"{nameDB}{NameTable}DatabaseUser";
    public string ReactPartialFileComponent => $"<{NameReactPartialFileComponent} />";
    public string ReactPartialFileUserComponent => $"<{NameReactPartialFileUserComponent} />";

    public string NameTable { get; }
    public DatabaseModelReact DatabaseModelReact { get; }
}