var isSecondaryStyle = false;
var reloadSchedules = false;

$('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
    hideTooltips();
});
//General
$("#GeneralSettings").on('change', '#AddLogoFileInput', function () { uploadFile(); });

function uploadFile() {
    $('#AddLogo').submit();
}

$('#GeneralSettings').on("submit", "#AddLogo", function (e) {
    $.ajax({
        url: '/Settings/AddLogo',
        type: 'POST',
        data: new FormData(this),
        processData: false,
        contentType: false,
        success: function (response) {
            if (response.success !== false) {
                $("#GeneralLogos").find(".selected").removeClass("selected");
                $("#GeneralLogos").prepend('<img src="' + response.imgUrl + '" class="selected" />');
                $("#LogoWrapper").html('<img src="' + response.imgUrl + '" />');
                $("#Logo").val(response.imgUrl);
            }
        },
        error: function () {

        }
    });
    e.preventDefault();
});

$("#GeneralSettings").on("submit", "#CompanyInfoForm", function (e) {
    var $btn = $("#CompanyInfoSaveBtn").button('loading');
    $.ajax({
        url: '/Settings/CompanyInfo',
        type: 'POST',
        data: $("#CompanyInfoForm").serialize(),
        success: function (response) {
            $btn.button('reset');
            $("#GeneralSettings").html(response);
            checkCustomMessages();

        },
        error: function () {
            $btn.button('reset');
        }
    });
    e.preventDefault();
});

$("#GeneralSettings").on("click", "#GeneralLogos img", function () {
    $("#GeneralLogos").find(".selected").removeClass("selected");
    $(this).addClass("selected");
    var src = $(this).attr("src");
    $("#LogoWrapper").html('<img src="' + src + '" />');
    $("#Logo").val(src);
});

$("#GeneralSettings").on("submit", "#GlobalChatSettingsForm", function (e) {
    var $btn = $("#GlobalChatSaveBtn").button('loading');
    $.ajax({
        url: '/Settings/GlobalChatSettings',
        type: 'POST',
        data: $("#GlobalChatSettingsForm").serialize(),
        success: function (response) {
            $btn.button('reset');
            $("#GeneralSettings").html(response);
            checkCustomMessages();
        },
        error: function () {
            $btn.button('reset');
        }
    });
    e.preventDefault();
});


//UseCustomSystemMessages
$("#SettingsPage").on("change", "#UseCustomSystemMessages", function () {
    checkCustomMessages();
});

function checkCustomMessages() {
    var useCustomMsgs = $("#UseCustomSystemMessages").is(':checked');
    if (useCustomMsgs) {
        getCustomSystemMessages();
    } else {
        $("#CustomSystemMessages").html("");
    }
}

function getCustomSystemMessages() {
    $.get("/Settings/GetCustomSystemMessages", function (response) {
        if (response !== '' && response.success !== false) {
            $("#CustomSystemMessages").html(response);
        }
    });
}

checkCustomMessages();

$("#GeneralSettings").on("submit", "#CustomSystemMessagesForm", function (e) {
    var $btn = $("#CustomMessageSaveBtn").button('loading');
    $.ajax({
        url: '/Settings/UpdateCustomSystemMessages',
        type: 'POST',
        data: $("#CustomSystemMessagesForm").serialize(),
        success: function (response) {
            $btn.button('reset');
            if (response !== '' && response.success !== false) {

            } else {
                alert("Error saving custom messages.");
            }
        },
        error: function () {
            $btn.button('reset');
            alert("Error saving custom messages.");
        }
    });
    e.preventDefault();
});


//Workgroups
$("#WorkgroupSettings").on("click", ".toggle-workgroup-assignable", function () {
    hideTooltips();
    var id = $(this).data("id");
    $.ajax({
        url: '/Settings/ToggleWorkgroupAssignable',
        type: 'POST',
        data: { id: id },
        success: function (response) {
            if (response.success !== false) {
                $("#WorkgroupSettings").find(".assignable-wrapper").html(response);
            } else {
                alert("Unable to toggle Workgroup Assignable");
            }
        },
        error: function () {
            alert("Unable to toggle Workgroup Assignable");
        }
    });
});

