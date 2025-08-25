using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleOpinionated;
internal class ExportOpinionated
{
    public async Task Generate(string folder)
    {
        string OpinionatedFolder = Path.Combine(folder, "Opinionated");
        if (!Directory.Exists(OpinionatedFolder))
        {
            Directory.CreateDirectory(OpinionatedFolder);
        }
        string zipPath = Path.Combine(folder,"HTML2.zip");
        if (File.Exists(zipPath))
        { 
            // Extract the zip file to the Opinionated folder
            ZipFile.ExtractToDirectory(zipPath, OpinionatedFolder, overwriteFiles: true);
        }
        else
        {
            throw new FileNotFoundException($"Zip file not found: {zipPath}");
        }


        var newFolder = Path.Combine(OpinionatedFolder, "html2-client");
        File.Move(Path.Combine(newFolder,"index.html"), Path.Combine(OpinionatedFolder, "index.html"), true);
        Directory.Delete(newFolder, true);
        var zipFiles = Directory.GetFiles(folder, "*.zip");
        var model = zipFiles.Select(f => Path.GetFileNameWithoutExtension(f)).ToArray();
        var template = new WebAPIDocsExtensionAspire.SimpleLink(model);
        var res = await template.RenderAsync();

        var str=File.ReadAllText(Path.Combine(OpinionatedFolder, "index.html"));
        str= str.Replace("<div id=\"app-description\"", res + "<div id=\"app-description\"");
        File.WriteAllText(Path.Combine(OpinionatedFolder, "index.html"), str);
        File.Move(Path.Combine(OpinionatedFolder, "index.html"), Path.Combine(folder, "index.html"), true);
        Directory.Delete(OpinionatedFolder, true);

    }
}
