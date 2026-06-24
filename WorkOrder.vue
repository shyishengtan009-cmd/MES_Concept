<template>
  <div class="root">

    <!-- Topbar -->
    <div class="topbar">
      <span class="pg-title">&#9632; Work Order</span>
      <div class="filters">
        <select class="sel" v-model="lineFilter">
          <option value="All">All Lines</option>
          <option value="Line 1">Line 1</option>
          <option value="Line 2">Line 2</option>
          <option value="Batch Plant 01">Batch Plant 01</option>
          <option value="Batch Plant 02">Batch Plant 02</option>
        </select>
        <button v-for="s in statuses" :key="s" class="sf-btn" :class="{ 'sf-btn--active': statusFilter === s }" @click="statusFilter = s">{{ s }}</button>
        <div class="search-box">
          <i class="fa-solid fa-magnifying-glass"></i>
          <input v-model="search" placeholder="Search WO / Product…" />
        </div>
        <button class="btn-primary"><i class="fa-solid fa-plus"></i> New WO</button>
        <button class="btn-outline" @click="exportToExcel"><i class="fa-solid fa-arrow-up-from-bracket"></i> Export</button>
      </div>
    </div>

    <!-- KPI Strip -->
    <div class="kpi-strip">
      <div class="kc" v-for="k in kpis" :key="k.label">
        <div class="kc-lbl">{{ k.label }}</div>
        <div class="kc-val" :class="k.cls">{{ k.val }}</div>
      </div>
    </div>

    <!-- Table -->
    <div class="tbl-wrap">
      <table class="tbl">
        <thead>
          <tr>
            <th>WO Number</th>
            <th>SO Reference</th>
            <th>Product</th>
            <th>Line</th>
            <th class="tr">Qty Planned</th>
            <th class="tr">Qty Actual</th>
            <th>Progress</th>
            <th>Start Date</th>
            <th>End Date</th>
            <th>Status</th>
            <th class="tc">Action</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="w in filtered" :key="w.woNo" class="tbl-row" @click="openPanel(w)">
            <td class="mono">{{ w.woNo }}</td>
            <td class="mono link">{{ w.soRef }}</td>
            <td>{{ w.product }}</td>
            <td><span class="line-badge">{{ w.line }}</span></td>
            <td class="tr">{{ w.qtyPlan.toLocaleString() }}</td>
            <td class="tr">{{ w.qtyActual.toLocaleString() }}</td>
            <td style="min-width:100px">
              <div class="prog-wrap">
                <div class="prog-bar" :style="{ width: progPct(w) + '%', background: progColor(w.status) }"></div>
              </div>
              <div class="prog-lbl">{{ progPct(w) }}%</div>
            </td>
            <td>{{ w.start }}</td>
            <td>{{ w.end }}</td>
            <td><span class="badge" :class="woCls(w.status)">{{ w.status }}</span></td>
            <td class="tc" @click.stop>
              <button class="act-btn" title="View"><i class="fa-solid fa-eye"></i></button>
              <button class="act-btn" title="Edit"><i class="fa-solid fa-pen"></i></button>
            </td>
          </tr>
          <tr v-if="!filtered.length">
            <td colspan="11" class="empty">No work orders match the current filter.</td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Overlay -->
    <div v-if="panel" class="overlay" @click="panel = null"></div>

    <!-- Detail Panel -->
    <transition name="panel-slide">
      <div v-if="panel" class="detail-panel">
        <div class="dp-head">
          <div class="dp-head-left">
            <div class="dp-wo">{{ panel.woNo }}</div>
            <div class="dp-prod">{{ panel.product }}</div>
          </div>
          <span class="badge" :class="woCls(panel.status)">{{ panel.status }}</span>
          <button class="dp-close" @click="panel = null"><i class="fa-solid fa-xmark"></i></button>
        </div>

        <div class="dp-body">
          <div class="dp-grid">
            <div class="dp-field"><span class="dfl">SO Reference</span><span class="mono">{{ panel.soRef }}</span></div>
            <div class="dp-field"><span class="dfl">Production Line</span><span>{{ panel.line }}</span></div>
            <div class="dp-field"><span class="dfl">Start Date</span><span>{{ panel.start }}</span></div>
            <div class="dp-field"><span class="dfl">End Date</span><span>{{ panel.end }}</span></div>
            <div class="dp-field"><span class="dfl">Qty Planned</span><span>{{ panel.qtyPlan.toLocaleString() }} pcs</span></div>
            <div class="dp-field"><span class="dfl">Qty Produced</span><span>{{ panel.qtyActual.toLocaleString() }} pcs</span></div>
            <div class="dp-field full">
              <span class="dfl">Overall Progress</span>
              <div style="display:flex; align-items:center; gap:10px; margin-top:4px">
                <div class="prog-wrap" style="flex:1; height:10px">
                  <div class="prog-bar" :style="{ width: progPct(panel) + '%', background: progColor(panel.status) }"></div>
                </div>
                <span style="font-weight:700; color:#1565c0">{{ progPct(panel) }}%</span>
              </div>
            </div>
          </div>

          <div class="sec-title">Material Requirements</div>
          <table class="dp-tbl">
            <thead><tr><th>Component</th><th>Required</th><th>Issued</th><th>UOM</th><th>Status</th></tr></thead>
            <tbody>
              <tr v-for="m in getMaterials(panel.woNo)" :key="m.name">
                <td>{{ m.name }}</td>
                <td class="tr">{{ m.required.toLocaleString() }}</td>
                <td class="tr">{{ m.issued.toLocaleString() }}</td>
                <td>{{ m.uom }}</td>
                <td><span class="badge" :class="m.issued >= m.required ? 'b-green' : 'b-amber'">{{ m.issued >= m.required ? 'Issued' : 'Partial' }}</span></td>
              </tr>
            </tbody>
          </table>

          <div class="sec-title" style="margin-top:14px">Production Log</div>
          <div class="prod-log">
            <div v-for="ev in getLog(panel)" :key="ev.time" class="log-row">
              <span class="log-time">{{ ev.time }}</span>
              <span class="log-dot" :class="ev.dot"></span>
              <span class="log-txt">{{ ev.text }}</span>
            </div>
          </div>
        </div>

        <div class="dp-foot">
          <button class="btn-outline" @click="panel = null">Close</button>
          <button class="btn-primary"><i class="fa-solid fa-print"></i> Print Traveller</button>
        </div>
      </div>
    </transition>

  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import ExcelJS from 'exceljs'