$("#WorkgroupSettings").on("click", ".collapse-arrow", function (e) {
    $("#WorkgroupSettings .assignable-wrapper").find(".settings-tab").toggle();
    var arrow = $(this);
    arrow.fadeOut("slow", function () {
        arrow.toggleClass("fa-chevron-circle-down").toggleClass("fa-chevron-circle-up");
    });
    arrow.fadeIn("slow");
});

//Skills
$("#SkillSettings").on("click", ".toggle-skill-assignable", function () {
    hideTooltips();
    var id = $(this).data("id");
    $.ajax({
        url: '/Settings/ToggleSkillAssignable',
        type: 'POST',
        data: { id: id },
        success: function (response) {
            if (response.success !== false) {
                $("#SkillSettings").find(".assignable-wrapper").html(response);
            } else {
                alert("Unable to toggle Skill Assignable");
            }
        },
        error: function () {
            alert("Unable to toggle Skill Assignable");
        }
    });
});

$("#SkillSettings").on("click", ".collapse-arrow", function () {
    $("#SkillSettings .assignable-wrapper").find(".settings-tab").toggle();
    var arrow = $(this);
    arrow.fadeOut("slow", function () {
        arrow.toggleClass("fa-chevron-circle-down").toggleClass("fa-chevron-circle-up");
    });
    arrow.fadeIn("slow");
});

//Schedules
$("#ScheduleSettings").on("click", ".toggle-schedule-assignable", function () {
    hideTooltips();
    var id = $(this).data("id");
    $.ajax({
        url: '/Settings/ToggleScheduleAssignable',
        type: 'POST',
        data: { id: id },
        success: function (response) {
            if (response.success !== false) {
                $("#ScheduleSettings").html(response);
            } else {
                alert("Unable to toggle Schedule Assignable");
            }
        },
        error: function () {
            alert("Unable to toggle Schedule Assignable");
        }
    });
});

$("#SettingsModal").on("click", "#SaveScheduleBtn", function () {
    var $btn = $(this).button('loading');
    var formData = $("#EditScheduleForm").serialize();
    $.ajax({
        url: '/Settings/EditSchedule',
        type: 'POST',
        data: formData,
        success: function (response) {
            $btn.button('reset');
            if (response !== '' && response.success !== true) {
                $("#SettingsModalBody").html(response);
            } else {
                $("#SettingsModalBody").html("Schedule updated successfully.");
                reloadSchedules = true;
            }
        },
        error: function () {
            $btn.button('reset');
            alert("Error Editing Schedule.");
        }
    });
});


$(window).resize(function () {
    setModalHeight();
});

setModalHeight();

function setModalHeight() {
    $("#SettingsModalBody").css("max-height", (window.innerHeight * .75));
}

function loadColorsAndTooltips() {
    $(".color-picker").each(function () {
        var colorPicker = $(this);
        colorPicker.spectrum({
            appendTo: "#SettingsModalBody",
            clickoutFiresChange: true,
            preferredFormat: "hex",
            showButtons: false,
            showPalette: true,
            showInput: true
        });
    });
    loadTooltips();
}

$("#SettingsModalBody").scroll(function () {
    hideTooltips();
    $("#ToggleTooltips").tooltip('hide');
});

$('#SettingsModal').on('shown.bs.modal', function (event) {
    $("#SettingsModalBody").scrollTop(0);
});

$("#SettingsModalBody").html($("#Loading").html());

