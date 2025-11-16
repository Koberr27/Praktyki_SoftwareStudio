import 'bootstrap/dist/css/bootstrap.min.css'
import { createApp } from 'vue'
import App from './App.vue'

const link = document.createElement('link')
link.href = 'https://fonts.googleapis.com/css2?family=Roboto:wght@300;400;500;700&display=swap'
link.rel = 'stylesheet'
document.head.appendChild(link)

document.body.style.fontFamily = "'Roboto', sans-serif"
createApp(App).mount('#app')