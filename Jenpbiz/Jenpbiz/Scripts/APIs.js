$(document).ready(function () {

    var maxResults = parseInt(getUrlParameter('maxResults'));
    var page = parseInt(getUrlParameter('page'));

    if (typeof maxResults == 'undefined' || isNaN(maxResults)) {
        maxResults = 5;
    }
    if (typeof page == 'undefined' || isNaN(page)) {
        page = 1;
    }


    accordionFunction();
    $('#maxProducts').append(maxResults);
    $('#maxResults').val(maxResults);

    CheckIfNextPageExists(maxResults, page);
    CheckIfPreviousPageExists(maxResults, page);


    $('.deleteClick').on('click', function () {

        var productId = $(this).attr('id');

        // Empties the modal so as to not show duplicate or out of date content.
        $('#deleteProductModalContent').html('');


        // Calls the getProduct function.
        getProductInfoForDelete(productId);

    });


    $('.editClick').on('click', function () {

        var productId = $(this).attr('id');

        // Empties the modal so as to not show duplicate or out of date content.
        $('#editProductModalContent').html('');


        // Calls the getProduct function.
        getProductInfoForEdit(productId);

    });

    $('#EditProductLink').on('click', function () {

        $('#editProductForm').submit();
        
    });
    
    $('#nextPage').on('click', function () {

        window.location.replace('GetProduct?maxResults=' + maxResults + '&page=' + (page + 1));
    });

    $('#previousPage').on('click', function () {

        window.location.replace('GetProduct?maxResults=' + maxResults + '&page=' + (page - 1));
    });

    $('#maxResults').on("change", function () {
        maxResults = $('#maxResults').val();
        page = 1;

        window.location.replace('GetProduct?maxResults=' + maxResults + '&page=' + page);
    });


});

function getUrlParameter(sParam) {
    var sPageURL = decodeURIComponent(window.location.search.substring(1)),
        sURLVariables = sPageURL.split('&'),
        sParameterName,
        i;

    for (i = 0; i < sURLVariables.length; i++) {
        sParameterName = sURLVariables[i].split('=');

        if (sParameterName[0] === sParam) {
            return sParameterName[1] === undefined ? true : sParameterName[1];
        }
    }
};


function accordionFunction() {

    //$("tr:not(.accordion-toggle)").hide();

    $("tr.accordion-toggle").click(function () {
        var id = $(this).attr('id');
        $(this).nextAll("tr.accordion-togglee-" + id).fadeToggle(500);
    });


};

function CheckIfNextPageExists(maxResults, page)
{
    $.ajax({
        method: 'POST',
        url: '/GoogleApi/NextPageExists/',
        dataType: 'json',
        data: { maxResults: maxResults, page: page },
        success: function (data) {
            var exists = data.exists;

            if (exists) {
                $('#nextPage').prop("disabled", false);
            }
            else {

                $('#nextPage').prop("disabled", true);
            }
        }
    });

};

function CheckIfPreviousPageExists(maxResults, page) {
    $.ajax({
        method: 'POST',
        url: '/GoogleApi/PreviousPageExists/',
        dataType: 'json',
        data: { maxResults: maxResults, page: page },
        success: function (data) {
            var exists = data.exists;

            if (!exists) {
                $('#previousPage').prop("disabled", true);
            }
            else {
                if (page > 1) {
                    $('#previousPage').prop("disabled", false);
                }
                else {
                    $('#previousPage').prop("disabled", true);
                }
            }
        }
    });

};

