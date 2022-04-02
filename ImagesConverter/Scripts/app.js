$(function () {
    $.request = function (options) {
        $.ajax({
            type: 'POST',
            url: options.url,
            data: options.data,
            complete: options.complete,
            processData: options.processData,
            contentType: options.contentType,
            crossDomain: false,
            beforeSend: function (xhr) {
                xhr.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
            },
            success: function (result) {
                $.processResult(result, options);
            },
            error: function (jqXHR) {
                if (jqXHR.status != 0) {
                    $.processResult({
                        Success: false,
                        Data: jqXHR,
                        Message: '(' + jqXHR.status + ')' + ' ' + jqXHR.statusText
                    }, options);
                }
            }
        });
    };

    $.processResult = (function () {
        var errorHandler = function (result, options) {
            var event = $.Event("error");
            if (options.error instanceof Function)
                options.error(event, result);

            if (!event.isDefaultPrevented()) {
                alert(result.Message || 'You are not permitted to execute this action');
            }
        };

        var successHandler = function (result, options) {
            if (options.renderTo) {
                result.RenderedData = $(result.Data);
                $(options.renderTo).html(result.RenderedData);
            }

            if (options.appendTo) {
                result.RenderedData = $(result.Data);
                $(options.appendTo).append(result.RenderedData);
            }

            var event = $.Event("success");
            if (options.success instanceof Function) {
                options.success(event, result);
            }
        };

        return function (result, options) {
            if (!$.isPlainObject(result)) {
                try {
                    result = $.parseJSON(result);
                }
                catch (e) {
                    result = { Success: false };
                }
            }

            if (result.Success) {
                successHandler(result, options);
            } else {
                errorHandler(result, options);
            }
        };
    })();

    let addSubmitListeners = function () {

    };


    $('body').on('submit', '#uploadForm', (function (event) {
        event.preventDefault();
        event.stopPropagation();

        let form = $(event.target);

        $.request({
            url: form.attr('action'),
            data: new FormData(form[0]),
            contentType: false,
            processData: false,
            success: function (e, result) {
                if (result.Data) {
                    for (var id in result.Data) {
                        $(`#${id}`).html(result.Data[id]);
                    }
                }
            }
        })
    }));

    let downloadFile = (ids) => {
        if (!(ids instanceof Array)) {
            ids = [ids];
        }

        fetch(`/Home/GetZipWithBase64s?ids=${(ids || []).join('-')}`)
            .then(resp => resp.blob())
            .then(blob => {
                if (blob.type == 'text/html') {
                    alert('Error while creating zip file');
                } else {
                    const url = window.URL.createObjectURL(blob);
                    const a = document.createElement('a');
                    a.style.display = 'none';
                    a.href = url;
                    a.download = 'images.zip';
                    document.body.appendChild(a);
                    a.click();
                    window.URL.revokeObjectURL(url);
                }
            })
            .catch((err) => {
                console.log(err);
                alert('Error while creating zip file');
            });
    };

    $('#downloadZip').click((event) => {
        event.preventDefault();
        event.stopPropagation();

        let ids = [];

        $.each($('#infoTable td input[type="checkbox"][data-image-id]'), (index, checkbox) => {
            if ($(checkbox).prop('checked')) {
                ids.push($(checkbox).attr('data-image-id'));
            }
        });

        if (ids.length) {
            downloadFile(ids);
        } else {
            alert('At least one item should be selected');
        }
    });
});