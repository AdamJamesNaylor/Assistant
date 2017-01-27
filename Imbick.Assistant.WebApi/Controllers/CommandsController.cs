﻿
namespace Imbick.Assistant.WebApi.Controllers {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Results;
    using Core;
    using Core.Commands;
    using Newtonsoft.Json;

    public class CommandsController
        : ApiController {

        public CommandsController(IState state) {
            _state = state;
        }

        [HttpGet]
        public async Task<JsonResult<IEnumerable<OutboundCommand>>> List(Guid deviceId) {
            var serialisedCommands = await _state.Get(deviceId.ToString());
            var deserialisedCommands = JsonConvert.DeserializeObject<IEnumerable<OutboundCommand>>(serialisedCommands);
            var pendingCommands = deserialisedCommands.Where(c => !c.Processed);
            return Json(pendingCommands);
        }

        [HttpPut]
        public async Task<JsonResult<OutboundCommand>> Update(OutboundCommand command) {
            return null;
        }

        private readonly IState _state;
    }
}