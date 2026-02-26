import { createRouter, createWebHistory } from 'vue-router'
import LoginView from '@/views/LoginView.vue'
import ContactsView from '@/views/ContactsView.vue'

const routes = [
  { path: '/', redirect: '/login'},
  { path: '/login', component: LoginView },
  { path: '/contacts', component: ContactsView }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

export default router
