window.blazorInterop = {
    scrollToBottom: function (element, smooth) {

        if (!element)
            return;

        requestAnimationFrame(() => {

            requestAnimationFrame(() => {

                element.scrollTo({
                    top: element.scrollHeight,
                    behavior: smooth ? 'smooth' : 'auto'
                });

            });

        });
    },
    esDispositivoMovil: function() {
        return window.innerWidth <= 768;
    }
}