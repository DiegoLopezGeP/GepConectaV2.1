async function submitLogin(username, password) {
    const formData = new FormData();
    formData.append('username', username);
    formData.append('password', password);

    try {
        const response = await fetch('api/account/login', {
            method: 'POST',
            body: formData,
            credentials: 'same-origin' // <-- Asegura que la cookie HTTP set-cookie se guarde
        });

        return response.ok;
    } catch (error) {
        console.error('Error en login:', error);
        return false;
    }
}