using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace powerplant_coding_challenge
{
    public static class ProductionPlanCalculator
    {
        private static Payload _payload;
        private static double _gasCO2Production;
        private static int _calculatedLoad;
        private static bool _loadReached;

        public static IEnumerable<ProductionPlan> Calculate(Payload payload, double gasCO2Production)
        {
            _payload = payload;
            _gasCO2Production = gasCO2Production;
            _calculatedLoad = 0;
            _loadReached = false;

            CalculateCostAndPmax();

            _payload.Powerplants = _payload.Powerplants.OrderBy(pp => pp.Cost).ToList();
            List<ProductionPlan> productionPlans = CreateProductionPlans();
            if (!_loadReached)
                throw new ApplicationException("Could not match exact load");

            productionPlans = productionPlans.OrderByDescending(ppl => ppl.P).ToList();
            return productionPlans;
        }

        private static List<ProductionPlan> CreateProductionPlans()
        {
            List<ProductionPlan> productionPlans = new List<ProductionPlan>();
            _calculatedLoad = 0;

            foreach (Powerplant powerplant in _payload.Powerplants)
            {
                ProductionPlan productionPlan = new ProductionPlan();
                productionPlan.Name = powerplant.Name;
                int requiredLoad = _payload.Load - _calculatedLoad;
                
                if (_loadReached)
                    productionPlan.P = 0;
                else if (powerplant.Pmax == 0)
                    productionPlan.P = 0;
                else
                {
                    if (_calculatedLoad + powerplant.Pmax >= _payload.Load)
                        if (_calculatedLoad + powerplant.Pmin <= _payload.Load)
                            productionPlan.P = requiredLoad;
                        else
                        {
                            _calculatedLoad -= productionPlans.Last().P;
                            productionPlans.Last().P -= powerplant.Pmin - requiredLoad;
                            _calculatedLoad += productionPlans.Last().P;

                            productionPlan.P = powerplant.Pmin;
                        }
                    else
                        productionPlan.P = powerplant.Pmax;

                    _calculatedLoad += productionPlan.P;
                    _loadReached = _calculatedLoad == _payload.Load;
                }

                productionPlans.Add(productionPlan);
            }

            return productionPlans;
        }

        private static void CalculateCostAndPmax()
        {
            foreach (Powerplant powerplant in _payload.Powerplants)
            {
                switch (powerplant.Type.ToUpper())
                {
                    case "WINDTURBINE":
                        powerplant.Cost = 0;
                        powerplant.Pmax = powerplant.Pmax * _payload.Fuels.Wind / 100;
                        break;
                    case "GASFIRED":
                        powerplant.Cost = _payload.Fuels.GasEuroMWh / powerplant.Efficiency;
                        break;
                    case "TURBOJET":
                        powerplant.Cost = _payload.Fuels.KerosineEuroMWh / powerplant.Efficiency;
                        break;
                    default:
                        throw new ApplicationException("Invalid powerplant type");
                }
            }
        }
    }
}

