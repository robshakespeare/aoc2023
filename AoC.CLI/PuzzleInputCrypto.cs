using System.Text;
using Microsoft.Extensions.Configuration;

namespace AoC.CLI;

internal static class PuzzleInputCrypto
{
    public static IInputCrypto Instance { get; } = Create();

    private static InputCrypto Create()
    {
        var config = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .AddUserSecrets<Program>()
            .Build();

        const string keyName = "AocPuzzleInputCryptoKey";
        var key = config[keyName];
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new Exception($"Missing config: {keyName}");
        }

        return new InputCrypto(Encoding.UTF8.GetBytes(key));
    }
}
