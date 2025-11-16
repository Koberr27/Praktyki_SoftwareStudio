<template>
  <div class="modal d-block" style="background: rgba(0,0,0,0.5)">
    <div class="modal-dialog">
      <div class="modal-content">
        <div class="modal-header">
          <h5 class="modal-title">{{ movie ? 'Edytuj film' : 'Dodaj film' }}</h5>
          <button type="button" class="btn-close" @click="$emit('close')"></button>
        </div>
        <div class="modal-body">
          <div class="mb-3">
            <label class="form-label">Tytuł *</label>
            <input
              v-model="form.title"
              type="text"
              class="form-control"
              :class="{ 'is-invalid': v$.form.title.$error }"
              @blur="v$.form.title.$touch()"
              maxlength="200"
            />
            <div v-if="v$.form.title.$error" class="invalid-feedback">
              <div v-if="v$.form.title.required.$invalid">Tytuł jest wymagany</div>
              <div v-if="v$.form.title.maxLength.$invalid">Tytuł może mieć maksymalnie 200 znaków</div>
            </div>
            <small class="text-muted">{{ form.title.length }}/200 znaków</small>
          </div>
          <div class="mb-3">
            <label class="form-label">Rok *</label>
            <input
              v-model.number="form.year"
              type="number"
              class="form-control"
              :class="{ 'is-invalid': v$.form.year.$error }"
              @blur="v$.form.year.$touch()"
              min="1900"
              max="2200"
            />
            <div v-if="v$.form.year.$error" class="invalid-feedback">
              <div v-if="v$.form.year.required.$invalid">Rok jest wymagany</div>
              <div v-if="v$.form.year.between.$invalid">Rok musi być między 1900 a 2200</div>
              <div v-if="v$.form.year.numeric.$invalid">Rok musi być liczbą</div>
            </div>
            <small class="text-muted">Podaj rok produkcji filmu (1900-2200)</small>
          </div>
          <div class="mb-3">
            <label class="form-label">Reżyser</label>
            <input 
              v-model="form.director" 
              type="text" 
              class="form-control"
              placeholder="np. Steven Spielberg" 
            />
            <small class="text-muted">Pole opcjonalne</small>
          </div>
          <div class="mb-3">
            <label class="form-label">Ocena</label>
            <input 
              v-model.number="form.rate" 
              type="number" 
              class="form-control"
              :class="{ 'is-invalid': rateError }"
              min="0"
              max="10"
              step="0.1"
              placeholder="np. 8.5"
              @input="validateRate"
            />
            <div v-if="rateError" class="invalid-feedback d-block">
              {{ rateError }}
            </div>
            <small class="text-muted">Opcjonalna ocena od 0 do 10</small>
          </div>
          <div v-if="error" class="alert alert-danger">
            <strong>Błąd:</strong> {{ error }}
          </div>
        </div>
        <div class="modal-footer">
          <button type="button" class="btn btn-secondary" @click="$emit('close')">
            Anuluj
          </button>
          <button type="button" class="btn btn-primary" @click="save" :disabled="saving">
            <span v-if="saving" class="spinner-border spinner-border-sm me-2"></span>
            {{ movie ? 'Zapisz zmiany' : 'Dodaj film' }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>
<script>
import { useVuelidate } from '@vuelidate/core'
import { required, maxLength, between, numeric } from '@vuelidate/validators'
import api from '../services/api'

export default {
  props: {
    movie: {
      type: Object,
      default: null
    }
  },
  setup() {
    return { v$: useVuelidate() }
  },
  data() {
    return {
      form: {
        title: '',
        year: new Date().getFullYear(),
        director: '',
        rate: null
      },
      saving: false,
      error: null,
      rateError: null
    }
  },
  validations() {
    return {
      form: {
        title: {
          required,
          maxLength: maxLength(200)
        },
        year: {
          required,
          numeric,
          between: between(1900, 2200)
        }
      }
    }
  },
  mounted() {
    if (this.movie) {
      this.form = {
        title: this.movie.title,
        year: this.movie.year,
        director: this.movie.director || '',
        rate: this.movie.rate || null
      }
    }
  },
  methods: {
    validateRate() {
      this.rateError = null
      
      if (this.form.rate !== null && this.form.rate !== '') {
        if (isNaN(this.form.rate)) {
          this.rateError = 'Ocena musi być liczbą'
        } else if (this.form.rate < 0 || this.form.rate > 10) {
          this.rateError = 'Ocena musi być między 0 a 10'
        }
      }
    },
    async save() {
      this.v$.$touch()
      this.validateRate()
      
      if (this.v$.$invalid || this.rateError) {
        this.error = 'Popraw błędy walidacji przed zapisaniem'
        return
      }
      
      this.saving = true
      this.error = null
      
      try {
        if (this.movie) {
          await api.update(this.movie.id, this.form)
        } else {
          await api.create(this.form)
        }
        this.$emit('save')
      } catch (err) {
        if (err.response && err.response.data) {
          this.error = err.response.data.message || 'Błąd podczas zapisywania filmu'
        } else {
          this.error = 'Nie można połączyć się z serwerem. Sprawdź czy backend działa.'
        }
      } finally {
        this.saving = false
      }
    }
  }
}
</script>
<style scoped>
.form-label {
  font-weight: 500;
}
.modal-content {
  font-family: 'Roboto', sans-serif;
}
</style>