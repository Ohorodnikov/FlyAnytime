using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FlyAnytime.SearchSettings.Test
{
    public class TestCaseRunner
    {
        public void Run(Func<Task> testCase)
        {
            Console.WriteLine($"Started {testCase.Method.Name}");
            try
            {
                new DbTest().ReCreateDb().Wait();
                testCase().Wait();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"{testCase.Method.Name}: Success");
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{testCase.Method.Name}: Err");
            }

            Console.ResetColor();
        }
    }
}
