namespace DataProject;

public class DBFiles
{
    public static IEnumerable<string> FilesToCreate
    {
        get
        {
            using (var reader = EmbeddedResources.sql_001_CreateTable_sql_Reader)
            {
                yield return FromStreamReader(reader);
            }
            using (var reader = EmbeddedResources.sql_002_InsertData_sql_Reader)
            {
                yield return FromStreamReader(reader);
            }
        }
    }
    static string FromStreamReader(StreamReader streamReader)
    {
        return streamReader.ReadToEnd();
    }
}
