﻿"use strict";

var model = namespace("FeatureDemandPlanning.Import");

model.MapTrimAction = function (params) {
    var uid = 0;
    var privateStore = {};
    var me = this;

    privateStore[me.id = uid++] = {};
    privateStore[me.id].Config = params.Configuration;
    privateStore[me.id].ActionUri = params.ModalActionUri;
    privateStore[me.id].SelectedTrimIdentifier = "";
    privateStore[me.id].SelectedTrim = "";
    privateStore[me.id].Parameters = params;

    me.action = function () {
        $("#Modal_Notify").html("").hide();
        $("#Modal_OK").html("Mapping...Wait").attr("disabled", true);
        sendData(me.getActionUri(), me.getActionParameters());
    };
    me.displaySelectedTrim = function () {
        $("#" + me.getIdentifierPrefix() + "_SelectedTrim").html(me.getSelectedTrim());
    };
    me.getActionParameters = function () {
        return $.extend({}, getData(), {
            "TrimIdentifier": me.getSelectedTrimIdentifier(),
            "ImportTrim": me.getImportTrim()
        });
    };
    me.getIdentifierPrefix = function () {
        return $("#Action_IdentifierPrefix").val();
    };
    me.getImportTrim = function () {
        return $("#" + me.getIdentifierPrefix() + "_ImportTrim").val();
    };
    me.getActionUri = function () {
        return privateStore[me.id].ActionUri;
    };
    me.getParameters = function () {
        return privateStore[me.id].Parameters;
    };
    me.getSelectedTrim = function () {
        return privateStore[me.id].SelectedTrim;
    };
    me.getSelectedTrimIdentifier = function () {
        return privateStore[me.id].SelectedTrimIdentifier;
    };
    me.trimSelectedEventHandler = function (sender) {
        me.setSelectedTrimIdentifier($(sender.target).attr("data-target"));
        me.setSelectedTrim($(sender.target).attr("data-content"));
        me.displaySelectedTrim();
    };
    me.initialise = function () {
        me.registerEvents();
        me.registerSubscribers();
        $("#Modal_OK").removeAttr("disabled").html("OK");
    };
    me.onSuccessEventHandler = function (sender, eventArgs) {
        $("#Modal_Notify")
            .removeClass("alert-danger")
            .removeClass("alert-warning")
            .addClass("alert-success")
            .html("Trim '" + me.getImportTrim() + "' mapped successfully to '" + me.getSelectedTrim() + "'")
            .show();
        $("#Modal_OK").hide();
        $("#Modal_Cancel").html("Close");
        $(document).trigger("Updated", {});
    };
    me.onErrorEventHandler = function (sender, eventArgs) {
        if (eventArgs.IsValidation) {
            $("#Modal_Notify")
                .removeClass("alert-danger")
                .removeClass("alert-success")
                .addClass("alert-warning").html(eventArgs.Message).show();
        } else {
            $("#Modal_Notify")
                .removeClass("alert-warning")
                .removeClass("alert-success")
                .addClass("alert-danger").html(eventArgs.Message).show();
        }
    };
    me.registerEvents = function () {
        var prefix = me.getIdentifierPrefix();
        $("#" + prefix + "_DerivativeList").find("a.derivative-item").on("click", function (e) {
            me.derivativeSelectedEventHandler(e);
            e.preventDefault();
        });
        $("#" + prefix + "_TrimList").find("a.trim-item").on("click", function (e) {
            me.trimSelectedEventHandler(e);
            e.preventDefault();
        });
        $("#Modal_OK").unbind("click").on("click", me.action);
        $(document)
            .unbind("Success").on("Success", function (sender, eventArgs) { $(".subscribers-notify").trigger("OnSuccessDelegate", [eventArgs]); })
            .unbind("Error").on("Error", function (sender, eventArgs) { $(".subscribers-notify").trigger("OnErrorDelegate", [eventArgs]); })
    };
    me.registerSubscribers = function () {
        $("#Modal_Notify")
            .unbind("OnSuccessDelegate").on("OnSuccessDelegate", me.onSuccessEventHandler)
            .unbind("OnErrorDelegate").on("OnErrorDelegate", me.onErrorEventHandler)
    };
    me.setParameters = function (parameters) {
        privateStore[me.id].Parameters = parameters;
    };
    me.setSelectedTrim = function (trim) {
        privateStore[me.id].SelectedTrim = trim;
    };
    me.setSelectedTrimIdentifier = function (dpck) {
        privateStore[me.id].SelectedTrimIdentifier = dpck;
    };
    function getData() {
        var params = me.getParameters();
        if (params.Data != undefined)
            return JSON.parse(params.Data)

        return {};
    };
    function sendData(uri, params) {
        $.ajax({
            "dataType": "json",
            "async": true,
            "type": "POST",
            "url": uri,
            "data": params,
            "success": function (json) {
                $(document).trigger("Success", json);
            },
            "error": function (jqXHR, textStatus, errorThrown) {
                $(document).trigger("Error", JSON.parse(jqXHR.responseText));
            }
        });
    };
}