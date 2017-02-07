
namespace Imbick.Assistant.Core.Steps.Loops {
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using ValueResolvers;

    public class ForeachLoopStep<T>
        : Step {
    
        public StepCollection Steps { get; set; }

        public ForeachLoopStep(WorkflowParameterValueResolver<IEnumerable<T>> valueResolver)
            : base("Foreach loop step") {
            _valueResolver = valueResolver;
        }

        public async override Task<RunResult> Run(IDictionary<string, WorkflowParameter> workflowParameters) {
            var values = _valueResolver.Resolve();
            foreach (var value in values) {
                
            }
            return RunResult.Passed;
        }

        private WorkflowParameterValueResolver<IEnumerable<T>> _valueResolver;
    }
}