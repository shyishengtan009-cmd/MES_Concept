<template>
  <div class="pg-root">
    <div class="topbar">
      <span class="pg-title">&#9632; Batch Manufacturing Record (BMR)</span>
      <div class="topbar-right"><span class="live-dot"></span><span>{{ clock }}</span></div>
    </div>

    <div class="kpi-row">
      <div class="kpi-card"><div class="kpi-label">BMRs This Month</div><div class="kpi-val c-b">38</div><div class="kpi-sub">batches recorded</div></div>
      <div class="kpi-card"><div class="kpi-label">Pending Review</div><div class="kpi-val c-y">5</div><div class="kpi-sub">awaiting QA sign-off</div></div>
      <div class="kpi-card"><div class="kpi-label">Approved</div><div class="kpi-val c-g">31</div><div class="kpi-sub">released for dispatch</div></div>
      <div class="kpi-card"><div class="kpi-label">Rejected / Deviation</div><div class="kpi-val c-r">2</div><div class="kpi-sub">non-conformance raised</div></div>
    </div>

    <div class="filter-bar">
      <span class="fl">Status:</span>
      <button class="f-btn" :class="{ on: statusFilter === 'all' }" @click="statusFilter = 'all'">All</button>
      <button class="f-btn" :class="{ on: statusFilter === 'Approved' }" @click="statusFilter = 'Approved'">Approved</button>
      <button class="f-btn" :class="{ on: statusFilter === 'Pending' }" @click="statusFilter = 'Pending'">Pending</button>
      <button class="f-btn" :class="{ on: statusFilter === 'Deviation' }" @click="statusFilter = 'Deviation'">Deviation</button>
      <input class="f-search" type="text" placeholder="Search product / BMR no…" v-model="search" />
      <span class="result-count">{{ filteredRows.length }} shown</span>
    </div>

    <div class="body-full">
      <div class="pc" style="flex:1">
        <div class="ph">
          BMR Log <span class="ph-r">{{ filteredRows.length }} records</span>
          <button class="export-btn" @click="exportToExcel"><i class="fa-solid fa-arrow-up-from-bracket"></i> Export</button>
        </div>
        <div class="tbl-wrap">
          <table>
            <thead>
              <tr><th>BMR No.</th><th>Product</th><th>Batch No.</th><th>Production Date</th><th>Approved By</th><th>Status</th></tr>
            </thead>
            <tbody>
              <tr v-for="row in filteredRows" :key="row.bmrNo" class="row-click" @click="openDetail(row)">
                <td>{{ row.bmrNo }}</td>
                <td>{{ row.product }}</td>
                <td>{{ row.batch }}</td>
                <td>{{ row.date }}</td>
                <td>{{ row.approvedBy }}</td>
                <td><span class="badge" :class="statusBadge(row.status)">{{ row.status }}</span></td>
              </tr>
              <tr v-if="filteredRows.length === 0"><td colspan="6" class="empty-msg">No records match the current filter.</td></tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>

    <!-- Detail Panel -->
    <div v-if="detail" class="overlay" @click="detail = null"></div>
    <transition name="panel-slide">
      <div v-if="detail" class="detail-panel">
        <div class="dp-head">
          <div class="dp-head-left">
            <div class="dp-id">{{ detail.bmrNo }}</div>
            <div class="dp-sub">{{ detail.product }}</div>
          </div>
          <span class="badge" :class="statusBadge(detail.status)">{{ detail.status }}</span>
          <button class="dp-close" @click="detail = null"><i class="fa-solid fa-xmark"></i></button>
        </div>
        <div class="dp-body">
          <div class="dp-grid">
            <div class="dp-field"><span class="dfl">Batch No.</span><span>{{ detail.batch }}</span></div>
            <div class="dp-field"><span class="dfl">Production Date</span><span>{{ detail.date }}</span></div>
            <div class="dp-field"><span class="dfl">Approved By</span><span>{{ detail.approvedBy }}</span></div>
            <div class="dp-field"><span class="dfl">Status</span><span>{{ detail.status }}</span></div>
          </div>
          <div class="sec-title">Sign-off Trail</div>
          <div class="hist-log">
            <div class="hist-row"><span class="hist-dot" :class="statusBadge(detail.status) === 'b-r' ? 'dot-red' : statusBadge(detail.status) === 'b-y' ? 'dot-amber' : 'dot-green'"></span><span class="hist-txt">{{ detail.status }} — {{ detail.approvedBy }}</span></div>
          </div>
        </div>
        <div class="dp-foot">
          <button class="btn-outline" @click="detail = null">Close</button>
          <button class="btn-primary"><i class="fa-solid fa-file-pdf"></i> View BMR</button>
        </div>
      </div>
    </transition>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onBeforeUnmount } from 'vue'
