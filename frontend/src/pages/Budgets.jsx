import React, { useState, useEffect } from 'react'
import api from '../services/api'
import { BudgetIcon, PlusIcon, CancelIcon, SaveIcon, EditIcon, DeleteIcon, EmptyIcon, FilterIcon } from '../components/Icons'

function Budgets() {
  const [budgets, setBudgets] = useState([])
  const [categories, setCategories] = useState([])
  const [loading, setLoading] = useState(true)
  const [showForm, setShowForm] = useState(false)
  const [editingId, setEditingId] = useState(null)
  const [month, setMonth] = useState(new Date().getMonth() + 1)
  const [year, setYear] = useState(new Date().getFullYear())
  const [formData, setFormData] = useState({
    categoryId: '',
    amount: '',
    month: new Date().getMonth() + 1,
    year: new Date().getFullYear()
  })

  useEffect(() => {
    loadCategories()
  }, [])

  useEffect(() => {
    loadBudgets()
  }, [month, year])

  const loadCategories = async () => {
    try {
      const response = await api.get('/categories')
      setCategories(response.data.filter(c => c.isActive))
    } catch (err) {
      console.error('Error loading categories:', err)
    } finally {
      setLoading(false)
    }
  }

  const loadBudgets = async () => {
    try {
      const response = await api.get(`/budgets?month=${month}&year=${year}`)
      setBudgets(response.data)
    } catch (err) {
      console.error('Error loading budgets:', err)
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
        await api.put(`/budgets/${editingId}`, { amount: data.amount })
      } else {
        await api.post('/budgets', data)
      }

      resetForm()
      loadBudgets()
    } catch (err) {
      alert(err.response?.data?.message || 'Error al guardar presupuesto')
    }
  }

  const handleDelete = async (id) => {
    if (!confirm('¿Estás seguro de eliminar este presupuesto?')) return

    try {
      await api.delete(`/budgets/${id}`)
      loadBudgets()
    } catch (err) {
      alert(err.response?.data?.message || 'Error al eliminar presupuesto')
    }
  }

  const handleEdit = (budget) => {
    setFormData({
      categoryId: budget.categoryId,
      amount: budget.amount.toString(),
      month: budget.month,
      year: budget.year
    })
    setEditingId(budget.id)
    setShowForm(true)
  }

  const resetForm = () => {
    setFormData({
      categoryId: '',
      amount: '',
      month: new Date().getMonth() + 1,
      year: new Date().getFullYear()
    })
    setEditingId(null)
    setShowForm(false)
  }

  if (loading) return <div className="loading">Cargando presupuestos</div>

  return (
    <div>
      <div className="page-header">
        <h1 className="page-title">
          <BudgetIcon size={48} color="#ffffff" />
          Presupuestos
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
              Nuevo Presupuesto
            </>
          )}
        </button>
      </div>

      <div className="card">
        <h2>
          <FilterIcon size={24} color="#6366f1" />
          Filtros
        </h2>
        <div className="filters">
          <div className="form-group">
            <label>Mes</label>
            <input
              type="number"
              min="1"
              max="12"
              value={month}
              onChange={(e) => setMonth(parseInt(e.target.value))}
            />
          </div>
          <div className="form-group">
            <label>Año</label>
            <input
              type="number"
              min="2020"
              max="2100"
              value={year}
              onChange={(e) => setYear(parseInt(e.target.value))}
            />
          </div>
        </div>
      </div>

      {showForm && (
        <div className="card" style={{ animation: 'slideIn 0.3s ease-out' }}>
          <h2>
            {editingId ? (
              <>
                <EditIcon size={24} color="#6366f1" />
                Editar Presupuesto
              </>
            ) : (
              <>
                <PlusIcon size={24} color="#6366f1" />
                Nuevo Presupuesto
              </>
            )}
          </h2>
          <form onSubmit={handleSubmit}>
            <div className="form-group">
              <label>Categoría *</label>
              <select
                value={formData.categoryId}
                onChange={(e) => setFormData({ ...formData, categoryId: e.target.value })}
                required
                disabled={!!editingId}
              >
                <option value="">Seleccione...</option>
                {categories.map(cat => (
                  <option key={cat.id} value={cat.id}>{cat.name}</option>
                ))}
              </select>
            </div>
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
            {!editingId && (
              <>
                <div className="form-group">
                  <label>Mes *</label>
                  <input
                    type="number"
                    min="1"
                    max="12"
                    value={formData.month}
                    onChange={(e) => setFormData({ ...formData, month: parseInt(e.target.value) })}
                    required
                  />
                </div>
                <div className="form-group">
                  <label>Año *</label>
                  <input
                    type="number"
                    min="2020"
                    max="2100"
                    value={formData.year}
                    onChange={(e) => setFormData({ ...formData, year: parseInt(e.target.value) })}
                    required
                  />
                </div>
              </>
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
          <BudgetIcon size={24} color="#6366f1" />
          Presupuestos del Mes {month}/{year}
        </h2>
        {budgets.length === 0 ? (
          <div className="empty-state">
            <EmptyIcon size={80} color="#6366f1" />
            <h3>No hay presupuestos configurados para este mes</h3>
            <p>Configura tus presupuestos mensuales para mantener un mejor control financiero</p>
            <button className="btn btn-primary" onClick={() => setShowForm(true)} style={{ marginTop: '20px' }}>
              <PlusIcon size={18} />
              Crear Primer Presupuesto
            </button>
          </div>
        ) : (
          <table>
            <thead>
              <tr>
                <th>Categoría</th>
                <th>Presupuesto</th>
                <th>Gastado</th>
                <th>Restante</th>
                <th>Porcentaje</th>
                <th>Alerta</th>
                <th>Acciones</th>
              </tr>
            </thead>
            <tbody>
              {budgets.map(budget => (
                <tr key={budget.id}>
                  <td style={{ fontWeight: '600' }}>{budget.categoryName}</td>
                  <td>${budget.amount.toFixed(2)}</td>
                  <td style={{ color: '#ef4444', fontWeight: '600' }}>${budget.spentAmount.toFixed(2)}</td>
                  <td style={{ color: budget.remainingAmount >= 0 ? '#10b981' : '#ef4444', fontWeight: '600' }}>
                    ${budget.remainingAmount.toFixed(2)}
                  </td>
                  <td>
                    <span className={`badge badge-${budget.alertLevel === 'Excedido' ? 'danger' : budget.alertLevel === 'Alto' ? 'warning' : budget.alertLevel === 'Medio' ? 'info' : 'success'}`}>
                      {budget.percentageUsed.toFixed(1)}%
                    </span>
                  </td>
                  <td>
                    <span className={`badge badge-${budget.alertLevel === 'Excedido' ? 'danger' : budget.alertLevel === 'Alto' ? 'warning' : budget.alertLevel === 'Medio' ? 'info' : 'success'}`}>
                      {budget.alertLevel}
                    </span>
                  </td>
                  <td>
                    <div style={{ display: 'flex', gap: '8px' }}>
                      <button className="btn btn-secondary btn-icon" onClick={() => handleEdit(budget)} title="Editar">
                        <EditIcon size={18} />
                      </button>
                      <button className="btn btn-danger btn-icon" onClick={() => handleDelete(budget.id)} title="Eliminar">
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

export default Budgets

