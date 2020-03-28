$.ajaxSetup({ cache: false });
var expanded = false;
var width = $(window).width();
var sidePanelWidth = 350;
if (sidePanelWidth > width) {
    sidePanelWidth = width;
}

var showTooltip = function () {
    $(this).tooltip('show');
}
, hideTooltip = function () {
    $(this).tooltip('hide');
};

getUserInfo();

function getUserInfo() {
    var url = "/GetUserInfo";
    $.get(url, function (response) {
        if (response !== '' && response.success !== false) {
            $("#AccountInfo").replaceWith(response);
        }
    });
}

function getProfiles() {
    $("#SidePanelProfiles").html($("#Loading").html());
    var url = "/Profiles/GetProfiles";
    $.get(url, function (response) {
        if (response !== '' && response.success !== false) {
            $("#SidePanelProfiles").html(response);
        } else {
            $("#SidePanelProfiles").html("No Profiles found.");
        }
    });
}

$("#SidePanelProfiles").on("click", ".profile-wrapper", function (e) {
    if (!$(e.target).is(".delete-profile-btn") && !$(e.target).is(".edit-profile-btn")) {
        $(this).find(".profile-details").toggle();
    }
});

$("#MainView").css("width", ($(window).width() - 65));
$("#MainView").css("height", ($(window).height() - 50));
$("#SidePanel").css("height", ($(window).height() - 50));

$("#ToggleTooltips").tooltip({
    trigger: 'hover focus',
    placement: 'left',
    container: 'body',
    title: 'Clicking here will show all tooltips available on this page for 5 seconds. You can hover over any element with a tooltip for 2 seconds for it to be shown. To close this tip, hover over the ? icon.'
});

$("#ToggleTooltips").tooltip('show');

setTimeout(function () {
    $("#ToggleTooltips").tooltip('hide');
}, 10000);

$(document).ajaxComplete(function () {
    loadTooltips();
});

function loadTooltips() {
    $(".wcb-tooltip").tooltip({
        trigger: 'hover focus',
        delay: { 'show': 1200, 'hide': 1000 },
        container: 'body'
    });
}

$("#ModalBody").html($("#Loading").html());

$('#MainModal').on('show.bs.modal', function (event) {
    $("#ToggleTooltips").tooltip('hide');
    hideTooltips();
    var button = $(event.relatedTarget); // Button that triggered the modal
    var title = button.data("modal-title");
    var type = button.data("type");
    $("#ModalTitle").text(title);
    if (type == "createprofile") {
        $.get("/Profiles/CreateProfile", function (response) {
            if (response !== '' && response.success !== false) {
                $("#ModalBody").html(response);
                //loadTooltips();
            } else {
                $("#ModalBody").html(response.message);
            }
        });
    }
    if (type == "editprofile") {
        var profile = button.data("profile");
        $.get("/Profiles/EditProfile", { profileId: profile }, function (response) {
            if (response !== '' && response.success !== false) {
                $("#ModalBody").html(response);
                //loadTooltips();
            } else {
                $("#ModalBody").html("Error loading profile.");
            }
        });
    }
    if (type == "deleteprofile") {
        var profile = button.data("profile");
        $.get("/Profiles/GetDeleteProfile", { profileId: profile }, function (response) {
            if (response !== '' && response.success !== false) {
                $("#ModalBody").html(response);
                //loadTooltips();
            } else {
                $("#ModalBody").html("Error loading profile.");
            }
        });
    }
    if (type == "checkprofile") {
        var profile = button.data("profile");
        $.get("/Profiles/CheckAvailability", { profileId: profile }, function (response) {
            if (response !== '' && response.success !== false) {
                $("#ModalBody").html(response);
            } else {
                $("#ModalBody").html("Error checking profile availability.");
            }
        });
    }
    if (type == "aboutwcb") {
        $.get("/AboutWcb", function (response) {
            if (response !== '' && response.success !== false) {
                $("#ModalBody").html(response);
                //loadTooltips();
            } else {
                $("#ModalBody").html("Qsect's Webchat Builder");
            }
        });
    }
    //aboutwcb
});

var reloadProfiles = false;

$('#MainModal').on('hide.bs.modal', function () {
    hideTooltips();
});

$('#MainModal').on('hidden.bs.modal', function () {
    $("#ModalTitle").text("");
    $("#ModalBody").html($("#Loading").html());
    hideTooltips();
    if (reloadProfiles) {
        getProfiles();
        reloadProfiles = false;
    }
});

var profilesLoaded = false;

$("#SideSlider").click(function () {
    if (!profilesLoaded) {
        getProfiles();
        profilesLoaded = true;
    }
    if ($("#DashboardPage").length) {
        return false;
    }
    if (!expanded) {
        $("#SidePanel").animate({ "width": sidePanelWidth }, "slow");
        $("#MainView").animate({ "left": sidePanelWidth }, "slow");
        $("#MainView").animate({ "width": ($(window).width() - 350) });
        if ($(window).width() < 390 && $("#AboutWcb").length) {
            $("#AboutWcb").hide();
        }
        expanded = true;
    } else {
        $("#SidePanel").animate({ "width": 65 }, "slow");
        $("#MainView").animate({ "left": 65 }, "slow");
        $("#MainView").css({ "width": ($(window).width() - 65) });
        if ($("#AboutWcb").length) {
            $("#AboutWcb").show();
        }
        $("#CreateProfileBtn").tooltip('hide');
        $("#SidePanelProfiles .wcb-tooltip").tooltip('hide');
        expanded = false;
    }
    var arrow = $(this).find("i");
    arrow.fadeOut("slow", function () {
        arrow.toggleClass("fa-chevron-right").toggleClass("fa-chevron-left");
    });
    arrow.fadeIn("slow");
});

