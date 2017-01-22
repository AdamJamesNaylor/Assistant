
namespace Imbick.Assistant.Core
{
    using System.Linq;
    using System.Collections.Generic;

    public class WorkflowRunner {
        private readonly List<Workflow> _workflows = new List<Workflow>();

        public WorkflowRunner() {
            var emailTrigger = new EmailReceivedTrigger("an.notrust@gmail.com", "EUJDoQbPhUGWNpQssY3egtfU5dg5tg");
            var subjectTrigger = new StringEqualsTrigger("email_subject", "test email");
            var emailNotify = new Workflow();
            emailNotify.AddTrigger(emailTrigger);
            emailNotify.AddTrigger(subjectTrigger);
        }

        public void Register(Workflow workflow) {
            _workflows.Add(workflow);
        }

        public void RunAllWorkflows() {
            while (true) {
                //if 100 milliseconds has passed

                foreach (var workflow in _workflows.Where(w => w.HaveAllTriggersFired())) {
                    workflow.Action.Run();
                }
            }
        }

    }
}