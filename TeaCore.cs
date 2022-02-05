namespace TEASharp;

using static TeaUtilities;

/// <summary>
/// Core class which provides methods for encrypting and decrypting
/// using the Tiny Encryption Algorithm (TEA).
/// </summary>
public static class TeaCore
{
    /// <summary>
    /// 2^32 / golden ratio, key scheduling constant.
    /// </summary>
    private const uint Delta = 0x9e3779b9;

    /// <summary>
    /// Encrypts the input text using the 128-bit <paramref name="key"/>
    /// using the Tiny Encryption Algorithm (TEA).
    /// </summary>
    /// <param name="text">the text to encrypt</param>
    /// <param name="key">the key to encrypt with</param>
    /// <returns>Encrypted text as a hex formatted string without '0x' prefix</returns>
    /// <exception cref="ArgumentException">
    /// Throws if either the <paramref name="text"/> or <paramref name="key"/> is null,
    /// empty, or whitespace.
    /// </exception>
    public static string Encrypt
    (
        string text,
        string key
    )
    {
        // Validate inputs
        ValidateStringNotNullOrWhiteSpace(key, nameof(key));
        ValidateStringNotNullOrWhiteSpace(text, nameof(text));

        // Chunk key
        uint[] K = GetK(key);

        // Split plaintext
        (uint L, uint R) = GetLR(text);

        // Run round function
        for (uint i = 1, sum = 0; i <= 32; ++i)
        {
            sum += Delta;

            L += ((R << 4) + K[0]) ^ (R + sum) ^ ((R >> 5) + K[1]);
            R += ((L << 4) + K[2]) ^ (L + sum) ^ ((L >> 5) + K[3]);
        }

        // Merge higher and lower
        return HexFormat(CombineLR(L, R));
    }

    /// <summary>
    /// Decrypts the ciphertext using the 128-bit <paramref name="key"/>
    /// using the Tiny Encryption Algorithm (TEA).
    /// </summary>
    /// <param name="ciphertext">the ciphertext to decrypt</param>
    /// <param name="key">the key used to encrypt</param>
    /// <returns>Decrypted plaintext as a hex formatted string without '0x' prefix</returns>
    /// <exception cref="ArgumentException">
    /// Throws if either the <paramref name="ciphertext"/> or <paramref name="key"/> is null,
    /// empty, or whitespace.
    /// </exception>
    public static string Decrypt
    (
        string ciphertext,
        string key
    )
    {
        // Validate inputs
        ValidateStringNotNullOrWhiteSpace(key, nameof(key));
        ValidateStringNotNullOrWhiteSpace(ciphertext, nameof(ciphertext));

        // Chunk key
        uint[] K = GetK(key);

        // Split ciphertext
        (uint L, uint R) = GetLR(ciphertext);

        // Run round function
        for (uint i = 1, sum = Delta << 5; i <= 32; ++i)
        {
            R -= ((L << 4) + K[2]) ^ (L + sum) ^ ((L >> 5) + K[3]);
            L -= ((R << 4) + K[0]) ^ (R + sum) ^ ((R >> 5) + K[1]);

            sum -= Delta;
        }

        // Merge higher and lower
        return HexFormat(CombineLR(L, R));
    }
}