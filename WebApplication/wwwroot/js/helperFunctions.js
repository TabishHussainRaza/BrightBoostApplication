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

function createForm(title, mainDivId, formFields, callBack, layout = 1) {
    $("#myModalLabel").html(title);
    var mainDiv = $('#' + mainDivId);
    var form = $("<div class='row'></div>");

    MyFormFields = formFields;

    var columnClass = 'col-md-' + (12 / layout); // Calculate column class based on layout

    // Initialize groupedFields array with empty arrays
    var groupedFields = [];
    for (var i = 0; i < layout; i++) {
        groupedFields.push([]);
    }

    // Split form fields into groups based on layout
    var columnIndex = 0;
    formFields.forEach(function (field, index) {
        groupedFields[columnIndex].push(field);
        columnIndex = (columnIndex + 1) % layout;
    });

    // Create columns and append form fields
    groupedFields.forEach(function (columnFields) {
        var columnDiv = $("<div class='" + columnClass + "'></div>");

        columnFields.forEach(function (field) {
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
            } else if (field.HtmlType === 'textarea') {
                input = $('<textarea></textarea>');
                if (field.inputType === 'input') {
                    input.attr('maxlength', field.inputType);
                }
            } else if (field.HtmlType === 'select') {
                input = $('<select></select>');
                input.append(`<option>Select ${field.placeHolder}</option>`);
                if (field.data != null) {
                    field.data.forEach(function (item) {
                        var option = $('<option></option>');
                        option.val(item.id);
                        option.text(item.typeName);
                        input.append(option);
                    });
                }
            }

            input.attr('id', field.id);
            input.attr('required', field.isRequired);
            fieldDiv.css('display', field.visible ? 'block' : 'none');
            input.addClass(field?.class);

            fieldDiv.append(label);
            fieldDiv.append(input);

            columnDiv.append(fieldDiv);
        });

        form.append(columnDiv);
    });

    mainDiv.append(form);

    if (callBack) {
        callBack();
    }
}


function showModalAndRegisterEventListener(buttonId, modalId, url, message, callBack, afterProcessing, dz) {
    $(`${buttonId}`).on('click', function (e) {
        $("#myModalLabel").html("Add " + message);
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
            processForm(modalId, url, message, afterProcessing, dz);
        }
    });
}

function processForm(modalId, url, message, afterProcessing, dz) {

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

                if (afterProcessing) {
                    processDropZone(result, dz);
                }

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

function renderUpdateData(url, callback, renderFile = false) {

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

                if (renderFile) {
                    renderFiles(data.imagePaths);
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

function processDropZone(id, dz) {
    var formData = new FormData();
    var files = dz.files;
    for (var i = 0; i < files.length; i++) {
        formData.append('files', files[i]);
        formData.append('productId', id);
    }

    $.ajax({
        url: '/Products/Upload',
        type: 'POST',
        data: formData,
        processData: false,
        contentType: false,
        xhrFields: {
            withCredentials: true
        },
        success: function (response) {
            dz.removeAllFiles(true)
        },
        error: function (xhr, status, error) {
            
        }
    });
}

function renderFiles(files) {
    $('#modalContentBody').append('<div id="filesProductDiv"> <table id="filesProduct" class="table table-bordered table-striped" style="border-spacing: 0;" width="100%"></table></div>')

    DataTable = $("#filesProduct").DataTable({
        data: files,
        columns: [
            {
                title: 'File Name',
                data: 'fileName',
                render: function (data, type, row, meta) {
                    var text = `<a href="Products/Download?id=${row.id}">${data}</a>`;
                    return text;
                }
            },
            {
                title: 'Action',
                render: function (data, type, row, meta) {
                    var text = '<div style="display:flex; justify-content:space-around">';
                    text += `
                            <a href="#" class="btn-icon-split btn-sm btn btn-danger js-delete-file" data-id="${row?.id}">
                            <span class="icon text-white-50"  data-id="${row?.id}"><i class="fa fa fa-trash" style='color: white'  data-id="${row?.id}"></i></span>
                            </a>`
                        ;
                    text += '</div>'
                    return text;
                }
            },
        ]
    });

    $("#filesProduct").on("click", ".js-delete-file", function (e) {
        var Id = $(this).data("id");
        var rowToRemove = $(this).closest('tr'); // Store reference to row
        $.ajax({
            type: "DELETE",
            url: `Products/DeleteDocument?id=${Id}`,
            dataType: "json",
            success: function (result) {
                DataTable.row(rowToRemove).remove().draw(); // Use stored reference
            }
        });
        
    });

}