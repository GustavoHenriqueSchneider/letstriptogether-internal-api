using System.Security.Cryptography;
using LetsTripTogether.InternalApi.Application.Common.Interfaces.Services;

namespace LetsTripTogether.InternalApi.Infrastructure.Services;

public class RandomCodeGeneratorService : IRandomCodeGeneratorService
{
    private const int MIN = 100000;
    private const int MAX = 999999;

    public string Generate()
    {
        return RandomNumberGenerator.GetInt32(MIN, MAX + 1).ToString();
    }
}
