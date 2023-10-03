var StartActionButton = true;
const ContentBox = document.getElementById("Content");
// Функци получения значений куки
function getCookieValue(cookieName) {
    var name = cookieName + "=";
    var decodedCookie = decodeURIComponent(document.cookie);
    var cookieArray = decodedCookie.split(';');
    for (var i = 0; i < cookieArray.length; i++) {
        var cookie = cookieArray[i];
        while (cookie.charAt(0) === ' ') {
            cookie = cookie.substring(1);
        }
        if (cookie.indexOf(name) === 0) {
            return cookie.substring(name.length, cookie.length);
        }
    }
    return "";
}

// Поль пользователя, отпределяет интерфейс и его функционал
var Role = getCookieValue("Role");

// Смена активной кнопки на другой цвет 
function UpdataActiveButton() {
    $(".ActiveButtonNav").removeClass("ActiveButtonNav");
}

// Создание записей
function CreateKnowledges(data) {

    let accordionWrapper = document.createElement('div');
    accordionWrapper.classList.add('accordion');
    accordionWrapper.id = 'accordionMain';

    for (let kn in data) {
        let accordionItem = document.createElement('div');
        accordionItem.classList.add('accordion-item');

        let header = document.createElement('h2');
        header.classList.add('accordion-header');
        header.id = 'heading' + data[kn].id;

        let btn = document.createElement('button');
        btn.classList.add('accordion-button');
        btn.classList.add('collapsed');
        btn.type = 'button';
        btn.dataset.bsToggle = 'collapse';
        btn.dataset.bsTarget = '#collapse' + data[kn].id;
        btn.setAttribute('aria-expanded', 'true');
        btn.setAttribute('aria-controls', 'collapse' + data[kn].id);

        let i = document.createElement('i');
        i.classList.add('bi');
        i.classList.add('bi-journal-bookmark');
        i.classList.add('SizeIconKnowledge');

        let span = document.createElement('span');
        span.classList.add('ms-2');
        span.textContent = data[kn].name;

        let collapseDiv = document.createElement('div');
        collapseDiv.id = 'collapse' + data[kn].id;
        collapseDiv.classList.add('accordion-collapse');
        collapseDiv.classList.add('collapse');
        //collapseDiv.classList.add('show');
        collapseDiv.setAttribute('aria-labelledby', 'heading' + data[kn].id);
        collapseDiv.setAttribute('data-bs-parent', '#accordionMain');

        let bodyDiv = document.createElement('div');
        bodyDiv.classList.add('accordion-body');

        let description = document.createElement('span');
        description.id = "description" + data[kn].id;

        if (data[kn].description == null)
            description.textContent = "Описания нет";
        else
            description.textContent = data[kn].description;

        description.style.display = "block";

        let ContainerFiles = document.createElement("div");
        ContainerFiles.classList.add("p-3");
        ContainerFiles.classList.add("d-flex");
        ContainerFiles.classList.add("flex-wrap");
        
        ContainerFiles.id = "ContainerFiles" + data[kn].id;

        // Контейнер с текстом и полем редактора записи
        let descriptionContainer = document.createElement("div");
        descriptionContainer.classList.add("d-flex");
        descriptionContainer.appendChild(description);

        // Поле для ввода нового описания записи, по умолчанию
        let inputText = document.createElement("textarea");
        inputText.style.display = "none";
        inputText.classList.add("form-control");

        inputText.addEventListener("keydown", function (e) {
            if (e.keyCode === 13) {
                $.ajax({
                    url: '/Api/PutKnowledge',
                    type: 'PUT',
                    data: {
                        Id: data[kn].id,
                        Text: e.target.value
                    },
                    success: function () {
                        inputText.style.display = "none";
                        description.style.display = "block";
                        description.textContent = e.target.value;

                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        console.error('Произошла ошибка:', textStatus,
                            errorThrown);
                    }
                });
            }
        });

        descriptionContainer.appendChild(inputText);


        btn.appendChild(i);
        btn.appendChild(span);
        header.appendChild(btn);
        accordionItem.appendChild(header);

        bodyDiv.appendChild(descriptionContainer);
        bodyDiv.appendChild(ContainerFiles);

        collapseDiv.appendChild(bodyDiv);
        accordionItem.appendChild(collapseDiv);




        if (Role == "Admin") {
            let button = document.createElement("div");
            button.classList.add("p-2");
            button.style.cursor = "pointer";
            //button.classList.add("bg-success");
            button.classList.add("rounded-pill");
            button.classList.add("border");
            button.classList.add("link-success");
            let icon = document.createElement("i");
            icon.classList.add("bi");
            icon.classList.add("bi-download");
            icon.classList.add("me-1");

            let span = document.createElement("span");
            span.textContent = "Загрузить";

            button.appendChild(icon);
            button.appendChild(span);

            span.setAttribute("data-KnoId", data[kn].id);
            span.addEventListener("click", ButtonDown);

            ContainerFiles.appendChild(button);


            // Кнопка редактирования текста
            let EditTextButtin = document.createElement("i");
            EditTextButtin.classList.add("bi");
            EditTextButtin.classList.add("bi-pencil-square");
            EditTextButtin.classList.add("ms-1");
            EditTextButtin.setAttribute("data-KnoId", data[kn].id);
            EditTextButtin.addEventListener("click", function () {
                description.style.display = "none";
                inputText.style.display = "";
                inputText.textContent = description.innerText;

            });
            descriptionContainer.appendChild(EditTextButtin);
        }
        accordionWrapper.appendChild(accordionItem);

        CreateFiles(ContainerFiles, data[kn].id);
    }
    ContentBox.appendChild(accordionWrapper);
}


