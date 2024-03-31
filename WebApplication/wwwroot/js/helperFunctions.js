var DataTable = null;
var MyFormFields = null;
var getUrl = null;

function validateForm() {
    var isValid = true;

    // Reset validation
    $('.is-invalid').removeClass('is-invalid');

    // Loop through each field
    MyFormFields.forEach(function (field) {

        if (field.HtmlType === 'input' && field.isRequired) {
            var value = $('#' + field.id).val().trim();
            if (value === '') {
                $('#' + field.id).addClass('is-invalid');
                isValid = false;
            }
        }
    });

    return isValid;
}

function getAll(url, tableId, Columns) {
    getUrl = url;
    $.blockUI({ message: '<p>Loading..</p>' });
    $.ajax({
        type: "GET",
        url: getUrl,
        success: function (result) {
            $.unblockUI();
            if (result) {
                if (DataTable == null) {
                    initializeDataTable(result, tableId, Columns);
                } else {
                    updateDataTable(result);
                }
            } else {
                console.log("No Items found.");
            }
        },
        error: function (error) {
            $.unblockUI();
        }
    });
}

function refreshPage() {
    $.blockUI({ message: '<p>Loading..</p>' });
    $.ajax({
        type: "GET",
        url: getUrl,
        success: function (result) {
            $.unblockUI();
            if (result) {
                if (DataTable != null) {
                    updateDataTable(result);
                }
            } else {
                console.log("No Items found.");
            }
        },
        error: function (error) {
            $.unblockUI();
        }
    });
}

function updateDataTable(updatedData) {
    DataTable.clear().rows.add(updatedData).draw();
}

function initializeDataTable(data, tableId, Columns) {
    DataTable = $(tableId).DataTable({
        data: data,
        columns: Columns
    });
}

function createForm(title, mainDivId, formFields, callBack) {

    $("#myModalLabel").html(title);
    var mainDiv = $('#' + mainDivId);
    var form = $("<div class='row'><div class='col-md-12'></div>");

    MyFormFields = formFields;

    formFields.forEach(function (field) {
        var fieldDiv = $("<div class='form-group'></div>");

        var label = $('<label>' + field.label + ': </label>');

        var input;
        if (field.HtmlType === 'input') {
            input = $('<input type="' + field.inputType + '">');
            input.attr('placeholder', `Enter ${field.placeHolder}`);

            if (field.data != null) {
                input.val(field.data);
            }

        } else if (field.HtmlType === 'div') {
            input = $('<div></div>');
        }

        input.attr('id', field.id);
        input.attr('required', field.isRequired);
        fieldDiv.css('display', field.visible ? 'block' : 'none'); 
        input.addClass(field?.class);

        fieldDiv.append(label);
        fieldDiv.append(input);

        form.append(fieldDiv);
    });

    mainDiv.append(form);

    if (callBack) {
        callBack();
    }
}

function showModalAndRegisterEventListener(buttonId, modalId, url, message, callBack) {
    $(`${buttonId}`).on('click', function (e) {
        $(`#${modalId}`).modal('show');
        $(`#${modalId}`).on('hidden.bs.modal', function () {

            if (callBack) {
                callBack();
            }

            MyFormFields.forEach(function (field) {
                $('#' + field.id).val('');
            });
        });
    });

    $(`#btn_${modalId}`).on('click', function (e) {
        if (!validateForm()) {
            return;
        } else {
            processForm(modalId, url, message);
        }
    });
}

function processForm(modalId, url, message) {

    var data = {};
    MyFormFields.forEach(function (field) {
        data[field.id] = $('#' + field.id).val();
    });

    var id = $(`#btn_${modalId}`).data('id') ?? 0;
    data['Id'] = id;

    $.ajax({
        type: "POST",
        url: url,
        headers: {
            'Content-Type': 'application/json',
        },
        data: JSON.stringify(data),
        success: function (result) {
            if (result) {

                $(`#${modalId}`).modal('hide');

                swal({
                    title: 'Success!',
                    text: `${message} ${id == 0 ? 'Added' : 'Updated'}`,
                    option: "success",
                    icon: "success",
                    timer: 3000
                });
                refreshPage();

            } else {
                swal({
                    title: 'Unsuccessful!',
                    text: `Unable to ${id == 0 ? 'added' : 'updated'} ${message}`,
                    option: "error",
                    icon: "error",
                    timer: 3000
                });
            }

            $(`#btn_${modalId}`).data('id', null);
        }
    });
}

function renderUpdateData(url, callback) {

    $.ajax({
        url: url,
        type: "GET",
        success: function (data) {
            if (data) {
                MyFormFields.forEach(function (field) {
                    if (data[field.attribute]) {
                        $('#' + field.id).val(data[field.attribute]);
                    }
                });

                if (callback) {
                    callback();
                }

            } else {
                swal({
                    title: 'No details found!',
                    text: 'Could Not find details!',
                    option: "error",
                    icon: "error",
                    timer: 3000
                });
            }
        },
        error: function () {
            swal({
                title: 'No details found!',
                text: 'Could Not find details!',
                option: "error",
                icon: "error",
                timer: 3000
            });
        }
    });
}

function deleteData(url) {
    swal({
        title: "Comfirmation",
        text: "Are you sure you want to delete?",
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
                url: url,
                dataType: "json",
                success: function (result) {
                    $.unblockUI();
                    if (result) {
                        swal({
                            title: 'Deleted!',
                            text: 'Successfully Deleted!',
                            option: "success",
                            icon: "success",
                            timer: 3000
                        });
                        refreshPage();
                    } else {
                        swal({
                            title: 'Deletion Failed!',
                            text: 'Could Not be Deleted!',
                            option: "error",
                            icon: "error",
                            timer: 3000
                        });
                    }
                }
            });
        } else {
            swal("Cancelled", "Not deleted", "error");
        }
    });
}