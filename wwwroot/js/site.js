
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

// Получаем куки имени и группы пользователя
var cookieValue = getCookieValue("Name");
var cookieValue2 = getCookieValue("Group");
var role = getCookieValue("Role");
document.getElementById("NameUser").innerText = cookieValue + " " + cookieValue2;

if (role == "User")
{
    let btn = document.getElementById("Button5");
    btn.parentNode.removeChild(btn);
}