interface WO { woNo: string; soRef: string; product: string; line: string; qtyPlan: number; qtyActual: number; start: string; end: string; status: string }
interface Material { name: string; required: number; issued: number; uom: string }

const statuses = ['All', 'Running', 'Pending', 'Completed', 'On Hold'] as const
type Status = typeof statuses[number]

const statusFilter = ref<Status>('All')
const lineFilter   = ref('All')
const search       = ref('')
const panel        = ref<WO | null>(null)

const workOrders = ref<WO[]>([
  { woNo:'WO-2026-041', soRef:'SO-2026-0412', product:'Halal Seasoning Blend A 500g', line:'Line 1',        qtyPlan:2400, qtyActual:1960, start:'22/04/2026', end:'23/04/2026', status:'Running'   },
  { woNo:'WO-2026-042', soRef:'SO-2026-0412', product:'Halal Marinade Sauce 250ml',   line:'Line 2',        qtyPlan:1800, qtyActual:0,    start:'23/04/2026', end:'24/04/2026', status:'Pending'   },
  { woNo:'WO-2026-043', soRef:'SO-2026-0415', product:'Halal Curry Paste 200g',       line:'Batch Plant 01',qtyPlan:600,  qtyActual:600,  start:'20/04/2026', end:'21/04/2026', status:'Completed' },
  { woNo:'WO-2026-044', soRef:'SO-2026-0418', product:'Halal Marinade Sauce 250ml',   line:'Line 1',        qtyPlan:2400, qtyActual:0,    start:'24/04/2026', end:'25/04/2026', status:'Pending'   },
  { woNo:'WO-2026-045', soRef:'SO-2026-0418', product:'Spice Mix Pack 1kg',           line:'Batch Plant 02',qtyPlan:600,  qtyActual:360,  start:'22/04/2026', end:'23/04/2026', status:'Running'   },
  { woNo:'WO-2026-038', soRef:'SO-2026-0407', product:'Halal Seasoning Blend A 500g', line:'Line 2',        qtyPlan:2400, qtyActual:2400, start:'18/04/2026', end:'19/04/2026', status:'Completed' },
  { woNo:'WO-2026-039', soRef:'SO-2026-0407', product:'Mixed Spice Sachet 50g',       line:'Line 1',        qtyPlan:4800, qtyActual:4760, start:'18/04/2026', end:'20/04/2026', status:'Completed' },
  { woNo:'WO-2026-040', soRef:'SO-2026-0410', product:'Palm Olein Oil 5L',            line:'Batch Plant 01',qtyPlan:600,  qtyActual:0,    start:'21/04/2026', end:'22/04/2026', status:'On Hold'   },
  { woNo:'WO-2026-046', soRef:'SO-2026-0420', product:'Halal Curry Paste 200g',       line:'Line 1',        qtyPlan:2400, qtyActual:0,    start:'25/04/2026', end:'26/04/2026', status:'Pending'   },
  { woNo:'WO-2026-047', soRef:'SO-2026-0420', product:'Spice Mix Pack 1kg',           line:'Batch Plant 02',qtyPlan:600,  qtyActual:0,    start:'25/04/2026', end:'26/04/2026', status:'Pending'   },
])

