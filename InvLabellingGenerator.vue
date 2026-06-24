<template>
  <div class="pg-root">
    <div class="topbar">
      <span class="pg-title">&#9632; Labelling Generator</span>
      <div class="topbar-right"><span class="live-dot"></span><span>{{ clock }}</span></div>
    </div>

    <div class="gen-row">
      <div class="pc gen-panel">
        <div class="ph">Generate Label</div>
        <div class="pc-body">
          <div class="form-row">
            <label>Product</label>
            <select v-model="product" @change="onProductChange">
              <option v-for="p in FINISHED_GOODS" :key="p.code" :value="p.name">{{ p.name }}</option>
            </select>
          </div>
          <div class="form-row">
            <label>Batch No.</label>
            <input type="text" v-model="batch" placeholder="e.g. B2026-0411" />
          </div>
          <div class="form-row">
            <label>Manufacturing Date</label>
            <input type="date" v-model="mfgDate" />
          </div>
          <div class="form-row">
            <label>Expiry Date</label>
            <input type="date" v-model="expDate" />
          </div>
          <div class="form-row">
            <label>Quantity to Print</label>
            <input type="number" v-model="qty" min="1" />
          </div>
          <button class="gen-btn" @click="generate">
            <i class="fa-solid fa-print"></i> Generate &amp; Print
          </button>
        </div>
      </div>

      <div class="pc preview-panel">
        <div class="ph">Label Preview</div>
        <div class="pc-body" style="display:flex; justify-content:center; padding:20px">
          <div class="label-preview">
            <div class="lp-brand">HIAS</div>
            <div class="lp-product">{{ product }}</div>
            <div class="lp-row"><span>Batch No.</span><span>{{ batch || '—' }}</span></div>
            <div class="lp-row"><span>Mfg Date</span><span>{{ mfgDate || '—' }}</span></div>
            <div class="lp-row"><span>Exp Date</span><span>{{ expDate || '—' }}</span></div>
            <div class="lp-barcode">{{ previewBarcode }}</div>
          </div>
        </div>
      </div>
    </div>

    <div class="body-full">
      <div class="pc" style="flex:1">
        <div class="ph">
          Recent Print Jobs <span class="ph-r">{{ history.length }} jobs</span>
          <button class="export-btn" @click="exportToExcel"><i class="fa-solid fa-arrow-up-from-bracket"></i> Export</button>
        </div>
        <div class="tbl-wrap">
          <table>
            <thead><tr><th>Time</th><th>Product</th><th>Batch No.</th><th style="text-align:right">Qty Printed</th><th>By</th></tr></thead>
            <tbody>
              <tr v-for="(h, i) in history" :key="i" class="hist-row" :class="{ active: i === selectedJob }" @click="loadJob(h, i)">
                <td>{{ h.time }}</td><td>{{ h.product }}</td><td>{{ h.batch }}</td>
                <td style="text-align:right">{{ h.qty }}</td><td>{{ h.by }}</td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onBeforeUnmount } from 'vue'
import { FINISHED_GOODS, findByName } from './src/inventoryCatalog'
import ExcelJS from 'exceljs'

const clock = ref('')
let clockTimer: ReturnType<typeof setInterval> | null = null
function tick() {
  clock.value = new Date().toLocaleString('en-MY', { day: '2-digit', month: 'short', year: 'numeric', hour: '2-digit', minute: '2-digit', hour12: true })
}

const product = ref(FINISHED_GOODS[0].name)
const batch    = ref(FINISHED_GOODS[0].latestBatch)
const mfgDate  = ref('2026-04-11')
const expDate  = ref('2027-04-11')
const qty      = ref(500)

function onProductChange() {
  batch.value = findByName(product.value)?.latestBatch ?? ''
}

// Deterministic "barcode" pattern seeded from product + batch — same item/batch
// always renders the same bars, different items/batches always look different.
const BAR_CHARS = ['▏', '▎', '▍', '▌', '▋', '█']
function seededBarcode(seed: string): string {
  let hash = 0
  for (let i = 0; i < seed.length; i++) {
    hash = (hash * 31 + seed.charCodeAt(i)) >>> 0
  }
  let out = ''
  for (let i = 0; i < 26; i++) {
    hash = (Math.imul(hash, 1103515245) + 12345) >>> 0
    out += BAR_CHARS[hash % BAR_CHARS.length]
    if (hash % 9 === 0) out += ' '
  }
  return out
}
const previewBarcode = computed(() => seededBarcode(`${product.value}|${batch.value}`))

interface PrintJob { time: string; product: string; batch: string; mfgDate: string; expDate: string; qty: number; by: string }

const history = ref<PrintJob[]>([
  { time: '10:38', product: 'Chili Sauce Pouch 250g', batch: 'B2026-0411', mfgDate: '2026-04-11', expDate: '2027-04-11', qty: 500, by: 'Amirul' },
  { time: '09:12', product: 'Curry Paste Tub 500g',    batch: 'B2026-0409', mfgDate: '2026-04-09', expDate: '2027-04-09', qty: 300, by: 'Farah'  },
  { time: 'Yest 16:40', product: 'Sambal Belacan Jar 200g', batch: 'B2025-0118', mfgDate: '2025-04-18', expDate: '2026-04-18', qty: 200, by: 'Saliza' },
])

const selectedJob = ref<number | null>(null)

function loadJob(h: PrintJob, i: number) {
  selectedJob.value = i
  product.value = h.product
  batch.value   = h.batch
  mfgDate.value = h.mfgDate
  expDate.value = h.expDate
  qty.value     = h.qty
}