function getProductInfoForEdit(productId) {

    $.ajax({
        method: 'GET',
        url: '/GoogleApi/getProductInfo/',
        dataType: 'json',
        data: { productId: productId },
        success: function (data) {
            var returnedProduct = data.clickedProduct;

            var container = $('#editProductModalContent');
            container.css('text-align', 'center');

            //var s = returnedProduct.Id.replace(/\:/g, '_');
            //var editLink = '../../GoogleApi/EditProduct/';
            //$('#editProductLink').attr('href', editLink);

            $('#editModalDisplayId').html("ID: " + returnedProduct.Id);


            container.html(container.html()

                //+ 'Title: ' + returnedProduct.Title + '<br />'
                //+ 'Description: ' + returnedProduct.Description + '<br />'
                //+ 'Price: ' + returnedProduct.Price.Value + ' ' + returnedProduct.Price.Currency + '<br />'
                //+ 'Availability: ' + returnedProduct.Availability + '<br />'
                //+ 'Target Country: ' + returnedProduct.TargetCountry + '<br />'
                //+ 'Content Language: ' + returnedProduct.ContentLanguage + '<br />'

                + '<center><img src="' + returnedProduct.ImageLink + '" alt="Missing Product Picture" class="img-responsive" style="width:150px; height:100px;" /></center>'

                + '<h3>ID: ' + returnedProduct.Id + '</h3>'


                

                + '<form action="../../GoogleApi/EditProduct" method="post" id="editProductForm">'

                    + '<input type="text" name="editProductId" id="editProductId" hidden readonly />'
                    //Select lists inputs
                    + '<div class="form-group" role="form">'
      
                        + '<select name="editSelectProductCategory" class="form-control formController input-sm" id="editSelectProductCategory">'
                            + '<option value="" disabled>Category</option>'
                            + '<option value="Apparel">Apparel</option>'
                            + '<option value="Books">Books</option>'
                            + '<option value="Electronics">Electronics</option>'
                            + '<option value="Toys">Toys</option>'
                        + '</select>'

                        + '<select name="editSelectProductAvailability" class="form-control formController input-sm" id="editSelectProductAvailability"">'
                            + '<option value="" disabled>Availability</option>'
                            + '<option value="in stock">in stock</option>'
                            + '<option value="preorder">preorder</option>'
                            + '<option value="out of stock">out of stock</option>'
                        + '</select>'

                        + '<select name="editSelectProductCondition" class="form-control formController input-sm" id="editSelectProductCondition">'
                            + '<option value="" disabled>Condition</option>'
                            + '<option value="new">New</option>'
                            + '<option value="used">Used</option>'
                            + '<option value="refurbished">Refurbished</option>'
                        + '</select>'

                        + '<select name="editSelectProductTargetCountry" class="form-control formController input-sm" id="editSelectProductTargetCountry">'
                            + '<option value="" disabled>Country</option>'
                            + '<option value="AU">Australia</option>'
                            + '<option value="GB">England</option>'
                            + '<option value="SE">Sweden</option>'
                            + '<option value="US">USA</option>'
                        + '</select>'

                        + '<center><div class="input-group" style="width:280px" name="">'
                            + '<span class="input-group-addon inputGroupAddon glyphicon glyphicon-calendar" id="editDateAddon"></span>'
                            + '<input type="text" name="editSelectProductAvailabilityDate" id="editDatepicker" class="form-control input-sm formController" placeholder="Availability date">'
                        + '</div>'

                        + '<div class="input-group" style="width:280px" name="">'
                            + '<span class="input-group-addon inputGroupAddon glyphicon glyphicon-calendar" id="editExpirydateAddon"></span>'
                            + '<input type="text" name="editSelectProductAvailabilityExpiryDate" id="editExpiryDatepicker" class="form-control input-sm formController" placeholder="Expiration date">'
                        + '</div></center>'


                    + '</div>'


                
                    // Inputs
                    + '<div class="form-group input-group-sm ">'

                        + '<label for="editProductTitle"><span class="text-danger">* </span>Title</label><br />'
                        + '<input type="text" name="editProductTitle" id="editProductTitle" class="form-control formController" placeholder="Title" /><br />'

                        + '<label for="editProductDescription"><span class="text-danger">* </span>Description</label><br />'
                        + '<textarea name="editProductDescription" id="editProductDescription" class="form-control formController" placeholder="Description" rows="1" cols="1"></textarea><br />'

                        + '<label for="editProductLink"><span class="text-danger">* </span>Link</label><br />'
                        + '<input type="text" name="editProductLink" id="editProductLink" class="form-control formController" placeholder="https://example.com/category/item33" /><br />'

                        + '<label for="editProductImageLink"><span class="text-danger">* </span>Image Link</label><br />'
                        + '<input type="text" name="editProductImageLink" id="editProductImageLink" class="form-control formController" placeholder="https://example.com/img/image33.png" /><br />'

                        + '<label for="editProductPrice"><span class="text-danger">* </span>Price</label><br />'
                        + '<input type="text" name="editProductPrice" id="editProductPrice" class="form-control formController" placeholder="Price" /><br />'

                        + '<label for="editProductGtin"><span class="text-danger">* </span>Gtin</label><br />'
                        + '<input type="text" name="editProductGtin" id="editProductGtin" class="form-control formController" placeholder="gtin" /><br />'

                    + '</div>'
                    

                + '</form>'
                );

            //Tilldelning av värden för inputs of selects lists

            $('#editProductId').val(returnedProduct.Id);
            $('#editProductTitle').val(returnedProduct.Title);
            $('#editProductDescription').val(returnedProduct.Description);
            $('#editProductLink').val(returnedProduct.Link);
            $('#editProductImageLink').val(returnedProduct.ImageLink);
            $('#editProductPrice').val(returnedProduct.Price.Value);
            $('#editProductGtin').val(returnedProduct.Gtin);

            $('#editSelectProductCategory option[value=' + returnedProduct.GoogleProductCategory + ']').attr('selected', 'selected');
            $('#editSelectProductAvailability option[value="' + returnedProduct.Availability + '"]').attr('selected', 'selected');

            $('#editSelectProductCondition option[value=' + returnedProduct.Condition + ']').attr('selected', 'selected');
            $('#editSelectProductTargetCountry option[value=' + returnedProduct.TargetCountry + ']').attr('selected', 'selected');
            //$('#editExpiryDatepicker').datepicker();
            //$('#editDatepicker').datepicker();

            //if (returnedProduct.ExpirationDate != null) {
            //    //$('#editExpiryDatepicker').datepicker('setDate', returnedProduct.ExpirationDate);
            //    $('#editExpiryDatepicker').val(returnedProduct.ExpirationDate);
                
            //}
            
            //if (returnedProduct.Availability == 'preorder') {
            //    //$('#editDatepicker').datepicker('setDate', Date(2016, 08,26));
            //    $('#editDatepicker').val(returnedProduct.AvailabilityDate);

            //}

            $('#editExpiryDatepicker').val(returnedProduct.ExpirationDate);
            $('#editDatepicker').val(returnedProduct.AvailabilityDate);

            container.children().css(({
                'margin-top': '10px'
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


function getProductInfoForDelete(productId) {

    $.ajax({
        method: 'GET',
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
                + 'Target Country: ' + returnedProduct.TargetCountry + '<br />'
                + 'Content Language: ' + returnedProduct.ContentLanguage + '<br />'

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
