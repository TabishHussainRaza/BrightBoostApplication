
var userDataTable = null;
var allUsers = null;
function getAllUsers() {
    $.blockUI({ message: '<p>Loading..</p>' });
    $.ajax({
        type: "GET",
        url: "/Staffs/GetAllusers",
        success: function (result) {
            $.unblockUI();
            if (result) {
                allUsers = result;
                if (userDataTable == null) {
                    initializeUserDataTable(result);
                } else {
                    updateUserDataTable(result);
                }

            } else {
                console.log("No Users found.");
            }
        },
        error: function (error) {
            $.unblockUI();
        }
    });
}

function updateUserDataTable(updatedData) {
    userDataTable.clear().rows.add(updatedData).draw();
}

function initializeUserDataTable(data) {
    userDataTable = $('#user').DataTable({
        data: data,
        columns: [
           
        ]
    });
}

getAllUsers();

$('#addNewUser').on('click', function (e) {
    $('#addNewUserModal').modal('show');
    $('#addNewUserModal').on('hidden.bs.modal', function () {
        $(".addField").val("");
        $(".text-danger").hide();
    });

});

$('#btnSaveitem').on('click', function (e) {
    $(".text-danger").css("display", "none");

    // Check for empty fields
    var hasErrors = false;
    $(".addField").each(function () {
        if ($(this).val().trim() === "") {
            var fieldId = $(this).attr("id");
            $("#" + fieldId + "_empty_error").css("display", "block");
            hasErrors = true;
        }
    });

    if (hasErrors) {
        return false;
    } else {
        var firstName = $("#first_name").val();
        var lastName = $("#last_name").val();
        var email = $("#email").val();
        var password = $("#password").val();

        var requestData = {
            firstName: firstName,
            lastName: lastName,
            email: email,
            password: password
        };

        $.ajax({
            url: '/User/Add',
            type: 'POST',
            data: requestData,
            dataType: 'json',
            success: function (response) {
                if (response) {
                    $('#addNewUserModal').modal('hide');
                    swal({
                        title: 'User Added!',
                        text: 'Successfully Created!',
                        option: "success",
                        icon: "success",
                        timer: 3000
                    });
                    getAllUsers();
                } else {
                    swal({
                        title: 'User Addition Failed!',
                        text: 'Could Not be Added!',
                        option: "error",
                        icon: "error",
                        timer: 3000
                    });
                }
            },
            error: function (error) {
                swal("Cancelled", "User Not deleted", "error");
            }
        });
    }
});

function findUserById(userId) {
    if (allUsers) {
        var foundUser = allUsers.find(function (user) {
            return user.id === userId;
        });

        return foundUser ? foundUser : null;
    }

    return null;
}
$("#user").on("click", ".js-update", function (e) {
    var user = findUserById($(e.target).data('id'));

    if (user != null) {
        $('#edit_first_name').val(user.firstName);
        $('#edit_last_name').val(user.lastName);
        $('#edit_email').val(user.email);
        $('#edit_id').val(user.id);
        $('#editUserModal').modal('show');

        $('#editUserModal').on('hidden.bs.modal', function () {
            $(".editField").val("");
            $(".text-danger").hide();
        });
    }
});

$('#btnEditUser').on('click', function (e) {
    $(".text-danger").css("display", "none");

    var hasErrors = false;
    $(".editField").each(function () {
        if ($(this).val().trim() === "") {
            var fieldId = $(this).attr("id");
            $("#" + fieldId + "_empty_error").css("display", "block");
            hasErrors = true;
        }
    });

    if (hasErrors) {
        return false;
    } else {
        var editedData = {
            firstName: $("#edit_first_name").val(),
            lastName: $("#edit_last_name").val(),
            email: $("#edit_email").val(),
            id: $("#edit_id").val()
        };

        $.ajax({
            url: '/User/Edit',
            type: 'POST',
            data: editedData,
            dataType: 'json',
            success: function (response) {
                if (response) {
                    $('#editUserModal').modal('hide');
                    swal({
                        title: 'User Edited!',
                        text: 'User information successfully updated!',
                        option: "success",
                        icon: "success",
                        timer: 3000
                    });
                    getAllUsers();
                } else {
                    swal({
                        title: 'Edit Failed!',
                        text: 'User information could not be updated!',
                        option: "error",
                        icon: "error",
                        timer: 3000
                    });
                }
            },
            error: function (error) {
                swal("Error", "An error occurred while editing the user.", "error");
            }
        });
    }
});

