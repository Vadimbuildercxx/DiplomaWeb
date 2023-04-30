// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.




$("document").ready(function () {

    var table = $('.GridPartialView').DataTable({
        "destroy": true,
        "filter": true,
        stateSave: true,
        language: {
                url: "//cdn.datatables.net/plug-ins/1.13.4/i18n/ru.json",
        },
    });

});


//$(document).ajaxComplete(function () {
//    /*$("#PersonListGridPartialView").dataTable().fnDestroy()*/
//    $("#PersonListGridPartialView").dataTable({
//        "destroy": true,
//    });
//});

//function AjaxInit() {
//    alert("test");
//}

function updateEntityList(handler, listForUpdateId) {
    var handlerPartialUrl = "Edit?handler=" + handler;
    
    $.ajax({
        url: handlerPartialUrl, //PersonListPartial
        method: "GET",
        success: function (data) {
            $("#" + listForUpdateId).html(data);
            $(".GridPartialView").each().DataTable({
                "destroy": true,
                stateSave: true,
            });
        }
    });
   
    
    //$("#" + listForUpdateId).dataTable({
    //});
}

function updatePersonListGridPartialView() {
    var actionUrl = form.attr('action');
    $.post(actionUrl, dataToSend).done(function (data) {
        var newBody = $('.modal-body', data);
        placeholderElement.find('.modal-body').replaceWith(newBody);

        var isValid = newBody.find('[name="IsValid"]').val() == 'True';
        if (isValid) {
            placeholderElement.find('.modal').modal('hide');
        }
    });
}

$(function () {
    var placeholderElement = $('#modal-placeholder');

    $('button[data-toggle="ajax-modal"]').click(function (event) {
        var url = $(this).data('url');
        console.log(url);
        $.get(url).done(function (data) {
            placeholderElement.html(data);
            placeholderElement.find('.modal').modal('show');
        });
    });

    placeholderElement.on('click', '[data-save="modal"]', function (event) {
        event.preventDefault();

        var form = $(this).parents('.modal').find('form');
        var actionUrl = form.attr('action');
        var dataToSend = form.serialize();

        var updateListUrlHandler = $(this).data('handler');
        var listForUpdateId = $(this).data('listid');
        console.log(listForUpdateId);

        $.post(actionUrl, dataToSend).done(function (data) {
            var newBody = $('.modal-body', data);
            placeholderElement.find('.modal-body').replaceWith(newBody);

            var isValid = newBody.find('[name="IsValid"]').val() == 'True';
            if (isValid) {
                placeholderElement.find('.modal').modal('hide');
                updateEntityList(updateListUrlHandler, listForUpdateId);
            }
        });
    });

    placeholderElement.on('click', '[data-dismiss="modal"]', function (event) {
        event.preventDefault();

        placeholderElement.find('.modal').modal('hide');
    });
});


$(function () {
    var placeholderElement = $('#modal-placeholder');

    $(document).on('click', 'button[data-toggle="ajax-modal-delete-person"]', function () {
        var url = $(this).data('url');
        var dataToSend = {
            id: $(this).data('id')
        };
        console.log(url);

        $.get(url, dataToSend).done(function (data) {
            placeholderElement.html(data);
            placeholderElement.find('.modal').modal('show');
        });
    });

    placeholderElement.on('click', '[data-delete="modal"]', function (event) {
        event.preventDefault();
        
        var dataToSend = {
            id: $(this).data('id')
        };
        var actionUrl = $(this).data('url');
        var updateListUrlHandler = $(this).data('handler');
        var listForUpdateId = $(this).data('listid');
        console.log(updateListUrlHandler);
        $.ajax({
            url: actionUrl,
            method: "POST",
            data: dataToSend,
            headers: { "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() },
            success: function (data) {
                placeholderElement.find('.modal').modal('hide');
                updateEntityList(updateListUrlHandler, listForUpdateId);
            },
        })

    });

});



$(function () {
    var placeholderElement = $('#modal-placeholder');

    $(document).on('click', 'button[data-toggle="ajax-modal-edit-person"]', function () {
        var url = $(this).data('url');
        var dataToSend = {
            id: $(this).data('id')
        };

        $.get(url, dataToSend).done(function (data) {
            placeholderElement.html(data);
            placeholderElement.find('.modal').modal('show');
        });
    });
});