$('#SettingsModal').on('show.bs.modal', function (event) {
    hideTooltips();
    var button = $(event.relatedTarget); // Button that triggered the modal
    var title = button.data("modal-title");
    var type = button.data("modal-type");
    if (type === "createtemplate") {
        $.get("/Settings/CreateTemplate", function (response) {
            if (response !== '' && response.success !== false) {
                $("#SettingsModalBody").html(response);
                loadColorsAndTooltips();
            }
        });
    }
    if (type === "edittemplate") {
        var id = button.data("id");
        $.get("/Settings/EditTemplate", { templateId: id }, function (response) {
            if (response !== '' && response.success !== false) {
                $("#SettingsModalBody").html(response);
                loadColorsAndTooltips();
            } else {
                $("#SettingsModalBody").html("Error loading template.");
            }
        });
    }
    if (type === "createwidget") {
        $.get("/Settings/CreateWidget", function (response) {
            if (response !== '' && response.success !== false) {
                $("#SettingsModalBody").html(response);
                loadColorsAndTooltips();
                checkSecondaryStyle();
            }
        });
    }
    if (type === "editwidget") {
        var id = button.data("id");
        $.get("/Settings/EditWidget", { widgetId: id }, function (response) {
            if (response !== '' && response.success !== false) {
                $("#SettingsModalBody").html(response);
                loadColorsAndTooltips();
                checkSecondaryStyle();
            } else {
                $("#SettingsModalBody").html("Error loading widget.");
            }
        });
    }
    if (type === "createform") {
        $.get("/Settings/CreateForm", function (response) {
            if (response !== '' && response.success !== false) {
                $("#SettingsModalBody").html(response);
                loadColorsAndTooltips();
                getFormFields();
            }
        });
    }
    if (type === "editform") {
        var id = button.data("id");
        $.get("/Settings/EditForm", { formId: id }, function (response) {
            if (response !== '' && response.success !== false) {
                $("#SettingsModalBody").html(response);
                loadColorsAndTooltips();
                getFormFields();
            } else {
                $("#SettingsModalBody").html("Error loading Form.");
            }
        });
    }
    if (type === "editschedule") {
        var id = button.data("id");
        $.get("/Settings/EditSchedule", { id: id }, function (response) {
            if (response !== '' && response.success !== false) {
                $("#SettingsModalBody").html(response);
            } else {
                $("#SettingsModalBody").html("Error loading widget.");
            }
        });
    }
    $("#SettingsModalTitle").text(title);
});

$('#SettingsModal').on('hidden.bs.modal', function () {
    $("#SettingsModalTitle").text("");
    $("#SettingsModalBody").html($("#Loading").html());

    if (reloadSchedules) {
        $.get("/Settings/ReloadSchedules", function (response) {
            if (response !== '' && response.success !== false) {
                $("#ScheduleSettings").html(response);
            }
        });
        reloadSchedules = false;
    }
    $.get("/Settings/ReloadTemplates", function (response) {
        if (response !== '' && response.success !== false) {
            $("#TemplateSettings").html(response);
        }
    });
    $.get("/Settings/ReloadWidgets", function (response) {
        if (response !== '' && response.success !== false) {
            $("#WidgetSettings").html(response);
        }
    });
    if ($("#wcb-webchat").length) {
        $("#wcb-webchat").remove();
    }
});

$("#SettingsModal").on("click", "#LogoSelectBox img", function () {
    $("#LogoSelectBox").find(".selected").removeClass("selected");
    $(this).addClass("selected");
    var src = $(this).attr("src");
    $("#HeaderLogoPath").val(src);
});

$("#SettingsModal").on("click", "#SendIconSelectBox img", function () {
    $("#SendIconSelectBox").find(".selected").removeClass("selected");
    $(this).addClass("selected");
    var src = $(this).attr("src");
    $("#SendButtonIcon").val(src);
});

$("#SettingsModal").on("click", "#CloseIconSelectBox img", function () {
    $("#CloseIconSelectBox").find(".selected").removeClass("selected");
    $(this).addClass("selected");
    var src = $(this).attr("src");
    $("#CloseButtonIcon").val(src);
});

$("#SettingsModal").on("click", "#DisconnectIconSelectBox img", function () {
    $("#DisconnectIconSelectBox").find(".selected").removeClass("selected");
    $(this).addClass("selected");
    var src = $(this).attr("src");
    $("#DisconnectButtonIcon").val(src);
});

$("#SettingsModal").on("click", "#PrintIconSelectBox img", function () {
    $("#PrintIconSelectBox").find(".selected").removeClass("selected");
    $(this).addClass("selected");
    var src = $(this).attr("src");
    $("#PrintButtonIcon").val(src);
});

$("#SettingsModal").on("click", "#EmailIconSelectBox img", function () {
    $("#EmailIconSelectBox").find(".selected").removeClass("selected");
    $(this).addClass("selected");
    var src = $(this).attr("src");
    $("#EmailButtonIcon").val(src);
});

$("#SettingsModal").on("click", "#ClearPrintSelection", function () {
    $("#PrintIconSelectBox").find(".selected").removeClass("selected");
    $("#PrintButtonIcon").val("");
});

