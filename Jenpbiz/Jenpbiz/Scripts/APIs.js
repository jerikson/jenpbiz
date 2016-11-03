$(document).ready(function () {

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


});


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

            var s = returnedProduct.Id.replace(/\:/g, '_');
            var editLink = '../../GoogleApi/EditProduct/?productId=' + s;
            $('#editProductLink').attr('href', editLink);

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


                

                + '<form action="../../GoogleApi/InsertProduct" method="post">'

                //Select lists inputs
                + '<div class="form-group" role="form">'
      
                    + '<select name="selectProductCategory" class="form-control formController input-sm" id="selectProductCategory">'
                        + '<option value="" disabled>Category</option>'
                        + '<option value="Apparel">Apparel</option>'
                        + '<option value="Books">Books</option>'
                        + '<option value="Electronics">Electronics</option>'
                        + '<option value="Toys">Toys</option>'
                    + '</select>'

                    + '<select name="selectProductAvailability" class="form-control formController input-sm" id="selectProductAvailability"">'
                        + '<option value="" disabled>Availability</option>'
                        + '<option value="in stock">in stock</option>'
                        + '<option value="preorder">preorder</option>'
                        + '<option value="out of stock">out of stock</option>'
                    + '</select>'

                    + '<select name="selectProductCondition" class="form-control formController input-sm" id="selectProductCondition">'
                        + '<option value="" disabled>Condition</option>'
                        + '<option value="new">New</option>'
                        + '<option value="used">Used</option>'
                        + '<option value="refurbished">Refurbished</option>'
                    + '</select>'

                    + '<select name="selectProductTargetCountry" id="selectProductTargetCountry" class="form-control formController input-sm">'
                        + '<option value="" disabled>Country</option>'
                        + '<option value="AU">Australia</option>'
                        + '<option value="GB">England</option>'
                        + '<option value="SE">Sweden</option>'
                        + '<option value="US">USA</option>'
                    + '</select>'

                    + '<div class="input-group" style="width:280px" name="">'
                        + '<span class="input-group-addon inputGroupAddon glyphicon glyphicon-calendar" id="dateAddon"></span>'
                        + '<input type="text" name="selectProductAvailabilityDate" id="datepicker" class="form-control input-sm" placeholder="Availability date">'
                    + '</div>'

                    + '<div class="input-group" style="width:280px" name="">'
                        + '<span class="input-group-addon inputGroupAddon glyphicon glyphicon-calendar" id="expirydateAddon"></span>'
                        + '<input type="text" name="selectProductAvailabilityExpiryDate" id="expirydatepicker" class="form-control input-sm" placeholder="Expiration date">'
                    + '</div>'


                + '</div>'


                
                // Inputs
                + '<div class="form-group input-group-sm ">'

                    + '<label for="inputProductTitle"><span class="text-danger">* </span>Title</label><br />'
                    + '<input type="text" name="inputProductTitle" id="inputProductTitle" class="form-control formController" placeholder="Title" />'

                    + '<label for="inputProductDescription"><span class="text-danger">* </span>Description</label><br />'
                    + '<textarea name="inputProductDescription" id="inputProductDescription" class="form-control formController" placeholder="Description" rows="1" cols="1"></textarea>'

                    + '<label for="inputProductLink"><span class="text-danger">* </span>Link</label><br />'
                    + '<input type="text" name="inputProductLink" id="inputProductLink" class="form-control formController" placeholder="https://example.com/category/item33" />'

                    + '<label for="inputProductImageLink"><span class="text-danger">* </span>Image Link</label><br />'
                    + '<input type="text" name="inputProductImageLink" id="inputProductImageLink" class="form-control formController" placeholder="https://example.com/img/image33.png" />'

                    + '<label for="inputProductPrice"><span class="text-danger">* </span>Price</label><br />'
                    + '<input type="text" name="inputProductPrice" id="inputProductPrice" class="form-control formController" placeholder="Price" />'

                    + '<label for="inputProductGtin"><span class="text-danger">* </span>Gtin</label><br />'
                    + '<input type="text" name="inputProductGtin" id="inputProductGtin" class="form-control formController" placeholder="gtin" />'

                + '</div>'

                );

            //Tilldelning av värden för inputs of selects lists

            $('#inputProductTitle').val(returnedProduct.Title);
            $('#inputProductDescription').val(returnedProduct.Description);
            $('#inputProductLink').val(returnedProduct.Link);
            $('#inputProductImageLink').val(returnedProduct.ImageLink);
            $('#inputProductPrice').val(returnedProduct.Price.Value);
            $('#inputProductGtin').val(returnedProduct.Gtin);

            $('#selectProductCategory option[value=' + returnedProduct.GoogleProductCategory + ']').attr('selected', 'selected');
            $('#selectProductAvailability option[value=' + returnedProduct.Availability + ']').attr('selected', 'selected');
            $('#selectProductCondition option[value=' + returnedProduct.Condition + ']').attr('selected', 'selected');
            $('#selectProductTargetCountry option[value=' + returnedProduct.TargetCountry + ']').attr('selected', 'selected');


            if (returnedProduct.ExpirationDate != null) {
                $('selectProductAvailabilityExpiryDate').val(returnedProduct.ExpirationDate);

            }

            if (returnedProduct.Availability == 'preorder') {
                $('selectProductAvailabilityDate').val(returnedProduct.AvailabilityDate)

            }

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
