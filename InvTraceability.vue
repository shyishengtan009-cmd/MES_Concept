<template>
  <div class="pg-root">
    <div class="topbar">
      <span class="pg-title">&#9632; Traceability</span>
      <div class="topbar-right"><span class="live-dot"></span><span>{{ clock }}</span></div>
    </div>

    <div class="filter-bar">
      <span class="fl">Trace by:</span>
      <button class="f-btn" :class="{ on: traceBy === 'batch' }" @click="traceBy = 'batch'">Finished Batch No.</button>
      <button class="f-btn" :class="{ on: traceBy === 'rm'    }" @click="traceBy = 'rm'">Raw Material Lot</button>
      <input class="f-search" type="text" v-model="query" placeholder="Enter batch / lot number…" />
      <button class="trace-btn" @click="search"><i class="fa-solid fa-magnifying-glass"></i> Trace</button>
    </div>

    <div class="body-full">
      <div class="pc" style="flex: 0 0 auto">
        <div class="ph">Trace Chain <span class="ph-r">{{ query || 'B2026-0411' }}</span></div>
        <div class="pc-body">
          <div class="chain-row">
            <div class="chain-node">
              <div class="cn-label">Raw Materials</div>
              <div class="cn-val">RM-001, RM-005, RM-009</div>
              <div class="cn-sub">3 lots consumed</div>
            </div>
            <span class="chain-arrow">&#8594;</span>
            <div class="chain-node">
              <div class="cn-label">WIP Batch</div>
              <div class="cn-val">WIP-2026-114</div>
              <div class="cn-sub">Batching Plant 01</div>
            </div>
            <span class="chain-arrow">&#8594;</span>
            <div class="chain-node">
              <div class="cn-label">Finished Batch</div>
              <div class="cn-val c-b">B2026-0411</div>
              <div class="cn-sub">Chili Sauce Pouch 250g</div>
            </div>
            <span class="chain-arrow">&#8594;</span>
            <div class="chain-node">
              <div class="cn-label">Dispatch</div>
              <div class="cn-val">ISO-00389, ISO-00392</div>
              <div class="cn-sub">2 customers</div>
            </div>
          </div>
        </div>
      </div>

      <div class="pc" style="flex:1; margin-top:10px">
        <div class="ph">
          Recent Trace Lookups <span class="ph-r">{{ rows.length }} lookups</span>
          <button class="export-btn" @click="exportToExcel"><i class="fa-solid fa-arrow-up-from-bracket"></i> Export</button>
        </div>
        <div class="tbl-wrap">
          <table>
            <thead><tr><th>Time</th><th>Trace Target</th><th>Type</th><th>Result</th><th>By</th></tr></thead>
            <tbody>
              <tr v-for="(row, i) in rows" :key="i">
                <td>{{ row.time }}</td>
                <td>{{ row.target }}</td>
                <td>{{ row.type }}</td>
                <td><span class="badge" :class="row.result === 'Complete' ? 'b-g' : 'b-y'">{{ row.result }}</span></td>
                <td>{{ row.by }}</td>
              </tr>
            </tbody>
          </table>
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
function tick() {
  clock.value = new Date().toLocaleString('en-MY', { day: '2-digit', month: 'short', year: 'numeric', hour: '2-digit', minute: '2-digit', hour12: true })
}

const traceBy = ref('batch')
const query   = ref('')

function search() { /* mock — wires to real trace lookup later */ }

const rows = [
  { time: '10:38', target: 'B2026-0411', type: 'Finished Batch', result: 'Complete', by: 'Amirul'   },
  { time: '09:55', target: 'RM-005-220', type: 'Raw Material Lot', result: 'Complete', by: 'Farah'  },
  { time: 'Yest 16:10', target: 'B2025-0118', type: 'Finished Batch', result: 'Partial', by: 'Saliza' },
]

