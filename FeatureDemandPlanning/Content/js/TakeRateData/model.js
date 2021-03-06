﻿"use strict";

var model = namespace("FeatureDemandPlanning.Volume");

model.OxoVolume = function (params) {
    var uid = 0;
    var privateStore = {};
    var me = this;

    privateStore[me.id = uid++] = {};
    privateStore[me.id].OxoDocId = params.OxoDocId;
    privateStore[me.id].TakeRateId = params.TakeRateId;
    privateStore[me.id].MarketGroupId = params.MarketGroupId;
    privateStore[me.id].MarketId = params.MarketId;
    privateStore[me.id].ModalContentUri = params.ModalContentUri;
    privateStore[me.id].ModalActionUri = params.ModalActionUri;
    privateStore[me.id].Vehicle = params.Vehicle;
    privateStore[me.id].Config = params.Configuration;
    privateStore[me.id].ActionsUri = params.ActionsUri;
    privateStore[me.id].EditVolumeUri = params.EditVolumeUri;
    privateStore[me.id].GetChangesetUri = params.GetChangesetUri;
    privateStore[me.id].RevertChangesetUri = params.RevertChangesetUri;
    privateStore[me.id].SaveChangesetUri = params.SaveChangesetUri;
    privateStore[me.id].MarketUri = params.MarketUri;
    privateStore[me.id].MarketReviewUri = params.MarketReviewUri;
    privateStore[me.id].MarketReviewConfirmUri = params.MarketReviewConfirmUri;
    privateStore[me.id].PersistChangesetUri = params.PersistChangesetUri;
    privateStore[me.id].PersistChangesetConfirmUri = params.PersistChangesetConfirmUri;
    privateStore[me.id].ChangesetHistoryUri = params.ChangesetHistoryUri;
    privateStore[me.id].ChangesetHistoryDetailsUri = params.ChangesetHistoryDetailsUri;
    privateStore[me.id].ValidationSummaryUri = params.ValidationSummaryUri;
    privateStore[me.id].FilterUri = params.FilterUri;
    privateStore[me.id].PowertrainUri = params.PowertrainUri;
    privateStore[me.id].UndoChangesetUri = params.UndoChangesetUri;
    privateStore[me.id].UndoAllChangesetUri = params.UndoAllChangesetUri;
    privateStore[me.id].UpdateFilteredDataUri = params.UpdateFilteredDataUri;
    privateStore[me.id].GetValidationUri = params.GetValidationUri;
    privateStore[me.id].ValidateUri = params.ValidateUri;
    privateStore[me.id].ValidationMessageUri = params.ValidationMessageUri;
    privateStore[me.id].AddNoteUri = params.AddNoteUri;
    privateStore[me.id].RefreshNotesUri = params.RefreshNotesUri;
    privateStore[me.id].FeatureDataUri = params.FeatureDataUri;
    privateStore[me.id].FeatureApplicabilityDataUri = params.FeatureApplicabilityDataUri;
    privateStore[me.id].ModelDataUri = params.ModelDataUri;
    privateStore[me.id].IsValid = true;
    privateStore[me.id].FdpVolumeHeaders = [];
    privateStore[me.id].CurrentEditValue = null;
    privateStore[me.id].IsImportCompleted = params.IsCompleted;
    privateStore[me.id].HasValidationErrors = false;
    privateStore[me.id].HasChanges = false;
    privateStore[me.id].Parameters = params;

    me.ModelName = "OxoVolume";

    me.initialise = function () {
        $(document).trigger("notifySuccess", me);
    };
    me.getActionsUri = function () {
        return privateStore[me.id].ActionsUri;
    };
    me.getActionModel = function (action) {
        var actionModel = null;
        switch (action) {
            case 4:
                actionModel = new FeatureDemandPlanning.Volume.DetailsAction(me.getParameters());
                break;
            case 6:
                actionModel = new FeatureDemandPlanning.Volume.SaveAction(me.getParameters());
                break;
            case 7:
                actionModel = new FeatureDemandPlanning.Volume.HistoryAction(me.getParameters());
                break;
            case 12:
                actionModel = new FeatureDemandPlanning.Volume.FilterAction(me.getParameters());
                break;
            case 13:
                actionModel = new FeatureDemandPlanning.Volume.ValidationSummaryAction(me.getParameters());
                break;
            case 15:
                actionModel = new FeatureDemandPlanning.Volume.PowertrainAction(me.getParameters());
                break;
            case 16:
                actionModel = new FeatureDemandPlanning.Volume.HistoryAction(me.getParameters());
            default:
                break;
        }
        return actionModel;
    };
    me.getActionContentUri = function (action) {
        return privateStore[me.id].ModalContentUri;
    };
    me.isImportCompleted = function() {
        return privateStore[me.id].IsImportCompleted;
    }
    me.getTakeRateId = function() {
        return privateStore[me.id].TakeRateId;
    }
    me.getOxoDocId = function () {
        return privateStore[me.id].OxoDocId;
    };
    me.getMarketGroupId = function () {
        return privateStore[me.id].MarketGroupId;
    };
    me.getMarketId = function () {
        return privateStore[me.id].MarketId;
    };
    me.getParameters = function () {
        return privateStore[me.id].Parameters;
    };
    me.getVehicle = function () {
        return privateStore[me.id].Vehicle;
    }
    me.getConfiguration = function () {
        return privateStore[me.id].Configuration;
    };
    me.getEditVolumeUri = function () {
        return privateStore[me.id].EditVolumeUri;
    };
    me.getChangesetUri = function () {
        return privateStore[me.id].GetChangesetUri;
    };
    me.getRevertChangsetUri = function () {
        return privateStore[me.id].RevertChangesetUri;
    };
    me.getSaveChangesetUri = function () {
        return privateStore[me.id].SaveChangesetUri;
    };
    me.getPersistChangesetUri = function () {
        return privateStore[me.id].PersistChangesetUri;
    };
    me.getPersistChangesetAction = function() {
        return 6; // Save changes
    }
    me.getChangesetHistoryAction = function() {
        return 7;
    };
    me.getValidationSummaryAction = function() {
        return 13;
    };
    me.getPowertrainAction = function() {
        return 15;
    };
    me.getChangesetHistoryDetailsAction = function() {
        return 16;
    };
    me.getMarketUri = function () {
        return privateStore[me.id].MarketUri;
    };
    me.getMarketReviewUri = function() {
        return privateStore[me.id].MarketReviewUri;
    };
    me.getPersistChangesetConfirmUri = function () {
        return privateStore[me.id].PersistChangesetConfirmUri;
    };
    me.getChangesetHistoryUri = function() {
        return privateStore[me.id].ChangesetHistoryUri;
    };
    me.getChangesetHistoryDetailsUri = function() {
        return privateStore[me.id].ChangesetHistoryDetailsUri;
    };
    me.getPowertrainUri = function() {
        return privateStore[me.id].PowertrainUri;
    };
    me.getValidationSummaryUri = function() {
        return privateStore[me.id].ValidationSummaryUri;
    };
    me.getFilterUri = function() {
        return privateStore[me.id].FilterUri;
    };
    me.getUndoChangesetUri = function() {
        return privateStore[me.id].UndoChangesetUri;
    };
    me.getUndoAllChangesetUri = function () {
        return privateStore[me.id].UndoAllChangesetUri;
    };
    me.getUpdateFilteredDataUri = function () {
        return privateStore[me.id].UpdateFilteredDataUri;
    };
    me.getValidationUri = function() {
        return privateStore[me.id].GetValidationUri;
    };
    me.getFeatureDataUri = function () {
        return privateStore[me.id].FeatureDataUri;
    };
    me.getFeatureApplicabilityDataUri = function () {
        return privateStore[me.id].FeatureApplicabilityDataUri;
    };
    me.getModelDataUri = function () {
        return privateStore[me.id].ModelDataUri;
    };
    me.getValidateUri = function () {
        return privateStore[me.id].ValidateUri;
    };
    me.getValidationMessageUri = function () {
        return privateStore[me.id].ValidationMessageUri;
    };
    me.getCurrentEditValue = function () {
        return privateStore[me.id].CurrentEditValue;
    };
    me.setCurrentEditValue = function (value) {
        privateStore[me.id].CurrentEditValue = value;
    };
    me.setDocument = function (oxoDocument) {
        privateStore[me.id].Document = oxoDocument;
    };
    me.setVehicle = function (vehicle) {
        privateStore[me.id].Vehicle = vehicle;
    };
    me.isValid = function () {
        return privateStore[me.id].IsValid;
    };
    me.getVolume = function () {
        return {
            Document: me.getDocument(),
            Vehicle: me.getVehicle(),
            FdpVolumeHeaders: me.getFdpVolumeHeaders()
        }
    };
    me.loadChangeset = function (callback) {
        var params = getFilter();
        $.ajax({
            "dataType": "json",
            "async": true,
            "type": "POST",
            "url": me.getChangesetUri(),
            "data": params,
            "success": function (response) {
                callback(response);
            },
            "error": function (response) {
                genericErrorCallback(response);
            }
        });
    };
    me.loadValidation = function(callback) {
        var params = getFilter();
        $.ajax({
            "dataType": "json",
            "async": true,
            "type": "POST",
            "url": me.getValidationUri(),
            "data": params,
            "success": function (response) {
                callback(response);
            },
            "error": function (response) {
                genericErrorCallback(response);
            }
        });
    };
    me.loadFeatureData = function(callback) {
        var params = getFilter();
        $.ajax({
            "dataType": "json",
            "async": true,
            "type": "POST",
            "url": me.getFeatureDataUri(),
            "data": params,
            "success": function(response) {
                callback(response);
            },
            "error": function(response) {
                genericErrorCallback(response);
            }
        });
    };
    me.loadFeatureApplicabilityData = function(callback) {
        var params = getFilter();
        $.ajax({
            "dataType": "json",
            "async": true,
            "type": "POST",
            "url": me.getFeatureApplicabilityDataUri(),
            "data": params,
            "success": function(response) {
                callback(response);
            },
            "error": function(response) {
                genericErrorCallback(response);
            }
        });
    };
    me.loadModelData = function (callback) {
        var params = getFilter();
        $.ajax({
            "dataType": "json",
            "async": true,
            "type": "POST",
            "url": me.getModelDataUri(),
            "data": params,
            "success": function (response) {
                callback(response);
            },
            "error": function (response) {
                genericErrorCallback(response);
            }
        });
    }
    me.marketReview = function(callback) {
        var params = getFilter();
        $.ajax({
            "dataType": "json",
            "method": "POST",
            "async": true,
            "url": me.getMarketReviewUri(),
            "data": params,
            "success": function (json) {
                $(document).trigger("Success", json);
                callback(json);
            },
            "error": function (response) {
                genericErrorCallback(response);
            }
        });
    }
    me.revertChangeset = function (callback) {
        var params = getFilter();
        $.ajax({
            "dataType": "json",
            "async": true,
            "type": "POST",
            "url": me.getRevertChangesetUri(),
            "data": params,
            "success": function (response) {
                callback(response);
            },
            "error": function (response) {
                genericErrorCallback(response);
            }
        });
    };
    me.saveData = function (changesToSave, callback) {
        var params = getFilter();
        params.Changeset = changesToSave;
        $.ajax({
            "dataType": "json",
            "async": true,
            "type": "POST",
            "url": me.getSaveChangesetUri(),
            "data": params,
            "success": function (json) {
                //$(document).trigger("Success", json);
                callback();
            },
            "error": function (response) {
                genericErrorCallback(response);
            }
        });
    };
    me.persistData = function (changesToPersist, callback) {
        var params = getFilter();
        params.Changeset = changesToPersist;
        $.ajax({
            "dataType": "json",
            "async": true,
            "type": "POST",
            "url": me.getPersistChangesetUri(),
            "data": params,
            "success": function (json) {
                $(document).trigger("Success", json);
                callback();
            },
            "error": function (response) {
                genericErrorCallback(response);
            }
        });
    };
    me.undoData = function(changesToUndo, callback) {
        var params = getFilter();
        params.Changeset = changesToUndo;
        $.ajax({
            "dataType": "json",
            "async": true,
            "type": "POST",
            "url": me.getUndoChangesetUri(),
            "data": params,
            "success": function(json) {
                $(document).trigger("Success", json);
                callback(json.Data);
            },
            "error": function(response) {
                genericErrorCallback(response);
            }
        });
    };
    me.undoAllData = function (changesToUndo, callback) {
        var params = getFilter();
        params.Changeset = changesToUndo;
        $.ajax({
            "dataType": "json",
            "async": true,
            "type": "POST",
            "url": me.getUndoAllChangesetUri(),
            "data": params,
            "success": function (json) {
                $(document).trigger("Success", json);
                callback(json.Data);
            },
            "error": function (response) {
                genericErrorCallback(response);
            }
        });
    }
    me.validate = function (callback) {
        var params = getFilter();
        $(document).trigger("BeforeValidation", volume);

        $.ajax({
            "dataType": "json",
            "method": "POST",
            "async": true,
            "url": me.getValidateUri(),
            "data": params,
            "success": function (json) {
                $(document).trigger("Success", json);
                callback(json);
            },
            "error": function (response) {
                genericErrorCallback(response);
            }
        });
    };
    me.getOxoDocuments = function () {
        return privateStore[me.id].OxoDocuments;
    };
    me.getFdpVolumeHeader = function (fdpVolumeHeaderId) {
        return {
            FdpVolumeHeaderId: fdpVolumeHeaderId
        };
    };
    me.getFdpVolumeHeaders = function () {
        return privateStore[me.id].FdpVolumeHeaders;
    };
    me.addFdpVolumeHeader = function (fdpVolumeHeaderId) {
        if (me.hasFdpVolumeHeader(fdpVolumeHeaderId))
            return;

        me.getFdpVolumeHeaders().push(me.getFdpVolumeHeader(fdpVolumeHeaderId));
    };
    me.clearFdpVolumeHeaders = function () {
        privateStore[me.id].FdpVolumeHeaders = [];
    };
    me.removeFdpVolumeHeader = function (fdpVolumeHeaderId) {
        if (!me.hasFdpVolumeHeader(fdpVolumeHeaderId))
            return;

        var i = 0;
        $(me.getFdpVolumeHeaders()).each(function () {
            if (this.FdpVolumeHeaderId === fdpVolumeHeaderId) {
                return false;
            }
            i++;
        });
        me.getFdpVolumeHeaders().splice(i, 1);
    };
    me.hasFdpVolumeHeader = function (fdpVolumeHeaderId) {
        var exists = false;
        $(me.getFdpVolumeHeaders()).each(function () {
            if (this.FdpVolumeHeaderId === fdpVolumeHeaderId) {
                exists = true;
                return false;
            }
        });
        return exists;
    };
    me.HasValidationErrors = function() {
        return privateStore[me.id].HasValidationErrors;
    };
    me.HasChanges = function() {
        return privateStore[me.id].HasChanges;
    }
    me.setHasValidationErrors = function(hasValidationErrors) {
        privateStore[me.id].HasValidationErrors = hasValidationErrors;
    };
    me.setHasChanges = function(hasChanges) {
        privateStore[me.id].HasChanges = hasChanges;
    }
    function genericErrorCallback(response) {
        if (response.status === 400) {
            var json = JSON.parse(response.responseText);
            privateStore[me.id].IsValid = false;
            $(document).trigger("Validation", [json]);
        } else {
            var json = JSON.parse(response.responseText);
            privateStore[me.id].IsValid = false;
            $(document).trigger("Error", [json]);
        }
    };
    function getFilter() {
        return {
            TakeRateId: me.getTakeRateId(),
            DocumentId: me.getOxoDocId(),
            MarketId: me.getMarketId(),
            Changeset: null
        }
    };
}

