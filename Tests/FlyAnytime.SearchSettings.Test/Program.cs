using System;

namespace FlyAnytime.SearchSettings.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var runner = new TestCaseRunner();

            runner.Run(LanguageTestCase.TestSaveLanguage);
            runner.Run(LanguageTestCase.TestPagination);

            Console.WriteLine("Test Passed!");
            Console.ReadKey();
        }
    }
}
