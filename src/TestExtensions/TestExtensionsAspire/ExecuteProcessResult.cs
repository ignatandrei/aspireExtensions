namespace TestExtensionsAspire;

/// <summary>
/// Represents the result of executing a process, containing output, error information, and exit code.
/// </summary>
/// <param name="Output">The standard output from the process execution.</param>
/// <param name="Error">The error output from the process execution.</param>
/// <param name="ExitCode">The exit code returned by the process.</param>
record ExecuteProcessResult(string Output, string Error, int ExitCode)
{
    /// <summary>
    /// Creates a successful execution result with the specified output.
    /// </summary>
    /// <param name="output">The standard output from the successful process execution.</param>
    /// <returns>An <see cref="ExecuteProcessResult"/> indicating success.</returns>
    public static ExecuteProcessResult SuccessResult(string output) => new ExecuteProcessResult(output, "", 0);
    
    /// <summary>
    /// Gets a value indicating whether the process execution was successful (exit code is 0).
    /// </summary>
    public bool Success => ExitCode == 0;
    
    /// <summary>
    /// Gets the error message if the process execution failed, or null if it was successful.
    /// </summary>
    public string? ErrorMessage => Success ? null : Error;
}


