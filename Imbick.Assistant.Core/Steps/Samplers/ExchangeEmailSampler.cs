
namespace Imbick.Assistant.Core.Steps.Samplers {
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Exchange.WebServices.Data;
    using NLog;

    public class ExchangeEmailSampler
        : SamplerStep {
        private readonly string _emailAddress;
        private readonly Logger _logger;
        private readonly ExchangeService _exchange;

        public ExchangeEmailSampler(string username, string password, string emailAddress)
            : base("Exchange email sampler") {
            _emailAddress = emailAddress;
            _logger = LogManager.GetCurrentClassLogger();
            _exchange = new ExchangeService {
                Credentials = new WebCredentials(username, password)
            };
        }

        public async override Task<RunResult> Run(IDictionary<string, WorkflowParameter> workflowParameters) {
            _logger.Trace("ExchangeEmailSampler running.");
            _exchange.AutodiscoverUrl(_emailAddress); //todo might be able to speed this up by not doing auto discovery

            GetEmails();

            _logger.Trace("ExchangeEmailSampler returning.");
            return new RunResult();
        }

        private void GetEmails() {
            TimeSpan ts = new TimeSpan(-1, 0, 0, 0);
            DateTime date = DateTime.Now.Add(ts);
            SearchFilter.IsGreaterThanOrEqualTo filter =
                new SearchFilter.IsGreaterThanOrEqualTo(ItemSchema.DateTimeReceived, date);
            //    new SearchFilter.IsGreaterThanOrEqualTo(EmailMessageSchema.IsRead, false);
            
            FindItemsResults<Item> findResults = _exchange.FindItems(WellKnownFolderName.Inbox, filter,
                    new ItemView(50));

                foreach (Item item in findResults) {

                    EmailMessage message = EmailMessage.Bind(_exchange, item.Id);
                }
                if (findResults.Items.Count <= 0) {

                }
        }
    }
}