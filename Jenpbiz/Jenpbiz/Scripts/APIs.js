$(document).ready(function () {


    $(".deleteClick").on("click", function () {

        var productId = $(this).attr("id");
        alert(productId);

        //$("#modalDeleteProduct").modal("show");

        //DeleteProduct(productId);
    });


});

function DeleteProduct(productId) {

    $.ajax({
        method: 'GET',
        url: 'GoogleApi/DeleteProduct/',
        contentType: 'json',
        dataType: 'json',
        data: { productId: productId },
        success: function (data) {
            var deleteResponse = data.successfullyDeleted;
            console.log("Product successfully deleted!");
            console.log("raw data; " + data);
            console.log("JSON stringify data: " + JSON.stringify(data));

        },
        error: function (jqXHR, statusText, errorThrown) {
            console.log('Ett fel inträffade: ' + statusText);
            console.log("jqXHR: " + jqXHR);
            console.log("jqXHR JSON Stringified: " + JSON.stringify(jqXHR));
            console.log("statusText: " + statusText);
            console.log("errorThrown: " + errorThrown);

        }
    });

}