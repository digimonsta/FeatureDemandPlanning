﻿using System;
using System.Diagnostics;
using System.Threading.Tasks;
using FeatureDemandPlanning.Bindings.Modules;
using FeatureDemandPlanning.Model.Filters;
using FeatureDemandPlanning.Model.Interfaces;
using FeatureDemandPlanning.Model.Parameters;
using FeatureDemandPlanning.Model.Validators;
using Ninject;
using Ninject.Parameters;

namespace FeatureDemandPlanning.ValidationTest
{
    public class ValidationTest
    {
        public static void Main(string[] args)
        {
            try
            {
                
                Task.Run(() =>
                {
                    var test = new ValidationTest();
                    RunAsync();
                }).Wait();
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            Console.ReadLine();
        }

        private static IKernel GetKernel()
        {
            var kernel = new StandardKernel();
            kernel.Load(new DataContextModule(), new SecurityModule(), new ControllerModule());

            return kernel;
        }

        private static async void RunAsync()
        {
            const int takeRateId = 2;
            const int marketId = 17;

            var kernel = GetKernel();
            var context = kernel.Get<IDataContext>(new ConstructorArgument("cdsId", "bweston2"));
            var p = new TakeRateParameters
            {
                TakeRateId = takeRateId,
                MarketId = marketId
            };
            var filter = TakeRateFilter.FromTakeRateParameters(p);
            var rawData = await context.TakeRate.GetRawData(filter);
            var results = Validator.Validate(rawData);
            
            foreach (var error in results.Errors)
            {
                Console.WriteLine(error.ErrorMessage);
            }

            var persistedResults = await Validator.Persist(context, filter, results);
        }
    }
}
