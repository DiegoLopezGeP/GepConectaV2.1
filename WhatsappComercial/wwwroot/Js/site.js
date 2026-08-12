// Instancias globales de audio y estado
let audioNotificacion = new Audio('audio/notificacion.mp3');
let audioDesbloqueado = false;

// Función interna para desbloquear el audio con la primera interacción del usuario
function desbloquearAudio() {
    if (audioDesbloqueado) return;

    audioNotificacion.volume = 0;
    audioNotificacion.play().then(() => {
        audioNotificacion.pause();
        audioNotificacion.currentTime = 0;
        audioNotificacion.volume = 0.5; // Restablecemos el volumen al 50%
        audioDesbloqueado = true;

        // Limpiamos los eventos una vez desbloqueado
        document.removeEventListener('click', desbloquearAudio);
        document.removeEventListener('keydown', desbloquearAudio);
        document.removeEventListener('touchstart', desbloquearAudio);
        console.log("Audio de notificaciones desbloqueado con éxito.");
    }).catch(() => {
        console.warn("Esperando interacción del usuario para habilitar audio...");
    });
}

// Escuchar el primer clic, toque o tecla del usuario
document.addEventListener('click', desbloquearAudio);
document.addEventListener('touchstart', desbloquearAudio);
document.addEventListener('keydown', desbloquearAudio);

// Objeto Global BlazorInterop con todos sus métodos
window.blazorInterop = {
    scrollToBottom: function (element, smooth) {
        if (!element) return;

        requestAnimationFrame(() => {
            requestAnimationFrame(() => {
                element.scrollTo({
                    top: element.scrollHeight,
                    behavior: smooth ? 'smooth' : 'auto'
                });
            });
        });
    },

    esDispositivoMovil: function () {
        return window.innerWidth <= 768;
    },

    reproducirSonidoNotificacion: function () {
        try {
            audioNotificacion.currentTime = 0;
            var playPromise = audioNotificacion.play();
            if (playPromise !== undefined) {
                playPromise.catch(function (error) {
                    console.warn('El audio aún no ha sido interactuado por el usuario o fue bloqueado:', error);
                });
            }
        } catch (e) {
            console.error('Error al reproducir el sonido:', e);
        }
    }
};
window.emojiHelper = {
    inicializar: function (dotNetHelper, pickerId) {
        const picker = document.getElementById(pickerId);
        if (!picker) return;

        // Limpiamos listeners previos para evitar duplicados al abrir/cerrar varias veces
        picker.replaceWith(picker.cloneNode(true));
        const nuevoPicker = document.getElementById(pickerId);

        nuevoPicker.addEventListener('emoji-click', event => {
            const emoji = event.detail.unicode;

            if (emoji) {
                // Enviamos el emoji directamente a C#
                dotNetHelper.invokeMethodAsync('OnEmojiSeleccionado', emoji);
            }
        });
    }
};