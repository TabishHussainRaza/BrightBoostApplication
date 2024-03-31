class HelperFunctionsClass {
    constructor() {
        this.DataTable = null;
        this.MyFormFields = null;
        this.getUrl = null;
    }

    validateForm() {
        let isValid = true;
        $('.is-invalid').removeClass('is-invalid');
        this.MyFormFields.forEach(function (field) {
            if (field.HtmlType === 'input' && field.isRequired) {
                let value = $('#' + field.id).val().trim();
                if (value === '') {
                    $('#' + field.id).addClass('is-invalid');
                    isValid = false;
                }
            }
        });
        return isValid;
    }

    getAll(url, tableId, Columns) {
        this.getUrl = url;
        $.blockUI({ message: '<p>Loading..</p>' });
        $.ajax({
            type: "GET",
            url: this.getUrl,
            success: (result) => {
                $.unblockUI();
                if (result) {
                    if (this.DataTable === null) {
                        this.initializeDataTable(result, tableId, Columns);
                    } else {
                        this.updateDataTable(result);
                    }
                } else {
                    console.log("No Items found.");
                }
            },
            error: (error) => {
                $.unblockUI();
            }
        });
    }

    refreshPage() {
        $.blockUI({ message: '<p>Loading..</p>' });
        $.ajax({
            type: "GET",
            url: this.getUrl,
            success: (result) => {
                $.unblockUI();
                if (result) {
                    if (this.DataTable !== null) {
                        this.updateDataTable(result);
                    }
                } else {
                    console.log("No Items found.");
                }
            },
            error: (error) => {
                $.unblockUI();
            }
        });
    }

    updateDataTable(updatedData) {
        this.DataTable.clear().rows.add(updatedData).draw();
    }

    initializeDataTable(data, tableId, Columns) {
        this.DataTable = $(tableId).DataTable({
            data: data,
            columns: Columns
        });
    }

    createForm(title, modalId, mainDivId, formFields, callBack) {
        $('#' + modalId).find("#myModalLabel").html(title);
        var mainDiv = $('#' + modalId).find('#' + mainDivId);
        var form = $("<div class='row'><div class='col-md-12'></div>");

        this.MyFormFields = formFields;

        formFields.forEach(function (field) {
            var fieldDiv = $(`<div class='form-group' id='${field.id}_group'></div>`);

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

    showModalAndRegisterEventListener(buttonId, modalId, url, message, callBack, title) {
        $(`${buttonId}`).on('click', (e) => {
            $(`#${modalId}`).find("#myModalLabel").html(title);
            $(`#${modalId}`).modal('show');
            $(`#${modalId}`).on('hidden.bs.modal', () => {
                if (callBack) {
                    callBack();
                }
                this.MyFormFields.forEach(function (field) {
                    $('#' + field.id).val('');
                });
            });
        });

        $(`#btn_${modalId}`).on('click', (e) => {
            if (!this.validateForm()) {
                return;
            } else {
                this.processForm(modalId, url, message);
            }
        });
    }

    processForm(modalId, url, message) {
        var data = {};
        this.MyFormFields.forEach(function (field) {
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
            success: (result) => {
                if (result) {
                    $(`#${modalId}`).modal('hide');
                    swal({
                        title: 'Success!',
                        text: `${message} ${id == 0 ? 'Added' : 'Updated'}`,
                        option: "success",
                        icon: "success",
                        timer: 3000
                    });
                    this.refreshPage();
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

    renderUpdateData(url, callback) {
        $.ajax({
            url: url,
            type: "GET",
            success: (data) => {
                if (data) {
                    this.MyFormFields.forEach(function (field) {
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
            error: () => {
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

    registerDeleteEvent(url) {
        this.deleteData(url, () => { this.refreshPage(); });
        
    }

    deleteData(url, callback) {
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

                            if (callback) {
                                callback();
                            } 

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
}