$("#SettingsModal").on("click", "#ClearEmailSelection", function () {
    $("#EmailIconSelectBox").find(".selected").removeClass("selected");
    $("#EmailButtonIcon").val("");
});

//Widget Select Icon
$("#SettingsModal").on("click", "#IconSelectBox img", function () {
    $("#IconSelectBox").find(".selected").removeClass("selected");
    $(this).addClass("selected");
    var src = $(this).attr("src");
    $("#IconPath").val(src);
});

//Widget Unavailable Select Icon
$("#SettingsModal").on("click", "#UnavailableIconSelectBox img", function () {
    $("#UnavailableIconSelectBox").find(".selected").removeClass("selected");
    $(this).addClass("selected");
    var src = $(this).attr("src");
    $("#UnavailableIconPath").val(src);
});

//Widget Resume Select Icon
$("#SettingsModal").on("click", "#ResumeIconSelectBox img", function () {
    $("#ResumeIconSelectBox").find(".selected").removeClass("selected");
    $(this).addClass("selected");
    var src = $(this).attr("src");
    $("#ResumeIconPath").val(src);
});

$("#SettingsModal").on("change", "#UnavailableFormId", function() {
    if ($(this).val() === "None") {
        $("#ExtendedUnavailableOptions").hide();
    } else {
        $("#ExtendedUnavailableOptions").show();
    }
});

//SendIncludeIcon
$("#SettingsModal").on("change", "#SendIncludeIcon", function () {
    $("#SendIconSelectBox").toggle();
    $("#SendIconSelectLabel").toggle();
});

//Widget UseIcon
$("#SettingsModal").on("change", "#UseIcon", function () {
    if (!isSecondaryStyle) {
        $("#IconSettingsWrapper").toggle();
        $("#CustomLaunchWrapper").toggle();
        $("#UnavailableIconSelectWrapper").toggle();
        $("#UnavailableLaunchText").toggle();
        $("#ResumeIconSelectWrapper").toggle();
        $("#ResumeLaunchText").toggle();
    }
});

////Widget ShowOnMobile
//$("#SettingsModal").on("change", "#ShowOnMobile", function () {
//    $("#MobileWidth").toggle();
//});

//Widget CheckForAgents
$("#SettingsModal").on("change", "#CheckForAgents", function () {
    $("#AgentEwtWrapper").toggle();
});

//Widget ShowTooltip
$("#SettingsModal").on("change", "#ShowTooltip", function () {
    $("#TooltipSettingWrapper").toggle();
    $("#UnavailableTooltip").toggle();
    $("#ResumeTooltip").toggle();
});

//Widget Launch upload
$("#WidgetSettings").on('change', '#AddLaunchFileInput', function () { uploadLaunchFile(); });

function uploadLaunchFile() {
    $('#AddLaunchIcon').submit();
}

$('#WidgetSettings').on("submit", "#AddLaunchIcon", function (e) {
    $.ajax({
        url: '/Settings/AddLaunchIcon',
        type: 'POST',
        data: new FormData(this),
        processData: false,
        contentType: false,
        success: function (response) {
            if (response.success !== false) {
                $("#LaunchIcons").prepend('<img src="' + response.imgUrl + '" />');
            }
        },
        error: function () {

        }
    });
    e.preventDefault();
});


//UpdatePreviewBtn
$("#SettingsModal").on("click", "#UpdatePreviewBtn", function () {
    var height = $("#PreviewHeight").val();
    var width = $("#PreviewWidth").val();
    if (height < 325) {
        height = 325;
        $("#PreviewHeight").val("325");
    }
    if (width < 325) {
        width = 325;
        $("#PreviewWidth").val("325");
    }
    $('#TemplatePreview').height(height);
    $('#TemplatePreview').width(width);
    document.getElementById('TemplatePreview').contentWindow.location.reload();
});

//ClearLogoSelection
$("#SettingsModal").on("click", "#ClearLogoSelection", function () {
    $("#LogoSelectBox").find(".selected").removeClass("selected");
    $("#HeaderLogoPath").val("");
});

$("#SettingsModal").on("click", "#ClearIconSelection", function () {
    $("#IconSelectBox").find(".selected").removeClass("selected");
    $("#IconPath").val("");
});

