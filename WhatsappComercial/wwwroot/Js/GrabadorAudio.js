window.grabadorAudio = {
    mediaRecorder: null,
    audioChunks: [],
    dotNetHelper: null,
    timerInterval: null,
    segundos: 0,

    iniciar: async function (dotNetRef) {
        try {
            this.dotNetHelper = dotNetRef;
            this.audioChunks = [];
            this.segundos = 0;

            const stream = await navigator.mediaDevices.getUserMedia({ audio: true });

            // Determinar formato soportado
            let mime = 'audio/webm;codecs=opus';
            if (!MediaRecorder.isTypeSupported(mime)) {
                mime = 'audio/ogg;codecs=opus';
            }
            if (!MediaRecorder.isTypeSupported(mime)) {
                mime = ''; // Usa el formato por defecto del navegador
            }

            const options = mime ? { mimeType: mime } : {};
            this.mediaRecorder = new MediaRecorder(stream, options);

            this.mediaRecorder.ondataavailable = (event) => {
                if (event.data && event.data.size > 0) {
                    this.audioChunks.push(event.data);
                }
            };

            // Pedir fragmentos de audio cada 250ms para asegurar que los chunks se llenen
            this.mediaRecorder.start(250);

            // Cronómetro
            this.timerInterval = setInterval(() => {
                this.segundos++;
                const min = Math.floor(this.segundos / 60).toString().padStart(2, '0');
                const sec = (this.segundos % 60).toString().padStart(2, '0');
                if (this.dotNetHelper) {
                    this.dotNetHelper.invokeMethodAsync('ActualizarTiempoJS', `${min}:${sec}`);
                }
            }, 1000);

            return true;
        } catch (err) {
            console.error("Error al acceder al micrófono:", err);
            alert("No se pudo acceder al micrófono. Verifica los permisos de tu navegador.");
            return false;
        }
    },

    detenerYObtenerData: function () {
        return new Promise((resolve) => {
            clearInterval(this.timerInterval);

            // Bandera para evitar llamar resolve() más de una vez
            // (podría pasar si, por ejemplo, onstop y un timeout de seguridad
            // se disparan casi al mismo tiempo)
            let yaResuelto = false;
            const resolverUnaVez = (valor) => {
                if (yaResuelto) return;
                yaResuelto = true;
                resolve(valor);
            };

            if (!this.mediaRecorder || this.mediaRecorder.state === "inactive") {
                resolverUnaVez(null);
                return;
            }

            // Salvavidas: si por cualquier razón onstop nunca dispara
            // (o el navegador se cuelga procesando el blob), no dejamos
            // la promesa colgada para siempre esperando el timeout de .NET.
            const timeoutSeguridad = setTimeout(() => {
                console.warn("Timeout de seguridad: no se recibió onstop a tiempo.");
                resolverUnaVez(null);
            }, 8000);

            this.mediaRecorder.onstop = () => {
                clearTimeout(timeoutSeguridad);

                try {
                    const mimeType = this.mediaRecorder.mimeType || 'audio/webm';
                    const blob = new Blob(this.audioChunks, { type: mimeType });

                    if (this.mediaRecorder.stream) {
                        this.mediaRecorder.stream.getTracks().forEach(track => track.stop());
                    }

                    if (blob.size === 0) {
                        console.warn("El audio grabado está vacío (0 bytes).");
                        resolverUnaVez(null);
                        return;
                    }

                    // En vez de convertir a Base64, guardamos el blob y devolvemos
                    // metadata + el tipo de contenido. El blob se recupera después
                    // vía streaming con obtenerStreamAudio().
                    this.ultimoBlobGrabado = blob;

                    resolverUnaVez({
                        contentType: mimeType.split(';')[0],
                        tamano: blob.size
                    });

                } catch (err) {
                    console.error("Error procesando audio:", err);
                    resolverUnaVez(null);
                }
            };

            this.mediaRecorder.stop();
        });
    },

    cancelar: function () {
        clearInterval(this.timerInterval);
        if (this.mediaRecorder && this.mediaRecorder.state !== "inactive") {
            this.mediaRecorder.stop();
            if (this.mediaRecorder.stream) {
                this.mediaRecorder.stream.getTracks().forEach(track => track.stop());
            }
        }
        this.audioChunks = [];
    },
    obtenerStreamAudio: function () {
        return this.ultimoBlobGrabado;
    }
};