const matMap: Record<string, Material[]> = {
  'WO-2026-041': [{ name:'Coriander Powder',    required:360000, issued:360000, uom:'g' }, { name:'Salt (Refined)',       required:288000, issued:288000, uom:'g' }, { name:'Stand-up Pouch 500g', required:2400,   issued:2400,   uom:'Pcs' }, { name:'Sticker Label',       required:2400,   issued:2400,   uom:'Pcs' }],
  'WO-2026-042': [{ name:'Chilli Paste',         required:432,    issued:216,   uom:'kg' }, { name:'Garlic Extract',      required:72,     issued:0,      uom:'kg' }, { name:'Glass Bottle 250ml',  required:1800,   issued:0,      uom:'Pcs' }],
  'WO-2026-043': [{ name:'Halal Chilli Paste',   required:120,    issued:120,   uom:'kg' }, { name:'Garlic Extract',      required:24,     issued:24,     uom:'kg' }, { name:'Jar 200g',            required:600,    issued:600,    uom:'Pcs' }],
  'WO-2026-045': [{ name:'Cumin Powder',          required:300,    issued:180,   uom:'kg' }, { name:'Coriander Powder',    required:240,    issued:120,    uom:'kg' }, { name:'Bag 1kg',             required:600,    issued:0,      uom:'Pcs' }],
  'WO-2026-040': [{ name:'Palm Olein (bulk)',     required:3000,   issued:0,     uom:'L'  }, { name:'Bottle 5L',           required:600,    issued:600,    uom:'Pcs' }, { name:'Cap',                 required:600,    issued:600,    uom:'Pcs' }],
}

const logMap: Record<string, Array<{ time: string; text: string; dot: string }>> = {
  'WO-2026-041': [
    { time:'22/04 06:00', text:'WO released to production floor — Line 1', dot:'dot-blue' },
    { time:'22/04 06:42', text:'Material issued: Coriander Powder, Salt, Packaging', dot:'dot-blue' },
    { time:'22/04 09:14', text:'Fill weight deviation alarm — M01 +3.8 g over tolerance', dot:'dot-red' },
    { time:'22/04 09:36', text:'Alarm cleared — calibration adjusted', dot:'dot-green' },
    { time:'22/04 14:00', text:'Shift 1 handover — 1,180 pcs produced, no defects', dot:'dot-blue' },
    { time:'22/04 14:30', text:'Shift 2 continues — current output 1,960 / 2,400', dot:'dot-blue' },
  ],
  'WO-2026-040': [
    { time:'21/04 08:00', text:'WO released — Batch Plant 01', dot:'dot-blue' },
    { time:'21/04 09:30', text:'Palm Olein bulk lot QC sampled — pending lab result', dot:'dot-amber' },
    { time:'21/04 11:15', text:'QC HOLD raised — FFA level exceeds spec. WO paused.', dot:'dot-red' },
    { time:'22/04 08:00', text:'Re-inspection scheduled — awaiting supplier response', dot:'dot-amber' },
  ],
}

