using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using NLog;

namespace Buildalyzer
{
    public class AnalyzerResults : IAnalyzerResults
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly ConcurrentDictionary<string, IAnalyzerResult> _results = new ConcurrentDictionary<string, IAnalyzerResult>();

        private bool? _overallSuccess = null;

        public bool OverallSuccess => _overallSuccess == true;

        internal void Add(IEnumerable<IAnalyzerResult> results, bool overallSuccess)
        {
            foreach (IAnalyzerResult result in results)
            {
                _logger.Debug($"Adding result {result.ProjectFilePath}");
                _results[result.TargetFramework ?? string.Empty] = result;
            }
            _overallSuccess = _overallSuccess.HasValue ? _overallSuccess.Value && overallSuccess : overallSuccess;
            _logger.Debug($"Overall success: {_overallSuccess}");
        }

        public IAnalyzerResult this[string targetFramework] => _results[targetFramework];

        public IEnumerable<string> TargetFrameworks => _results.Keys.OrderBy(e => e, TargetFrameworkComparer.Instance);

        public IEnumerable<IAnalyzerResult> Results => TargetFrameworks.Select(e => _results[e]);

        public int Count => _results.Count;

        public bool ContainsTargetFramework(string targetFramework) => _results.ContainsKey(targetFramework);

        public bool TryGetTargetFramework(string targetFramework, out IAnalyzerResult result) => _results.TryGetValue(targetFramework, out result);

        public IEnumerator<IAnalyzerResult> GetEnumerator() => Results.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private class TargetFrameworkComparer : IComparer<string>
        {
            public static TargetFrameworkComparer Instance { get; } = new TargetFrameworkComparer();

            public int Compare(string x, string y)
            {
                return (string.IsNullOrEmpty(x), string.IsNullOrEmpty(y)) switch
                {
                    (true, true) => 0,
                    (true, false) => +1,
                    (false, true) => -1,
                    _ => StringComparer.OrdinalIgnoreCase.Compare(x, y)
                };
            }
        }
    }
}