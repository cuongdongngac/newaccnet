using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NewaccNet.Core.Staging;

namespace NewaccNet.Core.Matching
{
    public class FuzzyMatchCandidate
    {
        public string MaterialId { get; set; } = string.Empty;
        public string MaterialName { get; set; } = string.Empty;
        public string? CategoryId { get; set; }
        public double SimilarityScore { get; set; }
    }

    /// <summary>
    /// Bộ so khớp mờ chuỗi ký tự tiếng Việt (Fast Offline Fuzzy Matching).
    /// Chạy trực tiếp trên máy không cần internet, sử dụng Bigram Dice & Token Jaccard.
    /// </summary>
    public static class FuzzyMatchHelper
    {
        private static readonly HashSet<string> StopWords = new(StringComparer.OrdinalIgnoreCase)
        {
            "bao", "thung", "hop", "chiec", "cai", "cuon", "can", "chai", "lo", "bo", "cay", "vien", "met", "kg", "m2", "m3", "lit"
        };

        public static string CleanTokens(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return string.Empty;

            var clean = MappingMemoryStore.RemoveDiacritics(text).ToLowerInvariant();
            clean = Regex.Replace(clean, @"[^\w\s]", " "); // bỏ ký tự đặc biệt
            var tokens = clean.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                              .Where(t => t.Length > 1 && !StopWords.Contains(t));

            return string.Join(" ", tokens);
        }

        public static double CalculateSimilarity(string source, string target)
        {
            if (string.IsNullOrWhiteSpace(source) || string.IsNullOrWhiteSpace(target)) return 0.0;

            var s1 = CleanTokens(source);
            var s2 = CleanTokens(target);

            if (s1 == s2) return 1.0;
            if (s1.Length == 0 || s2.Length == 0) return 0.0;

            // 1. Token Jaccard
            var tokens1 = new HashSet<string>(s1.Split(' '));
            var tokens2 = new HashSet<string>(s2.Split(' '));

            int intersection = tokens1.Count(t => tokens2.Contains(t));
            int union = tokens1.Union(tokens2).Count();
            double tokenJaccard = union > 0 ? (double)intersection / union : 0.0;

            // 2. Character Bigram Dice
            double bigramDice = CalculateBigramDice(s1, s2);

            return (tokenJaccard * 0.5) + (bigramDice * 0.5);
        }

        private static double CalculateBigramDice(string s1, string s2)
        {
            var bigrams1 = GetBigrams(s1);
            var bigrams2 = GetBigrams(s2);

            if (bigrams1.Count == 0 || bigrams2.Count == 0) return 0.0;

            int matches = 0;
            var copy2 = new List<string>(bigrams2);
            foreach (var b in bigrams1)
            {
                int idx = copy2.IndexOf(b);
                if (idx >= 0)
                {
                    matches++;
                    copy2.RemoveAt(idx);
                }
            }

            return (2.0 * matches) / (bigrams1.Count + bigrams2.Count);
        }

        private static List<string> GetBigrams(string s)
        {
            var list = new List<string>();
            for (int i = 0; i < s.Length - 1; i++)
            {
                list.Add(s.Substring(i, 2));
            }
            return list;
        }

        public static FuzzyMatchCandidate? FindBestMatch(string targetName, IEnumerable<(string MaterialId, string MaterialName, string? CategoryId)> dbItems, double threshold = 0.70)
        {
            if (string.IsNullOrWhiteSpace(targetName)) return null;

            FuzzyMatchCandidate? best = null;
            double maxScore = 0.0;

            foreach (var item in dbItems)
            {
                double score = CalculateSimilarity(targetName, item.MaterialName);
                if (score > maxScore && score >= threshold)
                {
                    maxScore = score;
                    best = new FuzzyMatchCandidate
                    {
                        MaterialId = item.MaterialId,
                        MaterialName = item.MaterialName,
                        CategoryId = item.CategoryId,
                        SimilarityScore = score
                    };
                }
            }

            return best;
        }
    }
}
