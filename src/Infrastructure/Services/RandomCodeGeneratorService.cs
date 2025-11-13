using System.Security.Cryptography;
using Application.Common.Interfaces.Services;
using Domain.Security;

namespace Infrastructure.Services;

public class RandomCodeGeneratorService : IRandomCodeGeneratorService
{
    public string Generate()
    {
        return RandomNumberGenerator.GetInt32(Code.MinValue, Code.MaxValue + 1).ToString();
    }
}