async function exportToExcel() {
  const wb = new ExcelJS.Workbook()
  wb.creator = 'HIAS'
  const ws = wb.addWorksheet('Trace Lookups')

  ws.columns = [
    { header: 'Time',         key: 'time',   width: 14 },
    { header: 'Trace Target', key: 'target', width: 16 },
    { header: 'Type',         key: 'type',   width: 18 },
    { header: 'Result',       key: 'result', width: 12 },
    { header: 'By',           key: 'by',     width: 12 },
  ]

  const headerRow = ws.getRow(1)
  headerRow.height = 22
  headerRow.eachCell(cell => {
    cell.fill = { type: 'pattern', pattern: 'solid', fgColor: { argb: 'FF1565C0' } }
    cell.font = { bold: true, color: { argb: 'FFFFFFFF' }, size: 11, name: 'Calibri' }
    cell.alignment = { vertical: 'middle', horizontal: 'left' }
    cell.border = { bottom: { style: 'medium', color: { argb: 'FF0D47A1' } } }
  })

  rows.forEach((r, i) => {
    const row = ws.addRow(r)
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
  a.download = `Trace_Lookups_${new Date().toISOString().slice(0, 10)}.xlsx`
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

.filter-bar { background: #fff; border-bottom: 1px solid #e8e8e8; padding: 6px 16px; display: flex; align-items: center; gap: 6px; flex-shrink: 0; flex-wrap: wrap; }
.fl    { font-size: 10px; color: #9e9e9e; font-weight: 600; text-transform: uppercase; letter-spacing: 0.4px; }
.f-btn { background: #f5f5f5; border: 1px solid #c3c6d4; border-radius: 3px; color: #515151; cursor: pointer; font-family: 'Poppins', sans-serif; font-size: 10px; font-weight: 600; padding: 2px 10px; transition: background .12s; }
.f-btn:hover { background: #e3f2fd; border-color: #90caf9; color: #1565c0; }
.f-btn.on    { background: #1565c0; border-color: #1565c0; color: #fff; }
.f-search    { border: 1px solid #c3c6d4; border-radius: 3px; font-family: 'Poppins', sans-serif; font-size: 11px; padding: 4px 8px; color: #515151; outline: none; width: 220px; }
.f-search:focus { border-color: #1565c0; }
.trace-btn { background: linear-gradient(135deg, #1976d2, #0d47a1); color: #fff; border: none; border-radius: 3px; padding: 4px 14px; font-family: 'Poppins', sans-serif; font-size: 11px; font-weight: 600; cursor: pointer; display: flex; align-items: center; gap: 6px; transition: transform .12s ease, box-shadow .12s ease; }
.trace-btn:hover { transform: translateY(-1px); box-shadow: 0 4px 10px rgba(13,71,161,.4); }

.body-full { display: flex; flex-direction: column; padding: 10px; flex: 1; overflow: hidden; min-height: 0; }
.pc { background: #fff; border: 1px solid #c3c6d4; border-radius: 6px; overflow: hidden; display: flex; flex-direction: column; }
.ph { padding: 8px 12px; font-size: 11px; font-weight: 700; color: #515151; text-transform: uppercase; letter-spacing: 0.4px; flex-shrink: 0; background: linear-gradient(0deg, #d7d7d7 0%, #fff 100%); border-bottom: 1px solid #c3c6d4; display: flex; align-items: center; gap: 8px; }
.ph-r { font-size: 10px; font-weight: 400; color: #9e9e9e; text-transform: none; letter-spacing: 0; margin-left: auto; }
.export-btn { background: #1565c0; color: #fff; border: none; border-radius: 4px; padding: 4px 12px; font-family: 'Poppins', sans-serif; font-size: 10px; font-weight: 700; text-transform: none; letter-spacing: 0; cursor: pointer; display: flex; align-items: center; gap: 6px; transition: background .12s; }
.export-btn:hover { background: #0d47a1; }
.pc-body { padding: 16px; overflow-x: auto; overflow-y: hidden; }

.chain-row { display: flex; align-items: center; gap: 14px; }
.chain-node { background: #f5f9ff; border: 1px solid #c3c6d4; border-radius: 6px; padding: 10px 14px; min-width: 150px; flex-shrink: 0; }
.cn-label { font-size: 10px; color: #9e9e9e; text-transform: uppercase; letter-spacing: 0.4px; margin-bottom: 4px; }
.cn-val { font-size: 13px; font-weight: 700; color: #333; }
.cn-sub { font-size: 10px; color: #7f7f7f; margin-top: 2px; }
.c-b { color: #1565c0 !important; }
.chain-arrow { font-size: 18px; color: #c3c6d4; flex-shrink: 0; }

.tbl-wrap { flex: 1; overflow-y: auto; overflow-x: hidden; }
table { width: 100%; border-collapse: collapse; font-size: 11px; }
th { background: linear-gradient(0deg, #d7d7d7 0%, #fff 100%); color: #515151; font-size: 10px; text-transform: uppercase; padding: 7px 10px; text-align: left; border-bottom: 2px solid #c3c6d4; position: sticky; top: 0; z-index: 1; letter-spacing: 0.3px; white-space: nowrap; }
td { padding: 7px 10px; border-bottom: 1px solid #e8e8e8; color: #515151; white-space: nowrap; }
tbody tr:nth-child(even) td { background: #f9f9f9; }
tbody tr:hover td { background: #f2f2f2 !important; }

.badge { display: inline-block; padding: 1px 7px; border-radius: 3px; font-size: 9px; font-weight: 700; }
.b-g   { background: #e8f5e9; color: #388E3C; border: 1px solid #a5d6a7; }
.b-y   { background: #fff9c4; color: #f9a825; border: 1px solid #fdd835; }
</style>
