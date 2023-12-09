using System.Text;

namespace AoC.Tests;

public class InputCryptoTests
{
    private static byte[] GenerateRandomKey() => Random.Shared.GetItems(Encoding.UTF8.GetBytes("0123456789ABCDEF"), 32);

    [Test]
    public void Encrypt_Decrypt_RoundTripTest()
    {
        using var sut = new InputCrypto(GenerateRandomKey());
        var inputText = "Hello world!";

        // ACT
        var cipherText = sut.Encrypt(inputText);
        var decipheredText = sut.Decrypt(cipherText);

        new { inputText, cipherText, decipheredText }.Dump();

        // ASSERT
        cipherText.Should()
            .NotBeEmpty()
            .And
            .NotBe(inputText);

        decipheredText.Should().Be(inputText);
    }

    [Test]
    public void Encrypt_GivenDifferentKey_ProducesDifferentCipherTextForSameInput()
    {
        var inputText = "Hello world!";

        // ACT
        var cipherTexts = Enumerable.Range(0, 5).Select(_ =>
        {
            using var sut = new InputCrypto(GenerateRandomKey());
            return sut.Encrypt(inputText);
        }).ToArray();

        cipherTexts.Dump();

        // ASSERT
        cipherTexts.Should().NotBeEmpty();
        cipherTexts.Distinct().Should().HaveCount(cipherTexts.Length);
    }
}
