// Mostrar/ocultar contraseña
function togglePassword(inputId, iconId) {
    const input = document.getElementById(inputId);
    const icon = document.getElementById(iconId);
    if (input.type === "password") {
        input.type = "text";
        icon.classList.remove("bi-eye-slash");
        icon.classList.add("bi-eye");
    } else {
        input.type = "password";
        icon.classList.remove("bi-eye");
        icon.classList.add("bi-eye-slash");
    }
}

// Generar contraseña segura
function generateSecurePassword() {
    const lower = "abcdefghijklmnopqrstuvwxyz";
    const upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    const numbers = "0123456789";
    const special = "!@#$%&*()-_=+";
    const all = lower + upper + numbers + special;

    let password = "";

    // Aseguramos al menos un número
    password += numbers[Math.floor(Math.random() * numbers.length)];

    // Seleccionamos 2 de las otras 3 categorías restantes (minúsculas, mayúsculas, especiales)
    let categories = [lower, upper, special];
    let selected = categories.sort(() => 0.5 - Math.random()).slice(0, 2);

    // Agregamos un carácter de cada categoría seleccionada
    selected.forEach(cat => {
        password += cat[Math.floor(Math.random() * cat.length)];
    });

    // Completa hasta 12 caracteres
    for (let i = password.length; i < 12; i++) {
        password += all[Math.floor(Math.random() * all.length)];
    }

    // Mezcla los caracteres
    password = password.split('').sort(() => Math.random() - 0.5).join('');

    // Devuelve la contraseña
    return password;
}

