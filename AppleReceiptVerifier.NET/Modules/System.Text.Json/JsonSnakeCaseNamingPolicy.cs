using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AppleReceiptVerifierNET.Modules.System.Text.Json
{
    // Taken from https://github.com/dotnet/runtime/pull/42009/
    class JsonSnakeCaseNamingPolicy : JsonSeparatedCaseNamingPolicy
    {
        public override string ConvertName(string name)
        {
            var snakeCasedName = ToSeparatedCase(name, '_');
            if (snakeCasedName.EndsWith("_date")) // we want to use the key with the raw timestamp 
                snakeCasedName += "_ms";
            return snakeCasedName;
        }
    }

    abstract class JsonSeparatedCaseNamingPolicy : JsonNamingPolicy
    {
        private enum SeparatedCaseState
        {
            Start,
            NewWord,
            Upper,
            Lower
        }

        protected static string ToSeparatedCase(string s, char separator)
        {
            if (string.IsNullOrEmpty(s))
            {
                return s;
            }

            StringBuilder sb = new StringBuilder();
            SeparatedCaseState state = SeparatedCaseState.Start;

            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == ' ')
                {
                    if (state != SeparatedCaseState.Start)
                    {
                        state = SeparatedCaseState.NewWord;
                    }
                }
                else if (char.IsUpper(s[i]))
                {
                    switch (state)
                    {
                        case SeparatedCaseState.Upper:
                            bool hasNext = (i + 1 < s.Length);
                            if (i > 0 && hasNext)
                            {
                                char nextChar = s[i + 1];
                                if (!char.IsUpper(nextChar) && nextChar != separator)
                                {
                                    sb.Append(separator);
                                }
                            }
                            break;
                        case SeparatedCaseState.Lower:
                        case SeparatedCaseState.NewWord:
                            sb.Append(separator);
                            break;
                    }

                    char c;
                    c = char.ToLowerInvariant(s[i]);
                    sb.Append(c);

                    state = SeparatedCaseState.Upper;
                }
                else if (s[i] == separator)
                {
                    sb.Append(separator);
                    state = SeparatedCaseState.Start;
                }
                else
                {
                    if (state == SeparatedCaseState.NewWord)
                    {
                        sb.Append(separator);
                    }

                    sb.Append(s[i]);
                    state = SeparatedCaseState.Lower;
                }
            }

            return sb.ToString();
        }
    }
}
