// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(document).ready(function () {
    // show the alert
    setTimeout(function () {
        $(".alert").alert('close');
    }, 2000);
    $('#resultTable').dataTable({
        dom: 'Bfrtip',
        buttons: [
            'copy', 'csv', 'excel', 'pdf', 'print'
        ]
    });
});

function getData() {
    $("#fileName").val('');
    $("#departmentName").val('');
    var barcode = $('#barcode').val();
    if (barcode === null || barcode == '') {
        return;
    }
    $.ajax({
        url: 'GetFileDetails',
        data: "barCode=" + barcode,
        dataType: "json",
        beforeSend: function (xhr) {
            xhr.overrideMimeType("text/plain; charset=x-user-defined");
        }
    })
        .done(function (data) {
            if (data == null) {
                return;
            }
            $("#fileName").val(data.filename);
            $("#departmentName").val(data.department);
        });

}
