using System.Security.Cryptography;
using LetsTripTogether.InternalApi.Application.Common.Interfaces.Services;

namespace LetsTripTogether.InternalApi.Infrastructure.Services;

public class RandomCodeGeneratorService : IRandomCodeGeneratorService
{
    private const int Min = 100000;
    private const int Max = 999999;

    public string Generate()
    {
        return RandomNumberGenerator.GetInt32(Min, Max + 1).ToString();
    }
}