$("#SettingsModal").on("click", "#ClearUnavailableIconSelection", function () {
    $("#UnavailableIconSelectBox").find(".selected").removeClass("selected");
    $("#UnavailableIconPath").val("");
});

$("#SettingsModal").on("click", "#ClearResumeIconSelection", function () {
    $("#ResumeIconSelectBox").find(".selected").removeClass("selected");
    $("#ResumeIconPath").val("");
});

$("#SettingsModal").on("click", "#GenerateCssBtn", function () {
    var $btn = $(this).button('loading');
    var formData = $("#CreateTemplateForm").serialize();
    $.ajax({
        url: '/Settings/GenerateCss',
        type: 'POST',
        data: formData,
        success: function (response) {
            $btn.button('reset');
            if (response !== '' && response.success === true) {
                $("#CustomCss").val(response.cssPath);
            } else {
                alert("Error Generating CSS");
            }
        },
        error: function () {
            $btn.button('reset');
            alert("Error Generating CSS");
        }
    });
});

$("#SettingsModal").on("click", "#SaveTemplateBtn", function () {
    var $btn = $(this).button('loading');
    var formData = $("#CreateTemplateForm").serialize();
    $.ajax({
        url: '/Settings/UpdateTemplate',
        type: 'POST',
        data: formData,
        success: function (response) {
            $btn.button('reset');
            loadColorsAndTooltips();
        },
        error: function () {
            $btn.button('reset');
            alert("Error Saving Template.");
        }
    });
});

$("#SettingsModal").on("click", "#SaveWidgetBtn", function () {
    var $btn = $(this).button('loading');
    var formData = $("#CreateWidgetForm").serialize();
    $.ajax({
        url: '/Settings/UpdateWidget',
        type: 'POST',
        data: formData,
        success: function (response) {
            $btn.button('reset');
            loadColorsAndTooltips();
        },
        error: function () {
            $btn.button('reset');
            alert("Error Saving Widget.");
        }
    });
});

$("#SettingsPage").on("click", ".delete-widget-btn", function () {
    var id = $(this).data("id");
    var c = confirm("Are you sure you want to delete this widget?");
    if (c === true) {
        $.ajax({
            url: '/Settings/DeleteWidget',
            type: 'POST',
            data: { widgetId: id },
            success: function(response) {
                if (response.success === true) {
                    $.get("/Settings/ReloadWidgets",
                        function(response) {
                            if (response !== '' && response.success !== false) {
                                $("#WidgetSettings").html(response);
                            }
                        });
                } else {
                    alert("Error Deleting Widget. Please ensure the widget is not in use and try again.");
                }
            },
            error: function() {
                alert("Error Deleting Widget. Please ensure the widget is not in use and try again.");
            }
        });
    }
});

$("#SettingsPage").on("click", ".delete-template-btn", function () {
    var id = $(this).data("id");
    var c = confirm("Are you sure you want to delete this template?");
    if (c === true) {
        $.ajax({
            url: '/Settings/DeleteTemplate',
            type: 'POST',
            data: { templateId: id },
            success: function (response) {
                if (response.success === true) {
                    $.get("/Settings/ReloadTemplates",
                        function (response) {
                            if (response !== '' && response.success !== false) {
                                $("#TemplateSettings").html(response);
                            }
                        });
                } else {
                    alert("Error Deleting Template. Please ensure the template is not in use and try again.");
                }
            },
            error: function () {
                alert("Error Deleting Template. Please ensure the template is not in use and try again.");
            }
        });
    }
});


$("#SettingsModal").on("click", "#SaveFormBtn", function () {
    var $btn = $(this).button('loading');
    var formData = $("#CreateFormForm").serialize();
    $.ajax({
        url: '/Settings/UpdateForm',
        type: 'POST',
        data: formData,
        success: function (response) {
            $btn.button('reset');
            loadColorsAndTooltips();
        },
        error: function () {
            $btn.button('reset');
            alert("Error Saving Form.");
        }
    });
});

function getFormFields() {
    var formId = $("#FormId").val();
    $("#NewFieldLabel").val("");
    $("#NewPlaceholder").val("");
    $("#NewFieldName").val("");
    $("#NewFieldClasses").val("");
    $.get("/Settings/GetFormFields", {formId: formId}, function(response) {
        $("#ExistingFormFields").html(response);
        updateFormFields();
    });
}

