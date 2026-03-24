import React, { useState, useEffect } from 'react'
import { BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, Legend, PieChart, Pie, Cell } from 'recharts'
import api from '../services/api'
import { ReportIcon, DownloadIcon, FilterIcon, ChartIcon, PieChartIcon, TrophyIcon, EmptyIcon, MoneyIcon, TrendingUpIcon } from '../components/Icons'

const COLORS = ['#0088FE', '#00C49F', '#FFBB28', '#FF8042', '#8884d8']

function Reports() {
  const [report, setReport] = useState(null)
  const [loading, setLoading] = useState(false)
  const [month, setMonth] = useState(new Date().getMonth() + 1)
  const [year, setYear] = useState(new Date().getFullYear())

  useEffect(() => {
    loadReport()
  }, [month, year])

  const loadReport = async () => {
    setLoading(true)
    try {
      const response = await api.get(`/reports/monthly?month=${month}&year=${year}`)
      setReport(response.data)
    } catch (err) {
      console.error('Error loading report:', err)
      alert('Error al cargar el reporte')
    } finally {
      setLoading(false)
    }
  }

  const handleExport = async (format) => {
    try {
      const response = await api.get(`/export/${format}?month=${month}&year=${year}`, {
        responseType: 'blob'
      })
      const url = window.URL.createObjectURL(new Blob([response.data]))
      const link = document.createElement('a')
      link.href = url
      link.setAttribute('download', `reporte_${month}_${year}.${format === 'excel' ? 'csv' : format}`)
      document.body.appendChild(link)
      link.click()
      link.remove()
    } catch (err) {
      alert('Error al exportar reporte')
    }
  }

  if (loading) return <div className="loading">Generando reporte</div>
  if (!report) return (
    <div className="empty-state">
      <div className="empty-state-icon">📊</div>
      <h3>No hay datos para mostrar</h3>
      <p>Registra algunos gastos para ver reportes</p>
    </div>
  )

  return (
    <div>
      <div className="page-header">
        <h1 className="page-title">
          <ReportIcon size={48} color="#ffffff" />
          Reportes Mensuales
        </h1>
        <div style={{ display: 'flex', gap: '10px', flexWrap: 'wrap' }}>
          <button className="btn btn-success" onClick={() => handleExport('excel')}>
            <DownloadIcon size={18} />
            Excel
          </button>
          <button className="btn btn-success" onClick={() => handleExport('txt')}>
            <DownloadIcon size={18} />
            TXT
          </button>
          <button className="btn btn-success" onClick={() => handleExport('json')}>
            <DownloadIcon size={18} />
            JSON
          </button>
        </div>
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

      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(250px, 1fr))', gap: '24px', marginBottom: '30px' }}>
        <div className="stats-card">
          <h3>
            <MoneyIcon size={16} color="#6b7280" />
            Total Gastado
          </h3>
          <div className="value" style={{ color: '#ef4444' }}>
            ${report.totalSpent.toFixed(2)}
          </div>
        </div>
        <div className="stats-card">
          <h3>
            <ChartIcon size={16} color="#6b7280" />
            Mes Anterior
          </h3>
          <div className="value" style={{ color: '#6366f1' }}>
            ${report.previousMonthTotal.toFixed(2)}
          </div>
        </div>
        <div className="stats-card">
          <h3>
            <TrendingUpIcon size={16} color="#6b7280" />
            Diferencia
          </h3>
          <div className="value" style={{ color: report.difference >= 0 ? '#ef4444' : '#10b981' }}>
            ${report.difference >= 0 ? '+' : ''}{report.difference.toFixed(2)}
          </div>
          <div className="change" style={{ color: report.percentageChange >= 0 ? '#ef4444' : '#10b981' }}>
            {report.percentageChange >= 0 ? '↑' : '↓'} {Math.abs(report.percentageChange).toFixed(2)}%
          </div>
        </div>
      </div>

      <div className="card">
        <h2>
          <ChartIcon size={24} color="#6366f1" />
          Desglose por Categoría
        </h2>
        <BarChart width={800} height={400} data={report.categorySummaries}>
          <CartesianGrid strokeDasharray="3 3" />
          <XAxis dataKey="categoryName" />
          <YAxis />
          <Tooltip />
          <Legend />
          <Bar dataKey="amount" fill="#8884d8" />
        </BarChart>
      </div>

      <div className="card">
        <h2>
          <PieChartIcon size={24} color="#6366f1" />
          Distribución por Categoría
        </h2>
        <PieChart width={400} height={400}>
          <Pie
            data={report.categorySummaries}
            cx={200}
            cy={200}
            labelLine={false}
            label={({ categoryName, percentage }) => `${categoryName}: ${percentage.toFixed(1)}%`}
            outerRadius={80}
            fill="#8884d8"
            dataKey="amount"
          >
            {report.categorySummaries.map((entry, index) => (
              <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
            ))}
          </Pie>
          <Tooltip />
        </PieChart>
      </div>

      <div className="card">
        <h2>
          <TrophyIcon size={24} color="#6366f1" />
          Top 5 Categorías
        </h2>
        <table>
          <thead>
            <tr>
              <th>Categoría</th>
              <th>Monto</th>
            </tr>
          </thead>
            <tbody>
              {report.topCategories.map((cat, index) => (
                <tr key={cat.categoryId}>
                  <td>
                    <span style={{ 
                      display: 'inline-flex', 
                      alignItems: 'center', 
                      justifyContent: 'center',
                      width: '32px', 
                      height: '32px',
                      borderRadius: '50%',
                      background: index === 0 ? 'linear-gradient(135deg, #fbbf24 0%, #f59e0b 100%)' :
                                   index === 1 ? 'linear-gradient(135deg, #94a3b8 0%, #64748b 100%)' :
                                   index === 2 ? 'linear-gradient(135deg, #f97316 0%, #ea580c 100%)' :
                                   'linear-gradient(135deg, #6366f1 0%, #4f46e5 100%)',
                      color: '#fff',
                      fontWeight: '700',
                      fontSize: '0.875rem',
                      marginRight: '12px'
                    }}>
                      {index + 1}
                    </span>
                    <strong>{cat.categoryName}</strong>
                  </td>
                  <td style={{ fontWeight: '700', color: '#ef4444', fontSize: '1.1rem' }}>
                    ${cat.amount.toFixed(2)}
                  </td>
                </tr>
              ))}
            </tbody>
        </table>
      </div>
    </div>
  )
}

export default Reports