$("#user").on("click", ".js-delete", function (e) {
    var data = {
        id: $(e.target).data('id')
    };
    swal({
        title: "Comfirmation",
        text: "Are you sure you want to delete the user!",
        icon: "warning",
        buttons: [
            'No',
            'Yes'
        ],
        dangerMode: true,
    }).then(function (isConfirm) {
        if (isConfirm) {
            $.blockUI({ message: '<p> Loading..</p>' });
            $.ajax({
                type: "DELETE",
                data: data,
                url: "/User/Delete",
                dataType: "json",
                success: function (result) {
                    $.unblockUI();
                    if (result) {
                        swal({
                            title: 'User Deleted!',
                            text: 'Successfully Deleted!',
                            option: "success",
                            icon: "success",
                            timer: 3000
                        });
                        getAllUsers();
                    } else {
                        swal({
                            title: 'User Deletion Failed!',
                            text: 'Could Not be Deleted!',
                            option: "error",
                            icon: "error",
                            timer: 3000
                        });
                    }
                }
            });
        } else {
            swal("Cancelled", "User Not deleted", "error");
        }
    })
});

$("#user").on("click", ".change_user_status", function (e) {
    var data = {
        id: $(e.target).data('id')
    };
    swal({
        title: "Comfirmation",
        text: "Are you sure you want to change the user status?",
        icon: "warning",
        buttons: [
            'No',
            'Yes'
        ],
        dangerMode: true,
    }).then(function (isConfirm) {
        if (isConfirm) {
            data.isActive = $(`#${data.id}_chk_status`).prop("checked");
            $.blockUI({ message: '<p> Loading..</p>' });
            $.ajax({
                type: "POST",
                data: data,
                url: "/User/ChangeUserStatus",
                dataType: "json",
                success: function (result) {
                    $.unblockUI();
                    if (result) {
                        swal({
                            title: 'User Status Updated!',
                            text: 'Successfully Updated!',
                            option: "success",
                            icon: "success",
                            timer: 3000
                        });
                        getAllUsers();
                    } else {
                        swal({
                            title: 'User Status Change Failed!',
                            text: 'Could Not be Changed!',
                            option: "error",
                            icon: "error",
                            timer: 3000
                        });
                    }
                }
            });
        } else {
            swal("Cancelled", "User Status Not Updated", "error");
        }
    })
});

$("#user").on("click", ".js-assign", function (e) {
    var data = {
        id: $(e.target).data('id')
    };
    $('#assignRoleModal').modal('show');
    loadOptionItems(data.id);

    $("#assignRoleId").val(data.id);
});

$('#btnAssignRole').on('click', function (e) {
    var roleIds = $("#select_item_list").val();
    var requestData = {
        userId: $("#assignRoleId").val()
    };

    if (roleIds && roleIds.length > 0) {
        requestData.RoleIds = roleIds;
        requestData.RoleName = $("#select_item_list").text();
    } else {
        requestData.removeAll = true;
    }

    $.ajax({
        url: "/Role/AddRoleMapping",
        type: "POST",
        data: requestData,
        dataType: "json",
        success: function (data) {
            $('#assignRoleModal').modal('hide');
            if (data) {
                swal({
                    title: 'User Role(s) Updated!',
                    text: 'Successfully Updated!',
                    option: "success",
                    icon: "success",
                    timer: 3000
                });
                getAllUsers();
            } else {
                swal({
                    title: 'User Role(s) Change Failed!',
                    text: 'Could Not be Changed!',
                    option: "error",
                    icon: "error",
                    timer: 3000
                });
            }
        },
        error: function (xhr, status, error) {

            console.error("Error:", status, error);
        },
    });
});

function loadOptionItems(id) {
    $.ajax({
        type: "GET",
        url: "/Role/GetAllRoles",
        success: function (result) {
            if (result) {
                $('#select_item_list').html('');
                $('#select_item_list').append($("<option></option>"));
                $.each(result, function (index, item) {
                    $('#select_item_list').append($('<option></option>').val(item.id).html(item.name));
                });

                $.ajax({
                    type: "GET",
                    dataType: "json",
                    data: {
                        userId: id
                    },
                    url: "/Role/GetMyUserRoles",
                    success: function (res) {
                        $('#assignRoleModal #select_item_list').select2({
                            placeholder: 'Select Roles',
                            dropdownParent: $('#assignRoleModal'),
                            tags: true
                        });
                        if (res) {
                            var roleMappedList = res.map(a => a.id);
                            $("#select_item_list").val(roleMappedList).trigger('change');
                        } else {
                            console.log("No Roles found.");
                        }
                    },
                    error: function (error) {
                        $.unblockUI();
                    }
                });
            }
        },
        error: function (error) {
            $.unblockUI();
        }
    });
}