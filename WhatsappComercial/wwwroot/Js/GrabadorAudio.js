window.grabadorAudio = {
    mediaRecorder: null,
    audioChunks: [],
    dotNetHelper: null,
    timerInterval: null,
    segundos: 0,
    ultimoBlobGrabado: null,

    // Método para consultar estado previo sin abrir micrófono
    verificarPermiso: async function () {
        try {
            if (!navigator.permissions || !navigator.permissions.query) return "prompt";
            const result = await navigator.permissions.query({ name: 'microphone' });
            return result.state; // 'granted', 'denied', 'prompt'
        } catch (e) {
            return "prompt";
        }
    },

    iniciar: async function (dotNetRef) {
        try {
            this.dotNetHelper = dotNetRef;
            this.audioChunks = [];
            this.segundos = 0;
            this.ultimoBlobGrabado = null;

            // 1. Pedir micrófono
            const stream = await navigator.mediaDevices.getUserMedia({ audio: true });

            // 2. Si la librería OpusMediaRecorder está cargada
            if (typeof OpusMediaRecorder !== 'undefined') {

                // TRUCO SEGURIDAD: Convertimos la URL remota del Worker en un Blob local para evitar SecurityError/CORS
                const workerUrl = 'https://cdn.jsdelivr.net/npm/opus-media-recorder@latest/encoderWorker.umd.js';
                const workerScript = `importScripts('${workerUrl}');`;
                const workerBlob = new Blob([workerScript], { type: 'text/javascript' });
                const workerBlobUrl = URL.createObjectURL(workerBlob);

                const workerOptions = {
                    encoderWorkerFactory: function () {
                        return new Worker(workerBlobUrl);
                    },
                    OggOpusEncoderWasmPath: 'https://cdn.jsdelivr.net/npm/opus-media-recorder@latest/OggOpusEncoder.wasm'
                };

                window.MediaRecorder = OpusMediaRecorder;
                this.mediaRecorder = new OpusMediaRecorder(stream, { mimeType: 'audio/ogg' }, workerOptions);

            } else {
                console.warn("OpusMediaRecorder no encontrado, usando MediaRecorder nativo.");
                this.mediaRecorder = new MediaRecorder(stream);
            }

            this.mediaRecorder.ondataavailable = (event) => {
                if (event.data && event.data.size > 0) {
                    this.audioChunks.push(event.data);
                }
            };

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

            if (err.name === 'NotAllowedError' || err.name === 'PermissionDeniedError') {
                alert("Permiso denegado. Permite el uso del micrófono en el navegador.");
            } else if (err.name === 'NotFoundError') {
                alert("No se detectó ningún micrófono conectado.");
            } else {
                alert("No se pudo iniciar la grabación de audio. Revisa la consola.");
            }

            return false;
        }
    },

    detenerYObtenerData: function () {
        return new Promise((resolve) => {
            clearInterval(this.timerInterval);

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

            const timeoutSeguridad = setTimeout(() => {
                console.warn("Timeout de seguridad: no se recibió onstop a tiempo.");
                resolverUnaVez(null);
            }, 8000);

            this.mediaRecorder.onstop = () => {
                clearTimeout(timeoutSeguridad);

                try {
                    let mimeType = this.mediaRecorder.mimeType || 'audio/ogg';
                    let cleanContentType = mimeType.split(';')[0].toLowerCase().trim();

                    if (cleanContentType.includes('webm')) {
                        cleanContentType = 'audio/ogg';
                    }

                    const blob = new Blob(this.audioChunks, { type: cleanContentType });

                    // Apagar los tracks del micrófono para liberar el hardware inmediatamente
                    if (this.mediaRecorder.stream) {
                        this.mediaRecorder.stream.getTracks().forEach(track => track.stop());
                    }

                    if (blob.size === 0) {
                        console.warn("El audio grabado está vacío (0 bytes).");
                        resolverUnaVez(null);
                        return;
                    }

                    this.ultimoBlobGrabado = blob;

                    let extension = '.ogg';
                    if (cleanContentType.includes('mp4')) extension = '.mp4';
                    if (cleanContentType.includes('aac')) extension = '.aac';

                    resolverUnaVez({
                        contentType: cleanContentType,
                        tamano: blob.size,
                        nombreSugerido: `nota_voz_${Date.now()}${extension}`
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
        this.ultimoBlobGrabado = null;
    },

    obtenerStreamAudio: function () {
        return this.ultimoBlobGrabado;
    }
};