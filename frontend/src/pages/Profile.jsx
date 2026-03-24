import React, { useState } from 'react'
import { useAuth } from '../contexts/AuthContext'
import api from '../services/api'
import { ProfileIcon, MailIcon, UserIcon, LockIcon, SaveIcon, AlertIcon, CheckIcon } from '../components/Icons'

function Profile() {
  const { user } = useAuth()
  const [name, setName] = useState(user?.name || '')
  const [currentPassword, setCurrentPassword] = useState('')
  const [newPassword, setNewPassword] = useState('')
  const [confirmPassword, setConfirmPassword] = useState('')
  const [message, setMessage] = useState('')
  const [error, setError] = useState('')

  const handleUpdateProfile = async (e) => {
    e.preventDefault()
    setError('')
    setMessage('')

    try {
      await api.put('/profile/update', { name })
      setMessage('Perfil actualizado correctamente')
      window.location.reload()
    } catch (err) {
      setError(err.response?.data?.message || 'Error al actualizar perfil')
    }
  }

  const handleChangePassword = async (e) => {
    e.preventDefault()
    setError('')
    setMessage('')

    if (newPassword !== confirmPassword) {
      setError('Las contraseñas no coinciden')
      return
    }

    if (newPassword.length < 6) {
      setError('La contraseña debe tener al menos 6 caracteres')
      return
    }

    try {
      await api.put('/profile/change-password', {
        currentPassword,
        newPassword
      })
      setMessage('Contraseña actualizada correctamente')
      setCurrentPassword('')
      setNewPassword('')
      setConfirmPassword('')
    } catch (err) {
      setError(err.response?.data?.message || 'Error al cambiar contraseña')
    }
  }

  return (
    <div>
      <h1 className="page-title">
        <ProfileIcon size={48} color="#ffffff" />
        Mi Perfil
      </h1>

      <div className="card">
        <h2>
          <UserIcon size={24} color="#6366f1" />
          Información Personal
        </h2>
        {message && (
          <div className="alert alert-success">
            <CheckIcon size={20} color="#065f46" />
            {message}
          </div>
        )}
        {error && (
          <div className="alert alert-error">
            <AlertIcon size={20} color="#991b1b" />
            {error}
          </div>
        )}
        <form onSubmit={handleUpdateProfile}>
          <div className="form-group">
            <label>
              <MailIcon size={16} color="#6366f1" />
              Email
            </label>
            <input 
              type="email" 
              value={user?.email || ''} 
              disabled 
              style={{ background: '#f3f4f6', cursor: 'not-allowed' }}
            />
          </div>
          <div className="form-group">
            <label>
              <UserIcon size={16} color="#6366f1" />
              Nombre
            </label>
            <input
              type="text"
              value={name}
              onChange={(e) => setName(e.target.value)}
              required
              placeholder="Tu nombre completo"
            />
          </div>
          <button type="submit" className="btn btn-primary" style={{ marginTop: '10px' }}>
            <SaveIcon size={18} />
            Actualizar Perfil
          </button>
        </form>
      </div>

      <div className="card">
        <h2>
          <LockIcon size={24} color="#6366f1" />
          Cambiar Contraseña
        </h2>
        {error && (
          <div className="alert alert-error">
            <AlertIcon size={20} color="#991b1b" />
            {error}
          </div>
        )}
        {message && (
          <div className="alert alert-success">
            <CheckIcon size={20} color="#065f46" />
            {message}
          </div>
        )}
        <form onSubmit={handleChangePassword}>
          <div className="form-group">
            <label>
              <LockIcon size={16} color="#6366f1" />
              Contraseña Actual
            </label>
            <input
              type="password"
              value={currentPassword}
              onChange={(e) => setCurrentPassword(e.target.value)}
              required
              placeholder="••••••••"
            />
          </div>
          <div className="form-group">
            <label>
              <LockIcon size={16} color="#6366f1" />
              Nueva Contraseña
            </label>
            <input
              type="password"
              value={newPassword}
              onChange={(e) => setNewPassword(e.target.value)}
              required
              placeholder="Mínimo 6 caracteres"
            />
          </div>
          <div className="form-group">
            <label>
              <LockIcon size={16} color="#6366f1" />
              Confirmar Nueva Contraseña
            </label>
            <input
              type="password"
              value={confirmPassword}
              onChange={(e) => setConfirmPassword(e.target.value)}
              required
              placeholder="Repite la nueva contraseña"
            />
          </div>
          <button type="submit" className="btn btn-primary" style={{ marginTop: '10px' }}>
            <SaveIcon size={18} />
            Cambiar Contraseña
          </button>
        </form>
      </div>
    </div>
  )
}

export default Profile