import { findByBatch } from './src/inventoryCatalog'
import ExcelJS from 'exceljs'

const clock = ref('')
let clockTimer: ReturnType<typeof setInterval> | null = null
function tick() {
  clock.value = new Date().toLocaleString('en-MY', { day: '2-digit', month: 'short', year: 'numeric', hour: '2-digit', minute: '2-digit', hour12: true })
}

// Product names are derived from the batch via the shared catalog, so BMR always
// agrees with Finished Goods / Sample Retention / Traceability on which product a batch belongs to.
const BMR_ROWS = [
  { bmrNo: 'BMR-2026-0411', batch: 'B2026-0411', date: '11 Apr 2026', approvedBy: 'QA – Hafiz',    status: 'Approved'  },
  { bmrNo: 'BMR-2026-0412', batch: 'B2026-0412', date: '12 Apr 2026', approvedBy: '—',             status: 'Pending'   },
  { bmrNo: 'BMR-2026-0413', batch: 'B2026-0413', date: '13 Apr 2026', approvedBy: '—',             status: 'Pending'   },
  { bmrNo: 'BMR-2026-0409', batch: 'B2026-0409', date: '09 Apr 2026', approvedBy: 'QA – Hafiz',    status: 'Approved'  },
  { bmrNo: 'BMR-2026-0408', batch: 'B2026-0408', date: '08 Apr 2026', approvedBy: 'QA – Mei Ling', status: 'Approved'  },
  { bmrNo: 'BMR-2026-0407', batch: 'B2026-0407', date: '07 Apr 2026', approvedBy: 'QA – Hafiz',    status: 'Deviation' },
]

const rows = BMR_ROWS.map(r => ({ ...r, product: findByBatch(r.batch)?.name ?? '—' }))

type BmrRow = typeof rows[number]

const statusFilter = ref('all')
const search        = ref('')

const filteredRows = computed(() => {
  const q = search.value.toLowerCase()
  return rows.filter(r => {
    const statusOk = statusFilter.value === 'all' || r.status === statusFilter.value
    const searchOk = !q || (r.bmrNo + r.product).toLowerCase().includes(q)
    return statusOk && searchOk
  })
})

function statusBadge(status: string): string {
  if (status === 'Approved')  return 'b-g'
  if (status === 'Pending')   return 'b-y'
  if (status === 'Deviation') return 'b-r'
  return 'b-grey'
}

const detail = ref<BmrRow | null>(null)
function openDetail(row: BmrRow) { detail.value = row }

