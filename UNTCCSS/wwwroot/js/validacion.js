window.SoloNumeros = (inputId) => {
    const inputField = document.getElementById(inputId);

    if (inputField) {
        // Elimina cualquier evento previo antes de añadir uno nuevo
        inputField.removeEventListener('keydown', validarSoloNumeros);
        inputField.addEventListener('keydown', validarSoloNumeros);
    } else {
        console.warn(`No se encontró el input con ID: ${inputId}`);
    }
};

// Función que valida si solo se ingresan números
function validarSoloNumeros(event) {
    if (!event || !event.key) return; 

    const key = event.key;
    if (!/^\d$/.test(key) && key !== "Backspace" && key !== "Tab" &&
        key !== "ArrowLeft" && key !== "ArrowRight") {
        event.preventDefault();
    }
}
