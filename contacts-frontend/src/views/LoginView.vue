<script lang="ts">
import {ref} from 'vue';
import {useRouter} from 'vue-router';

export default{
    setup() {
        const router = useRouter();
        const email = ref('');
        const password = ref('');

        const goToContacts = async (login = false) => {
            if(!login){
                localStorage.setItem('userEmail', 'Guest')
                localStorage.setItem('isLoggedIn', 'false')
                router.push('/contacts')
                return;
            }

            try {
                const response = await fetch('/api/auth/login',{
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({ email: email.value, password: password.value })
                })
                
                if(!response.ok) {
                    alert('Login failed: ' + response.statusText)
                    return;
                }

                const data = await response.json()
                localStorage.setItem('userEmail', email.value)
                localStorage.setItem('token', data.token)
                localStorage.setItem('isLoggedIn', 'true')
                router.push('/contacts')
            } catch (error) {
                console.error(error)
                alert("Server error")
            }
        }

        return {
            email,
            password,
            goToContacts
        }
    }
}
</script>

<template>
    <div>
        <h1>LOGIN</h1>
        <input v-model="email" placeholder="Email" />
        <input v-model="password" placeholder="Password" type="password" />
        <button @click="goToContacts(true)">Login</button>
        <button @click="goToContacts(false)">Go As Guest</button>
    </div>
</template>