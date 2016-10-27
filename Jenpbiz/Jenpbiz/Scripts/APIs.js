$(document).ready(function () {





});

function GetProducts() {

    $.ajax({
        method: 'GET',
        url: 'GoogleAPI/API_GetProducts/',
        //contentType: 'json',
        dataType: 'json',
        //data: {},
        success: function (data) {
            console.log("JQuery AJAX authorizer success function.");
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