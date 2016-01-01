﻿using FeatureDemandPlanning.Model;
using FeatureDemandPlanning.Model.Filters;
using FeatureDemandPlanning.Model.Validators;
using FeatureDemandPlanning.Model.Enumerations;
using FeatureDemandPlanning.Model.Interfaces;
using FeatureDemandPlanning.Model.ViewModel;
using FluentValidation;
using System.Linq;
using System.Web.Mvc;
using System.Threading.Tasks;
using FeatureDemandPlanning.Model.Parameters;
using FeatureDemandPlanning.Model.Attributes;
using System.Web.Script.Serialization;
using MvcSiteMapProvider.Web.Mvc.Filters;
using System;
using FeatureDemandPlanning.Model.Results;
using System.Collections.Generic;
using FeatureDemandPlanning.Model.Empty;

namespace FeatureDemandPlanning.Controllers
{
    /// <summary>
    /// Primary controller for handling viewing / editing and updating of take rate information
    /// </summary>
    public class TakeRateDataController : ControllerBase
    {
        #region "Constructors"

        public TakeRateDataController() : base()
        {
            ControllerType = ControllerType.SectionChild;
        }

        #endregion

        [HttpGet]
        [ActionName("Index")]
        public ActionResult TakeRatePage(int? oxoDocId,
                                         int? marketGroupId,
                                         int? marketId,
                                         TakeRateResultMode resultsMode = TakeRateResultMode.PercentageTakeRate)
        {
            return RedirectToAction("TakeRateDataPage", new TakeRateParameters()
            {
                TakeRateId = oxoDocId,
                MarketGroupId = marketGroupId,
                MarketId = marketId,
                Mode = resultsMode
            });
        }
        [HttpGet]
        [SiteMapTitle("DocumentName")]
        public async Task<ActionResult> TakeRateDataPage(TakeRateParameters parameters)
        {
            var filter = TakeRateFilter.FromTakeRateParameters(parameters);
            filter.Action = TakeRateDataItemAction.TakeRateDataPage;
            var model = await TakeRateViewModel.GetModel(DataContext, filter);

            ViewData["DocumentName"] = model.Document.UnderlyingOxoDocument.Name;
            ViewBag.Title = string.Format("{0} - {1} ({2}) - {3}", model.Document.Vehicle.Code,
                model.Document.Vehicle.ModelYear, model.Document.UnderlyingOxoDocument.Gateway, model.Document.TakeRateSummary.First().Version);

            return View("TakeRateDataPage", model);
        }
        [HttpPost]
        public async Task<ActionResult> ContextMenu(TakeRateParameters parameters)
        {
            TakeRateParametersValidator
                .ValidateTakeRateParameters(DataContext, parameters, TakeRateParametersValidator.TakeRateIdentifier);

            var filter = TakeRateFilter.FromTakeRateParameters(parameters);
            filter.Action = TakeRateDataItemAction.TakeRateDataItemDetails;
            var takeRateView = await TakeRateViewModel.GetModel(
                DataContext,
                filter);

            return PartialView("_ContextMenu", takeRateView);
        }
        [HttpPost]
        [HandleError(View = "_ModalError")]
        public async Task<ActionResult> ModalContent(TakeRateParameters parameters)
        {
            TakeRateParametersValidator
                .ValidateTakeRateParameters(DataContext, parameters, TakeRateParametersValidator.TakeRateIdentifier);

            var takeRateView = await GetModelFromParameters(parameters);

            return PartialView(GetContentPartialViewName(parameters.Action), takeRateView);
        }
        [HttpPost]
        [HandleErrorWithJson]
        public ActionResult ModalAction(TakeRateParameters parameters)
        {
            TakeRateParametersValidator
                .ValidateTakeRateParameters(DataContext, parameters, TakeRateParametersValidator.TakeRateIdentifier);
            TakeRateParametersValidator
                .ValidateTakeRateParameters(DataContext, parameters, Enum.GetName(parameters.Action.GetType(), parameters.Action));

            return RedirectToAction(Enum.GetName(parameters.Action.GetType(), parameters.Action), parameters.GetActionSpecificParameters());
        }
        [HandleErrorWithJson]
        [HttpPost]
        public async Task<ActionResult> SaveChangeset(TakeRateParameters parameters)
        {
            TakeRateParametersValidator
                .ValidateTakeRateParameters(DataContext, parameters, TakeRateParametersValidator.TakeRateIdentifierWithChangeset);

            var savedChangeset = await DataContext.TakeRate.SaveChangeset(TakeRateFilter.FromTakeRateParameters(parameters), parameters.Changeset);

            return Json(savedChangeset);
        }
        [HandleErrorWithJson]
        [HttpPost]
        public async Task<ActionResult> GetLatestChangeset(TakeRateParameters parameters)
        {
            TakeRateParametersValidator
                .ValidateTakeRateParameters(DataContext, parameters, TakeRateParametersValidator.TakeRateIdentifier);

            var changeset = await DataContext.TakeRate.GetUnsavedChangesForUser(TakeRateFilter.FromTakeRateParameters(parameters));

            return Json(changeset);
        }
        [HandleErrorWithJson]
        [HttpPost]
        public async Task<ActionResult> RevertLatestChangeset(TakeRateParameters parameters)
        {
            TakeRateParametersValidator
                .ValidateTakeRateParameters(DataContext, parameters, TakeRateParametersValidator.TakeRateIdentifier);

            var changeset = await DataContext.TakeRate.RevertUnsavedChangesForUser(TakeRateFilter.FromTakeRateParameters(parameters));

            return Json(changeset);
        }
        [HandleErrorWithJson]
        [HttpPost]
        public async Task<ActionResult> PersistChangeset(TakeRateParameters parameters)
        {
            TakeRateParametersValidator
               .ValidateTakeRateParameters(DataContext, parameters, TakeRateParametersValidator.TakeRateIdentifierWithChangeset);

            var persistedChangeset = await DataContext.TakeRate.PersistChangeset(TakeRateFilter.FromTakeRateParameters(parameters));

            return Json(persistedChangeset);
        }
        //[HttpPost]
        //public async Task<ActionResult> Validate(TakeRateParameters parameters,
        //                                         TakeRateDocumentValidationSection sectionToValidate = TakeRateDocumentValidationSection.All)
        //{
        //    //var volumeModel = TakeRateViewModel.GetModel(DataContext, volumeToValidate).Result;
        //    //var validator = new VolumeValidator(volumeModel.Volume);
        //    //var ruleSets = VolumeValidator.GetRulesetsToValidate(sectionToValidate);
        //    //var jsonResult = new JsonResult()
        //    //{
        //    //    Data = new { IsValid = true }
        //    //};

