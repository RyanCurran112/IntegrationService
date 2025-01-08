using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace BehrensGroup_MiddlewareIQ.Tools
{
    public sealed class CsvReader : System.IDisposable
    {
        public CsvReader(string fileName) : this(new FileStream(fileName, FileMode.Open, FileAccess.Read))
        {
        }

        public CsvReader(Stream stream)
        {
            __reader = new StreamReader(stream);
        }

        public System.Collections.IEnumerable RowEnumerator
        {
            get
            {
                if (null == __reader)
                {
                    throw new System.ApplicationException("I can't start reading without CSV input.");
                }

                __rowno = 0;

                string sLine;
                string sNextLine;

                while (null != (sLine = __reader.ReadLine()))
                {
                    while (rexRunOnLine.IsMatch(sLine) && null != (sNextLine = __reader.ReadLine()))
                    {
                        sLine += "\n" + sNextLine;
                    }
                    __rowno++;

                    string[] values = rexCsvSplitter.Split(sLine);

                    for (int i = 0; i < values.Length; i++)
                    {
                        values[i] = Csv.Unescape(values[i]);
                    }
                    yield return values;
                }

                __reader.Close();
            }
        }

        public long RowIndex { get { return __rowno; } }

        public void Dispose()
        {
            if (null != __reader)
            {
                __reader.Dispose();
            }
        }

        //============================================


        private long __rowno = 0;
        private readonly TextReader __reader;
        private readonly static Regex rexCsvSplitter = new Regex(@",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))");
        private readonly static Regex rexRunOnLine = new Regex(@"^[^""]*(?:""[^""]*""[^""]*)*""[^""]*$");
    }

    public static class Csv
    {
        public static string Escape(string s)
        {
            if (s.Contains(QUOTE))
                s = s.Replace(QUOTE, ESCAPED_QUOTE);

            if (s.IndexOfAny(CHARACTERS_THAT_MUST_BE_QUOTED) > -1)
                s = QUOTE + s + QUOTE;

            return s;
        }

        public static string Unescape(string s)
        {
            if (s.StartsWith(QUOTE) && s.EndsWith(QUOTE))
            {
                s = s.Substring(1, s.Length - 2);

                if (s.Contains(ESCAPED_QUOTE))
                    s = s.Replace(ESCAPED_QUOTE, QUOTE);
            }

            return s;
        }

        private const string QUOTE = "\"";
        private const string ESCAPED_QUOTE = "\"\"";
        private readonly static char[] CHARACTERS_THAT_MUST_BE_QUOTED = { ',', '"', '\n' };

        public static void SortCSV(string inFile, string outFile, int column, int column2)
        {
            string[] lines = File.ReadAllLines(inFile);
            var data = lines.Skip(0);
            var sorted = data.Select(line => new
            {
                SortKey = line.Split(',')[column],
                SortKeyThenBy = line.Split(',')[column2],
                Line = line
            })
            .OrderBy(x => x.SortKey).ThenByDescending(x => x.SortKeyThenBy)
            .Select(x => x.Line);
            File.WriteAllLines(outFile, lines.Take(0).Concat(sorted));
        }

        public static void SortMiscCSV()
        {
            string inFile;
            string outFile;
            int column1;
            int column2;

            inFile = "C:\\Users\\rcurran\\Desktop\\orders-export-2018_04_06_08_11_26 - SalesOrderSorted.csv";
            outFile = "C:\\Users\\rcurran\\Desktop\\orders-export-2018_04_06_08_11_26 - SalesOrderSorted2.csv";
            column1 = 0;
            column2 = 17;

            SortCSV(inFile, outFile, column1, column2);
        }
    }
}
