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
    public UInt16 GetDeterministicPort(string name)
    {
        int hash = Math.Abs(GetDeterministicHashCode(name));
        if (hash < UInt16.MaxValue)
            return (UInt16)hash;

        var remainder = hash % UInt16.MaxValue;
        return (UInt16)remainder;
    }


    public UInt16 GetDeterministicPort(string name, string tag)
    {
        if (string.IsNullOrWhiteSpace(tag))
            return GetDeterministicPort(name);


        return GetDeterministicPort(name + "_" + tag);

    }
}
