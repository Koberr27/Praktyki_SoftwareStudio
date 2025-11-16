import axios from 'axios'

const API_URL = 'http://localhost:5048/api/Movies'

export default {
  getAll() {
    return axios.get(API_URL)
  },
  get(id) {
    return axios.get(`${API_URL}/${id}`)
  },
  create(movie) {
    return axios.post(API_URL, movie)
  },
  update(id, movie) {
    return axios.put(`${API_URL}/${id}`, movie)
  },
  delete(id) {
    return axios.delete(`${API_URL}/${id}`)
  },
  import() {
    return axios.get(`${API_URL}/import`)
  }
}