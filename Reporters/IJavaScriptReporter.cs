namespace Noesis.Javascript.Headless.Reporters
{
    public interface IJavaScriptReporter
    {
        string Result { get; }
        void Passed(string name);
        void Failed(string name, object[] errors);
        void Finished();
    }
}