const kpis = computed(() => [
  { label: 'Total WOs',  val: workOrders.value.length,                                           cls:'c-blue'   },
  { label: 'Running',    val: workOrders.value.filter(w => w.status==='Running').length,         cls:'c-blue'   },
  { label: 'Pending',    val: workOrders.value.filter(w => w.status==='Pending').length,         cls:'c-amber'  },
  { label: 'Completed',  val: workOrders.value.filter(w => w.status==='Completed').length,       cls:'c-green'  },
  { label: 'On Hold',    val: workOrders.value.filter(w => w.status==='On Hold').length,         cls:'c-red'    },
])

const filtered = computed(() => {
  const q = search.value.toLowerCase()
  return workOrders.value.filter(w => {
    const sOk = statusFilter.value === 'All' || w.status === statusFilter.value
    const lOk = lineFilter.value   === 'All' || w.line   === lineFilter.value
    const qOk = !q || w.woNo.toLowerCase().includes(q) || w.product.toLowerCase().includes(q)
    return sOk && lOk && qOk
  })
})

function progPct(w: WO): number { return w.qtyPlan ? Math.round(w.qtyActual / w.qtyPlan * 100) : 0 }
function progColor(s: string) { return s==='Completed' ? '#388E3C' : s==='On Hold' ? '#e53935' : '#1565c0' }
function getMaterials(woNo: string): Material[] { return matMap[woNo] ?? [] }
function getLog(w: WO) { return logMap[w.woNo] ?? [{ time: w.start + ' 08:00', text: 'WO scheduled — awaiting production start', dot: 'dot-blue' }] }
function openPanel(w: WO) { panel.value = w }

function woCls(s: string) {
  return { 'b-blue': s==='Running', 'b-amber': s==='Pending', 'b-green': s==='Completed', 'b-red': s==='On Hold' }
}

