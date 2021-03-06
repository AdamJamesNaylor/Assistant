﻿namespace Imbick.Assistant.Core {
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NLog;
    using Steps;
    using System;

    public class Workflow
        : IRunnable {
        public string Name { get; }
        public IReadOnlyCollection<Step> Steps => _steps.AsReadOnly();
        public bool IsRunning { get; private set; }
        public bool IsEnabled { get; private set; }

        public Workflow(string name) {
            Name = name;
            _steps = new StepCollection();
            _logger = LogManager.GetCurrentClassLogger();
            IsRunning = false;
            IsEnabled = true;
        }

        public async Task<RunResult> Run(WorkflowState workflowState = null) {
            try {
                IsRunning = true;
                foreach (var step in Steps) {
                    _logger.Trace($"Running step {step.Name} in workflow {Name}.");
                    var result = await step.Run(workflowState);
                    if (!result.Continue) {
                        _logger.Debug($"Workflow {Name} terminated after step {step.Name}.");
                        break;
                    }
                    _logger.Trace($"Step {step.Name} passed in workflow {Name}.");
                }
            } catch (Exception ex) {
                _logger.Error(ex);
                _logger.Debug($"Disabling workflow {Name} due to exception.");
                IsEnabled = false;
                return RunResult.Failed;
            } finally {
                IsRunning = false;
            }
            return RunResult.Passed;
        }

        public void AddStep(Step step) {
            _logger.Trace($"Added step {step.Name} to workflow {Name}.");
            _steps.Add(step);
        }

        private readonly StepCollection _steps;
        private readonly Logger _logger;
    }
}