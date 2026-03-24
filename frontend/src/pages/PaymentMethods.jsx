import React, { useState, useEffect } from 'react'
import api from '../services/api'
import { PaymentIcon, PlusIcon, CancelIcon, SaveIcon, EditIcon, DeleteIcon, EmptyIcon } from '../components/Icons'

function PaymentMethods() {
  const [paymentMethods, setPaymentMethods] = useState([])
  const [loading, setLoading] = useState(true)
  const [showForm, setShowForm] = useState(false)
  const [editingId, setEditingId] = useState(null)
  const [formData, setFormData] = useState({
    name: '',
    icon: ''
  })

  useEffect(() => {
    loadPaymentMethods()
  }, [])

  const loadPaymentMethods = async () => {
    try {
      const response = await api.get('/payment-methods')
      setPaymentMethods(response.data)
    } catch (err) {
      console.error('Error loading payment methods:', err)
    } finally {
      setLoading(false)
    }
  }

  const handleSubmit = async (e) => {
    e.preventDefault()
    try {
      if (editingId) {
        await api.put(`/payment-methods/${editingId}`, formData)
      } else {
        await api.post('/payment-methods', formData)
      }
      resetForm()
      loadPaymentMethods()
    } catch (err) {
      alert(err.response?.data?.message || 'Error al guardar método de pago')
    }
  }

  const handleDelete = async (id) => {
    if (!confirm('¿Estás seguro de eliminar este método de pago?')) return

    try {
      await api.delete(`/payment-methods/${id}`)
      loadPaymentMethods()
    } catch (err) {
      alert(err.response?.data?.message || 'Error al eliminar método de pago')
    }
  }

  const handleEdit = (paymentMethod) => {
    setFormData({
      name: paymentMethod.name,
      icon: paymentMethod.icon || ''
    })
    setEditingId(paymentMethod.id)
    setShowForm(true)
  }

  const resetForm = () => {
    setFormData({
      name: '',
      icon: ''
    })
    setEditingId(null)
    setShowForm(false)
  }

  if (loading) return <div className="loading">Cargando métodos de pago</div>

  return (
    <div>
      <div className="page-header">
        <h1 className="page-title">
          <PaymentIcon size={48} color="#ffffff" />
          Métodos de Pago
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
              Nuevo Método de Pago
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
                Editar Método de Pago
              </>
            ) : (
              <>
                <PlusIcon size={24} color="#6366f1" />
                Nuevo Método de Pago
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
              <label>Ícono (opcional)</label>
              <input
                type="text"
                value={formData.icon}
                onChange={(e) => setFormData({ ...formData, icon: e.target.value })}
                placeholder="Ej: 💳"
              />
            </div>
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
          <PaymentIcon size={24} color="#6366f1" />
          Lista de Métodos de Pago
        </h2>
        {paymentMethods.length === 0 ? (
          <div className="empty-state">
            <EmptyIcon size={80} color="#6366f1" />
            <h3>No hay métodos de pago registrados</h3>
            <p>Crea métodos de pago para categorizar mejor tus gastos</p>
            <button className="btn btn-primary" onClick={() => setShowForm(true)} style={{ marginTop: '20px' }}>
              <PlusIcon size={18} />
              Crear Primer Método de Pago
            </button>
          </div>
        ) : (
          <table>
            <thead>
              <tr>
                <th>Nombre</th>
                <th>Ícono</th>
                <th>Acciones</th>
              </tr>
            </thead>
            <tbody>
              {paymentMethods.map(pm => (
                <tr key={pm.id}>
                  <td style={{ fontWeight: '600' }}>{pm.name}</td>
                  <td style={{ fontSize: '1.5rem' }}>{pm.icon || '💳'}</td>
                  <td>
                    <div style={{ display: 'flex', gap: '8px' }}>
                      <button className="btn btn-secondary btn-icon" onClick={() => handleEdit(pm)} title="Editar">
                        <EditIcon size={18} />
                      </button>
                      <button className="btn btn-danger btn-icon" onClick={() => handleDelete(pm.id)} title="Eliminar">
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

export default PaymentMethods

