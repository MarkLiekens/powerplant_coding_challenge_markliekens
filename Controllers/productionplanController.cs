using log4net.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace powerplant_coding_challenge.Controllers
{
    [ApiController]
    [Route("productionplan")]
    public class ProductionPlanController : ControllerBase
    {
        private double _gasCOProduction;
        public ProductionPlanController(IConfiguration configuration)
        {
            _gasCOProduction = double.Parse(configuration["GasCO2Production"]);
        }

        [HttpPost]
        public IEnumerable<ProductionPlan> Post(Payload payload)
        {
            LoggerManager.ServiceLogger.Info("Calculating payload");

            try
            {
                return ProductionPlanCalculator.Calculate(payload, _gasCOProduction);
            }
            catch (ApplicationException ex)
            {
                LoggerManager.ServiceLogger.Warn($"A problem occurred during payload calculation: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                LoggerManager.ServiceLogger.Error("An unknown error occurred", ex);
                throw;
            }
        }
    }
}
