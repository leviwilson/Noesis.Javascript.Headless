using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Noesis.Javascript.Headless.Helpers;
using Noesis.Javascript.Headless.Reporters;

namespace Noesis.Javascript.Headless
{
    public class JavaScriptTestRunner
    {
        private readonly JavascriptContext _javascriptContext = new JavascriptContext();
        private readonly List<string> _scriptFiles;
        private const string ScriptFileSeparator = ";null;";

        public JavaScriptTestRunner()
        {
            _scriptFiles = new List<string>();
            LoadFromResource<JavaScriptTestRunner>("env.therubyracer.js");
            LoadFromResource<JavaScriptTestRunner>("window.js");
        }

        public void Include(JavaScriptLibrary libary)
        {
            switch (libary)
            {
                case JavaScriptLibrary.Backbone:
                    LoadFromResource<JavaScriptTestRunner>("underscore.js");
                    LoadFromResource<JavaScriptTestRunner>("backbone.js");
                    LoadFromResource<JavaScriptTestRunner>("backbone.localStorage-min.js");
                    break;
                case JavaScriptLibrary.BackboneMin:
                    LoadFromResource<JavaScriptTestRunner>("underscore.min.js");
                    LoadFromResource<JavaScriptTestRunner>("backbone.min.js");
                    break;
                case JavaScriptLibrary.BackboneLocalStorage:
                    LoadFromResource<JavaScriptTestRunner>("backbone.localStorage-min.js");
                    break;
                case JavaScriptLibrary.jQuery_1_7_0:
                    LoadFromResource<JavaScriptTestRunner>("jquery-1.7.js");
                    break;
                case JavaScriptLibrary.jQuery_1_7_0_min:
                    LoadFromResource<JavaScriptTestRunner>("jquery-1.7.min.js");
                    break;
                case JavaScriptLibrary.jQuery_1_6_4:
                    LoadFromResource<JavaScriptTestRunner>("jquery-1.6.4.js");
                    break;
                case JavaScriptLibrary.jQuery_1_6_4_min:
                    LoadFromResource<JavaScriptTestRunner>("jquery-1.6.4.min.js");
                    break;
                case JavaScriptLibrary.jQuery_ui_1_8_16:
                    LoadFromResource<JavaScriptTestRunner>("jquery-ui-1.8.16.custom.min.js");
                    break;
                case JavaScriptLibrary.Jasmine_1_1_0:
                    LoadFromResource<JavaScriptTestRunner>("jasmine_1_1_0.jasmine.js");
                    LoadFromResource<JavaScriptTestRunner>("jasmine_1_1_0.jasmine-html.js");
                    break;
                case JavaScriptLibrary.Jasmine_Fixture:
                    LoadFromResource<JavaScriptTestRunner>("jasmine-fixture.js");
                    _javascriptContext.Run("window.jasmineFixture = \"Thing that owned jasmineFixture first\";");
                    break;
                case JavaScriptLibrary.jEditable:
                    LoadFromResource<JavaScriptTestRunner>("jquery.jeditable.mini.js");
                    break;
            }
        }

        public void LoadFromResource<T>(string javascriptFile)
        {
            var assembly = typeof(T).Assembly;

            var manifestResourceNames = assembly.GetManifestResourceNames();
            var resourceName = manifestResourceNames
                .Where(x => x.EndsWith(javascriptFile))
                .Single();

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (var streamReader = new StreamReader(stream))
                {
                    _javascriptContext.Run(streamReader.ReadToEnd() + ScriptFileSeparator);
                }
            }
        }

        public void LoadFile(string fullPath)
        {
            _javascriptContext.Run(File.ReadAllText(fullPath) + ScriptFileSeparator);
        }

        public object ExecuteScript(string javaScript)
        {
            return _javascriptContext.Run(javaScript + ScriptFileSeparator);
        }

        public void RunJasmineSpecs(IJavaScriptReporter javaScriptReporter)
        {
            _javascriptContext.SetParameter("dotNetReporter", javaScriptReporter);
            _javascriptContext.Run(@"
              jasmine.JsApiReporter.prototype.reportSpecResults = function(spec) {
                this.totalSpecs += 1;
                if (spec.results().failedCount == 0) {
                  dotNetReporter.Passed(spec.getFullName());
                
                } else {
                  this.totalFailures += 1;
                
                  var errors = [];
                  var results = spec.results().getItems();
                  for (var i = 0; i < results.length; i++) {
                    if (results[i].trace.stack) {
                      errors.push(results[i].trace.stack);
                    }
                  }
                  dotNetReporter.Failed(spec.getFullName(), errors);
                }
              };

              var jasmineEnv = jasmine.getEnv();
              var reporter = new jasmine.JsApiReporter();
              reporter.totalSpecs = 0;
              reporter.totalFailures = 0;
              
              jasmineEnv.addReporter(reporter);
              jasmineEnv.execute();

              dotNetReporter.Finished();
            ");
        }

        public void FindScriptFilesIn(string baseFolder)
        {
            _scriptFiles.AddRange(DirectorySearcher.GetFilesRecursive(baseFolder, "*.js"));
        }

        public void AddInclude(string includeFile)
        {
            var foundFile = _scriptFiles
                .Where(x => x.EndsWith(includeFile, StringComparison.InvariantCultureIgnoreCase))
                .First();
            LoadFile(foundFile);
        }
    }
}