function updateFormFields() {
    var btnText = $("#ButtonText").val();
    var btnTextColor = $("#ButtonTextColor").val();
    var btnColor = $("#ButtonColor").val();
    var round = $("#Rounded").val();
    var borderColor = $("#BorderColor").val();
    var labelColor = $("#LabelColor").val();
    $("#ExistingFormFields label").each(function() {
        $(this).css("color", labelColor);
    });
    $("#ExistingFormFields input").each(function () {
        $(this).css({
            border: "solid 1px " + borderColor,
            padding: "5px"
        });
        if (round) {
            $(this).css("border-radius", "4px");
        }
    });
    $("#ExistingFormFields textarea").each(function () {
        $(this).css({
            border: "solid 1px " + borderColor,
            padding: "5px"
        });
        if (round) {
            $(this).css("border-radius", "4px");
        }
    });
    $("#ExistingFormFields select").each(function () {
        $(this).css({
            border: "solid 1px " + borderColor,
            padding: "5px"
        });
        if (round) {
            $(this).css("border-radius", "4px");
        }
    });
    var radius = "0";
    if (round) {
        radius = "4px";
    }
    $("#ExistingFormFields button").css({
        backgroundColor: btnColor,
        color: btnTextColor,
        border: "none",
        borderRadius: radius,
        padding: "6px 12px"
    }).text(btnText);
}

$("#SettingsModal").on("click", "#CreateNewFormFieldBtn", function () {
    var $btn = $(this).button('loading');
    var formId = $("#FormId").val();
    var name = $("#NewFieldName").val();
    var classes = $("#NewFieldClasses").val();
    var label = $("#NewFieldLabel").val();
    var placeholder = $("#NewPlaceholder").val();
    var fieldType = $("#NewFieldType").val();
    var isUserField = $("#NewFieldIsUser").is(':checked');
    var isCutomInfo = $("#NewFieldCustomInfo").is(':checked');
    var isAttribute = $("#NewFieldAttribute").is(':checked');
    var isRequired = $("#NewFieldIsRequired").is(':checked');
    var isPhone = $("#NewFieldIsPhone").is(':checked');
    var maxLength = $("#NewFieldMaxLength").val();

    $.ajax({
        url: '/Settings/CreateFormField',
        type: 'POST',
        data: { formId: formId, fieldName: name, customClasses: classes, label: label, placeholder: placeholder, fieldType: fieldType, isUserField: isUserField, isCustomInfo: isCutomInfo, isAttribute: isAttribute, isRequired: isRequired, isPhone: isPhone, maxLength: maxLength },
        success: function (response) {
            $btn.button('reset');
            if (response.success === true) {
                getFormFields();
            } else {
                alert("Error adding form field.");
            }
        },
        error: function() {
            $btn.button('reset');
            alert("Error adding form field.");
        }
    });
});

$("#SettingsModal").on("click", "#SaveFieldBtn", function (e) {
    var $btn = $("#SaveFieldBtn").button('loading');
    $.ajax({
        url: '/Settings/UpdateFormField',
        type: 'POST',
        data: $("#EditFormField").serialize(),
        success: function (response) {
            $btn.button('reset');
            if (response.success === false) {
                alert("Error updating field.");
            } else {
                alert("Field updated successfully.");
                getFormFields();
            }
        },
        error: function () {
            $btn.button('reset');
        }
    });
    e.preventDefault();
});

$('#WidgetSettings').on("click", ".delete-form-btn", function () {
    var formId = $(this).data("id");
    var wrapper = $(this).closest(".form-wrapper");
    $.ajax({
        url: '/Settings/DeleteForm',
        type: 'POST',
        data: {formid: formId},
        success: function (response) {
            if (response.success === true) {
                wrapper.remove();
            } else {
                alert("Error removing Form");
            }
        },
        error: function () {
            alert("Error removing Form");
        }
    });
});

$("#SettingsModal").on("click", ".delete-form-field", function () {
    var id = $(this).data("id");
    var formId = $("#FormId").val();
    $.ajax({
        url: '/Settings/DeleteFormField',
        type: 'POST',
        data: {formId: formId, id: id},
        success: function (response) {
            if (response.success === true) {
                getFormFields();
            } else {
                alert("Error removing form field.");
            }
        },
        error: function () {
            alert("Error removing form field.");
        }
    });
});

