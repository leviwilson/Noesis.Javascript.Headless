using System;
using System.Collections.Generic;
using System.Linq;

namespace Noesis.Javascript.Headless.Reporters
{
    public class ResultsReporter : IJavaScriptReporter
    {
        private readonly Dictionary<string, string> _results = new Dictionary<string, string>();
        public Dictionary<string, string> Results
        {
            get { return _results; }
        }

        public string Result
        {
            get { throw new NotImplementedException(); }
        }

        public void Passed(string name)
        {
            _results.Add(name, null);
        }

        public void Failed(string name, object[] errors)
        {
            var scriptError = errors.Cast<string>()
                .Aggregate((error, nextError) => error + Environment.NewLine + nextError);
            _results.Add(name, scriptError);
        }

        public void Finished()
        {
        }
    }
}