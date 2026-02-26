<script lang="ts">
import {ref, onMounted} from 'vue';

interface Category { id: number; name: string; }
interface Subcategory { id: number; name: string; categoryId: number; }

interface Contact {
    id: number;
    name: string;
    surname: string;
    email: string;
    phone: string;
    birthDate: string;
    password: string;
    categoryId: number;
    subcategoryId: number | null;
    customSubcategory?: string;
}

export default {
    setup() {
        const contacts = ref<Contact[]>([])
        const selectedContact = ref<Contact | null>(null)

        const categories = ref<Category[]>([])
        const subcategories = ref<Subcategory[]>([])

        const userEmail = ref('Guest')
        const isLoggedIn = ref(false)
        const token = localStorage.getItem('token') || ''

        const fetchContacts = async () => {
            try {
                const response = await fetch('/api/contacts')
                if(!response.ok) {
                    throw new Error('Error fetching contacts: ' + response.statusText)
                }
                const data: Contact[] = await response.json()
                contacts.value = data

                console.log("Contacts loaded:", contacts.value)
            } catch (error) {
                console.error('Błąd:', error)
            }
        }

        const fetchCategories = async () => {
            const res = await fetch('/api/categories')
            categories.value = await res.json()
            console.log("Categories loaded:", categories.value);
        }

        const fetchSubcategories = async () => {
            const res = await fetch('/api/subcategories')
            subcategories.value = await res.json()
        }

        onMounted(async () => {
            userEmail.value = localStorage.getItem('userEmail') || 'Guest'
            isLoggedIn.value = localStorage.getItem('isLoggedIn') === 'true'
            
            await fetchContacts()
            await fetchCategories()
            await fetchSubcategories()
        })

        const selectContact = (contact: Contact) => {
            // Kopia kontaktu, żeby nie modyfikować listy bezpośrednio
            selectedContact.value = { ...contact }

            console.log("Selected contact:", selectedContact.value)
            console.log("contact.subcategoryId:", contact.subcategoryId)
            console.log("Subcategories:", subcategories.value)
        }

        const addContact = async () => {
            console.log("Adding contact:", selectedContact.value)

            try {
                const response = await fetch('/api/contacts', {
                    method: 'POST',
                    headers: { 
                        'Content-Type': 'application/json',
                        'Authorization': `Bearer ${token}`
                    },
                    body: JSON.stringify(selectedContact.value)
                })
                if(!response.ok) {
                    throw new Error('Error adding contact: ' + response.statusText)
                }
                const addedContact = await response.json()
                contacts.value.push(addedContact)
                selectedContact.value = createEmptyContact();
            } catch (error) {
                console.error('Error:', error)
            }
        }

        const updateContact = async () => {
            if (!selectedContact.value) return
            try {
                const response = await fetch(`/api/contacts/${selectedContact.value.id}`, {
                    method: 'PUT',
                    headers: { 
                        'Content-Type': 'application/json',
                        'Authorization': `Bearer ${token}`
                    },
                    body: JSON.stringify(selectedContact.value)
                })
                if(!response.ok) {
                    throw new Error('Error updating contact: ' + response.statusText)
                }
                await fetchContacts()
                selectedContact.value = null
            } catch (error) {
                console.error('Error:', error)
            }
        }

        const deleteContact = async (id: number) => {
            try {
                await fetch(`/api/contacts/${id}`, { 
                    method: 'DELETE',
                    headers: {
                        'Authorization': `Bearer ${token}`
                    }
                })
                contacts.value = contacts.value.filter(c => c.id !== id)
                if(selectedContact.value?.id === id) {
                    selectedContact.value = null
                }
            } catch (err){
                console.error('Error:', err)
            }
        }

        function createEmptyContact(): Contact {
            return {
                id: 0,
                name: '',
                surname: '',
                email: '',
                phone: '',
                birthDate: '',
                password: '',
                categoryId: 0,
                subcategoryId: null,
                customSubcategory: ''
            }
        }

        const logout = () => {
            localStorage.clear()
            window.location.href = '/'
        }

        return {
            contacts,
            selectedContact,
            categories,
            subcategories,
            selectContact,
            userEmail,
            isLoggedIn,
            addContact,
            updateContact,
            deleteContact,
            createEmptyContact,
            logout
        }
    }
}
</script>

