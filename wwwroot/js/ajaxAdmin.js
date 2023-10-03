var ClickFistButton = false;
var ContentAdmin = $('#AdminContent');

function ButtomClick(button) {
    $('#AdminContent').empty();
    $(".AdminButton").removeClass("active");
    $("#AdminContentName").text(button.text());
    button.addClass("active");
}

function GetTable(type) {
    $("#PanelTable").attr("data-KnoId", type);

    $.get('/Api/GetTable',
    {
        Id: type
    },
    function (data) {
        ContentAdmin.html(data);

    }).fail(function (jqXHR, textStatus, errorThrown) {
        if (jqXHR.status == 404) {
            ContentAdmin.html("<div class=\"text-center\">Сервер не нашел данные</div>");
        }
    });
}

function GetResultTests() {
    
    $.get('/AdminPanel/GetResultTests',
    function (data) {
        ContentAdmin.html(data);

    }).fail(function (jqXHR, textStatus, errorThrown) {
        if (jqXHR.status == 404) {
            ContentAdmin.html("<div class=\"text-center\">Сервер не нашел данные</div>");
        }
    });
}

function ExcelForm() {
    
    $.get('/AdminPanel/GetExcelForm',
    function (data) {
        ContentAdmin.html(data);

    }).fail(function (jqXHR, textStatus, errorThrown) {
        if (jqXHR.status == 404) {
            ContentAdmin.html("<div class=\"text-center\">Сервер не нашел данные</div>");
        }
    });
}

$(function () {

    $("#AdminButton1").click(function (e) {
        ButtomClick($(this));
        GetTable(1)
    });

    $("#AdminButton2").click(function (e) {
        ButtomClick($(this));
        GetTable(2)
    });

    $("#AdminButton3").click(function (e) {
        ButtomClick($(this));
        GetTable(3)
    });

    $("#AdminButton4").click(function (e) {
        ButtomClick($(this));
        GetTable(4)
    });

    $("#AdminButton5").click(function (e) {
        ButtomClick($(this));
        GetResultTests();
    });

    $("#AdminButton6").click(function (e) {
        ButtomClick($(this));
        ExcelForm();
    });


    if (ClickFistButton)
        $(".AdminButton").first().click();
    
    
});