$("#SettingsModal").on("click", ".raise-form-field", function () {
    var id = $(this).data("id");
    var formId = $("#FormId").val();
    $.ajax({
        url: '/Settings/RaiseFormField',
        type: 'POST',
        data: { formId: formId, id: id },
        success: function (response) {
            if (response.success === true) {
                getFormFields();
            } else {
                alert("Error moving form field.");
            }
        },
        error: function () {
            alert("Error moving form field.");
        }
    });
});

$("#SettingsModal").on("click", ".edit-select-options", function () {
    var id = $(this).data("id");
    getFieldSelectOptions(id);
});

function getFieldSelectOptions(id) {
    $.get("/Settings/GetFieldSelectOptions", { fieldId: id }, function (response) {
        $("#SelectOptionEditor").html(response);
        $("#SelectOptionEditor").show();
        $("#FormPreview").hide();
    });
}

$("#SettingsModal").on("click", "#CloseSelectOptions", function () {
    $("#SelectOptionEditor").hide();
    $("#FormPreview").show();
});

$("#SettingsModal").on("click", "#AddSelectOption", function () {
    var $btn = $(this).button('loading');
    var fieldId = $(this).data("field");
    var text = $("#NewSelectionOptionName").val();
    var value = $("#NewSelectionOptionValue").val();
    $.ajax({
        url: '/Settings/AddSelectOption',
        type: 'POST',
        data: { fieldId: fieldId, text: text, value: value },
        success: function (response) {
            $btn.button('reset');
            if (response.success === true) {
                getFieldSelectOptions(fieldId);
            } else {
                alert("Error adding select option.");
            }
        },
        error: function () {
            $btn.button('reset');
            alert("Error adding select option.");
        }
    });
});

$("#SettingsModal").on("click", ".remove-select-option", function () {
    var id = $(this).data("id");
    var fieldId = $(this).data("field");
    $.ajax({
        url: '/Settings/RemoveSelectOption',
        type: 'POST',
        data: { fieldId: fieldId, fieldOption: id },
        success: function (response) {
            if (response.success === true) {
                getFieldSelectOptions(fieldId);
            } else {
                alert("Error removing select option.");
            }
        },
        error: function () {
            alert("Error removing select option.");
        }
    });
});

$("#SettingsModal").on("click", ".set-default-select-option", function () {
    var id = $(this).data("id");
    var fieldId = $(this).data("field");
    $.ajax({
        url: '/Settings/SetDefaultSelectOption',
        type: 'POST',
        data: { fieldId: fieldId, fieldOption: id },
        success: function (response) {
            if (response.success === true) {
                getFieldSelectOptions(fieldId);
            } else {
                alert("Error changing default select option.");
            }
        },
        error: function () {
            alert("Error changing default select option.");
        }
    });
});

$("#SettingsModal").on("click", ".edit-form-field", function () {
    var fieldId = $(this).data("id");
    var fieldDetails = $(this).parent().find(".form-field-details");
    if (fieldDetails.is(':visible')) {
        fieldDetails.hide();
    } else {
        var form = document.getElementById("EditFormField");
        if (typeof form !== "undefined" && form !== null) {
            $("#EditFormField").closest(".form-field-details").hide();
            $("#EditFormField").remove();
        }
        $.get("/Settings/GetFormField", { fieldId: fieldId }, function (response) {
            if (response !== '' && response.success !== false) {
                fieldDetails.html(response);
            } else {
                fieldDetails.html("Error retrieving field details");
            }
        });
        fieldDetails.show();
    }
});

$("#SettingsModal").on("click", ".refresh-form-fields", function () {
    getFormFields();
});


$("#SettingsModal").on("click", "#PreviewBtn", function (e) {
    e.preventDefault();
});

var hasScrollbar = window.innerWidth > document.documentElement.clientWidth;
var scrollbarWidth = 0;
if (hasScrollbar) {
    scrollbarWidth = window.innerWidth - document.documentElement.clientWidth;
}

