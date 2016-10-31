$(document).ready(function () {


    $('.deleteClick').on('click', function () {

        var productId = $(this).attr('id');

        //$("#modalDeleteProduct").modal("show");

        // Empties the modal so as to not show duplicate or out of date content.
        $('#deleteProductModalContent').html('');


        // Calls the getProduct function.
        getProductInfo(productId);

    });


});



function getProductInfo(productId) {

    $.ajax({
        method: 'POST',
        url: '/GoogleApi/getProductInfo/',
        dataType: 'json',
        data: {productId : productId},
        success: function (data) {
            var returnedProduct = data.clickedProduct;

            var container = $('#deleteProductModalContent');
            container.css('text-align', 'center');

            var s = returnedProduct.Id.replace(/\:/g, '_');
            var deleteLink = '../../GoogleApi/DeleteProduct/?productId=' + s;
            $('#deleteProductLink').attr('href', deleteLink);

            container.html(
                '<h3> ID: ' + returnedProduct.Id + '</h3>'

                + '<h4>'

                + 'Title: ' + returnedProduct.Title + '<br />'
                + 'Description: ' + returnedProduct.Description + '<br />'
                + 'Price: ' + returnedProduct.Price.Value + ' ' + returnedProduct.Price.Currency + '<br />'
                + 'Availability: ' + returnedProduct.Availability + '<br />'

                + '</h4>'


                );

            if (returnedProduct.ExpirationDate != null)
            {
                container.html(container.html()
                    + '<h4> Expiration date: ' + returnedProduct.ExpirationDate + '</h4><br />'
                    );

            }

            if (returnedProduct.Availability == 'preorder')
            {
                container.html(container.html()
                    + '<h4>' + returnedProduct.AvailabilityDate + '</h4><br />'
                    );

            }

            container.children().css(({
                'margin-top' : '10px'
            }));

        },
        error: function (jqXHR, statusText, errorThrown) {
            console.log('An error occurred.');
            console.log('Status Text: ' + statusText);
            console.log('Error Thrown: ' + errorThrown);
            console.log('jqXHR: ' + jqXHR);
            console.log('jqXHR stringified: ' + JSON.stringify(jqXHR));
        }
    });

};
