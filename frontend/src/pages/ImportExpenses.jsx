import React, { useState } from 'react'
import api from '../services/api'
import { ImportIcon, FileIcon, DownloadIcon, EmptyIcon, ChartIcon, AlertIcon } from '../components/Icons'

function ImportExpenses() {
  const [file, setFile] = useState(null)
  const [fileType, setFileType] = useState('csv')
  const [loading, setLoading] = useState(false)
  const [result, setResult] = useState(null)

  const handleFileChange = (e) => {
    setFile(e.target.files[0])
    setResult(null)
  }

  const handleSubmit = async (e) => {
    e.preventDefault()
    if (!file) {
      alert('Por favor seleccione un archivo')
      return
    }

    setLoading(true)
    setResult(null)

    try {
      const formData = new FormData()
      formData.append('file', file)

      const endpoint = `/import/${fileType}`
      const response = await api.post(endpoint, formData, {
        headers: {
          'Content-Type': 'multipart/form-data'
        }
      })

      setResult(response.data)
    } catch (err) {
      alert(err.response?.data?.message || 'Error al importar archivo')
    } finally {
      setLoading(false)
    }
  }

  return (
    <div>
      <h1 className="page-title">
        <ImportIcon size={48} color="#ffffff" />
        Importar Gastos
      </h1>

      <div className="card">
        <h2>
          <DownloadIcon size={24} color="#6366f1" />
          Subir Archivo
        </h2>
        <form onSubmit={handleSubmit}>
          <div className="form-group">
            <label>Tipo de Archivo</label>
            <select
              value={fileType}
              onChange={(e) => setFileType(e.target.value)}
            >
              <option value="csv">CSV</option>
              <option value="excel">Excel</option>
              <option value="json">JSON</option>
            </select>
          </div>
          <div className="form-group">
            <label>Archivo</label>
            <input
              type="file"
              onChange={handleFileChange}
              accept={fileType === 'json' ? '.json' : fileType === 'excel' ? '.xlsx,.xls' : '.csv'}
              required
            />
          </div>
          <button type="submit" className="btn btn-primary" disabled={loading || !file} style={{ width: '100%', marginTop: '10px' }}>
            {loading ? (
              <>
                <span className="pulse">Importando</span>
              </>
            ) : (
              <>
                <ImportIcon size={18} />
                Importar Archivo
              </>
            )}
          </button>
        </form>
      </div>

      {result && (
        <div className="card" style={{ animation: 'slideIn 0.3s ease-out' }}>
          <h2>
            <ChartIcon size={24} color="#6366f1" />
            Resultado de la Importación
          </h2>
          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(200px, 1fr))', gap: '20px', marginBottom: '20px' }}>
            <div style={{ padding: '16px', background: 'linear-gradient(135deg, #dbeafe 0%, #bfdbfe 100%)', borderRadius: '12px' }}>
              <div style={{ fontSize: '0.875rem', color: '#6b7280', marginBottom: '8px' }}>Total de Filas</div>
              <div style={{ fontSize: '1.5rem', fontWeight: '800', color: '#1e40af' }}>{result.totalRows}</div>
            </div>
            <div style={{ padding: '16px', background: 'linear-gradient(135deg, #d1fae5 0%, #a7f3d0 100%)', borderRadius: '12px' }}>
              <div style={{ fontSize: '0.875rem', color: '#6b7280', marginBottom: '8px' }}>✅ Exitosas</div>
              <div style={{ fontSize: '1.5rem', fontWeight: '800', color: '#065f46' }}>{result.successCount}</div>
            </div>
            <div style={{ padding: '16px', background: 'linear-gradient(135deg, #fee2e2 0%, #fecaca 100%)', borderRadius: '12px' }}>
              <div style={{ fontSize: '0.875rem', color: '#6b7280', marginBottom: '8px' }}>❌ Con Errores</div>
              <div style={{ fontSize: '1.5rem', fontWeight: '800', color: '#991b1b' }}>{result.errorCount}</div>
            </div>
          </div>

          {result.errors && result.errors.length > 0 && (
            <div style={{ marginTop: '20px' }}>
              <h3 style={{ marginBottom: '15px', color: '#ef4444', display: 'flex', alignItems: 'center', gap: '8px' }}>
                <AlertIcon size={20} color="#ef4444" />
                Errores Encontrados:
              </h3>
              <table>
                <thead>
                  <tr>
                    <th>Fila</th>
                    <th>Mensaje</th>
                    <th>Datos</th>
                  </tr>
                </thead>
                <tbody>
                  {result.errors.map((error, index) => (
                    <tr key={index}>
                      <td><strong>{error.rowNumber}</strong></td>
                      <td style={{ color: '#ef4444' }}>{error.message}</td>
                      <td style={{ fontSize: '0.875rem' }}>{error.data ? JSON.stringify(error.data) : '-'}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </div>
      )}

      <div className="card">
        <h2>
          <FileIcon size={24} color="#6366f1" />
          Formato de Archivos
        </h2>
        <div style={{ marginTop: '20px' }}>
          <div style={{ marginBottom: '30px', padding: '20px', background: 'linear-gradient(135deg, #f3f4f6 0%, #e5e7eb 100%)', borderRadius: '12px' }}>
            <h3 style={{ marginBottom: '15px', color: '#6366f1', display: 'flex', alignItems: 'center', gap: '8px' }}>
              <FileIcon size={20} color="#6366f1" />
              CSV
            </h3>
            <p style={{ marginBottom: '8px' }}><strong>Formato:</strong> Monto,Fecha,CategoriaId,PaymentMethodId,Descripción</p>
            <p style={{ padding: '12px', background: '#fff', borderRadius: '8px', fontFamily: 'monospace', fontSize: '0.875rem' }}>
              100.50,2024-12-01,guid-categoria,guid-metodo,Comida
            </p>
          </div>

          <div style={{ padding: '20px', background: 'linear-gradient(135deg, #f3f4f6 0%, #e5e7eb 100%)', borderRadius: '12px' }}>
            <h3 style={{ marginBottom: '15px', color: '#6366f1', display: 'flex', alignItems: 'center', gap: '8px' }}>
              <FileIcon size={20} color="#6366f1" />
              JSON
            </h3>
            <pre style={{ 
              padding: '16px', 
              background: '#1f2937', 
              color: '#10b981', 
              borderRadius: '8px', 
              overflow: 'auto',
              fontSize: '0.875rem',
              lineHeight: '1.6'
            }}>{`[
  {
    "amount": 100.50,
    "date": "2024-12-01",
    "categoryId": "guid-categoria",
    "paymentMethodId": "guid-metodo",
    "description": "Comida"
  }
]`}</pre>
          </div>
        </div>
      </div>
    </div>
  )
}

export default ImportExpenses

