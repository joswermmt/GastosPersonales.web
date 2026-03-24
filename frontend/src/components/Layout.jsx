import React from 'react'
import { Link, useNavigate, useLocation } from 'react-router-dom'
import { useAuth } from '../contexts/AuthContext'
import { 
  DashboardIcon, ExpenseIcon, CategoryIcon, PaymentIcon, 
  BudgetIcon, ReportIcon, ImportIcon, ProfileIcon, LogoutIcon,
  MoneyIcon
} from './Icons'
import './Layout.css'

function Layout({ children }) {
  const { user, logout } = useAuth()
  const navigate = useNavigate()
  const location = useLocation()

  const handleLogout = () => {
    logout()
    navigate('/login')
  }

  const isActive = (path) => location.pathname === path

  return (
    <div>
      <nav className="navbar">
        <h1>
          <MoneyIcon size={28} color="#fbbf24" />
          Gastos Personales
        </h1>
        <Link to="/" className={isActive('/') ? 'active' : ''}>
          <DashboardIcon size={18} />
          Dashboard
        </Link>
        <Link to="/expenses" className={isActive('/expenses') ? 'active' : ''}>
          <ExpenseIcon size={18} />
          Gastos
        </Link>
        <Link to="/categories" className={isActive('/categories') ? 'active' : ''}>
          <CategoryIcon size={18} />
          Categorías
        </Link>
        <Link to="/payment-methods" className={isActive('/payment-methods') ? 'active' : ''}>
          <PaymentIcon size={18} />
          Métodos de Pago
        </Link>
        <Link to="/budgets" className={isActive('/budgets') ? 'active' : ''}>
          <BudgetIcon size={18} />
          Presupuestos
        </Link>
        <Link to="/reports" className={isActive('/reports') ? 'active' : ''}>
          <ReportIcon size={18} />
          Reportes
        </Link>
        <Link to="/import" className={isActive('/import') ? 'active' : ''}>
          <ImportIcon size={18} />
          Importar
        </Link>
        <Link to="/profile" className={isActive('/profile') ? 'active' : ''}>
          <ProfileIcon size={18} />
          Perfil
        </Link>
        <div className="user-info">
          <span>
            <ProfileIcon size={18} />
            {user?.name}
          </span>
          <button onClick={handleLogout} className="btn btn-secondary btn-sm">
            <LogoutIcon size={16} />
            Salir
          </button>
        </div>
      </nav>
      <div className="container">
        {children}
      </div>
    </div>
  )
}

export default Layout

