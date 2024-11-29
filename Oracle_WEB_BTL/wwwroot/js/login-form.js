var inputFields = document.querySelectorAll('.input-field');
var btn = document.querySelector('.login');
var Warning = document.querySelector(".warning");

function check_null(input) {
    return input.value.trim() === '';
}

function check() {
    for (let input of inputFields) {
        if (check_null(input)) {
            return false;
        }
    }
    return true;
}

Object.prototype.enable = function () {
    this.removeAttribute('disabled');
    this.style.backgroundColor = "gray";
    this.style.color = 'white';
}

Object.prototype.disable = function () {
    this.setAttribute('disabled', 'disabled');
    this.style.backgroundColor = "whitesmoke";
    this.style.color = 'black';
    Warning.innerText = "";
}

inputFields.forEach(input => {
    input.oninput = function () {
        if (!check()) {
            btn.disable();
        } else {
            btn.enable();
        }
    }
});