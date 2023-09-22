﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace FuzzyString
{
    public static partial class ComparisonMetrics
    {
        public static bool ApproximatelyEquals(this string source, string target, FuzzyStringComparisonTolerance tolerance, out double averageScore, params FuzzyStringComparisonOptions[] options)
        {
            List<double> comparisonResults = new List<double>();

            if (!options.Contains(FuzzyStringComparisonOptions.CaseSensitive))
            {
                source = source.Capitalize();
                target = target.Capitalize();
            }

            // Min: 0    Max: source.Length = target.Length
            if (options.Contains(FuzzyStringComparisonOptions.UseHammingDistance))
            {
                if (source.Length == target.Length)
                {
                    comparisonResults.Add(source.HammingDistance(target) / target.Length);
                }
            }

            // Min: 0    Max: 1
            if (options.Contains(FuzzyStringComparisonOptions.UseJaccardDistance))
            {
                comparisonResults.Add(source.JaccardDistance(target));
            }

            // Min: 0    Max: 1
            if (options.Contains(FuzzyStringComparisonOptions.UseJaroDistance))
            {
                comparisonResults.Add(source.JaroDistance(target));
            }

            // Min: 0    Max: 1
            if (options.Contains(FuzzyStringComparisonOptions.UseJaroWinklerDistance))
            {
                comparisonResults.Add(source.JaroWinklerDistance(target));
            }

            // Min: 0    Max: LevenshteinDistanceUpperBounds - LevenshteinDistanceLowerBounds
            // Min: LevenshteinDistanceLowerBounds    Max: LevenshteinDistanceUpperBounds
            if (options.Contains(FuzzyStringComparisonOptions.UseNormalizedLevenshteinDistance))
            {
                comparisonResults.Add(Convert.ToDouble(source.NormalizedLevenshteinDistance(target)) / Convert.ToDouble(Math.Max(source.Length, target.Length) - source.LevenshteinDistanceLowerBounds(target)));
            }
            else if (options.Contains(FuzzyStringComparisonOptions.UseLevenshteinDistance))
            {
                comparisonResults.Add(Convert.ToDouble(source.LevenshteinDistance(target)) / Convert.ToDouble(source.LevenshteinDistanceUpperBounds(target)));
            }

            if (options.Contains(FuzzyStringComparisonOptions.UseLongestCommonSubsequence))
            {
                comparisonResults.Add(1 - Convert.ToDouble(source.LongestCommonSubsequence(target).Length / Convert.ToDouble(Math.Min(source.Length, target.Length))));
            }

            if (options.Contains(FuzzyStringComparisonOptions.UseLongestCommonSubstring))
            {
                comparisonResults.Add(1 - Convert.ToDouble(source.LongestCommonSubstring(target).Length / Convert.ToDouble(Math.Min(source.Length, target.Length))));
            }

            // Min: 0    Max: 1
            if (options.Contains(FuzzyStringComparisonOptions.UseSorensenDiceDistance))
            {
                comparisonResults.Add(source.SorensenDiceDistance(target));
            }

            // Min: 0    Max: 1
            if (options.Contains(FuzzyStringComparisonOptions.UseOverlapCoefficient))
            {
                comparisonResults.Add(1 - source.OverlapCoefficient(target));
            }

            // Min: 0    Max: 1
            if (options.Contains(FuzzyStringComparisonOptions.UseRatcliffObershelpSimilarity))
            {
                comparisonResults.Add(1 - source.RatcliffObershelpSimilarity(target));
            }

            if (comparisonResults.Count == 0)
            {
                averageScore = 0;
                return false;
            }

            averageScore = comparisonResults.Average();
            if (tolerance == FuzzyStringComparisonTolerance.Strong) return averageScore < 0.25;
            if (tolerance == FuzzyStringComparisonTolerance.Normal) return averageScore < 0.5;
            if (tolerance == FuzzyStringComparisonTolerance.Weak) return averageScore < 0.75;
            if (tolerance == FuzzyStringComparisonTolerance.Manual) return averageScore > 0.6;
            return false;
        }

        public static bool ApproximatelyEquals(this string source, string target, FuzzyStringComparisonTolerance tolerance, params FuzzyStringComparisonOptions[] options) =>
            ApproximatelyEquals(source, target, tolerance, out double _, options);
    }
}