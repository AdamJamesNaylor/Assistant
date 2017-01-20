using System;
using System.Collections.Generic;

namespace Imbick.Assistant.WebApi
{
    using System.Linq;

    public interface IFireable {
        bool HasFired();
    }

    public class IntervalTrigger
        : IFireable {

        public IntervalTrigger(TimeSpan interval) {
            _interval = interval;
            _lastFired = DateTime.Now;
        }

        public bool HasFired() {
            var now = DateTime.Now;
            if (_lastFired + _interval < now)
                return false;

            _lastFired = now;
            return true;
        }

        private readonly TimeSpan _interval;
        private DateTime _lastFired;
    }

    public interface IRunnable {
        void Run();
    }

    public class SendEmailAction
        : IRunnable {

        public void Run() {
            throw new NotImplementedException();
        }
    }

    public interface IActionable {
        IRunnable Action { get; }
    }

    public interface ITriggerable {
        IEnumerable<IFireable> Triggers { get; }

        bool HaveAllTriggersFired();
    }

    public class Workflow
        : ITriggerable, IActionable {

        public IEnumerable<IFireable> Triggers { get; }

        public bool HaveAllTriggersFired() {
            return Triggers.All(trigger => trigger.HasFired());
        }

        public IRunnable Action { get; }
    }

    public class WorkflowRunner {
        private readonly List<Workflow> _workflows = new List<Workflow>();

        public void Register(Workflow workflow) {
            _workflows.Add(workflow);
        }

        public void RunAllWorkflows() {
            while (true) {
                //if 100 milliseconds has passed

                foreach (var workflow in _workflows.Where(workflow => workflow.HaveAllTriggersFired())) {
                    workflow.Action.Run();
                }
            }
        }

    }
}