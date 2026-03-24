import React, { useState, useEffect } from 'react'
import api from '../services/api'
import { CategoryIcon, PlusIcon, CancelIcon, SaveIcon, EditIcon, DeleteIcon, EmptyIcon } from '../components/Icons'

function Categories() {
  const [categories, setCategories] = useState([])
  const [loading, setLoading] = useState(true)
  const [showForm, setShowForm] = useState(false)
  const [editingId, setEditingId] = useState(null)
  const [formData, setFormData] = useState({
    name: '',
    description: '',
    isActive: true
  })

  useEffect(() => {
    loadCategories()
  }, [])

  const loadCategories = async () => {
    try {
      const response = await api.get('/categories')
      setCategories(response.data)
    } catch (err) {
      console.error('Error loading categories:', err)
    } finally {
      setLoading(false)
    }
  }

  const handleSubmit = async (e) => {
    e.preventDefault()
    try {
      if (editingId) {
        await api.put(`/categories/${editingId}`, formData)
      } else {
        await api.post('/categories', { name: formData.name, description: formData.description })
      }
      resetForm()
      loadCategories()
    } catch (err) {
      alert(err.response?.data?.message || 'Error al guardar categoría')
    }
  }

  const handleDelete = async (id) => {
    if (!confirm('¿Estás seguro de eliminar esta categoría?')) return

    try {
      await api.delete(`/categories/${id}`)
      loadCategories()
    } catch (err) {
      alert(err.response?.data?.message || 'Error al eliminar categoría')
    }
  }

  const handleEdit = (category) => {
    setFormData({
      name: category.name,
      description: category.description || '',
      isActive: category.isActive
    })
    setEditingId(category.id)
    setShowForm(true)
  }

  const resetForm = () => {
    setFormData({
      name: '',
      description: '',
      isActive: true
    })
    setEditingId(null)
    setShowForm(false)
  }

  if (loading) return <div className="loading">Cargando categorías</div>

  return (
    <div>
      <div className="page-header">
        <h1 className="page-title">
          <CategoryIcon size={48} color="#ffffff" />
          Categorías
        </h1>
        <button className="btn btn-primary" onClick={() => setShowForm(!showForm)}>
          {showForm ? (
            <>
              <CancelIcon size={18} />
              Cancelar
            </>
          ) : (
            <>
              <PlusIcon size={18} />
              Nueva Categoría
            </>
          )}
        </button>
      </div>

      {showForm && (
        <div className="card" style={{ animation: 'slideIn 0.3s ease-out' }}>
          <h2>
            {editingId ? (
              <>
                <EditIcon size={24} color="#6366f1" />
                Editar Categoría
              </>
            ) : (
              <>
                <PlusIcon size={24} color="#6366f1" />
                Nueva Categoría
              </>
            )}
          </h2>
          <form onSubmit={handleSubmit}>
            <div className="form-group">
              <label>Nombre *</label>
              <input
                type="text"
                value={formData.name}
                onChange={(e) => setFormData({ ...formData, name: e.target.value })}
                required
              />
            </div>
            <div className="form-group">
              <label>Descripción</label>
              <textarea
                value={formData.description}
                onChange={(e) => setFormData({ ...formData, description: e.target.value })}
                rows="3"
              />
            </div>
            {editingId && (
              <div className="form-group">
                <label>
                  <input
                    type="checkbox"
                    checked={formData.isActive}
                    onChange={(e) => setFormData({ ...formData, isActive: e.target.checked })}
                  />
                  {' '}Activa
                </label>
              </div>
            )}
            <div style={{ display: 'flex', gap: '12px', marginTop: '20px' }}>
              <button type="submit" className="btn btn-primary">
                <SaveIcon size={18} />
                Guardar
              </button>
              <button type="button" className="btn btn-secondary" onClick={resetForm}>
                <CancelIcon size={18} />
                Cancelar
              </button>
            </div>
          </form>
        </div>
      )}

      <div className="card">
        <h2>
          <CategoryIcon size={24} color="#6366f1" />
          Lista de Categorías
        </h2>
        {categories.length === 0 ? (
          <div className="empty-state">
            <EmptyIcon size={80} color="#6366f1" />
            <h3>No hay categorías registradas</h3>
            <p>Crea categorías para organizar mejor tus gastos</p>
            <button className="btn btn-primary" onClick={() => setShowForm(true)} style={{ marginTop: '20px' }}>
              <PlusIcon size={18} />
              Crear Primera Categoría
            </button>
          </div>
        ) : (
          <table>
            <thead>
              <tr>
                <th>Nombre</th>
                <th>Descripción</th>
                <th>Estado</th>
                <th>Acciones</th>
              </tr>
            </thead>
            <tbody>
              {categories.map(category => (
                <tr key={category.id}>
                  <td style={{ fontWeight: '600' }}>{category.name}</td>
                  <td>{category.description || '-'}</td>
                  <td>
                    <span className={`badge ${category.isActive ? 'badge-success' : 'badge-danger'}`}>
                      {category.isActive ? 'Activa' : 'Inactiva'}
                    </span>
                  </td>
                  <td>
                    <div style={{ display: 'flex', gap: '8px' }}>
                      <button className="btn btn-secondary btn-icon" onClick={() => handleEdit(category)} title="Editar">
                        <EditIcon size={18} />
                      </button>
                      <button className="btn btn-danger btn-icon" onClick={() => handleDelete(category.id)} title="Eliminar">
                        <DeleteIcon size={18} />
                      </button>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>
    </div>
  )
}

export default Categories