async function exportToExcel() {
  const wb = new ExcelJS.Workbook()
  wb.creator = 'HIAS'
  const ws = wb.addWorksheet('BMR Log')

  ws.columns = [
    { header: 'BMR No.',         key: 'bmrNo',       width: 16 },
    { header: 'Product',         key: 'product',     width: 26 },
    { header: 'Batch No.',       key: 'batch',        width: 14 },
    { header: 'Production Date', key: 'date',         width: 14 },
    { header: 'Approved By',     key: 'approvedBy',   width: 16 },
    { header: 'Status',          key: 'status',       width: 14 },
  ]

  const headerRow = ws.getRow(1)
  headerRow.height = 22
  headerRow.eachCell(cell => {
    cell.fill = { type: 'pattern', pattern: 'solid', fgColor: { argb: 'FF1565C0' } }
    cell.font = { bold: true, color: { argb: 'FFFFFFFF' }, size: 11, name: 'Calibri' }
    cell.alignment = { vertical: 'middle', horizontal: 'left' }
    cell.border = { bottom: { style: 'medium', color: { argb: 'FF0D47A1' } } }
  })

  filteredRows.value.forEach((r, i) => {
    const row = ws.addRow(r)
    row.height = 18
    const fillColor = i % 2 === 0 ? 'FFFFFFFF' : 'FFF3F7FB'
    row.eachCell(cell => {
      cell.fill = { type: 'pattern', pattern: 'solid', fgColor: { argb: fillColor } }
      cell.font = { size: 10, name: 'Calibri', color: { argb: 'FF333333' } }
      cell.alignment = { vertical: 'middle', horizontal: 'left' }
      cell.border = { bottom: { style: 'thin', color: { argb: 'FFE0E0E0' } } }
    })
    const statusCell = row.getCell('status')
    if (r.status === 'Approved')  { statusCell.font = { ...statusCell.font as ExcelJS.Font, color: { argb: 'FF2E7D32' }, bold: true } }
    if (r.status === 'Pending')   { statusCell.font = { ...statusCell.font as ExcelJS.Font, color: { argb: 'FFE65100' }, bold: true } }
    if (r.status === 'Deviation') { statusCell.font = { ...statusCell.font as ExcelJS.Font, color: { argb: 'FFC62828' }, bold: true } }
  })

  const buf  = await wb.xlsx.writeBuffer()
  const blob = new Blob([buf], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' })
  const url  = URL.createObjectURL(blob)
  const a    = document.createElement('a')
  a.href     = url
  a.download = `BMR_Log_${new Date().toISOString().slice(0, 10)}.xlsx`
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
.c-g { color: #388E3C; } .c-b { color: #1565c0; } .c-y { color: #f9a825; } .c-r { color: #e53935; } .c-d { color: #515151; }

.filter-bar { background: #fff; border-bottom: 1px solid #e8e8e8; padding: 5px 16px; display: flex; align-items: center; gap: 6px; flex-shrink: 0; flex-wrap: wrap; }
.fl    { font-size: 10px; color: #9e9e9e; font-weight: 600; text-transform: uppercase; letter-spacing: 0.4px; }
.f-btn { background: #f5f5f5; border: 1px solid #c3c6d4; border-radius: 3px; color: #515151; cursor: pointer; font-family: 'Poppins', sans-serif; font-size: 10px; font-weight: 600; padding: 2px 10px; transition: background .12s; }
.f-btn:hover { background: #e3f2fd; border-color: #90caf9; color: #1565c0; }
.f-btn.on    { background: #1565c0; border-color: #1565c0; color: #fff; }
.f-search    { border: 1px solid #c3c6d4; border-radius: 3px; font-family: 'Poppins', sans-serif; font-size: 11px; padding: 3px 8px; color: #515151; outline: none; width: 200px; margin-left: auto; }
.f-search:focus { border-color: #1565c0; }
.result-count { font-size: 10px; color: #9e9e9e; margin-left: 6px; }

.body-full { display: flex; flex-direction: column; padding: 10px; flex: 1; overflow: hidden; min-height: 0; }
.pc { background: #fff; border: 1px solid #c3c6d4; border-radius: 6px; overflow: hidden; display: flex; flex-direction: column; }
.ph { padding: 8px 12px; font-size: 11px; font-weight: 700; color: #515151; text-transform: uppercase; letter-spacing: 0.4px; flex-shrink: 0; background: linear-gradient(0deg, #d7d7d7 0%, #fff 100%); border-bottom: 1px solid #c3c6d4; display: flex; align-items: center; gap: 8px; }
.ph-r { font-size: 10px; font-weight: 400; color: #9e9e9e; text-transform: none; letter-spacing: 0; }
.export-btn { margin-left: auto; background: #1565c0; color: #fff; border: none; border-radius: 4px; padding: 4px 12px; font-family: 'Poppins', sans-serif; font-size: 10px; font-weight: 700; text-transform: none; letter-spacing: 0; cursor: pointer; display: flex; align-items: center; gap: 6px; transition: background .12s; }
.export-btn:hover { background: #0d47a1; }

.tbl-wrap { flex: 1; overflow-y: auto; overflow-x: hidden; }
.tbl-wrap::-webkit-scrollbar { width: 4px; }
.tbl-wrap::-webkit-scrollbar-thumb { background: #c3c6d4; border-radius: 2px; }
table { width: 100%; border-collapse: collapse; font-size: 11px; }
th { background: linear-gradient(0deg, #d7d7d7 0%, #fff 100%); color: #515151; font-size: 10px; text-transform: uppercase; padding: 7px 10px; text-align: left; border-bottom: 2px solid #c3c6d4; position: sticky; top: 0; z-index: 1; letter-spacing: 0.3px; white-space: nowrap; }
td { padding: 7px 10px; border-bottom: 1px solid #e8e8e8; color: #515151; white-space: nowrap; }
tbody tr:nth-child(even) td { background: #f9f9f9; }
.row-click { cursor: pointer; }
.row-click:hover td { background: #e3f2fd !important; }
.badge  { display: inline-block; padding: 1px 7px; border-radius: 3px; font-size: 9px; font-weight: 700; }
.b-g    { background: #e8f5e9; color: #388E3C; border: 1px solid #a5d6a7; }
.b-y    { background: #fff9c4; color: #f9a825; border: 1px solid #fdd835; }
.b-r    { background: #ffebee; color: #e53935; border: 1px solid #ef9a9a; }
.b-grey { background: #f5f5f5; color: #757575; border: 1px solid #bdbdbd; }
.empty-msg { padding: 24px; text-align: center; color: #9e9e9e; font-size: 11px; }

/* Detail Panel */
.overlay { background: rgba(0,0,0,.3); inset: 0; position: fixed; z-index: 200; }
.detail-panel { background: #fff; box-shadow: -4px 0 20px rgba(0,0,0,.12); display: flex; flex-direction: column; height: 100vh; position: fixed; right: 0; top: 0; width: 460px; z-index: 201; }
.panel-slide-enter-active, .panel-slide-leave-active { transition: transform .25s ease; }
.panel-slide-enter-from, .panel-slide-leave-to { transform: translateX(100%); }
.dp-head { align-items: flex-start; background: linear-gradient(0deg,#d7d7d7,#fff); border-bottom: 1px solid #c3c6d4; display: flex; gap: 10px; padding: 14px 16px; flex-shrink: 0; }
.dp-head-left { flex: 1; }
.dp-id  { font-size: 14px; font-weight: 700; color: #1565c0; }
.dp-sub { font-size: 11px; color: #757575; margin-top: 2px; }
.dp-close { background: transparent; border: none; color: #757575; cursor: pointer; font-size: 16px; margin-left: auto; padding: 2px 6px; }
.dp-close:hover { color: #e53935; }
.dp-body { flex: 1; overflow-y: auto; overflow-x: hidden; padding: 14px 16px; }
.dp-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 10px; margin-bottom: 16px; }
.dp-field { display: flex; flex-direction: column; gap: 2px; }
.dfl { color: #9e9e9e; font-size: 10px; text-transform: uppercase; }
.sec-title { color: #515151; font-size: 11px; font-weight: 700; margin-bottom: 6px; text-transform: uppercase; }
.hist-log { display: flex; flex-direction: column; gap: 8px; }
.hist-row { align-items: flex-start; display: flex; gap: 8px; }
.hist-dot { border-radius: 50%; flex-shrink: 0; height: 8px; margin-top: 3px; width: 8px; }
.dot-green { background: #388E3C; } .dot-amber { background: #f9a825; } .dot-red { background: #e53935; }
.hist-txt  { color: #515151; font-size: 11px; }
.dp-foot { border-top: 1px solid #c3c6d4; display: flex; gap: 8px; justify-content: flex-end; padding: 12px 16px; flex-shrink: 0; }
.btn-outline { background: #fff; border: 1px solid #d0d3e0; border-radius: 3px; color: #515151; cursor: pointer; font-family: 'Poppins', sans-serif; font-size: 11px; padding: 6px 16px; }
.btn-outline:hover { background: #f5f5f5; }
.btn-primary { background: linear-gradient(135deg, #1976d2, #0d47a1); border: none; border-radius: 3px; color: #fff; cursor: pointer; font-family: 'Poppins', sans-serif; font-size: 11px; gap: 6px; display: flex; align-items: center; padding: 6px 14px; transition: transform .12s ease, box-shadow .12s ease; }
.btn-primary:hover { transform: translateY(-1px); box-shadow: 0 4px 10px rgba(13,71,161,.4); }
</style>
