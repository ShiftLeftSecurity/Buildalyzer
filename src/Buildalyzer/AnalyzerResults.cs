﻿using System.Collections;
using System.Collections.Generic;

namespace Buildalyzer
{
    public class AnalyzerResults : IEnumerable<AnalyzerResult>
    {
        private readonly Dictionary<string, AnalyzerResult> _results = new Dictionary<string, AnalyzerResult>();

        private bool? _overallSuccess = null;

        public bool OverallSuccess => _overallSuccess.HasValue ? _overallSuccess.Value : false;
        
        internal void Add(IEnumerable<AnalyzerResult> results, bool overallSuccess)
        {
            foreach (AnalyzerResult result in results)
            {
                _results[result.TargetFramework ?? string.Empty] = result;
            }
            _overallSuccess = _overallSuccess.HasValue ? _overallSuccess.Value && overallSuccess : overallSuccess;
        }

        public AnalyzerResult this[string targetFramework] => _results[targetFramework];

        public IEnumerable<string> TargetFrameworks => _results.Keys;

        public IEnumerable<AnalyzerResult> Results => _results.Values;

        public int Count => _results.Count;

        public bool ContainsTargetFramework(string targetFramework) => _results.ContainsKey(targetFramework);

        public bool TryGetTargetFramework(string targetFramework, out AnalyzerResult result) => _results.TryGetValue(targetFramework, out result);

        public IEnumerator<AnalyzerResult> GetEnumerator() => _results.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}