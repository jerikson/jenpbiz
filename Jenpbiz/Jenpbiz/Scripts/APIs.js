$(document).ready(function () {


    $(".deleteClick").on("click", function () {

        var productId = $(this).attr("id");

        //$("#modalDeleteProduct").modal("show");

        // Empties the modal so as to not show duplicate or out of date content.
        $("#deleteProductModalContent").html("");

        // Calls the getProduct function.
        getProductInfo(productId);

    });


});



function getProductInfo(productId) {

    $.ajax({
        method: 'POST',
        url: '/GoogleApi/getProductInfo/',
        dataType: 'json',
        data: {productId: productId},
        success: function (data) {
            var returnedProduct = data.clickedProduct;

            var 

        },
        error: function (jqXHR, statusText, errorThrown) {
            console.log("An error occurred.");
            console.log("Status Text: " + statusText);
            console.log("Error Thrown: " + errorThrown);
            console.log("jqXHR: " + jqXHR);
            console.log("jqXHR stringified: " + JSON.stringify(jqXHR));
        }
    });

};
