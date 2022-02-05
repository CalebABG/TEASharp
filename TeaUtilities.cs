namespace TEASharp;

/// <summary>
/// Utility class containing useful helper functions
/// for TEA.
/// </summary>
internal static class TeaUtilities
{
    /// <inheritdoc cref="GetChunksFromKey"/>
    internal static uint[] GetK
    (
        string key
    )
    {
        return GetChunksFromKey(RemoveHexPrefixIfPresent(key));
    }

    /// <inheritdoc cref="GetLRFromString"/>
    internal static (uint, uint) GetLR
    (
        string hexStr
    )
    {
        return GetLRFromString(RemoveHexPrefixIfPresent(hexStr));
    }

    /// <summary>
    /// Chunks the provided 128-bit <paramref name="key"/> into 4
    /// unsigned 32-bit integers.
    /// </summary>
    /// <param name="key">the key to chunk</param>
    /// <returns>Chunked key as an array of unsigned 32-bit integers.</returns>
    private static uint[] GetChunksFromKey
    (
        string key
    )
    {
        const int numHexChars = 8;

        uint[] keyChunks = new uint[4];

        string paddedHexKey = GetPaddedHexKey(key);

        for (int i = 0, offset = 0; i < keyChunks.Length; ++i, offset += numHexChars)
        {
            var chunk = paddedHexKey.Substring(offset, numHexChars);
            keyChunks[i] = Convert.ToUInt32(chunk, 16);
        }

        return keyChunks;
    }

    /// <summary>
    /// Pads the key string to ensure that
    /// the key length is always a multiple of 2.
    /// </summary>
    /// <param name="keyStr">the key string to pad</param>
    /// <returns>
    /// Padded key string if the length of the provided key string was not
    /// a multiple of two, otherwise returns the provided key string.
    /// </returns>
    private static string GetPaddedHexKey
    (
        string keyStr
    )
    {
        return keyStr.Length % 2 != 0
            ? keyStr.Insert(0, "0")
            : keyStr;
    }

    /// <summary>
    /// Checks whether the string has a hex prefix.
    /// </summary>
    /// <param name="str">the string to check</param>
    /// <returns>
    /// True if the string starts with the sequence: '0x', otherwise returns false
    /// </returns>
    private static bool HasHexPrefix
    (
        string str
    )
    {
        return str.StartsWith("0x", StringComparison.InvariantCultureIgnoreCase);
    }

    /// <summary>
    /// Removes the hex prefix from the provided string
    /// if the prefix is present.
    /// </summary>
    /// <param name="str">the string to remove the prefix from</param>
    /// <returns>
    /// New string with the hex prefix removed if it was present,
    /// otherwise returns provided string
    /// </returns>
    private static string RemoveHexPrefixIfPresent
    (
        string str
    )
    {
        return HasHexPrefix(str) ? str[2..] : str;
    }

    /// <summary>
    /// Merge 2 unsigned 32-bit integers (L = higher, R = lower) to 1
    /// unsigned 64-bit integer. 
    /// </summary>
    /// <param name="l">the higher 4 bytes</param>
    /// <param name="r">the lower 4 bytes</param>
    /// <returns></returns>
    internal static ulong CombineLR
    (
        uint l,
        uint r
    )
    {
        return (ulong) l << 32 | r;
    }

    /// <summary>
    /// Formats the provided number as a hex string.
    /// </summary>
    /// <param name="result">the number to format</param>
    /// <returns>
    /// Hex formatted string representation of the number
    /// without the '0x' prefix
    /// </returns>
    internal static string HexFormat
    (
        ulong result
    )
    {
        return result.ToString("x");
    }

    /// <summary>
    /// Splits the hex string into two (higher and lower)
    /// unsigned 32-bit integers.
    /// </summary>
    /// <param name="hexStr">the hex string to split</param>
    /// <returns>Tuple containing 2 unsigned 32-bit integers (L = higher, R = lower)</returns>
    private static (uint, uint) GetLRFromString
    (
        string hexStr
    )
    {
        ulong num = Convert.ToUInt64(hexStr, 16);

        uint high = (uint) (num >> 32);
        uint low = (uint) (num & uint.MaxValue);

        return (high, low);
    }

    /// <summary>
    /// Ensures that the input text is not not null,
    /// empty or consists of only whitespace.
    /// </summary>
    /// <param name="input">the string to validate</param>
    /// <param name="paramName">the name of the input parameter</param>
    /// <exception cref="ArgumentException">
    /// Throws if the <paramref name="input"/> is null,
    /// empty, or whitespace.
    /// </exception>
    internal static void ValidateStringNotNullOrWhiteSpace
    (
        string input,
        string paramName
    )
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new ArgumentException("Value cannot be null or whitespace.", paramName);
    }

    /// <summary>
    /// Gets the plaintext and key inputs from available input providers.
    /// <remarks>
    /// Input providers are evaluated in the following sequence: <br/>
    /// 1. File provider - (checks for file in the current working directory) <br/>
    /// 2. Command line arguments - (first argument is 'plaintext', second argument is 'key') <br/>
    /// 3. Console readline - ('plaintext' is requested first, 'key' is requested second) <br/>
    /// </remarks>
    /// </summary>
    /// <param name="args">the command line arguments</param>
    /// <param name="filePath">the path to the file which contains the plaintext and key</param>
    /// <returns>Tuple containing the plaintext and key)</returns>
    internal static (string plaintext, string key) GetPlaintextAndKey
    (
        string[]? args = null,
        string filePath = "./pk.txt"
    )
    {
        if (File.Exists(filePath))
        {
            string[] lines = File.ReadAllLines(filePath);
            return (lines[0], lines[1]);
        }

        if (args is not null && args.Length > 1)
        {
            return (args[0], args[1]);
        }

        Console.WriteLine("\nPlease enter the 'Plaintext': ");
        string plaintext = Console.ReadLine() ?? "";

        Console.WriteLine();

        Console.WriteLine("Please enter the 'Key': ");
        string key = Console.ReadLine() ?? "";

        return (plaintext, key);
    }
}