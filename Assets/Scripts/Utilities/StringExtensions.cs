using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class StringExtensions
{
    public static string FirstCharToUpper(this string input) =>
        input switch
        {
            null => throw new ArgumentNullException(nameof(input)),
            "" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
            _ => input[0].ToString().ToUpper() + input.Substring(1)
        };

    public static string ToSnakeCase(this string input)
    {
        if (input == null)
        {
            throw new ArgumentNullException(nameof(input));
        }
        if (input.Length < 2)
        {
            return input;
        }
        var sb = new StringBuilder();
        sb.Append(char.ToLowerInvariant(input[0]));
        for (int i = 1; i < input.Length; ++i)
        {
            char c = input[i];
            if (char.IsUpper(c))
            {
                sb.Append('_');
                sb.Append(char.ToLowerInvariant(c));
            }
            else
            {
                sb.Append(c);
            }
        }
        return sb.ToString();
    }

    public static string ToPascalCase(this string input)
    {
        return string.Join("", input.Split(new[] { '_', ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Substring(0, 1).ToUpper() + s.Substring(1)).ToArray());
    }

    /// <summary>
    /// Calculate the difference between 2 strings using the Levenshtein distance algorithm.
    /// </summary>
    /// <see cref="https://gist.github.com/Davidblkx/e12ab0bb2aff7fd8072632b396538560"/>
    /// <param name="source1">First string</param>
    /// <param name="source2">Second string</param>
    /// <returns></returns>
    public static int LevenshteinDistance(this string source1, string source2) //O(n*m)
    {
        var source1Length = source1.Length;
        var source2Length = source2.Length;

        var matrix = new int[source1Length + 1, source2Length + 1];

        // First calculation, if one entry is empty return full length
        if (source1Length == 0)
            return source2Length;

        if (source2Length == 0)
            return source1Length;

        // Initialization of matrix with row size source1Length and columns size source2Length
        for (var i = 0; i <= source1Length; matrix[i, 0] = i++) { }
        for (var j = 0; j <= source2Length; matrix[0, j] = j++) { }

        // Calculate rows and collumns distances
        for (var i = 1; i <= source1Length; i++)
        {
            for (var j = 1; j <= source2Length; j++)
            {
                var cost = (source2[j - 1] == source1[i - 1]) ? 0 : 1;

                matrix[i, j] = Math.Min(
                    Math.Min(matrix[i - 1, j] + 1, matrix[i, j - 1] + 1),
                    matrix[i - 1, j - 1] + cost);
            }
        }
        // return result
        return matrix[source1Length, source2Length];
    }
}
