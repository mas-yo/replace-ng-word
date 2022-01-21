using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace replace_ng_word
{
    public static class StringExtension
    {
        public static List<int> AllIndexOf(this string @this, string word)
        {
            var list = new List<int>();
            var idx = @this.IndexOf(word);
            while (0 <= idx)
            {
                list.Add(idx);
                idx = @this.IndexOf(word, idx + word.Length);
            }

            return list;
        }
    }

    public class Program
    {
        public static List<(int, int)> FindAllWords(string str, IEnumerable<string> words)
        {
            var allIdx = new List<(int, int)>();
            foreach (var word in words)
            {
                var listIdx = str.AllIndexOf(word);
                foreach (var i in listIdx)
                {
                    allIdx.Add((i, i + word.Length));
                }
            }

            return allIdx;
        }

        public static string ReplaceNgWord(string originalString, char replaceCharacter, int? fixedRepeatCount,
            IReadOnlyCollection<string> ngWords, IReadOnlyCollection<string> okWords)
        {

            var ngWordIndexes = FindAllWords(originalString, ngWords);
            var okWordIndexes = FindAllWords(originalString, okWords);

            var exactNgWordIndexes = ngWordIndexes.Where(ngIdx =>
            {
                if (okWordIndexes.Any(okIdx => { return (okIdx.Item1 <= ngIdx.Item1 && ngIdx.Item2 <= okIdx.Item2); }))
                {
                    return false;
                }

                return true;
            });

            var builder = new StringBuilder();
            int lastIdx = 0;
            foreach ((var startIdx, var endIdx) in exactNgWordIndexes)
            {
                builder.Append(originalString.Substring(lastIdx, startIdx - lastIdx));
                int repeatCount = fixedRepeatCount.HasValue ? fixedRepeatCount.Value : endIdx - startIdx;
                for (int c = 0; c < repeatCount; c++)
                {
                    builder.Append(replaceCharacter);
                }

                lastIdx = endIdx;
            }

            builder.Append(originalString.Substring(lastIdx));

            return builder.ToString();
        }

        public static void Main()
        {
            string[] ngWords = {"NG1", "NG2"};
            string[] okWords = {"OKNG1OK", "OK2NG1OK2"};

            var replaced = ReplaceNgWord("aaaNG1bbbOKNG1OKccc", '_', null, ngWords, okWords);

            Console.WriteLine(replaced);
        }
    }
}
