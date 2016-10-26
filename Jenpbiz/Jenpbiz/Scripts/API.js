$(document).ready(function () {
        $.ajax({
            type: 'GET',
            url: "/Product/GetMerchantProduct",
            dataType: 'json',
            success: function (data) {
                console.log('success')
                data = JSON.parse(data);
                data.response.games.forEach(function (p) {
                    $('#placeholder').append('<li>' + p.productId + '</li>');
                });
            }
        });
    });
