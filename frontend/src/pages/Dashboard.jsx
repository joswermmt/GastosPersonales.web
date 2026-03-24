import React, { useState, useEffect } from 'react'
import { Link } from 'react-router-dom'
import api from '../services/api'
import { DashboardIcon, MoneyIcon, BudgetIcon, TrendingUpIcon, ExpenseIcon, EmptyIcon, PlusIcon } from '../components/Icons'

function Dashboard() {
  const [expenses, setExpenses] = useState([])
  const [budgets, setBudgets] = useState([])
  const [loading, setLoading] = useState(true)
  const now = new Date()

  useEffect(() => {
    loadData()
  }, [])

  const loadData = async () => {
    try {
      const [expensesRes, budgetsRes] = await Promise.all([
        api.get('/expenses'),
        api.get(`/budgets?month=${now.getMonth() + 1}&year=${now.getFullYear()}`)
      ])
      setExpenses(expensesRes.data)
      setBudgets(budgetsRes.data)
    } catch (err) {
      console.error('Error loading data:', err)
    } finally {
      setLoading(false)
    }
  }

  if (loading) return <div className="loading">Cargando datos</div>

  const totalSpent = expenses.reduce((sum, e) => sum + e.amount, 0)
  const recentExpenses = expenses.slice(0, 5)
  const budgetsExceeded = budgets.filter(b => b.percentageUsed >= 100).length
  const budgetsWarning = budgets.filter(b => b.percentageUsed >= 80 && b.percentageUsed < 100).length

  return (
    <div>
      <h1 className="page-title">
        <DashboardIcon size={48} color="#ffffff" />
        Dashboard
      </h1>
      
      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(280px, 1fr))', gap: '24px', marginBottom: '40px' }}>
        <div className="stats-card">
          <h3>
            <MoneyIcon size={16} color="#6b7280" />
            Total Gastado
          </h3>
          <div className="value" style={{ color: '#ef4444' }}>
            ${totalSpent.toFixed(2)}
          </div>
          <div className="change" style={{ color: '#6b7280' }}>
            {expenses.length} {expenses.length === 1 ? 'gasto' : 'gastos'}
          </div>
        </div>

        <div className="stats-card">
          <h3>
            <BudgetIcon size={16} color="#6b7280" />
            Presupuestos Activos
          </h3>
          <div className="value" style={{ color: '#6366f1' }}>
            {budgets.length}
          </div>
          <div className="change" style={{ color: '#6b7280' }}>
            {budgetsWarning > 0 && `${budgetsWarning} en advertencia`}
          </div>
        </div>

        <div className="stats-card">
          <h3>
            <TrendingUpIcon size={16} color="#6b7280" />
            Presupuestos Excedidos
          </h3>
          <div className="value" style={{ color: budgetsExceeded > 0 ? '#ef4444' : '#10b981' }}>
            {budgetsExceeded}
          </div>
          <div className="change" style={{ color: '#6b7280' }}>
            {budgetsExceeded === 0 ? 'Todo bajo control' : 'Requiere atención'}
          </div>
        </div>
      </div>

      <div className="card">
        <h2>
          <BudgetIcon size={24} color="#6366f1" />
          Presupuestos del Mes
        </h2>
        {budgets.length === 0 ? (
          <div className="empty-state">
            <EmptyIcon size={80} color="#6366f1" />
            <h3>No hay presupuestos configurados</h3>
            <p>Configura tus presupuestos mensuales para mantener un mejor control</p>
            <Link to="/budgets" className="btn btn-primary" style={{ marginTop: '20px' }}>
              <PlusIcon size={18} />
              Crear Presupuesto
            </Link>
          </div>
        ) : (
          budgets.map(budget => (
            <div key={budget.id} className={`budget-alert ${budget.alertLevel.toLowerCase()}`}>
              <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '8px' }}>
                <strong style={{ fontSize: '1.1rem' }}>{budget.categoryName}</strong>
                <span className={`badge badge-${budget.alertLevel === 'Excedido' ? 'danger' : budget.alertLevel === 'Alto' ? 'warning' : budget.alertLevel === 'Medio' ? 'info' : 'success'}`}>
                  {budget.percentageUsed.toFixed(1)}%
                </span>
              </div>
              <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(200px, 1fr))', gap: '12px', marginTop: '12px' }}>
                <div>
                  <strong>Presupuesto:</strong> ${budget.amount.toFixed(2)}
                </div>
                <div>
                  <strong>Gastado:</strong> ${budget.spentAmount.toFixed(2)}
                </div>
                <div>
                  <strong>Restante:</strong> ${budget.remainingAmount.toFixed(2)}
                </div>
              </div>
              <div style={{ marginTop: '12px', height: '8px', background: '#e5e7eb', borderRadius: '4px', overflow: 'hidden' }}>
                <div 
                  style={{ 
                    height: '100%', 
                    background: budget.alertLevel === 'Excedido' ? 'linear-gradient(135deg, #ef4444 0%, #dc2626 100%)' :
                               budget.alertLevel === 'Alto' ? 'linear-gradient(135deg, #f59e0b 0%, #d97706 100%)' :
                               budget.alertLevel === 'Medio' ? 'linear-gradient(135deg, #3b82f6 0%, #2563eb 100%)' :
                               'linear-gradient(135deg, #10b981 0%, #059669 100%)',
                    width: `${Math.min(budget.percentageUsed, 100)}%`,
                    transition: 'width 0.5s ease'
                  }}
                />
              </div>
            </div>
          ))
        )}
      </div>

      <div className="card">
        <h2>
          <ExpenseIcon size={24} color="#6366f1" />
          Gastos Recientes
        </h2>
        {recentExpenses.length === 0 ? (
          <div className="empty-state">
            <EmptyIcon size={80} color="#6366f1" />
            <h3>No hay gastos registrados</h3>
            <p>Comienza registrando tus primeros gastos</p>
            <Link to="/expenses" className="btn btn-primary" style={{ marginTop: '20px' }}>
              <PlusIcon size={18} />
              Registrar Gasto
            </Link>
          </div>
        ) : (
          <table>
            <thead>
              <tr>
                <th>Fecha</th>
                <th>Descripción</th>
                <th>Categoría</th>
                <th>Monto</th>
              </tr>
            </thead>
            <tbody>
              {recentExpenses.map(expense => (
                <tr key={expense.id}>
                  <td>{new Date(expense.date).toLocaleDateString('es-ES', { day: '2-digit', month: '2-digit', year: 'numeric' })}</td>
                  <td>{expense.description || '-'}</td>
                  <td>
                    <span className="badge badge-info">{expense.category.name}</span>
                  </td>
                  <td style={{ fontWeight: '700', color: '#ef4444' }}>${expense.amount.toFixed(2)}</td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>
    </div>
  )
}

export default Dashboard

