using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoogleEngine
{
    public static class GlobalVariables
    {
        public static List<List<string>> Content = new();
        public static List<List<string>> TrueContent = new();
        public static string[] filesPath;
        static Language language;
        public static string[]? fileNames;
        public static List<string> allwords;
        public static (List<Dictionary<string, int>>, List<Dictionary<string, int>>) dataset;
        public static float[][]? documents_matrix;
        static string? path;

        private static Thread thread_start;

        public static bool first_search;

        public static void Initialize()
        {
            path = "Content";
            language = Language.English;
            allwords = new();
            dataset = new();
            thread_start = new Thread(new ThreadStart(SearchMethod.Start));
            thread_start.Start();
            first_search = true;
        }
        public static string? Directory { get { return path; } }
        public static Language Language { get { return language; } }
    }
}
