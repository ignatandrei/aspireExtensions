using System.Collections;

Console.WriteLine("Hello, World!");
foreach (DictionaryEntry de in Environment.GetEnvironmentVariables())
{

    if (de.Key?.ToString()?.StartsWith("PORT") == true)
        Console.WriteLine(de.Key + " " + de.Value);
}