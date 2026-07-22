let estado = {
    cargando: false,
    altoPrevio: 0,
    scrollTopPrevio: 0,
    elemento: null,
    dotNetRef: null
};

export function inicializar(elemento, dotNetRef) {
    if (!elemento) return;

    estado.elemento = elemento;
    estado.dotNetRef = dotNetRef;

    elemento.addEventListener('scroll', () => {
        // Si está cargando un bloque, ignoramos el evento de scroll
        if (estado.cargando) return;

        // Cuando la barra esté a 30px o menos del borde superior
        if (elemento.scrollTop <= 30) {
            estado.cargando = true;

            // 1. Guardamos la foto exactísima de las dimensiones antes de pedir los nuevos mensajes
            estado.altoPrevio = elemento.scrollHeight;
            estado.scrollTopPrevio = elemento.scrollTop;

            // 2. Avisamos a Blazor
            dotNetRef.invokeMethodAsync("NotificarScrollArriba");
        }
    });
}

// Esta función la llama Blazor justo DESPUÉS de actualizar la lista en el DOM
export function restaurarPosicionScroll() {
    const el = estado.elemento;
    if (!el) return;

    // Usamos doble requestAnimationFrame para garantizar que el DOM ya calculó las nuevas alturas
    requestAnimationFrame(() => {
        requestAnimationFrame(() => {
            const nuevoAlto = el.scrollHeight;
            const diferenciaAlturas = nuevoAlto - estado.altoPrevio;

            // Desplazamos exactamente la diferencia para dar la sensación de continuidad fluida
            el.scrollTop = estado.scrollTopPrevio + diferenciaAlturas;

            // Desbloqueamos para que el usuario pueda seguir scrolleando
            estado.cargando = false;
        });
    });
}

export function irAlFinal(elemento) {
    if (!elemento) return;
    requestAnimationFrame(() => {
        elemento.scrollTop = elemento.scrollHeight;
        estado.cargando = false;
    });
}