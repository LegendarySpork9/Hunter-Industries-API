$(function () {
    var interval = setInterval(function () {
        var select = $('#input_baseUrl');

        if (select.is('select') && select.find('option').length > 0) {
            clearInterval(interval);

            select.find('option').each(function () {
                var url = $(this).text();
                var match = url.match(/v(\d+\.\d+)/);

                if (match) {
                    $(this).val(url);
                    $(this).text('v' + match[1]);
                }
            });
        }
    }, 100);
});
