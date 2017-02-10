
namespace Imbick.Assistant.Core.Steps.Actions {
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class Match {
        public List<string> Terms { get; set; } 
        public List<Step> Steps { get; set; } 
    }

    public class FuzzyTextMatchAction
        : Step {

        public List<Match> Matches { get; set; }

        public FuzzyTextMatchAction(Func<WorkflowState, string> valueResolver)
            : base("Match chat text action") {
            _valueResolver = valueResolver;
            Matches = new List<Match>();
        }

        public override async Task<RunResult> Run(WorkflowState workflowState) {
            var request = _valueResolver(workflowState);
            var lowestScore = 0;
            List<Step> matchedSteps;
            foreach (var potentialMatch in Matches) {
                foreach (var term in potentialMatch.Terms) {
                    var score = CalculateSimilarity(request, term);
                    if (score >= lowestScore)
                        continue;
                    lowestScore = score;
                    matchedSteps = potentialMatch.Steps;
                }
            }
            const int ACCURACY_THRESHOLD = 4;
            if (lowestScore > ACCURACY_THRESHOLD)
                workflowState.Payload = "I'm not sure what you mean?";

            //matchedSteps.Run();
            //workflowState.Payload = response;
            return RunResult.Passed;
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

        private readonly Dictionary<string, string> _responseTemplatesMap = new Dictionary<string, string> {
            {"Hello", "Hello"}
        };

        private readonly Func<WorkflowState, string> _valueResolver;
    }
}