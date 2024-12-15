console.log("validateForm.js cargado correctamente");
$(document).ready(function () {
    const emailInput = $("#email");
    const passwordInput = $("#password");
    const emailError = $("#emailError");
    const passwordError = $("#passwordError");
    const submitBtn = $("#submitBtn");

    // Función para validar el email
    function validateEmail(email) {
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        return emailRegex.test(email);
    }

    // Función para validar la contraseña
    function validatePassword(password) {
        const passwordRegex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$/;
        return passwordRegex.test(password);
    }

    // Validar formulario en tiempo real
    function validateForm() {
        const isEmailValid = validateEmail(emailInput.val());
        const isPasswordValid = validatePassword(passwordInput.val());

        console.log("Email valid:", isEmailValid); // Log para depuración
        console.log("Password valid:", isPasswordValid); // Log para depuración

        if (!isEmailValid) {
            emailError.show();
        } else {
            emailError.hide();
        }

        if (!isPasswordValid) {
            passwordError.show();
        } else {
            passwordError.hide();
        }

        // Habilitar el botón solo si ambas validaciones son correctas
        submitBtn.prop("disabled", !(isEmailValid && isPasswordValid));
    }

    // Eventos en tiempo real
    emailInput.on("input", validateForm);
    passwordInput.on("input", validateForm);
});
