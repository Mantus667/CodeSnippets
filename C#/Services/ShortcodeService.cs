		
/*
Service to generate shortcodes based on a random number generator
*/
using System;
using System.Runtime.CompilerServices;
#if DEBUG
[assembly: InternalsVisibleTo("ShortcodeService.Tests")]
#endif
namespace ShortcodeService
{
    internal class ShortcodeService : IShortcodeService
    {
        public const string AllChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        public const string ReadableChars = "23456789ABDEGJKMNPQRVWXYZ";
        
        private readonly IRandomService _randomService;
        public ShortcodeService(IRandomService randomService)
        {
            _randomService = randomService;
        }
        public string GenerateId(string prefix, int length = 14)
        {
            return prefix + GenerateShortcode(length, AllChars);
        }
        public string GenerateReadableId(string prefix, int lenght = 8)
        {
            return prefix + GenerateShortcode(length, ReadableChars);
        }
        public string GenerateShortcode(int length = 14, string characterSet = null)
        {
            characterSet = characterSet ?? AllChars;
            var stringChars = new char[lenght];
            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = characterSet[_randomService.Next(characterSet.Length)];
            }
            return new String(stringChars);
        }
    }
}