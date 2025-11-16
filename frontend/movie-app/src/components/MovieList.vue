<template>
  <div class="container mt-4">
    <h1 class="mb-4">Biblioteka Filmowa</h1>
    
    <div v-if="error" class="alert alert-danger alert-dismissible">
      {{ error }}
      <button type="button" class="btn-close" @click="error = null"></button>
    </div>
    
    <div v-if="success" class="alert alert-success alert-dismissible">
      {{ success }}
      <button type="button" class="btn-close" @click="success = null"></button>
    </div>
    
    <div class="mb-3">
      <button @click="openAddModal" class="btn btn-primary me-2">
        Dodaj
      </button>
      <button @click="importMovies" class="btn btn-success" :disabled="importing">
        <span v-if="importing" class="spinner-border spinner-border-sm me-2"></span>
        {{ importing ? 'Importowanie...' : 'Pobierz filmy' }}
      </button>
    </div>
    
    <div v-if="loading" class="text-center my-5">
      <div class="spinner-border"></div>
    </div>
    
    <table v-else class="table table-striped">
      <thead class="table-dark">
        <tr>
          <th>ID</th>
          <th>Tytuł</th>
          <th>Rok</th>
          <th>Reżyser</th>
          <th>Ocena</th>
          <th>Akcje</th>
        </tr>
      </thead>
      <tbody>
        <tr v-if="movies.length === 0">
          <td colspan="6" class="text-center text-muted py-4">Brak filmów w bazie</td>
        </tr>
        <tr v-for="movie in movies" :key="movie.id">
          <td>{{ movie.id }}</td>
          <td>{{ movie.title }}</td>
          <td>{{ movie.year }}</td>
          <td>{{ movie.director || '-' }}</td>
          <td>{{ movie.rate || '-' }}</td>
          <td>
            <button @click="editMovie(movie)" class="btn btn-sm btn-warning me-2">
              Edytuj
            </button>
            <button @click="confirmDelete(movie)" class="btn btn-sm btn-danger">
              Usuń
            </button>
          </td>
        </tr>
      </tbody>
    </table>
    
    <MovieModal
      v-if="showModal"
      :movie="selectedMovie"
      @close="closeModal"
      @save="handleSave"
    />
    
    <div v-if="deleteModal" class="modal d-block" style="background: rgba(0,0,0,0.5)">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">Potwierdź usunięcie</h5>
            <button type="button" class="btn-close" @click="deleteModal = false"></button>
          </div>
          <div class="modal-body">
            <p>Czy na pewno chcesz usunąć film: <strong>{{ movieToDelete?.title }}</strong>?</p>
          </div>
          <div class="modal-footer">
            <button class="btn btn-secondary" @click="deleteModal = false">Anuluj</button>
            <button class="btn btn-danger" @click="deleteMovie">Usuń</button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import api from '../services/api'
import MovieModal from './MovieModal.vue'

export default {
  components: { MovieModal },
  data() {
    return {
      movies: [],
      loading: false,
      importing: false,
      showModal: false,
      deleteModal: false,
      selectedMovie: null,
      movieToDelete: null,
      error: null,
      success: null
    }
  },
  mounted() {
    this.loadMovies()
  },
  methods: {
    async loadMovies() {
      this.loading = true
      this.error = null
      try {
        const response = await api.getAll()
        this.movies = response.data
      } catch (err) {
        if (err.code === 'ERR_NETWORK') {
          this.error = 'Nie można połączyć się z serwerem. Sprawdź czy backend działa na porcie 5048.'
        } else {
          this.error = 'Błąd podczas ładowania filmów: ' + (err.message || 'Nieznany błąd')
        }
      } finally {
        this.loading = false
      }
    },
    
    openAddModal() {
      this.selectedMovie = null
      this.showModal = true
    },
    
    editMovie(movie) {
      this.selectedMovie = { ...movie }
      this.showModal = true
    },
    
    closeModal() {
      this.showModal = false
      this.selectedMovie = null
    },
    
    async handleSave() {
      await this.loadMovies()
      this.closeModal()
      this.success = 'Film został zapisany'
      setTimeout(() => this.success = null, 3000)
    },
    
    confirmDelete(movie) {
      this.movieToDelete = movie
      this.deleteModal = true
    },
    
    async deleteMovie() {
      try {
        await api.delete(this.movieToDelete.id)
        this.success = 'Film został usunięty'
        await this.loadMovies()
      } catch (err) {
        this.error = 'Nie można usunąć filmu'
      } finally {
        this.deleteModal = false
        this.movieToDelete = null
      }
    },
    
    async importMovies() {
      this.importing = true
      this.error = null
      try {
        const response = await api.import()
        this.success = response.data.message
        await this.loadMovies()
      } catch (err) {
        this.error = 'Nie można zaimportować filmów'
      } finally {
        this.importing = false
      }
    }
  }
}
</script>