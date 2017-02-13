
namespace Imbick.Assistant.Core.Steps.Actions {
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NLog;

    public class FuzzyTextMatch {
        public List<string> Terms { get; set; } 
        public StepCollection Steps { get; set; } 
    }

    public class FuzzyTextMatchAction
        : Step {

        public List<FuzzyTextMatch> Matches { get; set; }
        public StepCollection FailureSteps { get; set; }

        public FuzzyTextMatchAction(Func<WorkflowState, string> valueResolver, int accuracyThreshold = 4)
            : base("Match chat text action") {
            _valueResolver = valueResolver;
            _accuracyThreshold = accuracyThreshold;
            Matches = new List<FuzzyTextMatch>();
            FailureSteps = new StepCollection();
            _logger = LogManager.GetCurrentClassLogger();
        }

        public override async Task<RunResult> Run(WorkflowState workflowState) {
            var request = _valueResolver(workflowState);
            var lowestScore = _accuracyThreshold;
            StepCollection matchedSteps = null;
            foreach (var potentialMatch in Matches) {
                foreach (var term in potentialMatch.Terms) {
                    var score = CalculateSimilarity(request, term);
                    if (score >= lowestScore)
                        continue;
                    lowestScore = score;
                    matchedSteps = potentialMatch.Steps;
                }
            }
            if (lowestScore >= _accuracyThreshold || matchedSteps == null) {
                _logger.Trace($"No match was found for term '{request}'. Lowest score was {lowestScore} but should have been less than {_accuracyThreshold}.");
                return await FailureSteps.Run(workflowState);
            }

            return await matchedSteps.Run(workflowState);
        }

        private int CalculateSimilarity(string s, string t) {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            // Step 1
            if (n == 0)
                return m;

            if (m == 0)
                return n;

            // Step 2
            for (int i = 0; i <= n; d[i, 0] = i++) {
            }

            for (int j = 0; j <= m; d[0, j] = j++) {
            }

            // Step 3
            for (int i = 1; i <= n; i++) {
                //Step 4
                for (int j = 1; j <= m; j++) {
                    // Step 5
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

                    // Step 6
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            // Step 7
            return d[n, m];
        }

        private readonly Func<WorkflowState, string> _valueResolver;
        private readonly int _accuracyThreshold;
        private readonly Logger _logger;
    }
}