// Добавление файла к записи при загрузке станицы
function CreateFiles(parent, id) {
    $.get("/Api/GetFiles",
        { Id: id })
        .done(function (data) {

            for (const key in data) {
                let file = document.createElement("div");
                file.classList.add("p-2");
                file.classList.add("d-flex");
                file.classList.add("rounded-pill");
                file.classList.add("border");
                file.classList.add("m-1");
                file.style.cursor = "pointer";

                let icon = document.createElement("i");
                icon.classList.add("bi");

                switch (data[key].type) {
                    case ".txt":
                        icon.classList.add("bi-filetype-txt");
                        break;
                    case ".doc":
                    case ".dox":
                    case ".docx":
                        icon.classList.add("bi-filetype-docx");
                        break;

                    default:
                        icon.classList.add("bi-file-earmark");
                        break;
                }


                let a = document.createElement("a");
                a.textContent = data[key].name;
                a.setAttribute("href", "/files/" + data[key].name);
                a.setAttribute("download", data[key].name);

                file.appendChild(icon);
                file.appendChild(a);

                if (Role == "Admin") {
                    let DeleteButton = document.createElement("i");
                    DeleteButton.classList.add("bi");
                    DeleteButton.classList.add("bi-x-lg");
                    DeleteButton.style.color = "Red";
                    DeleteButton.addEventListener("click", function (e) {

                        $.ajax({
                            url: '/Api/DeleteFile',
                            type: 'DELETE',
                            data: {
                                idKnowledge: id,
                                idFile: data[key].id
                            },
                            success: function () {
                                file.remove();
                            },
                            error: function (jqXHR, textStatus, errorThrown) {
                                console.error('Произошла ошибка:', textStatus,
                                    errorThrown);
                            }
                        });
                    });
                    file.appendChild(DeleteButton);
                }

                parent.appendChild(file);

            }
        })
        .fail(function (error) {

            console.error(error);
            reject(error);
        });
}

// Создание файла на клиенте
function CreateFile(parent, data, id) {
    let file = document.createElement("div");
    file.classList.add("p-2");
    file.classList.add("d-flex");
    file.classList.add("rounded-pill");
    file.classList.add("border");
    file.classList.add("m-1");
    file.style.cursor = "pointer";

    let icon = document.createElement("i");
    icon.classList.add("bi");

    switch (data["type"]) {
        case ".txt":
            icon.classList.add("bi-filetype-txt");
            break;
        case ".doc":
        case ".dox":
        case ".docx":
            icon.classList.add("bi-filetype-docx");
            break;

        default:
            icon.classList.add("bi-file-earmark");
            break;
    }


    let span = document.createElement("span");
    span.textContent = data["name"];

    file.appendChild(icon);
    file.appendChild(span);


    if (Role == "Admin") {
        let DeleteButton = document.createElement("i");
        DeleteButton.classList.add("bi");
        DeleteButton.classList.add("bi-x-lg");
        DeleteButton.style.color = "Red";
        DeleteButton.addEventListener("click", function (e) {

            $.ajax({
                url: '/Api/DeleteFile',
                type: 'DELETE',
                data: {
                    idKnowledge: id,
                    idFile: data["id"]
                },
                success: function () {
                    file.remove();
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    console.error('Произошла ошибка:', textStatus,
                        errorThrown);
                }
            });
        });
        file.appendChild(DeleteButton);
    };
    parent.appendChild(file);
}

// Нажатие кнопки загрузки файла
async function ButtonDown(e) {
    e.stopPropagation();
    try {
        const [fileHandle] = await window.showOpenFilePicker();
        const file = await fileHandle.getFile();

        const formData = new FormData();
        formData.append('file', file);
        formData.append('Id', e.target.getAttribute("data-KnoId"));
        let id = "ContainerFiles" + e.target.getAttribute("data-KnoId");
        $.ajax({
            url: '/Api/PostFile',
            type: 'POST',
            data: formData,
            contentType: false,
            processData: false,
            success: function (data) {


                var parent = document.getElementById(id);
                CreateFile(parent, data, e.target.getAttribute("data-KnoId"));

                console.log("Файл успешно отправлен на сервер");
            },
            error: function (data) {

                console.log("Ошибка при отправке файла на сервер");
            }
        });

    } catch (error) {
        console.error(error);

    }
}

