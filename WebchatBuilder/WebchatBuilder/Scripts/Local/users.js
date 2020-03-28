$(".role-users-list").each(function () {
    var role = $(this).data("id");
    var roleUserList = $(this);
    roleUserList.html("<i class=\"fa fa-refresh fa-spin\"></i>");
    $.get("/Account/GetRoleAdmins", { roleName: role }, function (response) {
        if (response !== '' && response.success !== false) {
            var source = $("#assigned-role-template").html();
            var template = WcbHandlebars.compile(source);
            var html = template({ users: response.UserList });
            roleUserList.html(html);
        } else {
            roleUserList.html("No Users in Role.");
        }
    });
});

$(".user-wrapper").draggable({
    appendTo: "#UsersContainer",
    helper: "clone",
    containment: "#MainView"
});

$(".role-users-list").droppable({
    activeClass: "ui-state-default",
    hoverClass: "ui-state-hover",
    accept: ":not(.ui-sortable-helper)",
    drop: function (event, ui) {
        var id = ui.draggable.data("id");
        var role = $(event.target).data("id");
        var roleUserList = $(event.target);
        $.ajax({
            url: '/Account/AddUserToRole',
            type: 'POST',
            data: { roleName: role, userId: id },
            success: function (response) {
                if (response.success == true) {
                    roleUserList.html("<i class=\"fa fa-refresh fa-spin\"></i>");
                    $.get("/Account/GetRoleAdmins", { roleName: role }, function (response) {
                        if (response !== '' && response.success !== false) {
                            var source = $("#assigned-role-template").html();
                            var template = WcbHandlebars.compile(source);
                            var html = template({ users: response.UserList });
                            roleUserList.html(html);
                        } else {
                            roleUserList.html("No Users in Role.");
                        }
                    });
                }
            },
            error: function () {
                alert("Error adding User to Role.");
            }
        });
    }
});

var reloadPage = false;

$('#UserModal').on('hidden.bs.modal', function () {
    $("#UserModalBody").html($("#Loading").html());
    $("#UserModalTitle").text("");
    if (reloadPage) {
        location.reload();
    }
});

$("#UserModalBody").html($("#Loading").html());

$('#UserModal').on('show.bs.modal', function (event) {
    $(".wcb-tooltip").tooltip('hide');
    var button = $(event.relatedTarget); // Button that triggered the modal
    var buttonId = button.data("id");
    var type = button.data("type");
    if (type == "edit") {
        $("#UserModalTitle").text("Edit User");
        $.get("/Account/GetUserInfo", { userId: buttonId }, function (response) {
            if (response !== '' && response.success !== false) {
                $("#UserModalBody").html("<form id=\"UserEditForm\">" + response + "</form>");
            }
        });
    }
    if (type == "pwd") {
        $("#UserModalTitle").text("Change Password");
        $.get("/Account/GetPwdForm", { userId: buttonId }, function (response) {
            if (response !== '' && response.success !== false) {
                $("#UserModalBody").html("<form id=\"ChangePasswordForm\">" + response + "</form>");
            }
        });
    }
    if (type == "add") {
        $("#UserModalTitle").text("Create User");
        $.get("/Account/GetNewUserForm", function (response) {
            if (response !== '' && response.success !== false) {
                $("#UserModalBody").html("<form id=\"CreateUserForm\">" + response + "</form>");
            }
        });
    }
});

$('#UserModalBody').on("submit", "#CreateUserForm", function (e) {
    $.ajax({
        url: '/Account/CreateUser',
        type: 'POST',
        data: new FormData(this),
        processData: false,
        contentType: false,
        success: function (response) {
            if (response.success == true) {
                $("#UserModalBody").html("User Created Sucessfully!");
                reloadPage = true;
            } else {
                $("#UserModalBody").html("<form id=\"CreateUserForm\">" + response + "</form>");
            }
        },
        error: function () {
            alert("Error creating user.");
        }
    });
    e.preventDefault();
});

$('#UserModalBody').on("submit", "#ChangePasswordForm", function (e) {
    $.ajax({
        url: '/Account/ChangePassword',
        type: 'POST',
        data: new FormData(this),
        processData: false,
        contentType: false,
        success: function (response) {
            if (response.success == true) {
                $("#UserModalBody").html("Password Changed Sucessfully!");
            } else {
                $("#UserModalBody").html("<form id=\"ChangePasswordForm\">" + response + "</form>");
            }
        },
        error: function () {
            alert("Error changing password.");
        }
    });
    e.preventDefault();
});

$('#UserModalBody').on("submit", "#UserEditForm", function (e) {
    $.ajax({
        url: '/Account/UpdateUser',
        type: 'POST',
        data: new FormData(this),
        processData: false,
        contentType: false,
        success: function (response) {
            if (response.success == true) {
                $("#UserModalBody").html("User Saved Sucessfully!");
            } else {
                $("#UserModalBody").html("<form id=\"UserEditForm\">" + response + "</form>");
            }
        },
        error: function () {
            alert("Error updating User.");
        }
    });
    e.preventDefault();
});

$("#WcbRoles").on("click", ".role-user-remove", function () {
    var id = $(this).data("user");
    var role = $(this).data("role");
    var btn = $(this);
    $.ajax({
        url: '/Account/RemoveUserFromRole',
        type: 'POST',
        data: { roleId: role, userId: id },
        success: function (response) {
            if (response.success == true) {
                btn.closest(".role-user").remove();
            } else {
                alert("Error removing User from Role.");
            }
        },
        error: function () {
            alert("Error removing User from Role.");
        }
    });
});

$('.user-active-toggle').change(function () {
    var checkbox = this;
    var checked = checkbox.checked;
    checkbox.checked = !checked;
    $(this).attr("checked", !checked);
    var userId = $(this).data("id");
    $.ajax({
        url: '/Account/ToggleActive',
        type: 'POST',
        data: { userId: userId, isActive: checked },
        success: function (response) {
            if (response.success == true) {
                checkbox.checked = checked;
                $(this).attr("checked", checked);
            } else {

            }
        },
        error: function () {

        }
    });
});

$('.user-delete-btn').click(function () {
    var c = confirm("Are you sure you want to delete this user?");
    if (c === true) {
        var userId = $(this).data("id");
        $.ajax({
            url: '/Account/DeleteUser',
            type: 'POST',
            data: { userId: userId },
            success: function (response) {
                if (response.success == true) {
                    location.reload();
                } else {
                    alert("Error deleting user.");
                }
            },
            error: function () {
                alert("Error deleting user.");
            }
        });
    }
});