async function exportToExcel() {
  const wb = new ExcelJS.Workbook()
  wb.creator = 'HIAS'
  const ws = wb.addWorksheet('Work Orders')

  ws.columns = [
    { header: 'WO Number',    key: 'woNo',      width: 16 },
    { header: 'SO Reference', key: 'soRef',     width: 16 },
    { header: 'Product',      key: 'product',   width: 26 },
    { header: 'Line',         key: 'line',      width: 16 },
    { header: 'Qty Planned',  key: 'qtyPlan',   width: 12 },
    { header: 'Qty Actual',   key: 'qtyActual', width: 12 },
    { header: 'Start Date',   key: 'start',     width: 12 },
    { header: 'End Date',     key: 'end',       width: 12 },
    { header: 'Status',       key: 'status',    width: 12 },
  ]

  const headerRow = ws.getRow(1)
  headerRow.height = 22
  headerRow.eachCell(cell => {
    cell.fill = { type: 'pattern', pattern: 'solid', fgColor: { argb: 'FF1565C0' } }
    cell.font = { bold: true, color: { argb: 'FFFFFFFF' }, size: 11, name: 'Calibri' }
    cell.alignment = { vertical: 'middle', horizontal: 'left' }
    cell.border = { bottom: { style: 'medium', color: { argb: 'FF0D47A1' } } }
  })

  filtered.value.forEach((w, i) => {
    const row = ws.addRow(w)
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
  a.download = `Work_Orders_${new Date().toISOString().slice(0, 10)}.xlsx`
  a.click()
  URL.revokeObjectURL(url)
}
</script>

<style scoped>
*, *::before, *::after { box-sizing: border-box; }
.root { height: 100%; display: flex; flex-direction: column; font-family: 'Poppins', sans-serif; font-size: 12px; background: #f5f5f5; overflow: hidden; }

.topbar { background: #fff; border-bottom: 1px solid #c3c6d4; padding: 8px 14px; display: flex; align-items: center; gap: 10px; flex-shrink: 0; flex-wrap: wrap; box-shadow: 0 1px 3px rgba(0,0,0,.06); }
.pg-title { font-size: 13px; font-weight: 700; color: #515151; letter-spacing: 0.5px; text-transform: uppercase; flex-shrink: 0; }
.filters  { display: flex; align-items: center; gap: 6px; flex-wrap: wrap; margin-left: auto; }

.sel { border: 1px solid #d0d3e0; border-radius: 3px; font-family: 'Poppins', sans-serif; font-size: 11px; padding: 4px 8px; color: #515151; background: #fff; }
.sf-btn { background: #f5f5f5; border: 1px solid #d0d3e0; border-radius: 3px; color: #515151; cursor: pointer; font-family: 'Poppins', sans-serif; font-size: 10px; padding: 4px 10px; }
.sf-btn:hover { background: #e3f2fd; border-color: #1565c0; color: #1565c0; }
.sf-btn--active { background: #1565c0; border-color: #1565c0; color: #fff; }

.search-box { align-items: center; background: #fff; border: 1px solid #d0d3e0; border-radius: 3px; display: flex; gap: 6px; padding: 4px 8px; }
.search-box i { color: #9e9e9e; font-size: 11px; }
.search-box input { border: none; font-family: 'Poppins', sans-serif; font-size: 11px; outline: none; width: 180px; color: #333; }
.btn-primary { background: linear-gradient(135deg, #1976d2, #0d47a1); border: none; border-radius: 3px; color: #fff; cursor: pointer; font-family: 'Poppins', sans-serif; font-size: 11px; gap: 5px; display: flex; align-items: center; padding: 5px 12px; transition: transform .12s ease, box-shadow .12s ease; }
.btn-primary:hover { transform: translateY(-1px); box-shadow: 0 4px 10px rgba(13,71,161,.4); }

.kpi-strip { background: #c3c6d4; display: flex; gap: 1px; flex-shrink: 0; border-bottom: 2px solid #c3c6d4; }
.kc { background: #fff; flex: 1; padding: 9px 16px; display: flex; flex-direction: column; gap: 3px; position: relative; overflow: hidden; border-left: 3px solid #1565c0; transition: transform .15s ease, box-shadow .15s ease; }
.kc:hover { transform: translateY(-2px); box-shadow: 0 6px 14px rgba(0,0,0,.12); z-index: 2; }
.kc::before { content: '\f201'; font-family: 'Font Awesome 6 Free'; font-weight: 900; position: absolute; right: 6px; top: 2px; font-size: 36px; color: #1565c0; opacity: .08; pointer-events: none; }
.kc::after { content: ''; position: absolute; left: 0; right: 0; bottom: 0; height: 3px; background: linear-gradient(90deg, transparent, #1565c0, transparent); background-size: 200% 100%; animation: kpi-shimmer 2.5s linear infinite; opacity: .35; }
@keyframes kpi-shimmer { 0% { background-position: 200% 0; } 100% { background-position: -200% 0; } }
.kc-lbl { font-size: 10px; color: #7f7f7f; text-transform: uppercase; letter-spacing: 0.5px; position: relative; z-index: 1; }
.kc-val { font-size: 22px; font-weight: 800; line-height: 1.1; position: relative; z-index: 1; }
.c-blue   { color: #1565c0; } .c-amber { color: #f9a825; }
.c-green  { color: #388E3C; } .c-red   { color: #e53935; }

.tbl-wrap { flex: 1; overflow: auto; }
.tbl { border-collapse: collapse; width: 100%; }
.tbl thead th { background: #f5f5f5; border-bottom: 1px solid #c3c6d4; color: #515151; font-size: 10px; font-weight: 600; padding: 8px 10px; position: sticky; text-align: left; text-transform: uppercase; top: 0; white-space: nowrap; }
.tbl tbody td { border-bottom: 1px solid #f0f0f0; color: #333; font-size: 11px; padding: 8px 10px; }
.tbl-row { cursor: pointer; transition: background .1s; }
.tbl-row:hover { background: #e3f2fd; }
.tc { text-align: center; } .tr { text-align: right; }
.mono { font-family: monospace; font-size: 11px; }
.link { color: #1565c0; }
.empty { color: #9e9e9e; padding: 24px; text-align: center; }

.line-badge { background: #f5f5f5; border: 1px solid #d0d3e0; border-radius: 3px; font-size: 10px; padding: 2px 6px; white-space: nowrap; }

.prog-wrap { background: #e0e0e0; border-radius: 3px; height: 6px; overflow: hidden; width: 100%; }
.prog-bar  { border-radius: 3px; height: 100%; transition: width .3s; }
.prog-lbl  { color: #757575; font-size: 10px; margin-top: 1px; text-align: right; }

.badge { border-radius: 3px; font-size: 10px; font-weight: 600; padding: 2px 8px; white-space: nowrap; }
.b-green  { background: #e8f5e9; color: #388E3C; }
.b-blue   { background: #e3f2fd; color: #1565c0; }
.b-amber  { background: #fff8e1; color: #f9a825; }
.b-red    { background: #ffebee; color: #e53935; }

.act-btn { background: transparent; border: 1px solid #d0d3e0; border-radius: 3px; color: #757575; cursor: pointer; font-size: 11px; margin: 0 2px; padding: 3px 7px; }
.act-btn:hover { background: #e3f2fd; border-color: #1565c0; color: #1565c0; }

.overlay { background: rgba(0,0,0,.3); inset: 0; position: fixed; z-index: 200; }
.detail-panel { background: #fff; box-shadow: -4px 0 20px rgba(0,0,0,.12); display: flex; flex-direction: column; height: 100vh; position: fixed; right: 0; top: 0; width: 580px; z-index: 201; }
.panel-slide-enter-active, .panel-slide-leave-active { transition: transform .25s ease; }
.panel-slide-enter-from, .panel-slide-leave-to { transform: translateX(100%); }

.dp-head { align-items: flex-start; background: linear-gradient(0deg,#d7d7d7,#fff); border-bottom: 1px solid #c3c6d4; display: flex; gap: 10px; padding: 14px 16px; flex-shrink: 0; }
.dp-head-left { flex: 1; }
.dp-wo   { font-size: 14px; font-weight: 700; color: #1565c0; }
.dp-prod { font-size: 11px; color: #757575; margin-top: 2px; }
.dp-close { background: transparent; border: none; color: #757575; cursor: pointer; font-size: 16px; padding: 2px 6px; }
.dp-close:hover { color: #e53935; }

.dp-body { flex: 1; overflow-y: auto; overflow-x: hidden; padding: 14px 16px; }
.dp-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 10px; margin-bottom: 14px; }
.dp-field { display: flex; flex-direction: column; gap: 2px; }
.dp-field.full { grid-column: 1/-1; }
.dfl { color: #9e9e9e; font-size: 10px; text-transform: uppercase; }

.sec-title { color: #515151; font-size: 11px; font-weight: 700; margin-bottom: 6px; text-transform: uppercase; }
.dp-tbl { border-collapse: collapse; width: 100%; }
.dp-tbl th { background: #f5f5f5; border-bottom: 1px solid #c3c6d4; color: #515151; font-size: 10px; padding: 6px 8px; text-align: left; text-transform: uppercase; }
.dp-tbl td { border-bottom: 1px solid #f0f0f0; font-size: 11px; padding: 6px 8px; }

.prod-log { display: flex; flex-direction: column; gap: 6px; }
.log-row  { align-items: flex-start; display: flex; gap: 8px; }
.log-time { color: #9e9e9e; font-size: 10px; white-space: nowrap; width: 100px; flex-shrink: 0; }
.log-dot  { border-radius: 50%; flex-shrink: 0; height: 8px; margin-top: 3px; width: 8px; }
.dot-blue  { background: #1565c0; } .dot-green { background: #388E3C; }
.dot-red   { background: #e53935; } .dot-amber { background: #f9a825; }
.log-txt   { color: #515151; font-size: 11px; }

.dp-foot { border-top: 1px solid #c3c6d4; display: flex; gap: 8px; justify-content: flex-end; padding: 12px 16px; flex-shrink: 0; }
.btn-outline { background: #fff; border: 1px solid #d0d3e0; border-radius: 3px; color: #515151; cursor: pointer; font-family: 'Poppins', sans-serif; font-size: 11px; padding: 6px 16px; }
.btn-outline:hover { background: #f5f5f5; }
</style>
