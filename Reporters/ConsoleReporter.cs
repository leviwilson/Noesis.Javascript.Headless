using System;

namespace Noesis.Javascript.Headless.Reporters
{
    public class ConsoleReporter : IJavaScriptReporter
    {
        public int TotalSpecs;
        public int TotalFailures;

        public string Result
        {
            get { return string.Format("\n\n{0} specs run, {1} failures\n\n", TotalSpecs, TotalFailures); }
        }

        public void Passed(string name)
        {
            TotalSpecs++;

            Console.Write(".");
        }

        public void Failed(string name, object[] errors)
        {
            TotalSpecs++;
            TotalFailures++;

            Console.WriteLine("X\n{0}", name);
            foreach (var error in errors)
            {
                Console.WriteLine(error);
            }
        }

        public void Finished()
        {
            Console.WriteLine(Result);
        }
    }
}