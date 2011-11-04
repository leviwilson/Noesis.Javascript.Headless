using System.Text;

namespace Noesis.Javascript.Headless.Reporters
{
    public class StringReporter : IJavaScriptReporter
    {
        public int TotalSpecs;
        public int TotalFailures;

        private readonly StringBuilder _resultOutput = new StringBuilder();
        public string Result
        {
            get { return _resultOutput.ToString(); }
        }

        public void Passed(string name)
        {
            TotalSpecs++;

            _resultOutput.Append(".");
        }

        public void Failed(string name, object[] errors)
        {
            TotalSpecs++;
            TotalFailures++;

            _resultOutput.AppendFormat("X\n{0}\r\n", name);
            foreach (var error in errors)
            {
                _resultOutput.AppendLine(error.ToString());
            }
        }

        public void Finished()
        {
            _resultOutput.AppendFormat("{0} specs run, {1} failures\n\n", TotalSpecs, TotalFailures);
        }
    }
}