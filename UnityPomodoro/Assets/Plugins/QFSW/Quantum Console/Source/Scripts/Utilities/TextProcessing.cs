using QFSW.QC.Containers;
using QFSW.QC.Pooling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QFSW.QC
{
    public static class TextProcessing
    {
        public static readonly char[] DefaultLeftScopers = { '<', '[', '(', '{', '"' };
        public static readonly char[] DefaultRightScopers = { '>', ']', ')', '}', '"' };

        private static readonly ConcurrentStringBuilderPool _stringBuilderPool = new ConcurrentStringBuilderPool();

        /// <summary>
        /// Options to provide the ReduceScoped functions with
        /// </summary>
        public struct ReduceScopeOptions
        {
            /// <summary>
            /// The maximum number of times the scope can be reduced by.
            /// Setting to -1 will allow for an unlimited number.
            /// </summary>
            public int MaxReductions;

            /// <summary>
            /// If incomplete scopes should also be reduced
            /// For example, the following text -> "((foo0 foo1)" when using () for scoping
            /// - false: "(foo0 foo1"
            /// - true:  "foo0 foo1"
            /// </summary>
            public bool ReduceIncompleteScope;

            /// <summary>
            /// The default set of options for the ReduceScope functions
            /// </summary>
            public static readonly ReduceScopeOptions Default = new ReduceScopeOptions
            {
                MaxReductions = -1,
                ReduceIncompleteScope = false
            };
        }

        /// <summary>
        /// Options to provide the SplitScoped functions with
        /// </summary>
        public struct ScopedSplitOptions
        {
            /// <summary>
            /// The maximum number of items to split the string into.
            /// Setting to -1 will allow for an unlimited number.
            /// </summary>
            public int MaxCount;

            /// <summary>
            /// If the scope should automatically be reduced when performing scoped splitting.
            /// </summary>
            public bool AutoReduceScope;

            /// <summary>
            /// The default set of options for the SplitScoped functions
            /// </summary>
            public static readonly ScopedSplitOptions Default = new ScopedSplitOptions
            {
                MaxCount = -1,
                AutoReduceScope = false,
            };
        }

        #region GetMaxScopeDepthAtEnd

        public static int GetMaxScopeDepthAtEnd(this string input)
        {
            return input.GetMaxScopeDepthAtEnd(DefaultLeftScopers, DefaultRightScopers);
        }

        public static int GetMaxScopeDepthAtEnd(this string input, char leftScoper, char rightScoper)
        {
            return input.GetMaxScopeDepthAtEnd(leftScoper.AsArraySingle(), rightScoper.AsArraySingle());
        }

        public static int GetMaxScopeDepthAtEnd<T>(this string input, T leftScopers, T rightScopers)
            where T : IReadOnlyList<char>
        {
            return input.GetMaxScopeDepthAt(input.Length - 1, leftScopers, rightScopers);
        }

        #endregion

        #region GetMaxScopeDepthAt

        public static int GetMaxScopeDepthAt(this string input, int cursor)
        {
            return input.GetMaxScopeDepthAt(cursor, DefaultLeftScopers, DefaultRightScopers);
        }

        public static int GetMaxScopeDepthAt(this string input, int cursor, char leftScoper, char rightScoper)
        {
            return input.GetMaxScopeDepthAt(cursor, leftScoper.AsArraySingle(), rightScoper.AsArraySingle());
        }

        public static int GetMaxScopeDepthAt<T>(this string input, int cursor, T leftScopers, T rightScopers)
            where T : IReadOnlyList<char>
        {
            int[] scopes = new int[leftScopers.Count];
            for (int i = 0; i <= cursor; i++)
            {
                if (i == 0 || input[i - 1] != '\\')
                {
                    for (int j = 0; j < leftScopers.Count; j++)
                    {
                        char leftScoper = leftScopers[j];
                        char rightScoper = rightScopers[j];

                        if (input[i] == leftScoper && leftScoper == rightScoper) { scopes[j] = 1 - scopes[j]; }
                        else if (input[i] == leftScoper) { scopes[j]++; }
                        else if (input[i] == rightScoper) { scopes[j]--; }
                    }
                }
            }

            return scopes.Max();
        }

        #endregion

        #region ReduceScope

        public static string ReduceScope(this string input)
        {
            return input.ReduceScope(DefaultLeftScopers, DefaultRightScopers, ReduceScopeOptions.Default);
        }

        public static string ReduceScope(this string input, ReduceScopeOptions options)
        {
            return input.ReduceScope(DefaultLeftScopers, DefaultRightScopers, options);
        }

        public static string ReduceScope(this string input, char leftScoper, char rightScoper)
        {
            return input.ReduceScope(leftScoper.AsArraySingle(), rightScoper.AsArraySingle(), ReduceScopeOptions.Default);
        }

        public static string ReduceScope(this string input, char leftScoper, char rightScoper, ReduceScopeOptions options)
        {
            return input.ReduceScope(leftScoper.AsArraySingle(), rightScoper.AsArraySingle(), options);
        }

        public static string ReduceScope<T>(this string input, T leftScopers, T rightScopers)
            where T : IReadOnlyList<char>
        {
            return ReduceScope(input, leftScopers, rightScopers, ReduceScopeOptions.Default);
        }

        public static string ReduceScope<T>(this string input, T leftScopers, T rightScopers, ReduceScopeOptions options)
            where T : IReadOnlyList<char>
        {
            if (leftScopers.Count != rightScopers.Count)
            {
                throw new ArgumentException("There must be an equal number of corresponding left and right scopers");
            }

            if (string.IsNullOrWhiteSpace(input))
            {
                return string.Empty;
            }

            if (options.MaxReductions == 0)
            {
                return input;
            }

            // Use cursors to point into the current string instead of repeated
            // substrings for improved performance
            int leftCursor = 0;
            int rightCursor = input.Length - 1;

            // Determines if a cursor is pointing to an escaped character
            bool IsEscaped(int cursor)
            {
                return cursor > 0 && input[cursor - 1] == '\\';
            }

            int totalScopeReductions = 0;
            bool workRemaining = true;

            // Keep descoping the string until we either run out of work to do, or we hit the maximum number of reductions
            while (workRemaining && (totalScopeReductions < options.MaxReductions || options.MaxReductions < 0))
            {
                // If the left cursor ever surpasses the right, we have descoped to an empty string
                if (leftCursor > rightCursor)
                {
                    return string.Empty;
                }

                // Update cursors to skip over any whitespace to emulate Trim
                workRemaining = false;
                while (char.IsWhiteSpace(input[leftCursor]))  { leftCursor++; }
                while (char.IsWhiteSpace(input[rightCursor])) { rightCursor--; }

                // If the right cursor is escaped, then finish here
                if (IsEscaped(rightCursor))
                {
                    break;
                }

                // Check each pair of scopers
                for (int i = 0; i < leftScopers.Count; i++)
                {
                    char leftScoper = leftScopers[i];
                    char rightScoper = rightScopers[i];
                    bool sameScoper = leftScoper == rightScoper;

                    // Determines if we've hit a valid scoper pair
                    bool validScoperPair = input[leftCursor] == leftScoper && input[rightCursor] == rightScoper;
                    bool incompleteReduction = false;

                    if (!validScoperPair && options.ReduceIncompleteScope)
                    {
                        // Only the left cursor needs to match for incomplete scope reduction
                        validScoperPair = input[leftCursor] == leftScoper;
                        incompleteReduction = validScoperPair;
                    }
                    
                    if (validScoperPair)
                    {
                        // Search between the two cursors to make sure scope never drops down to 0 between them
                        // as this would be two separate scopes being incorrectly descoped
                        bool scopeBreaks = false;
                        int currentScope = 1;
                        int leftSearch = leftCursor + 1;
                        int rightSearch = rightCursor - 1;

                        // Only perform search if there is a valid search range
                        if (leftSearch <= rightSearch)
                        {
                            // Logic for same scoper is a bit different since we can't really define a scope depth
                            // If it's impossible to remove inner scope characters by stripping the outer pair
                            // of scopers then this is a broken scope, otherwise its fine - update the search range to allow for this
                            // fine     : ""foo""
                            // not fine : "foo1""foo2"
                            if (sameScoper)
                            {
                                // Determines if the cursor location can skip searching for same scoper case
                                bool SkipSearch(int cursor)
                                {
                                    if (IsEscaped(cursor))
                                    {
                                        return false;
                                    }

                                    return input[cursor] == leftScoper || char.IsWhiteSpace(input[cursor]);
                                }

                                while (SkipSearch(leftSearch))  { leftSearch++; }
                                while (SkipSearch(rightSearch)) { rightSearch--; }
                            }

                            // Perform the search
                            for (int j = leftSearch; j <= rightSearch; j++)
                            {
                                // Ignore escaped characters
                                if (IsEscaped(j))
                                {
                                    continue;
                                }

                                if (sameScoper)
                                {
                                    // If we find any scopers inside the refined scope range then we can't descope
                                    if (input[j] == leftScoper)
                                    {
                                        scopeBreaks = true;
                                        break;
                                    }
                                }
                                else
                                {
                                    // For normal scopers, just check that scope never hits 0
                                    // Update the current scope level
                                    if      (input[j] == leftScoper)  { currentScope++; }
                                    else if (input[j] == rightScoper) { currentScope--; }

                                    // Scope broken if it hits 0
                                    if (currentScope == 0)
                                    {
                                        scopeBreaks = true;
                                        break;
                                    }
                                }
                            }
                        }

                        // If the scope never breaks, then we can successfully descope
                        if (!scopeBreaks)
                        {
                            // Update the cursors and break out of for loop
                            // Don't move the right cursor for incomplete reduction
                            if (!incompleteReduction)
                            {
                                rightCursor--;
                            }

                            leftCursor++;
                            totalScopeReductions++;
                            workRemaining = true;
                            break;
                        }
                    }
                }
            }

            // Perform substring on cursors to get final descoped output if required
            return totalScopeReductions > 0
                ? input.Substring(leftCursor, rightCursor - leftCursor + 1)
                : input;
        }

        #endregion

        #region SplitScoped

        public static string[] SplitScoped(this string input, char splitChar)
        {
            return input.SplitScoped(splitChar, ScopedSplitOptions.Default);
        }

        public static string[] SplitScoped(this string input, char splitChar, ScopedSplitOptions options)
        {
            return input.SplitScoped(splitChar, DefaultLeftScopers, DefaultRightScopers, options);
        }

        public static string[] SplitScoped(this string input, char splitChar, char leftScoper, char rightScoper)
        {
            return input.SplitScoped(splitChar, leftScoper.AsArraySingle(), rightScoper.AsArraySingle(), ScopedSplitOptions.Default);
        }

        public static string[] SplitScoped(this string input, char splitChar, char leftScoper, char rightScoper, ScopedSplitOptions options)
        {
            return input.SplitScoped(splitChar, leftScoper.AsArraySingle(), rightScoper.AsArraySingle(), options);
        }

        public static string[] SplitScoped<T>(this string input, char splitChar, T leftScopers, T rightScopers)
            where T : IReadOnlyList<char>
        {
            return SplitScoped(input, splitChar, leftScopers, rightScopers, ScopedSplitOptions.Default);
        }

        public static string[] SplitScoped<T>(this string input, char splitChar, T leftScopers, T rightScopers, ScopedSplitOptions options)
            where T : IReadOnlyList<char>
        {
            if (options.AutoReduceScope)
            {
                input = input.ReduceScope(leftScopers, rightScopers);
            }

            if (string.IsNullOrWhiteSpace(input))
            {
                return Array.Empty<string>();
            }

            IEnumerable<int> rawSplitIndices = GetScopedSplitPoints(input, splitChar, leftScopers, rightScopers);
            int[] splitIndices = 
                options.MaxCount > 0 
                    ? rawSplitIndices.Take(options.MaxCount - 1).ToArray() 
                    : rawSplitIndices.ToArray();

            // Return single array when no splits occurred
            if (splitIndices.Length == 0)
            {
                return new[] { input };
            }

            string[] splitString = new string[splitIndices.Length + 1];
            int lastSplitIndex = 0;
            for (int i = 0; i < splitIndices.Length; i++)
            {
                splitString[i] = input.Substring(lastSplitIndex, splitIndices[i] - lastSplitIndex).Trim();
                lastSplitIndex = splitIndices[i] + 1;
            }

            splitString[splitIndices.Length] = input.Substring(lastSplitIndex).Trim();
            return splitString;
        }

        #endregion

        #region GetScopedSplitPoints

        public static IEnumerable<int> GetScopedSplitPoints<T>(string input, char splitChar, T leftScopers, T rightScopers)
            where T : IReadOnlyList<char>
        {
            return GetScopedSplitPoints(input, splitChar, leftScopers, rightScopers, ScopedSplitOptions.Default);
        }

        public static IEnumerable<int> GetScopedSplitPoints<T>(
            string input, char splitChar, T leftScopers, T rightScopers, ScopedSplitOptions options)
            where T : IReadOnlyList<char>
        {
            if (leftScopers.Count != rightScopers.Count)
            {
                throw new ArgumentException("There must be an equal number of corresponding left and right scopers");
            }

            int[] scopes = new int[leftScopers.Count];
            for (int i = 0; i < input.Length; i++)
            {
                if (i == 0 || input[i - 1] != '\\')
                {
                    for (int j = 0; j < leftScopers.Count; j++)
                    {
                        char leftScoper = leftScopers[j];
                        char rightScoper = rightScopers[j];

                        if (input[i] == leftScoper && leftScoper == rightScoper) { scopes[j] = 1 - scopes[j]; }
                        else if (input[i] == leftScoper) { scopes[j]++; }
                        else if (input[i] == rightScoper) { scopes[j]--; }
                    }
                }

                if (input[i] == splitChar && scopes.All(x => x == 0))
                {
                    yield return i;
                }
            }
        }

        #endregion

        public static bool CanSplitScoped(this string input, char splitChar)
        {
            return input.CanSplitScoped(splitChar, DefaultLeftScopers, DefaultRightScopers);
        }

        public static bool CanSplitScoped(this string input, char splitChar, char leftScoper, char rightScoper)
        {
            return input.CanSplitScoped(splitChar, leftScoper.AsArraySingle(), rightScoper.AsArraySingle());
        }

        public static bool CanSplitScoped<T>(this string input, char splitChar, T leftScopers, T rightScopers)
            where T : IReadOnlyList<char>
        {
            return GetScopedSplitPoints(input, splitChar, leftScopers, rightScopers).Any();
        }

        public static string SplitFirst(this string input, char splitChar)
        {
            return input.SplitScopedFirst(splitChar, Array.Empty<char>(), Array.Empty<char>());
        }

        public static string SplitScopedFirst(this string input, char splitChar)
        {
            return input.SplitScopedFirst(splitChar, DefaultLeftScopers, DefaultRightScopers);
        }

        public static string SplitScopedFirst(this string input, char splitChar, char leftScoper, char rightScoper)
        {
            return input.SplitScopedFirst(splitChar, leftScoper.AsArraySingle(), rightScoper.AsArraySingle());
        }

        public static string SplitScopedFirst<T>(this string input, char splitChar, T leftScopers, T rightScopers)
            where T : IReadOnlyList<char>
        {
            IEnumerable<int> splitPoints = GetScopedSplitPoints(input, splitChar, leftScopers, rightScopers);
            foreach (int splitPoint in splitPoints)
            {
                return input.Substring(0, splitPoint).Trim();
            }

            return input;
        }

        public static string UnescapeText(this string input, char escapeChar) { return input.UnescapeText(escapeChar.AsArraySingle()); }
        public static string UnescapeText<T>(this string input, T escapeChars)
            where T : IReadOnlyCollection<char>
        {
            foreach (char escapeChar in escapeChars)
            {
                input = input.Replace($"\\{escapeChar}", escapeChar.ToString());
            }

            return input;
        }

        public static string ReverseItems(this string input, char splitChar)
        {
            int lastSplit = input.Length;
            StringBuilder buffer = _stringBuilderPool.GetStringBuilder(input.Length);

            for (int i = input.Length - 1; i >= 0; i--)
            {
                if (input[i] == splitChar)
                {
                    int substringIndex = i + 1;
                    if (substringIndex < input.Length)
                    {
                        buffer.Append(input, substringIndex, lastSplit - substringIndex);
                    }

                    buffer.Append(splitChar);
                    lastSplit = i;
                }
                else if (i == 0)
                {
                    buffer.Append(input, 0, lastSplit);
                }
            }

            return _stringBuilderPool.ReleaseAndToString(buffer);
        }
    }
}