$("#SettingsModal").on("click", "#WidgetPreviewBtn", function () {
    if ($("#wcb-webchat").length) {
        $("#wcb-webchat").remove();
    }
    var id = $("#WidgetId").val();
    $.get("/Settings/GetWidgetPreview", { widgetId: id }, function (response) {
        if (response.success !== false) {
            $("body").append(response);
            if ($("#wcb-webchat-launch").length) {
                var right = getStyle("wcb-webchat-launch","right");
                if (right !== "auto" && right !== "") {
                    right = Number(right.replace(/[^-\d\.]/g, ''));
                    $("#wcb-webchat-launch").css("right", (right + scrollbarWidth) + "px");
                }
                if ($("#wcb-launch-tooltip").length) {
                    var bottom = getStyle("wcb-webchat-launch", "bottom");
                    var top = getStyle("wcb-webchat-launch", "top");
                    var left = getStyle("wcb-webchat-launch", "left");
                    var height = $("#wcb-webchat-launch").height();
                    var width = $("#wcb-webchat-launch").width();
                    if (bottom !== "auto" && bottom !== "") {
                        bottom = Number(bottom.replace(/[^-\d\.]/g, ''));
                    }
                    if (left !== "auto" && left !== "") {
                        left = Number(left.replace(/[^-\d\.]/g, ''));
                    }
                    if (top === "auto" || top === "") {
                        $("#wcb-launch-tooltip").css("bottom", (bottom + height + 5));
                    } else {
                        if ($("#Vertical").is(':checked')) {
                            if (right === "auto" || right === "") {
                                $("#wcb-launch-tooltip").css("left", (left + height + 5));
                            } else {
                                $("#wcb-launch-tooltip").css("right", (right + height + 5));
                            }
                        } else {
                            if (right === "auto" || right === "") {
                                $("#wcb-launch-tooltip").css("left", (left + width + 5));
                            } else {
                                $("#wcb-launch-tooltip").css("right", (right + width + 5));
                            }
                        }
                    }

                    var timeout;
                    $("#wcb-webchat-launch").on("mouseover", function () {
                        $("#wcb-launch-tooltip").show();
                        if (typeof timeout !== "undefined" && timeout !== null) {
                            clearTimeout(timeout);
                        }
                    });
                    $("#wcb-webchat-launch").on("mouseout", function () {
                        timeout = setTimeout(function () {
                            $("#wcb-launch-tooltip").hide();
                        }, 500);
                    });
                }
            }
        }
    });
});

$("#SettingsModal").on("change", "#IsSecondaryStyle", function () {
    var isCollapsingStyle = $("#IsSecondaryStyle").is(':checked');
    if (isCollapsingStyle) {
        $("#IsSecondaryStyle").addClass("disabled");
    } else {
        alert("To see all options properly, save and relaunch the widget editor.");
    }
    checkSecondaryStyle();
});

function checkSecondaryStyle() {
    var isCollapsingStyle = $("#IsSecondaryStyle").is(':checked');
    if (isCollapsingStyle) {
        $("#ResumeChatOptions").find("h3").text("Active Chat Options");
        isSecondaryStyle = true;
        $("#IconSettingsWrapper").show();
        $("#CustomLaunchWrapper").show();
        $(".wcb-hidden-secondary-style").each(function() {
            $(this).hide();
        });
    } else {
        $("#ResumeChatOptions").find("h3").text("Resume Chat Options");
        isSecondaryStyle = false;
        $(".wcb-hidden-secondary-style").each(function () {
            $(this).show();
        });
    }
}

$("#WidgetSettings").on("submit", "#UnavailableOptionsForm", function (e) {
    var $btn = $("#UnavailableOptionsSaveBtn").button('loading');
    $.ajax({
        url: '/Settings/UnavailableOptions',
        type: 'POST',
        data: $("#UnavailableOptionsForm").serialize(),
        success: function (response) {
            $btn.button('reset');
            if (response.success === false) {
                alert("Error saving Unavailable Options");
            }
        },
        error: function () {
            $btn.button('reset');
        }
    });
    e.preventDefault();
});

function getStyle(element, styleProperty) {
    var x = document.getElementById(element);
    if (x.currentStyle)
        var y = x.currentStyle[styleProperty];
    else
        var y = x.style.getPropertyValue(styleProperty);
    return y;
}