$("#MainView").scroll(function () {
    hideTooltips();
    $("#ToggleTooltips").tooltip('hide');
    if ($("#MainView").scrollTop() > 50) {
        $("#ScrollTop").fadeIn();
    } else {
        $("#ScrollTop").fadeOut();
    }
    $("#MainView").scrollLeft(0);
});

$("#SidePanelInner").scroll(function () {
    hideTooltips();
    $("#ToggleTooltips").tooltip('hide');
    $("#SidePanelInner").scrollLeft(0);
});

$("#ModalBody").scroll(function () {
    hideTooltips();
    $("#ToggleTooltips").tooltip('hide');
});

$("#ScrollTop").click(function () {
    $("#MainView").animate({ scrollTop: 0 }, 500);
});

$(window).resize(function () {
    var width = $(window).width();
    if (sidePanelWidth > width) {
        sidePanelWidth = width;
    } else {
        sidePanelWidth = 350;
    }
    if (expanded) {
        $("#SidePanel").css({ "width": sidePanelWidth });
    }
    hideTooltips();
    $("#ToggleTooltips").tooltip('hide');
    $("#MainView").css("width", ($(window).width() - 65));
    $("#MainView").css("height", ($(window).height() - 50));
    $("#SidePanel").css("height", ($(window).height() - 50));
});

function hideTooltips() {
    $(".wcb-tooltip").tooltip('hide');
    tooltipsShown = false;
    modalTipsShown = false;
}
var tooltipsShown = false;
var modalTipsShown = false;
$(".modal").on("click", ".show-modal-tooltips", function () {
    if (!modalTipsShown) {
        $(".modal .wcb-tooltip").tooltip('show');
        modalTipsShown = true;
        setTimeout(function () {
            hideTooltips();
        }, 5000);
    }
});

$("#MainView").on("click", "#ToggleTooltips", function () {
    if (!tooltipsShown) {
        if (!expanded) {

        } else {
            $("#CreateProfileBtn").tooltip('show');
            $("#SidePanelProfiles .wcb-tooltip").tooltip('show');
        }
        $(".wcb-tooltip").not("#CreateProfileBtn").not("#SidePanelProfiles .wcb-tooltip").tooltip('show');
        tooltipsShown = true;
        setTimeout(function () {
            hideTooltips();
        }, 5000);
    }
});

//. li-link
$(".li-link").click(function (e) {
    var url = $(this).find("a").prop('href');
    window.location.assign(url);
});

//ClearLogoSelection
$("#MainModal").on("click", "#ClearLogoSelection", function () {
    $("#LogoSelectBox").find(".selected").removeClass("selected");
    $("#HeaderLogoPath").val("");
});

$("#MainModal").on("click", "#LogoSelectBox img", function () {
    $("#LogoSelectBox").find(".selected").removeClass("selected");
    $(this).addClass("selected");
    var src = $(this).attr("src");
    $("#HeaderLogoPath").val(src);
});

$("#MainModal").on("click", "#SaveProfileBtn", function (e) {
    var $btn = $(this).button('loading');
    var formData = $("#CreateProfileForm").serialize();
    $.ajax({
        url: '/Profiles/UpdateProfile',
        type: 'POST',
        data: formData,
        success: function (response) {
            $btn.button('reset');
            if (response !== '' && response.success == true) {
                $("#ModalBody").html(response.message);
                reloadProfiles = true;
            } else {
                $("#ModalBody").html(response);
            }
        },
        error: function () {
            $btn.button('reset');
            alert("Error Updating Profile");
        }
    });
});

$("#MainModal").on("click", "#DeleteProfileBtn", function (e) {
    var $btn = $(this).button('loading');
    var id = $(this).data("id");
    $.ajax({
        url: '/Profiles/DeleteProfile',
        type: 'POST',
        data: { profileId: id },
        success: function (response) {
            $btn.button('reset');
            if (response !== '' && response.success == true) {
                $("#ModalBody").html(response.message);
                reloadProfiles = true;
            } else {
                alert("Error Deleting Profile");
            }
        },
        error: function () {
            $btn.button('reset');
            alert("Error Deleting Profile");
        }
    });
});


$("#MainView").on("click", "#ReportIssueBtn", function () {
    var r = confirm("Would you like to report an issue and restart the service?");
    if (r === true) {
        $.get("/ReportIssue", function (response) {
            if (response.success) {
                alert("Service is restarting.");
            } else {
                alert("Unable to restart service.");
            }
        }).fail(function () {
            alert("Unable to restart service.");
        });
    }
});