<template>
    <div>
        <div style="margin-bottom: 20px;">
            <strong>Zalogowano jako:</strong> {{ userEmail }}
            <button @click="logout" style="margin-left: 10px;">Wyloguj</button>
        </div>
        <h2>Lista Kontaktów</h2>
        <ul>
            <li v-for="contact in contacts" :key="contact.id" @click="selectContact(contact)" style="cursor: pointer; padding: 4px; border-bottom: 1px solid #ccc;">
                {{ contact.name}} {{ contact.surname }} - {{ contact.email}}
            </li>
        </ul>

        <button v-if="isLoggedIn" @click="selectedContact = createEmptyContact()" style="margin-top: 10px;">
            Dodaj nowy kontakt
        </button>

        <div v-if="selectedContact" style="margin-top: 20px; padding: 10px; border: 1px solid #888;">
            <h3>Szczegóły kontaktu</h3>

            <label>Imię:</label>
            <input v-if="isLoggedIn" v-model="selectedContact.name" placeholder="Imię" />
            <span v-else>{{ selectedContact.name }}</span>

            <label>Nazwisko:</label>
            <input v-if="isLoggedIn" v-model="selectedContact.surname" placeholder="Nazwisko" />
            <span v-else>{{ selectedContact.surname }}</span>


            <label>Email:</label>
            <input v-if="isLoggedIn" v-model="selectedContact.email" placeholder="Email" />
            <span v-else>{{ selectedContact.email }}</span>


            <label>Telefon:</label>
            <input v-if="isLoggedIn" v-model="selectedContact.phone" placeholder="Telefon" />
            <span v-else>{{ selectedContact.phone }}</span>


            <label>Data urodzenia:</label>
            <input v-if="isLoggedIn" type="date" v-model="selectedContact.birthDate" />
            <span v-else>{{ selectedContact.birthDate }}</span>


            <label>Hasło:</label>
            <input v-if="isLoggedIn" v-model="selectedContact.password" placeholder="Hasło" />
            <span v-else>{{ selectedContact.password }}</span>


            <label>Kategoria:</label>
            <select v-if="isLoggedIn" v-model.number="selectedContact.categoryId">
                <option v-for="cat in categories" :key="cat.id" :value="cat.id">{{ cat.name }}</option>
            </select>
            <span v-else>{{ categories.find(c => c.id === selectedContact?.categoryId)?.name }}</span>


            <label v-if="selectedContact.categoryId === 3 || subcategories.some(s => s.categoryId === selectedContact?.categoryId)">
            Podkategoria:
            </label>
            
            <tempalte v-if="isLoggedIn">
                <input v-if="selectedContact.categoryId === 3" v-model="selectedContact.customSubcategory" placeholder="Nazwa podkategorii" />

                <select
                v-else-if="subcategories.some(s => s.categoryId === selectedContact?.categoryId)" v-model.number="selectedContact.subcategoryId">
                    <option
                        v-for="sub in subcategories"
                        :key="sub.id"
                        :value="sub.id">
                        {{ sub.name }}
                    </option>
                </select>
            </tempalte>

            <span v-else>
            {{
                selectedContact.categoryId === 3
                ? selectedContact.customSubcategory
                : subcategories.find(s => s.id === selectedContact?.subcategoryId)?.name
            }}
            </span>


            <div v-if="isLoggedIn" style="margin-top: 10px;">
                <!-- Jeśli id=0 → nowy kontakt -->
                <button @click="addContact" v-if="selectedContact.id === 0">Dodaj kontakt</button>

                <!-- Jeśli istniejący kontakt → edycja i usunięcie -->
                <button @click="updateContact" v-else>Zapisz zmiany</button>
                <button @click="deleteContact(selectedContact.id)" v-if="selectedContact.id !== 0">Usuń kontakt</button>
            </div>
        </div>
    </div>
</template>

<style>
    input, span{
        margin-right: 10px;
        margin-left: 5px;
    }
</style>