// Формирование ответа от поиска
async function CreateResultSearh(data) {

    for (const key in data) {
        var Title = document.createElement("span");
        Title.classList.add("fs-3");
        Title.classList.add("FontAbel");
        Title.classList.add("FontAbel");
        Title.style.color = "#0D6EFD"
        var TitleDiv = document.createElement("div");
        TitleDiv.appendChild(Title)
        TitleDiv.classList.add("text-center");
        TitleDiv.classList.add("border-bottom");
        TitleDiv.classList.add("border-primary");
        TitleDiv.classList.add("mb-2");
        console.log(key);


        if (key == 1)
            Title.innerHTML = "Лекции";
        if (key == 2)
            Title.innerHTML = "Лабы";
        if (key == 3)
            Title.innerHTML = "Тесты";
        if (key == 4)
            Title.innerHTML = "ОКР";


        ContentBox.appendChild(TitleDiv);
        if (key == 3) {
            CreateTests(data[key]);
        }
        else {
            CreateKnowledges(data[key]);
        };
    }


}

async function BuildTest(KnowledgeId)
{
    console.log(KnowledgeId);
    $('#Content').empty();
    UpdataActiveButton();
    $("#Title").text("Тест");

    $.ajax({
        url: "/Api/GetTest",
        type: "GET",
        data : {
            Id : KnowledgeId
        },
        success: function (data)
        {
            $('#Content').html(data);
        }
    });
}


// Формирование тестов
async function CreateTests(data) {
    let accordionWrapper = document.createElement('div');
    accordionWrapper.classList.add('accordion');
    accordionWrapper.id = 'accordionMain';

    for (let kn in data) {
        let accordionItem = document.createElement('div');
        accordionItem.classList.add('accordion-item');

        let header = document.createElement('h2');
        header.classList.add('accordion-header');
        header.id = 'heading' + data[kn].id;

        let btn = document.createElement('button');
        btn.classList.add('accordion-button');
        btn.classList.add('collapsed');
        btn.type = 'button';
        btn.dataset.bsToggle = 'collapse';
        btn.dataset.bsTarget = '#collapse' + data[kn].id;
        btn.setAttribute('aria-expanded', 'false');
        btn.setAttribute('aria-controls', 'collapse' + data[kn].id);

        let i = document.createElement('i');
        i.classList.add('bi');
        i.classList.add('bi-clipboard-check');
        i.classList.add('SizeIconKnowledge');

        let span = document.createElement('span');
        span.classList.add('ms-2');
        span.textContent = data[kn].name;

        let collapseDiv = document.createElement('div');
        collapseDiv.id = 'collapse' + data[kn].id;
        collapseDiv.classList.add('accordion-collapse');
        collapseDiv.classList.add('collapse');
        //collapseDiv.classList.add('show');
        collapseDiv.setAttribute('aria-labelledby', 'heading' + data[kn].id);
        collapseDiv.setAttribute('data-bs-parent', '#accordionMain');

        let bodyDiv = document.createElement('div');
        bodyDiv.classList.add('accordion-body');

        let description = document.createElement('span');
        description.id = "description" + data[kn].id;

        if (data[kn].description == null)
            description.textContent = "Описания нет";
        else
            description.textContent = data[kn].description;

        description.style.display = "block";

        // Контейнер с текстом и полем редактора записи
        let descriptionContainer = document.createElement("div");
        descriptionContainer.classList.add("d-flex");
        descriptionContainer.appendChild(description);

        // Поле для ввода нового описания записи, по умолчанию
        let inputText = document.createElement("textarea");
        inputText.style.display = "none";
        inputText.classList.add("form-control");

        let StartButton = document.createElement("button")
        StartButton.classList.add("btn");
        StartButton.classList.add("mt-2");
        StartButton.classList.add("btn-info");
        StartButton.innerText = "Начать";
        StartButton.addEventListener("click", function(e)
        {
            BuildTest(data[kn].id);
        });
        

        inputText.addEventListener("keydown", function (e) {
            if (e.keyCode === 13) {
                $.ajax({
                    url: '/Api/PutKnowledge',
                    type: 'PUT',
                    data: {
                        Id: data[kn].id,
                        Text: e.target.value
                    },
                    success: function () {
                        inputText.style.display = "none";
                        description.style.display = "block";
                        description.textContent = e.target.value;

                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        console.error('Произошла ошибка:', textStatus,
                            errorThrown);
                    }
                });
            }
        });

        descriptionContainer.appendChild(inputText);


        btn.appendChild(i);
        btn.appendChild(span);
        header.appendChild(btn);
        accordionItem.appendChild(header);

        bodyDiv.appendChild(descriptionContainer);
        bodyDiv.appendChild(StartButton);
        collapseDiv.appendChild(bodyDiv);
        accordionItem.appendChild(collapseDiv);

        if (Role == "Admin") {
            // Кнопка редактирования текста
            let EditTextButtin = document.createElement("i");
            EditTextButtin.classList.add("bi");
            EditTextButtin.classList.add("bi-pencil-square");
            EditTextButtin.classList.add("ms-1");
            EditTextButtin.setAttribute("data-KnoId", data[kn].id);
            EditTextButtin.addEventListener("click", function () {
                description.style.display = "none";
                inputText.style.display = "";
                inputText.textContent = description.innerText;

            });
            descriptionContainer.appendChild(EditTextButtin);
        }
        accordionWrapper.appendChild(accordionItem);
    }
    ContentBox.appendChild(accordionWrapper);
}