function generate() {
  history.value.unshift({ time: 'Just now', product: product.value, batch: batch.value, mfgDate: mfgDate.value, expDate: expDate.value, qty: qty.value, by: 'You' })
  selectedJob.value = 0
}

async function exportToExcel() {
  const wb = new ExcelJS.Workbook()
  wb.creator = 'HIAS'
  const ws = wb.addWorksheet('Print Jobs')

  ws.columns = [
    { header: 'Time',         key: 'time',    width: 14 },
    { header: 'Product',      key: 'product', width: 26 },
    { header: 'Batch No.',     key: 'batch',   width: 14 },
    { header: 'Qty Printed',  key: 'qty',      width: 12 },
    { header: 'By',           key: 'by',       width: 12 },
  ]

  const headerRow = ws.getRow(1)
  headerRow.height = 22
  headerRow.eachCell(cell => {
    cell.fill = { type: 'pattern', pattern: 'solid', fgColor: { argb: 'FF1565C0' } }
    cell.font = { bold: true, color: { argb: 'FFFFFFFF' }, size: 11, name: 'Calibri' }
    cell.alignment = { vertical: 'middle', horizontal: 'left' }
    cell.border = { bottom: { style: 'medium', color: { argb: 'FF0D47A1' } } }
  })

  history.value.forEach((h, i) => {
    const row = ws.addRow(h)
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
  a.download = `Print_Jobs_${new Date().toISOString().slice(0, 10)}.xlsx`
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

.gen-row { display: flex; gap: 10px; padding: 10px 10px 0 10px; flex-shrink: 0; }
.gen-panel, .preview-panel { flex: 0 0 320px; }
.body-full { display: flex; flex-direction: column; padding: 10px; flex: 1; overflow: hidden; min-height: 0; }

.pc { background: #fff; border: 1px solid #c3c6d4; border-radius: 6px; overflow: hidden; display: flex; flex-direction: column; }
.ph { padding: 8px 12px; font-size: 11px; font-weight: 700; color: #515151; text-transform: uppercase; letter-spacing: 0.4px; flex-shrink: 0; background: linear-gradient(0deg, #d7d7d7 0%, #fff 100%); border-bottom: 1px solid #c3c6d4; display: flex; align-items: center; gap: 8px; }
.export-btn { margin-left: auto; background: #1565c0; color: #fff; border: none; border-radius: 4px; padding: 4px 12px; font-family: 'Poppins', sans-serif; font-size: 10px; font-weight: 700; text-transform: none; letter-spacing: 0; cursor: pointer; display: flex; align-items: center; gap: 6px; transition: background .12s; }
.export-btn:hover { background: #0d47a1; }
.ph-r { font-size: 10px; font-weight: 400; color: #9e9e9e; text-transform: none; letter-spacing: 0; }
.pc-body { padding: 14px; overflow-y: auto; overflow-x: hidden; flex: 1; }

.form-row { display: flex; flex-direction: column; gap: 4px; margin-bottom: 12px; }
.form-row label { font-size: 10px; color: #7f7f7f; font-weight: 600; text-transform: uppercase; letter-spacing: 0.4px; }
.form-row select, .form-row input { border: 1px solid #c3c6d4; border-radius: 4px; padding: 6px 8px; font-family: 'Poppins', sans-serif; font-size: 12px; color: #333; outline: none; }
.form-row select:focus, .form-row input:focus { border-color: #1565c0; }

.gen-btn { width: 100%; background: linear-gradient(135deg, #1976d2, #0d47a1); color: #fff; border: none; border-radius: 5px; padding: 10px; font-family: 'Poppins', sans-serif; font-size: 12px; font-weight: 600; cursor: pointer; display: flex; align-items: center; justify-content: center; gap: 8px; margin-top: 4px; transition: transform .12s ease, box-shadow .12s ease; }
.gen-btn:hover { transform: translateY(-1px); box-shadow: 0 4px 10px rgba(13,71,161,.4); }

.label-preview { width: 220px; border: 2px dashed #c3c6d4; border-radius: 6px; padding: 14px; display: flex; flex-direction: column; gap: 6px; background: #fafafa; }
.lp-brand { font-size: 16px; font-weight: 800; color: #1565c0; letter-spacing: 1px; }
.lp-product { font-size: 12px; font-weight: 600; color: #333; margin-bottom: 6px; }
.lp-row { display: flex; justify-content: space-between; font-size: 10px; color: #515151; }
.lp-row span:first-child { color: #9e9e9e; }
.lp-barcode { margin-top: 8px; font-size: 15px; letter-spacing: 0px; color: #333; text-align: center; white-space: nowrap; overflow: hidden; }

.tbl-wrap { flex: 1; overflow-y: auto; overflow-x: hidden; }
table { width: 100%; border-collapse: collapse; font-size: 11px; }
th { background: linear-gradient(0deg, #d7d7d7 0%, #fff 100%); color: #515151; font-size: 10px; text-transform: uppercase; padding: 7px 10px; text-align: left; border-bottom: 2px solid #c3c6d4; position: sticky; top: 0; z-index: 1; letter-spacing: 0.3px; white-space: nowrap; }
td { padding: 7px 10px; border-bottom: 1px solid #e8e8e8; color: #515151; white-space: nowrap; }
tbody tr:nth-child(even) td { background: #f9f9f9; }
.hist-row { cursor: pointer; }
.hist-row:hover td { background: #e3f2fd !important; }
.hist-row.active td { background: #e3f2fd !important; color: #1565c0; font-weight: 600; }
</style>
