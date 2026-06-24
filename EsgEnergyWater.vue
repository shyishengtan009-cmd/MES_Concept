<template>
  <div class="pg-root">
    <div class="topbar">
      <span class="pg-title">&#9632; Energy & Water Monitoring</span>
      <div class="topbar-right"><span class="live-dot"></span><span>{{ clock }}</span></div>
    </div>

    <div class="kpi-row">
      <div class="kpi-card"><div class="kpi-label">Energy Today</div><div class="kpi-val c-b">8,420 kWh</div><div class="kpi-sub">vs 8,900 target</div></div>
      <div class="kpi-card"><div class="kpi-label">Water Today</div><div class="kpi-val c-g">142 m³</div><div class="kpi-sub">vs 160 m³ target</div></div>
      <div class="kpi-card"><div class="kpi-label">Energy Intensity</div><div class="kpi-val c-d">0.46 kWh/kg</div><div class="kpi-sub">per kg output</div></div>
      <div class="kpi-card"><div class="kpi-label">CO2 Equivalent</div><div class="kpi-val c-y">5.8 t</div><div class="kpi-sub">today's emissions</div></div>
    </div>

    <div class="body-full">
      <div class="pc" style="flex:1">
        <div class="ph">
          Utility Meters <span class="ph-r">live readings</span>
          <button class="export-btn" @click="exportToExcel"><i class="fa-solid fa-arrow-up-from-bracket"></i> Export</button>
        </div>
        <div class="pc-body">
          <div class="cat-row" v-for="m in meters" :key="m.name">
            <span class="cat-name">{{ m.name }}</span>
            <div class="cat-bg"><div class="cat-fill" :style="{ width: m.pct + '%', background: m.color }"></div></div>
            <span class="cat-val">{{ m.val }}</span>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onBeforeUnmount } from 'vue'
import ExcelJS from 'exceljs'
const clock = ref('')
let clockTimer: ReturnType<typeof setInterval> | null = null
function tick() { clock.value = new Date().toLocaleString('en-MY', { day: '2-digit', month: 'short', year: 'numeric', hour: '2-digit', minute: '2-digit', hour12: true }) }

const meters = [
  { name: 'FFS Packaging Line 01', val: '2,140 kWh', pct: 84, color: '#1565c0' },
  { name: 'FFS Packaging Line 02', val: '1,980 kWh', pct: 78, color: '#1565c0' },
  { name: 'Batching Plant',         val: '2,800 kWh', pct: 92, color: '#7b1fa2' },
  { name: 'Cold Storage',           val: '1,500 kWh', pct: 60, color: '#388E3C' },
  { name: 'Process Water',          val: '98 m³',      pct: 70, color: '#0288d1' },
  { name: 'Sanitation Water',       val: '44 m³',      pct: 45, color: '#0288d1' },
]

