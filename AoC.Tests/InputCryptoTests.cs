namespace AoC.Tests;

public class InputCryptoTests
{
    [Test]
    public void Encrypt_Decrypt_RoundTripTest()
    {
        var sut = IInputCrypto.Instance;
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
}
