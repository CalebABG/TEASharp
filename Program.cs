namespace TEASharp;

public static class Program
{
    /// <summary>
    /// Program to test the Tiny Encryption Algorithm (TEA) implementation.
    /// <remarks>
    /// This implementation <b>STRICTLY</b> handles keys which are 128-bits, and
    /// plaintext inputs which are 64-bits. Inputs for either that
    /// are under or over the respective lengths is not guaranteed to work.
    /// </remarks>
    /// </summary>
    public static void Main(string[] args)
    {
        (string plaintext, string key) = TeaUtilities.GetPlaintextAndKey(args);

        string cipherText = TeaCore.Encrypt(plaintext, key);
        string decryptedText = TeaCore.Decrypt(cipherText, key);

        Console.WriteLine($"\nCipher Text: {cipherText}");
        Console.WriteLine($"Decrypted Text: {decryptedText}");
    }
}