async function exportToExcel() {
  const wb = new ExcelJS.Workbook()
  wb.creator = 'HIAS'
  const ws = wb.addWorksheet('Utility Meters')

  ws.columns = [
    { header: 'Meter',   key: 'name', width: 24 },
    { header: 'Reading', key: 'val',  width: 16 },
    { header: '% of Target', key: 'pct', width: 14 },
  ]

  const headerRow = ws.getRow(1)
  headerRow.height = 22
  headerRow.eachCell(cell => {
    cell.fill = { type: 'pattern', pattern: 'solid', fgColor: { argb: 'FF1565C0' } }
    cell.font = { bold: true, color: { argb: 'FFFFFFFF' }, size: 11, name: 'Calibri' }
    cell.alignment = { vertical: 'middle', horizontal: 'left' }
    cell.border = { bottom: { style: 'medium', color: { argb: 'FF0D47A1' } } }
  })

  meters.forEach((m, i) => {
    const row = ws.addRow(m)
    row.height = 18
    const fillColor = i % 2 === 0 ? 'FFFFFFFF' : 'FFF3F7FB'
    row.eachCell(cell => {
      cell.fill = { type: 'pattern', pattern: 'solid', fgColor: { argb: fillColor } }
      cell.font = { size: 10, name: 'Calibri', color: { argb: 'FF333333' } }
      cell.alignment = { vertical: 'middle', horizontal: 'left' }
      cell.border = { bottom: { style: 'thin', color: { argb: 'FFE0E0E0' } } }
    })
  })

  const buf  = await wb.xlsx.writeBuffer()
  const blob = new Blob([buf], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' })
  const url  = URL.createObjectURL(blob)
  const a    = document.createElement('a')
  a.href     = url
  a.download = `Energy_Water_${new Date().toISOString().slice(0, 10)}.xlsx`
  a.click()
  URL.revokeObjectURL(url)
}

onMounted(() => { tick(); clockTimer = setInterval(tick, 1000) })
onBeforeUnmount(() => { if (clockTimer) clearInterval(clockTimer) })
</script>

<style scoped>
*, *::before, *::after { box-sizing: border-box; margin: 0; padding: 0; }
.pg-root { height: 100%; overflow: hidden; font-family: 'Poppins', sans-serif; font-size: 12px; background: #f5f5f5; color: #333; display: flex; flex-direction: column; }
.topbar { background: #fff; border-bottom: 1px solid #c3c6d4; padding: 0 16px; display: flex; align-items: center; flex-shrink: 0; box-shadow: 0 1px 3px rgba(0,0,0,.06); height: 42px; }
.pg-title { font-size: 13px; font-weight: 700; color: #515151; letter-spacing: 0.5px; text-transform: uppercase; white-space: nowrap; }
.topbar-right { margin-left: auto; display: flex; align-items: center; font-size: 10px; color: #9e9e9e; gap: 6px; }
.live-dot { width: 7px; height: 7px; border-radius: 50%; background: #43a047; animation: pulse 1.4s infinite; display: inline-block; }
@keyframes pulse { 0%,100%{ opacity:1 } 50%{ opacity:0.3 } }
.kpi-row  { display: flex; gap: 1px; background: #c3c6d4; border-bottom: 2px solid #c3c6d4; flex-shrink: 0; }
.kpi-card { flex: 1; background: #fff; padding: 9px 16px; display: flex; flex-direction: column; gap: 3px; position: relative; overflow: hidden; border-left: 3px solid #1565c0; transition: transform .15s ease, box-shadow .15s ease; }
.kpi-card:hover { transform: translateY(-2px); box-shadow: 0 6px 14px rgba(0,0,0,.12); z-index: 2; }
.kpi-card::before { content: '\f201'; font-family: 'Font Awesome 6 Free'; font-weight: 900; position: absolute; right: 6px; top: 2px; font-size: 36px; color: #1565c0; opacity: .08; pointer-events: none; }
.kpi-card::after { content: ''; position: absolute; left: 0; right: 0; bottom: 0; height: 3px; background: linear-gradient(90deg, transparent, #1565c0, transparent); background-size: 200% 100%; animation: kpi-shimmer 2.5s linear infinite; opacity: .35; }
@keyframes kpi-shimmer { 0% { background-position: 200% 0; } 100% { background-position: -200% 0; } }
.kpi-label{ font-size: 10px; color: #7f7f7f; text-transform: uppercase; letter-spacing: 0.5px; position: relative; z-index: 1; }
.kpi-val  { font-size: 22px; font-weight: 800; line-height: 1.1; position: relative; z-index: 1; }
.kpi-sub  { font-size: 10px; color: #9e9e9e; }
.c-g { color: #388E3C; } .c-b { color: #1565c0; } .c-y { color: #f9a825; } .c-d { color: #515151; }
.body-full { display: flex; flex-direction: column; padding: 10px; flex: 1; overflow: hidden; min-height: 0; }
.pc { background: #fff; border: 1px solid #c3c6d4; border-radius: 6px; overflow: hidden; display: flex; flex-direction: column; }
.ph { padding: 8px 12px; font-size: 11px; font-weight: 700; color: #515151; text-transform: uppercase; letter-spacing: 0.4px; flex-shrink: 0; background: linear-gradient(0deg, #d7d7d7 0%, #fff 100%); border-bottom: 1px solid #c3c6d4; display: flex; align-items: center; gap: 8px; }
.ph-r { font-size: 10px; font-weight: 400; color: #9e9e9e; text-transform: none; letter-spacing: 0; }
.export-btn { margin-left: auto; background: #1565c0; color: #fff; border: none; border-radius: 4px; padding: 4px 12px; font-family: 'Poppins', sans-serif; font-size: 10px; font-weight: 700; text-transform: none; letter-spacing: 0; cursor: pointer; display: flex; align-items: center; gap: 6px; transition: background .12s; }
.export-btn:hover { background: #0d47a1; }
.pc-body { padding: 14px 16px; overflow-y: auto; overflow-x: hidden; flex: 1; }
.cat-row  { display: flex; align-items: center; gap: 8px; margin-bottom: 12px; }
.cat-name { width: 160px; font-size: 11px; color: #515151; flex-shrink: 0; }
.cat-bg   { flex: 1; background: #e0e0e0; border-radius: 3px; height: 8px; }
.cat-fill { height: 8px; border-radius: 3px; }
.cat-val  { font-size: 11px; font-weight: 600; color: #333; width: 80px; text-align: right; flex-shrink: 0; }
</style>
