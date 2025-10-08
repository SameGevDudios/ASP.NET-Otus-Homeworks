using PromoCodeFactory.Core.Abstractions;
using System.Linq;
using System.Security.Cryptography;

namespace PromoCodeFactory.Core.Random
{
    public class HexPromoCodeGenerator : ICodeGenerator
    {
        public string Code()
        {
            const string hexChars = "ABCDEF1234567890";
            const int groups = 4, groupSize = 4;

            var allChars = Enumerable.Range(0, groups * groupSize)
                .Select(c => hexChars[RandomNumberGenerator.GetInt32(hexChars.Length)])
                .ToArray();

            var joined = string.Concat(allChars);

            var code = string.Join('-', 
                Enumerable.Range(0, groups)
                    .Select(i => joined.Substring(i * groupSize, groupSize)));

            return code;
        }
    }
}
