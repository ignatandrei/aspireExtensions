namespace PortExtensionAspire;

/// <summary>
/// read https://docs.microsoft.com/en-us/dotnet/api/system.string.gethashcode?view=net-5.0
/// read https://andrewlock.net/why-is-string-gethashcode-different-each-time-i-run-my-program-in-net-core/
/// read https://rehansaeed.com/gethashcode-made-easy/
/// </summary>
class MSPC
{
    static int GetDeterministicHashCode(string str)
    {
        unchecked
        {
            int hash1 = (5381 << 16) + 5381;
            int hash2 = hash1;

            for (int i = 0; i < str.Length; i += 2)
            {
                hash1 = ((hash1 << 5) + hash1) ^ str[i];
                if (i == str.Length - 1)
                    break;
                hash2 = ((hash2 << 5) + hash2) ^ str[i + 1];
            }

            return hash1 + (hash2 * 1566083941);
        }
    }
    /// <summary>
    /// Computes a deterministic (repeatable) port for <paramref name="name"/> by hashing it and reducing
    /// the result into the <see cref="UInt16"/> range.
    /// WARNING: this only guarantees the same name always maps to the same port; it does NOT guarantee
    /// uniqueness across different names. Two different names can hash to the same port (a collision),
    /// which would assign the same port to two different resources. If that happens, choose a different
    /// name/tag for one of the resources or override the port manually.
    /// </summary>
    public UInt16 GetDeterministicPort(string name)
    {
        int hash = Math.Abs(GetDeterministicHashCode(name));
        if (hash < UInt16.MaxValue)
            return (UInt16)hash;

        var remainder = hash % UInt16.MaxValue;
        return (UInt16)remainder;
    }


    /// <summary>
    /// Computes a deterministic port for <paramref name="name"/> combined with <paramref name="tag"/>.
    /// See <see cref="GetDeterministicPort(string)"/> for the same uniqueness caveat: collisions between
    /// different name/tag combinations are possible and are not detected.
    /// </summary>
    public UInt16 GetDeterministicPort(string name, string tag)
    {
        if (string.IsNullOrWhiteSpace(tag))
            return GetDeterministicPort(name);


        return GetDeterministicPort(name + "_" + tag);

    }
}