        //    //var results = validator.Validate(volumeModel.Volume, ruleSet: ruleSets);
        //    //if (results.IsValid) return jsonResult;
        //    //var errorModel = results.Errors
        //    //    .Select(e => new ValidationError(new ValidationErrorItem
        //    //    {
        //    //        ErrorMessage = e.ErrorMessage,
        //    //        CustomState = e.CustomState
        //    //    })
        //    //    {
        //    //        key = e.PropertyName
        //    //    });

        //    //jsonResult = new JsonResult()
        //    //{
        //    //    Data = new ValidationMessage(false, errorModel)
        //    //};
        //    //return jsonResult;

        //    ValidateTakeRateParameters(parameters, TakeRateParametersValidator.NoValidation);

        //    var filter = new TakeRateFilter()
        //    {
        //        FilterMessage = parameters.FilterMessage,
        //        TakeRateStatusId = parameters.TakeRateStatusId
        //    };
        //    filter.InitialiseFromJson(parameters);

        //    var results = await TakeRateViewModel.GetModel(DataContext, filter);
        //    var jQueryResult = new JQueryDataTableResultModel(results);

        //    foreach (var result in results.TakeRates.CurrentPage)
        //    {
        //        jQueryResult.aaData.Add(result.ToJQueryDataTableResult());
        //    }

        //    return Json(jQueryResult);
        //}
        public ActionResult ValidationMessage(ValidationMessage message)
        {
            // Something is making a GET request to this page and I can't figure out what
            return PartialView("_ValidationMessage", message);
        }

        #region "Private Methods"

        private async Task<TakeRateViewModel> GetModelFromParameters(TakeRateParameters parameters)
        {
            return await TakeRateViewModel.GetModel(
                DataContext,
                TakeRateFilter.FromTakeRateParameters(parameters));
        }
        private void ProcessTakeRateData(ITakeRateDocument document)
        {
            DataContext.TakeRate.SaveTakeRateDocument(document);
            DataContext.TakeRate.ProcessMappedData(document);
        }
        //private static void ValidateTakeRateParameters(TakeRateParameters parameters, string ruleSetName)
        //{
        //    var validator = new TakeRateParametersValidator(DataContext);
        //    var result = validator.Validate(parameters, ruleSet: ruleSetName);
        //    if (!result.IsValid)
        //    {
        //        throw new ValidationException(result.Errors);
        //    }
        //}

        #endregion
    }
}