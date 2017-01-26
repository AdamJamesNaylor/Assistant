
namespace Imbick.Assistant.WebApi.Controllers {
    using System.ServiceProcess;
    using System.Web.Http;

    public class DefaultController
        : ApiController {

        public DefaultController() {
            _serviceController = new ServiceController("Imbick.Assistant.Service");
        }



        private ServiceController _serviceController;
    }
}