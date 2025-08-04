namespace TestExtensionsAspire;

record ExecuteProcessResult(string Output, string Error, int ExitCode)
{
    public static ExecuteProcessResult SuccessResult(string output) => new ExecuteProcessResult(output, "", 0);
    public bool Success => ExitCode == 0;
    public string? ErrorMessage => Success ? null : Error;
}


