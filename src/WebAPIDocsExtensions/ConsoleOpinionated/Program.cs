// See https://aka.ms/new-console-template for more information
using ConsoleOpinionated;
using System.Runtime.Serialization;

Console.WriteLine("Hello, World!");
ExportOpinionated exportOpinionated = new ExportOpinionated();
await exportOpinionated.Generate(@"D:\eu\test");