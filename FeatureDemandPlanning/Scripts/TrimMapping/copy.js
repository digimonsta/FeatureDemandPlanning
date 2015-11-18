﻿"use strict";

var model = namespace("FeatureDemandPlanning.Trim");

model.CopyTrimMappingAction = function (params) {
    var uid = 0;
    var privateStore = {};
    var me = this;

    privateStore[me.id = uid++] = {};
    privateStore[me.id].Config = params.Configuration;
    privateStore[me.id].ActionUri = params.ModalActionUri;
    privateStore[me.id].Parameters = params;
    privateStore[me.id].TrimId = params.TrimId;
    privateStore[me.id].TrimCode = params.TrimCode;
    privateStore[me.id].SelectedGateway = params.SelectedGateway;

    me.action = function () {
        sendData(me.getActionUri(), me.getActionParameters());
    };
    me.actionImmediate = function (params) {
        sendData(me.getActionUri(), params);
    };
    me.displaySelectedGateway = function () {
        var selectedGateway = me.getSelectedGateway();
        if (selectedGateway == "") {
            selectedGateway = "Select Gateway";
        }
        $("#" + me.getIdentifierPrefix() + "_SelectedGateway").html(selectedGateway);
    };
    me.gatewaySelectedEventHandler = function (sender) {
        me.setSelectedGateway($(sender.target).attr("data-target"));
        me.displaySelectedGateway();
    };
    me.getActionParameters = function () {
        return $.extend({}, getData(), {
            "TrimId": me.getTrimId(),
            "Gateway": me.getSelectedGateway()
        });
    };
    me.getIdentifierPrefix = function () {
        return $("#Action_IdentifierPrefix").val();
    };
    me.getActionUri = function () {
        return privateStore[me.id].ActionUri;
    };
    me.getParameters = function () {
        return privateStore[me.id].Parameters;
    };
    me.getTrimId = function () {
        return $("#" + me.getIdentifierPrefix() + "_TrimId").val();
    };
    me.getTrimCode = function () {
        return privateStore[me.id].TrimCode;
    };
    me.getSelectedGateway = function () {
        return privateStore[me.id].SelectedGateway;
    }
    me.initialise = function () {
        me.registerEvents();
        me.registerSubscribers();
    };
    me.isModalAction = function () {
        return true;
    };
    me.onSuccessEventHandler = function (sender, eventArgs) {
        $("#Modal_Notify")
            .removeClass("alert-danger")
            .removeClass("alert-warning")
            .addClass("alert-success")
            .html("Trim mapping copied successfully")
            .show();
        $("#Modal_OK").hide();
        $("#Modal_Cancel").html("Close");
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
        $("#Modal_OK").unbind("click").on("click", me.action);
        $(document)
            .unbind("Success").on("Success", function (sender, eventArgs) { $(".subscribers-notify").trigger("OnSuccessDelegate", [eventArgs]); })
            .unbind("Error").on("Error", function (sender, eventArgs) { $(".subscribers-notify").trigger("OnErrorDelegate", [eventArgs]); })

        $("#" + prefix + "_GatewayList").find("a.gateway-item").on("click", function (e) {
            me.gatewaySelectedEventHandler(e);
            e.preventDefault();
        });
    };
    me.registerSubscribers = function () {
        $("#Modal_Notify")
            .unbind("OnSuccessDelegate").on("OnSuccessDelegate", me.onSuccessEventHandler)
            .unbind("OnErrorDelegate").on("OnErrorDelegate", me.onErrorEventHandler)
    };
    me.setParameters = function (parameters) {
        privateStore[me.id].Parameters = parameters;
    };
    me.setSelectedGateway = function (gateway) {
        privateStore[me.id].SelectedGateway = gateway;
    };
    function getData() {
        var params = me.getParameters();
        if (params.Data != undefined)
            return JSON.parse(params.Data);

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
                // This is if the action succeeded, but we have trappable errors such as validation errors
                if (json.Success) {
                    $(document).trigger("Success", json);
                }
                else {
                    $(document).trigger("Error", json);
                }
            },
            "error": function (jqXHR, textStatus, errorThrown) {
                // This error handler is called if an unexpected status code is thrown from the call
                $(document).trigger("Error", JSON.parse(jqXHR.responseText));
            }
        });
    };
}