$(function () {
    $("#Button1").click(function () {
        $('#Content').empty();
        UpdataActiveButton();
        $(this).addClass("ActiveButtonNav");
        $("#Title").text("Лекции");

        $.get('/Api/GetKnowledge',
            {
                Type: 1
            },
            function (data) {
                $('#Content').html(data);
                console.log(data);
                CreateKnowledges(data);

            }).fail(function (jqXHR, textStatus, errorThrown) {

                if (jqXHR.status == 404) {
                    $('#Content').html("<div class=\"text-center\">Сервер не нашел данные</div>");
                }
            }
            );

    });
    $("#Button2").click(function () {
        $('#Content').empty();
        UpdataActiveButton();
        $(this).addClass("ActiveButtonNav");
        $("#Title").text("Л/р");

        $.get('/Api/GetKnowledge',
            {
                Type: 2
            },
            function (data) {
                $('#Content').html(data);
                console.log(data);
                CreateKnowledges(data);

            }).fail(function (jqXHR, textStatus, errorThrown) {

                if (jqXHR.status == 404) {
                    $('#Content').html("<div class=\"text-center\">Сервер не нашел данные</div>");
                }
            }
            );

    });
    $("#Button3").click(function () {
        $('#Content').empty();
        UpdataActiveButton();
        $(this).addClass("ActiveButtonNav");
        $("#Title").text("Тесты");

        $.get('/Api/GetKnowledge',
            {
                Type: 3
            },
            function (data) {
                $('#Content').html(data);
                console.log(data);
                CreateTests(data);

            }).fail(function (jqXHR, textStatus, errorThrown) {

                if (jqXHR.status == 404) {
                    $('#Content').html("<div class=\"text-center\">Сервер не нашел данные</div>");
                }
            }
            );
    });
    $("#Button4").click(function () {
        $('#Content').empty();
        UpdataActiveButton();
        $(this).addClass("ActiveButtonNav");
        $("#Title").text("ОКР");

        $.get('/Api/GetKnowledge',
            {
                Type: 4
            },
            function (data) {
                $('#Content').html(data);
                console.log(data);
                CreateKnowledges(data);

            }).fail(function (jqXHR, textStatus, errorThrown) {

                if (jqXHR.status == 404) {
                    $('#Content').html("<div class=\"text-center\">Сервер не нашел данные</div>");
                }
            }
            );


    });
    $("#Button5").click(function () {
        $('#Content').empty();
        UpdataActiveButton();
        $(this).addClass("ActiveButtonNav");
        $("#Title").text("Admin");

        $.get(
            '/Api/AdminPanel',
            function (data) {
                $('#Content').html(data);

            }).fail(function (jqXHR, textStatus, errorThrown) {

                if (jqXHR.status == 404) {
                    $('#Content').html("<div class=\"text-center\">Сервер не нашел данные</div>");
                }
            }
            );
            

    });
    $("#search-button").click(function () {
        $('#Content').empty();
        UpdataActiveButton();
        $("#Title").text("Поиск..");


        //let input = document.getElementById("search-input");

        $.get('/Api/GetKnowledgeAll',
            {
                str: $('#search-input').val()
            },
            function (data) {
                $('#Content').html(data);
                console.log(data);
                CreateResultSearh(data);

            }).fail(function (jqXHR, textStatus, errorThrown) {

                if (jqXHR.status == 404) {
                    $('#Content').html("<div class=\"text-center\">Сервер не нашел данные</div>");
                }
            }
            );

    });



    // По-умолчанию грузится раздел "Лекций"
    if (StartActionButton) {
        let btn = document.getElementById('Button1')
        btn.click();
    }


});
