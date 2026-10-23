using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace NewaccNet.TaxInvoice
{
    public static class SvgCaptchaSolver
    {
        private static readonly Dictionary<string, char> CommandSignatures = new Dictionary<string, char>(StringComparer.OrdinalIgnoreCase)
        {
            // Digits 0-9
            ["MQMQMQMQZ"] = '0',
            ["MQMQZ"] = '1',
            ["MQMQMQMQMQZ"] = '2',
            ["MQMQMQMQMQMQZ"] = '3',
            ["MQMQMQZ"] = '4',
            ["MQMQMQMQMQMQMQZ"] = '5',
            ["MQMQMQMQMQMQMQZ"] = '6',
            ["MQMQMQMQMQMQMQMQMQZ"] = '8',
            // Lowercase letters
            ["MQMQMQMQMQMQMQMQMQMQZ"] = 'a',
            ["MQMQMQMQMQMQMQMQMQMQMQZ"] = 'b',
            ["MQMQMQMQMQMQMQMQMQMQMQMQZ"] = 'd',
            ["MQMQMQMQMQMQMQMQMQMQMQMQMQZ"] = 'e',
            ["MQMQMQMQMQMQMQMQMQMQMQMQMQMQZ"] = 'g',
        };

        private static readonly Regex PathRegex = new Regex(
            @"<path\s+[^>]*d=""([^""]+)""[^>]*>",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex CommandRegex = new Regex(
            @"[A-Za-z]",
            RegexOptions.Compiled);

        public static string Solve(string? svgContent)
        {
            if (string.IsNullOrWhiteSpace(svgContent)) return string.Empty;

            var characterList = new List<(double positionX, char character)>();
            var pathMatches = PathRegex.Matches(svgContent);

            foreach (Match match in pathMatches)
            {
                var pathData = match.Groups[1].Value;
                if (string.IsNullOrWhiteSpace(pathData)) continue;

                var initialMoveMatch = Regex.Match(pathData, @"M\s*(-?[\d.]+)[,\s]+(-?[\d.]+)");
                double positionX = 0;
                if (initialMoveMatch.Success && double.TryParse(initialMoveMatch.Groups[1].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsedX))
                {
                    positionX = parsedX;
                }

                var commandSignature = ExtractCommandSignature(pathData);
                var identifiedChar = IdentifyCharacter(commandSignature, pathData);
                if (identifiedChar.HasValue)
                {
                    characterList.Add((positionX, identifiedChar.Value));
                }
            }

            characterList.Sort((a, b) => a.positionX.CompareTo(b.positionX));

            var resultChars = new char[characterList.Count];
            for (int i = 0; i < characterList.Count; i++)
            {
                resultChars[i] = characterList[i].character;
            }
            return new string(resultChars);
        }

        private static string ExtractCommandSignature(string pathData)
        {
            var sb = new StringBuilder();
            var matches = CommandRegex.Matches(pathData);
            foreach (Match m in matches)
            {
                sb.Append(m.Value);
            }
            return sb.ToString();
        }

        private static char? IdentifyCharacter(string signature, string pathData)
        {
            if (CommandSignatures.TryGetValue(signature, out var character))
            {
                if (signature == "MQMQMQMQMQZ")
                {
                    if (pathData.Contains("10") || pathData.Contains("11")) return '2';
                    return 'z';
                }
                return character;
            }

            int countQ = 0;
            for (int i = 0; i < signature.Length; i++)
            {
                if (signature[i] == 'Q' || signature[i] == 'q') countQ++;
            }

            return countQ switch
            {
                1 => '1',
                2 => '4',
                3 => '7',
                4 => '0',
                5 => '2',
                6 => '3',
                7 => '5',
                8 => '6',
                9 => '9',
                10 => '8',
                _ => null,
            };
        }
    }
}
