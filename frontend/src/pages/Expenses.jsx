import React, { useState, useEffect } from 'react'
import api from '../services/api'
import { ExpenseIcon, PlusIcon, CancelIcon, SaveIcon, SearchIcon, EditIcon, DeleteIcon, EmptyIcon, FilterIcon } from '../components/Icons'

function Expenses() {
  const [expenses, setExpenses] = useState([])
  const [categories, setCategories] = useState([])
  const [paymentMethods, setPaymentMethods] = useState([])
  const [loading, setLoading] = useState(true)
  const [showForm, setShowForm] = useState(false)
  const [editingId, setEditingId] = useState(null)
  const [filters, setFilters] = useState({
    startDate: '',
    endDate: '',
    categoryId: '',
    paymentMethodId: '',
    searchText: ''
  })
  const [formData, setFormData] = useState({
    amount: '',
    date: new Date().toISOString().split('T')[0],
    categoryId: '',
    paymentMethodId: '',
    description: ''
  })

  useEffect(() => {
    loadData()
  }, [])

  useEffect(() => {
    loadExpenses()
  }, [filters])

  const loadData = async () => {
    try {
      const [expensesRes, categoriesRes, paymentMethodsRes] = await Promise.all([
        api.get('/expenses'),
        api.get('/categories'),
        api.get('/payment-methods')
      ])
      setExpenses(expensesRes.data)
      setCategories(categoriesRes.data.filter(c => c.isActive))
      setPaymentMethods(paymentMethodsRes.data)
    } catch (err) {
      console.error('Error loading data:', err)
    } finally {
      setLoading(false)
    }
  }

  const loadExpenses = async () => {
    try {
      const params = {}
      if (filters.startDate) params.startDate = filters.startDate
      if (filters.endDate) params.endDate = filters.endDate
      if (filters.categoryId) params.categoryId = filters.categoryId
      if (filters.paymentMethodId) params.paymentMethodId = filters.paymentMethodId
      if (filters.searchText) params.searchText = filters.searchText

      const response = await api.get('/expenses', { params })
      setExpenses(response.data)
    } catch (err) {
      console.error('Error loading expenses:', err)
    }
  }

  const handleSubmit = async (e) => {
    e.preventDefault()
    try {
      const data = {
        ...formData,
        amount: parseFloat(formData.amount)
      }

      if (editingId) {
        await api.put(`/expenses/${editingId}`, data)
      } else {
        await api.post('/expenses', data)
      }

      resetForm()
      loadExpenses()
    } catch (err) {
      alert(err.response?.data?.message || 'Error al guardar gasto')
    }
  }

  const handleDelete = async (id) => {
    if (!confirm('¿Estás seguro de eliminar este gasto?')) return

    try {
      await api.delete(`/expenses/${id}`)
      loadExpenses()
    } catch (err) {
      alert(err.response?.data?.message || 'Error al eliminar gasto')
    }
  }

  const handleEdit = (expense) => {
    setFormData({
      amount: expense.amount.toString(),
      date: expense.date.split('T')[0],
      categoryId: expense.category.id,
      paymentMethodId: expense.paymentMethod.id,
      description: expense.description || ''
    })
    setEditingId(expense.id)
    setShowForm(true)
  }

  const resetForm = () => {
    setFormData({
      amount: '',
      date: new Date().toISOString().split('T')[0],
      categoryId: '',
      paymentMethodId: '',
      description: ''
    })
    setEditingId(null)
    setShowForm(false)
  }

  if (loading) return <div className="loading">Cargando gastos</div>

  const totalFiltered = expenses.reduce((sum, e) => sum + e.amount, 0)

  return (
    <div>
      <div className="page-header">
        <h1 className="page-title">
          <ExpenseIcon size={48} color="#ffffff" />
          Gastos
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
              Nuevo Gasto
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
                Editar Gasto
              </>
            ) : (
              <>
                <PlusIcon size={24} color="#6366f1" />
                Nuevo Gasto
              </>
            )}
          </h2>
          <form onSubmit={handleSubmit}>
            <div className="form-group">
              <label>Monto *</label>
              <input
                type="number"
                step="0.01"
                min="0.01"
                value={formData.amount}
                onChange={(e) => setFormData({ ...formData, amount: e.target.value })}
                required
              />
            </div>
            <div className="form-group">
              <label>Fecha *</label>
              <input
                type="date"
                value={formData.date}
                onChange={(e) => setFormData({ ...formData, date: e.target.value })}
                required
              />
            </div>
            <div className="form-group">
              <label>Categoría *</label>
              <select
                value={formData.categoryId}
                onChange={(e) => setFormData({ ...formData, categoryId: e.target.value })}
                required
              >
                <option value="">Seleccione...</option>
                {categories.map(cat => (
                  <option key={cat.id} value={cat.id}>{cat.name}</option>
                ))}
              </select>
            </div>
            <div className="form-group">
              <label>Método de Pago *</label>
              <select
                value={formData.paymentMethodId}
                onChange={(e) => setFormData({ ...formData, paymentMethodId: e.target.value })}
                required
              >
                <option value="">Seleccione...</option>
                {paymentMethods.map(pm => (
                  <option key={pm.id} value={pm.id}>{pm.name}</option>
                ))}
              </select>
            </div>
            <div className="form-group">
              <label>Descripción</label>
              <textarea
                value={formData.description}
                onChange={(e) => setFormData({ ...formData, description: e.target.value })}
                rows="3"
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
          <FilterIcon size={24} color="#6366f1" />
          Filtros
        </h2>
        {expenses.length > 0 && (
          <div style={{ marginBottom: '20px', padding: '12px', background: 'linear-gradient(135deg, #dbeafe 0%, #bfdbfe 100%)', borderRadius: '8px' }}>
            <strong>Total filtrado: </strong>
            <span style={{ fontSize: '1.2rem', fontWeight: '700', color: '#1e40af' }}>
              ${totalFiltered.toFixed(2)}
            </span>
          </div>
        )}
        <div className="filters">
          <div className="form-group">
            <label>Fecha Inicio</label>
            <input
              type="date"
              value={filters.startDate}
              onChange={(e) => setFilters({ ...filters, startDate: e.target.value })}
            />
          </div>
          <div className="form-group">
            <label>Fecha Fin</label>
            <input
              type="date"
              value={filters.endDate}
              onChange={(e) => setFilters({ ...filters, endDate: e.target.value })}
            />
          </div>
          <div className="form-group">
            <label>Categoría</label>
            <select
              value={filters.categoryId}
              onChange={(e) => setFilters({ ...filters, categoryId: e.target.value })}
            >
              <option value="">Todas</option>
              {categories.map(cat => (
                <option key={cat.id} value={cat.id}>{cat.name}</option>
              ))}
            </select>
          </div>
          <div className="form-group">
            <label>Método de Pago</label>
            <select
              value={filters.paymentMethodId}
              onChange={(e) => setFilters({ ...filters, paymentMethodId: e.target.value })}
            >
              <option value="">Todos</option>
              {paymentMethods.map(pm => (
                <option key={pm.id} value={pm.id}>{pm.name}</option>
              ))}
            </select>
          </div>
          <div className="form-group">
            <label>Buscar en Descripción</label>
            <input
              type="text"
              value={filters.searchText}
              onChange={(e) => setFilters({ ...filters, searchText: e.target.value })}
              placeholder="Texto a buscar..."
            />
          </div>
        </div>
      </div>

      <div className="card">
        <h2>
          <ExpenseIcon size={24} color="#6366f1" />
          Lista de Gastos
        </h2>
        {expenses.length === 0 ? (
          <div className="empty-state">
            <EmptyIcon size={80} color="#6366f1" />
            <h3>No hay gastos registrados</h3>
            <p>Comienza registrando tus primeros gastos para llevar un mejor control</p>
            <button className="btn btn-primary" onClick={() => setShowForm(true)} style={{ marginTop: '20px' }}>
              <PlusIcon size={18} />
              Registrar Primer Gasto
            </button>
          </div>
        ) : (
          <table>
            <thead>
              <tr>
                <th>Fecha</th>
                <th>Descripción</th>
                <th>Categoría</th>
                <th>Método de Pago</th>
                <th>Monto</th>
                <th>Acciones</th>
              </tr>
            </thead>
            <tbody>
              {expenses.map(expense => (
                <tr key={expense.id}>
                  <td>{new Date(expense.date).toLocaleDateString('es-ES', { day: '2-digit', month: '2-digit', year: 'numeric' })}</td>
                  <td>{expense.description || '-'}</td>
                  <td><span className="badge badge-info">{expense.category.name}</span></td>
                  <td>{expense.paymentMethod.name}</td>
                  <td style={{ fontWeight: '700', color: '#ef4444', fontSize: '1.1rem' }}>${expense.amount.toFixed(2)}</td>
                  <td>
                    <div style={{ display: 'flex', gap: '8px' }}>
                      <button className="btn btn-secondary btn-icon" onClick={() => handleEdit(expense)} title="Editar">
                        <EditIcon size={18} />
                      </button>
                      <button className="btn btn-danger btn-icon" onClick={() => handleDelete(expense.id)} title="Eliminar">
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

export default Expenses

