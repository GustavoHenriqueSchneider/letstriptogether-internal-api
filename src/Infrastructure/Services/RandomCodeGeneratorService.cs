using System.Security.Cryptography;
using Application.Common.Interfaces.Services;

namespace Infrastructure.Services;

public class RandomCodeGeneratorService : IRandomCodeGeneratorService
{
    private const int Min = 100000;
    private const int Max = 999999;

    public string Generate()
    {
        return RandomNumberGenerator.GetInt32(Min, Max + 1).ToString();
    }
}
