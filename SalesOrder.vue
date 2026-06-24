<template>
  <div class="root">

    <!-- Topbar -->
    <div class="topbar">
      <span class="pg-title">&#9632; Sales Order</span>
      <div class="filters">
        <button
          v-for="s in statuses" :key="s"
          class="sf-btn" :class="{ 'sf-btn--active': statusFilter === s }"
          @click="statusFilter = s"
        >{{ s }}</button>
        <div class="search-box">
          <i class="fa-solid fa-magnifying-glass"></i>
          <input v-model="search" placeholder="Search SO No / Customer…" />
        </div>
        <button class="btn-primary"><i class="fa-solid fa-plus"></i> New SO</button>
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
            <th>SO Number</th>
            <th>Customer</th>
            <th>Date</th>
            <th class="tc">Lines</th>
            <th class="tr">Total Qty</th>
            <th class="tr">Value (RM)</th>
            <th>Status</th>
            <th class="tc">Action</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="o in filtered" :key="o.soNo" class="tbl-row" @click="openPanel(o)">
            <td class="mono">{{ o.soNo }}</td>
            <td>{{ o.customer }}</td>
            <td>{{ o.date }}</td>
            <td class="tc">{{ o.lines }}</td>
            <td class="tr">{{ o.qty.toLocaleString() }}</td>
            <td class="tr">{{ o.value.toLocaleString() }}</td>
            <td><span class="badge" :class="sCls(o.status)">{{ o.status }}</span></td>
            <td class="tc" @click.stop>
              <button class="act-btn" title="View"><i class="fa-solid fa-eye"></i></button>
              <button class="act-btn" title="Print"><i class="fa-solid fa-print"></i></button>
            </td>
          </tr>
          <tr v-if="!filtered.length">
            <td colspan="8" class="empty">No orders match the current filter.</td>
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
            <div class="dp-so">{{ panel.soNo }}</div>
            <div class="dp-cust">{{ panel.customer }}</div>
          </div>
          <span class="badge" :class="sCls(panel.status)">{{ panel.status }}</span>
          <button class="dp-close" @click="panel = null"><i class="fa-solid fa-xmark"></i></button>
        </div>

        <div class="dp-body">
          <div class="dp-grid">
            <div class="dp-field"><span class="dfl">Order Date</span><span>{{ panel.date }}</span></div>
            <div class="dp-field"><span class="dfl">Delivery Date</span><span>{{ delivDate(panel.date) }}</span></div>
            <div class="dp-field"><span class="dfl">Total Lines</span><span>{{ panel.lines }}</span></div>
            <div class="dp-field"><span class="dfl">Total Qty</span><span>{{ panel.qty.toLocaleString() }} pcs</span></div>
            <div class="dp-field full"><span class="dfl">Order Value</span><span class="big-val">RM {{ panel.value.toLocaleString() }}</span></div>
          </div>

          <div class="sec-title">Order Lines</div>
          <table class="dp-tbl">
            <thead><tr><th>#</th><th>SKU</th><th>Product</th><th class="tr">Qty</th><th>UOM</th><th class="tr">Unit Price</th><th class="tr">Subtotal</th></tr></thead>
            <tbody>
              <tr v-for="(l, i) in getLines(panel.soNo)" :key="i">
                <td>{{ i + 1 }}</td>
                <td class="mono">{{ l.sku }}</td>
                <td>{{ l.product }}</td>
                <td class="tr">{{ l.qty.toLocaleString() }}</td>
                <td>{{ l.uom }}</td>
                <td class="tr">{{ l.price.toFixed(2) }}</td>
                <td class="tr">{{ (l.qty * l.price).toLocaleString('en-MY', { minimumFractionDigits: 2 }) }}</td>
              </tr>
            </tbody>
          </table>

          <div class="sec-title" style="margin-top:14px">Status History</div>
          <div class="hist-log">
            <div v-for="ev in getHistory(panel)" :key="ev.label" class="hist-row">
              <span class="hist-dot" :class="ev.dot"></span>
              <span class="hist-date">{{ ev.date }}</span>
              <span class="hist-txt">{{ ev.label }}</span>
            </div>
          </div>
        </div>

        <div class="dp-foot">
          <button class="btn-outline" @click="panel = null">Close</button>
          <button class="btn-primary"><i class="fa-solid fa-file-pdf"></i> Export PDF</button>
        </div>
      </div>
    </transition>

  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import ExcelJS from 'exceljs'

interface Order { soNo: string; customer: string; date: string; lines: number; qty: number; value: number; status: string }
interface Line  { sku: string; product: string; qty: number; uom: string; price: number }

const statuses = ['All', 'Pending', 'Confirmed', 'In Production', 'Shipped', 'Cancelled'] as const
type Status = typeof statuses[number]

const statusFilter = ref<Status>('All')
const search = ref('')
const panel  = ref<Order | null>(null)

const orders = ref<Order[]>([
  { soNo: 'SO-2026-0401',  customer: 'Al-Barakah Trading Sdn Bhd',  date: '01/04/2026', lines: 3, qty: 4800,  value: 28400, status: 'Shipped'       },
  { soNo: 'SO-2026-0407',  customer: 'Halima Food Distributor',      date: '07/04/2026', lines: 2, qty: 2400,  value: 14200, status: 'Shipped'       },
  { soNo: 'SO-2026-0410',  customer: 'Sunrise Halal Mart',           date: '10/04/2026', lines: 4, qty: 5600,  value: 33200, status: 'Shipped'       },
  { soNo: 'SO-2026-0412',  customer: 'Saad Global Sdn Bhd',          date: '12/04/2026', lines: 4, qty: 6200,  value: 38600, status: 'In Production' },
  { soNo: 'SO-2026-0415',  customer: 'Berkah Niaga Enterprise',      date: '15/04/2026', lines: 2, qty: 1800,  value: 9800,  status: 'In Production' },
  { soNo: 'SO-2026-0418',  customer: 'Masood Foods International',   date: '18/04/2026', lines: 5, qty: 8400,  value: 52000, status: 'Confirmed'     },
  { soNo: 'SO-2026-0419',  customer: 'Rahmat Jaya Foodservice',      date: '19/04/2026', lines: 1, qty: 600,   value: 3600,  status: 'Confirmed'     },
  { soNo: 'SO-2026-0420',  customer: 'Nur Amalina Foods Sdn Bhd',    date: '20/04/2026', lines: 3, qty: 3600,  value: 21200, status: 'Confirmed'     },
  { soNo: 'SO-2026-0422',  customer: 'Badr Trading Co.',             date: '22/04/2026', lines: 2, qty: 2400,  value: 14400, status: 'Pending'       },
  { soNo: 'SO-2026-0423',  customer: 'Green Palm Resources',         date: '23/04/2026', lines: 3, qty: 3000,  value: 18600, status: 'Pending'       },
  { soNo: 'SO-2026-0424',  customer: 'Salam Food Industries',        date: '24/04/2026', lines: 1, qty: 1200,  value: 6800,  status: 'Pending'       },
  { soNo: 'SO-2026-0415B', customer: 'Toppleware F&B Sdn Bhd',       date: '15/04/2026', lines: 2, qty: 960,   value: 4800,  status: 'Cancelled'     },
])

const lineMap: Record<string, Line[]> = {
  'SO-2026-0401':  [{ sku:'FG-001', product:'Halal Seasoning Blend A 500g',  qty:2400, uom:'Pcs', price:4.80  }, { sku:'FG-002', product:'Palm Olein Oil 5L',           qty:600,  uom:'Btl', price:12.40 }, { sku:'FG-005', product:'Mixed Spice Sachet 50g',     qty:1800, uom:'Pcs', price:1.60 }],
  'SO-2026-0407':  [{ sku:'FG-001', product:'Halal Seasoning Blend A 500g',  qty:2400, uom:'Pcs', price:4.80  }, { sku:'FG-005', product:'Mixed Spice Sachet 50g',      qty:4800, uom:'Pcs', price:1.60  }],
  'SO-2026-0410':  [{ sku:'FG-002', product:'Palm Olein Oil 5L',             qty:600,  uom:'Btl', price:12.40 }, { sku:'FG-001', product:'Halal Seasoning Blend A 500g',qty:2400, uom:'Pcs', price:4.80  }, { sku:'FG-003', product:'Halal Marinade Sauce 250ml', qty:1800, uom:'Btl', price:7.20 }, { sku:'FG-005', product:'Mixed Spice Sachet 50g',qty:2400,uom:'Pcs',price:1.60 }],
  'SO-2026-0412':  [{ sku:'FG-001', product:'Halal Seasoning Blend A 500g',  qty:2400, uom:'Pcs', price:4.80  }, { sku:'FG-003', product:'Halal Marinade Sauce 250ml',  qty:1800, uom:'Btl', price:7.20  }, { sku:'FG-004', product:'Halal Curry Paste 200g',    qty:1200, uom:'Pcs', price:5.60 }, { sku:'FG-006', product:'Spice Mix Pack 1kg',qty:800,uom:'Pcs',price:11.20 }],
  'SO-2026-0415':  [{ sku:'FG-003', product:'Halal Marinade Sauce 250ml',    qty:1200, uom:'Btl', price:7.20  }, { sku:'FG-004', product:'Halal Curry Paste 200g',      qty:600,  uom:'Pcs', price:5.60  }],
  'SO-2026-0418':  [{ sku:'FG-001', product:'Halal Seasoning Blend A 500g',  qty:3600, uom:'Pcs', price:4.80  }, { sku:'FG-002', product:'Palm Olein Oil 5L',           qty:600,  uom:'Btl', price:12.40 }, { sku:'FG-003', product:'Halal Marinade Sauce 250ml',qty:2400,uom:'Btl',price:7.20 }, { sku:'FG-004', product:'Halal Curry Paste 200g',qty:1200,uom:'Pcs',price:5.60}, { sku:'FG-006', product:'Spice Mix Pack 1kg',qty:600,uom:'Pcs',price:11.20 }],
  'SO-2026-0419':  [{ sku:'FG-005', product:'Mixed Spice Sachet 50g',         qty:600,  uom:'Pcs', price:1.60  }],
  'SO-2026-0420':  [{ sku:'FG-004', product:'Halal Curry Paste 200g',         qty:2400, uom:'Pcs', price:5.60  }, { sku:'FG-006', product:'Spice Mix Pack 1kg',         qty:600,  uom:'Pcs', price:11.20 }, { sku:'FG-005', product:'Mixed Spice Sachet 50g',qty:600,uom:'Pcs',price:1.60 }],
  'SO-2026-0422':  [{ sku:'FG-001', product:'Halal Seasoning Blend A 500g',  qty:1200, uom:'Pcs', price:4.80  }, { sku:'FG-003', product:'Halal Marinade Sauce 250ml',  qty:1200, uom:'Btl', price:7.20  }],
  'SO-2026-0423':  [{ sku:'FG-002', product:'Palm Olein Oil 5L',             qty:600,  uom:'Btl', price:12.40 }, { sku:'FG-004', product:'Halal Curry Paste 200g',      qty:1200, uom:'Pcs', price:5.60  }, { sku:'FG-005', product:'Mixed Spice Sachet 50g',qty:1200,uom:'Pcs',price:1.60 }],
  'SO-2026-0424':  [{ sku:'FG-006', product:'Spice Mix Pack 1kg',             qty:1200, uom:'Pcs', price:11.20 }],
  'SO-2026-0415B': [{ sku:'FG-001', product:'Halal Seasoning Blend A 500g',  qty:600,  uom:'Pcs', price:4.80  }, { sku:'FG-003', product:'Halal Marinade Sauce 250ml',  qty:360,  uom:'Btl', price:7.20  }],
}

const kpis = computed(() => [
  { label: 'Total Orders',  val: orders.value.length,                                             cls: 'c-blue'   },
  { label: 'Pending',       val: orders.value.filter(o => o.status === 'Pending').length,         cls: 'c-amber'  },
  { label: 'Confirmed',     val: orders.value.filter(o => o.status === 'Confirmed').length,       cls: 'c-blue'   },
  { label: 'In Production', val: orders.value.filter(o => o.status === 'In Production').length,   cls: 'c-purple' },
  { label: 'Shipped',       val: orders.value.filter(o => o.status === 'Shipped').length,         cls: 'c-green'  },
])

const filtered = computed(() => {
  const q = search.value.toLowerCase()
  return orders.value.filter(o => {
    const sOk = statusFilter.value === 'All' || o.status === statusFilter.value
    const qOk = !q || o.soNo.toLowerCase().includes(q) || o.customer.toLowerCase().includes(q)
    return sOk && qOk
  })
})

function getLines(soNo: string): Line[] { return lineMap[soNo] ?? [] }

function getHistory(o: Order) {
  const evs = [{ date: o.date + ' 09:00', label: 'Order received and created', dot: 'dot-blue' }]
  if (['Confirmed','In Production','Shipped'].includes(o.status))
    evs.push({ date: o.date.replace(/^(\d+)/, d => String(+d+1).padStart(2,'0')) + ' 11:30', label: 'Order confirmed by Sales Manager', dot: 'dot-blue' })
  if (['In Production','Shipped'].includes(o.status))
    evs.push({ date: o.date.replace(/^(\d+)/, d => String(+d+2).padStart(2,'0')) + ' 08:00', label: 'Production Work Order raised', dot: 'dot-purple' })
  if (o.status === 'Shipped')
    evs.push({ date: o.date.replace(/^(\d+)/, d => String(+d+5).padStart(2,'0')) + ' 14:20', label: 'Order packed and dispatched to customer', dot: 'dot-green' })
  if (o.status === 'Cancelled')
    evs.push({ date: o.date + ' 16:45', label: 'Order cancelled by customer request', dot: 'dot-red' })
  return evs
}

function delivDate(date: string): string {
  const [d, m, y] = date.split('/').map(Number)
  const dt = new Date(y, m - 1, d + 7)
  return `${String(dt.getDate()).padStart(2,'0')}/${String(dt.getMonth()+1).padStart(2,'0')}/${dt.getFullYear()}`
}

function openPanel(o: Order) { panel.value = o }

function sCls(s: string) {
  return { 'b-amber': s==='Pending', 'b-blue': s==='Confirmed', 'b-purple': s==='In Production', 'b-green': s==='Shipped', 'b-red': s==='Cancelled' }
}

async function exportToExcel() {
  const wb = new ExcelJS.Workbook()
  wb.creator = 'HIAS'
  const ws = wb.addWorksheet('Sales Orders')

  ws.columns = [
    { header: 'SO Number', key: 'soNo',     width: 16 },
    { header: 'Customer',  key: 'customer', width: 28 },
    { header: 'Date',      key: 'date',     width: 12 },
    { header: 'Lines',     key: 'lines',    width: 8  },
    { header: 'Total Qty', key: 'qty',      width: 12 },
    { header: 'Value (RM)', key: 'value',   width: 14 },
    { header: 'Status',    key: 'status',   width: 14 },
  ]

  const headerRow = ws.getRow(1)
  headerRow.height = 22
  headerRow.eachCell(cell => {
    cell.fill = { type: 'pattern', pattern: 'solid', fgColor: { argb: 'FF1565C0' } }
    cell.font = { bold: true, color: { argb: 'FFFFFFFF' }, size: 11, name: 'Calibri' }
    cell.alignment = { vertical: 'middle', horizontal: 'left' }
    cell.border = { bottom: { style: 'medium', color: { argb: 'FF0D47A1' } } }
  })

  filtered.value.forEach((o, i) => {
    const row = ws.addRow(o)
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
  a.download = `Sales_Orders_${new Date().toISOString().slice(0, 10)}.xlsx`
  a.click()
  URL.revokeObjectURL(url)
}
</script>

<style scoped>
*, *::before, *::after { box-sizing: border-box; }
.root { height: 100%; display: flex; flex-direction: column; font-family: 'Poppins', sans-serif; font-size: 12px; background: #f5f5f5; overflow: hidden; }

/* Topbar */
.topbar { background: #fff; border-bottom: 1px solid #c3c6d4; padding: 8px 14px; display: flex; align-items: center; gap: 10px; flex-shrink: 0; flex-wrap: wrap; box-shadow: 0 1px 3px rgba(0,0,0,.06); }
.pg-title { font-size: 13px; font-weight: 700; color: #515151; letter-spacing: 0.5px; text-transform: uppercase; flex-shrink: 0; }
.filters  { display: flex; align-items: center; gap: 6px; flex-wrap: wrap; margin-left: auto; }

.sf-btn { background: #f5f5f5; border: 1px solid #d0d3e0; border-radius: 3px; color: #515151; cursor: pointer; font-family: 'Poppins', sans-serif; font-size: 10px; font-weight: 500; padding: 4px 10px; }
.sf-btn:hover { background: #e3f2fd; border-color: #1565c0; color: #1565c0; }
.sf-btn--active { background: #1565c0; border-color: #1565c0; color: #fff; }

.search-box { align-items: center; background: #fff; border: 1px solid #d0d3e0; border-radius: 3px; display: flex; gap: 6px; padding: 4px 8px; }
.search-box i { color: #9e9e9e; font-size: 11px; }
.search-box input { border: none; font-family: 'Poppins', sans-serif; font-size: 11px; outline: none; width: 200px; color: #333; }

.btn-primary { background: linear-gradient(135deg, #1976d2, #0d47a1); border: none; border-radius: 3px; color: #fff; cursor: pointer; font-family: 'Poppins', sans-serif; font-size: 11px; gap: 5px; display: flex; align-items: center; padding: 5px 12px; transition: transform .12s ease, box-shadow .12s ease; }
.btn-primary:hover { transform: translateY(-1px); box-shadow: 0 4px 10px rgba(13,71,161,.4); }

/* KPI */
.kpi-strip { background: #c3c6d4; display: flex; gap: 1px; flex-shrink: 0; border-bottom: 2px solid #c3c6d4; }
.kc { background: #fff; flex: 1; padding: 9px 16px; display: flex; flex-direction: column; gap: 3px; position: relative; overflow: hidden; border-left: 3px solid #1565c0; transition: transform .15s ease, box-shadow .15s ease; }
.kc:hover { transform: translateY(-2px); box-shadow: 0 6px 14px rgba(0,0,0,.12); z-index: 2; }
.kc::before { content: '\f201'; font-family: 'Font Awesome 6 Free'; font-weight: 900; position: absolute; right: 6px; top: 2px; font-size: 36px; color: #1565c0; opacity: .08; pointer-events: none; }
.kc::after { content: ''; position: absolute; left: 0; right: 0; bottom: 0; height: 3px; background: linear-gradient(90deg, transparent, #1565c0, transparent); background-size: 200% 100%; animation: kpi-shimmer 2.5s linear infinite; opacity: .35; }
@keyframes kpi-shimmer { 0% { background-position: 200% 0; } 100% { background-position: -200% 0; } }
.kc-lbl { font-size: 10px; color: #7f7f7f; text-transform: uppercase; letter-spacing: 0.5px; position: relative; z-index: 1; }
.kc-val { font-size: 22px; font-weight: 800; line-height: 1.1; position: relative; z-index: 1; }
.c-blue   { color: #1565c0; } .c-amber  { color: #f9a825; }
.c-green  { color: #388E3C; } .c-red    { color: #e53935; }
.c-purple { color: #7b1fa2; }

/* Table */
.tbl-wrap { flex: 1; overflow: auto; }
.tbl { border-collapse: collapse; width: 100%; }
.tbl thead th { background: #f5f5f5; border-bottom: 1px solid #c3c6d4; color: #515151; font-size: 10px; font-weight: 600; padding: 8px 10px; position: sticky; text-align: left; text-transform: uppercase; top: 0; white-space: nowrap; }
.tbl tbody td { border-bottom: 1px solid #f0f0f0; color: #333; font-size: 11px; padding: 8px 10px; }
.tbl-row { cursor: pointer; transition: background .1s; }
.tbl-row:hover { background: #e3f2fd; }
.tc { text-align: center; }
.tr { text-align: right; }
.mono { font-family: monospace; font-size: 11px; }
.empty { color: #9e9e9e; padding: 24px; text-align: center; }

/* Action buttons */
.act-btn { background: transparent; border: 1px solid #d0d3e0; border-radius: 3px; color: #757575; cursor: pointer; font-size: 11px; margin: 0 2px; padding: 3px 7px; }
.act-btn:hover { background: #e3f2fd; border-color: #1565c0; color: #1565c0; }

/* Badges */
.badge { border-radius: 3px; font-size: 10px; font-weight: 600; padding: 2px 8px; white-space: nowrap; }
.b-green  { background: #e8f5e9; color: #388E3C; }
.b-blue   { background: #e3f2fd; color: #1565c0; }
.b-amber  { background: #fff8e1; color: #f9a825; }
.b-red    { background: #ffebee; color: #e53935; }
.b-purple { background: #f3e5f5; color: #7b1fa2; }

/* Overlay + Panel */
.overlay { background: rgba(0,0,0,.3); inset: 0; position: fixed; z-index: 200; }
.detail-panel {
  background: #fff;
  box-shadow: -4px 0 20px rgba(0,0,0,.12);
  display: flex;
  flex-direction: column;
  height: 100vh;
  position: fixed;
  right: 0;
  top: 0;
  width: 580px;
  z-index: 201;
}
.panel-slide-enter-active, .panel-slide-leave-active { transition: transform .25s ease; }
.panel-slide-enter-from, .panel-slide-leave-to { transform: translateX(100%); }

.dp-head { align-items: flex-start; background: linear-gradient(0deg,#d7d7d7,#fff); border-bottom: 1px solid #c3c6d4; display: flex; gap: 10px; padding: 14px 16px; flex-shrink: 0; }
.dp-head-left { flex: 1; }
.dp-so   { font-size: 14px; font-weight: 700; color: #1565c0; }
.dp-cust { font-size: 11px; color: #757575; margin-top: 2px; }
.dp-close { background: transparent; border: none; color: #757575; cursor: pointer; font-size: 16px; margin-left: auto; padding: 2px 6px; }
.dp-close:hover { color: #e53935; }

.dp-body { flex: 1; overflow-y: auto; overflow-x: hidden; padding: 14px 16px; }
.dp-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 10px; margin-bottom: 14px; }
.dp-field { display: flex; flex-direction: column; gap: 2px; }
.dp-field.full { grid-column: 1 / -1; }
.dfl { color: #9e9e9e; font-size: 10px; text-transform: uppercase; }
.big-val { color: #1565c0; font-size: 16px; font-weight: 700; }

.sec-title { color: #515151; font-size: 11px; font-weight: 700; margin-bottom: 6px; text-transform: uppercase; }

.dp-tbl { border-collapse: collapse; width: 100%; }
.dp-tbl th { background: #f5f5f5; border-bottom: 1px solid #c3c6d4; color: #515151; font-size: 10px; padding: 6px 8px; text-align: left; text-transform: uppercase; }
.dp-tbl td { border-bottom: 1px solid #f0f0f0; font-size: 11px; padding: 6px 8px; }

.hist-log { display: flex; flex-direction: column; gap: 8px; }
.hist-row { align-items: flex-start; display: flex; gap: 8px; }
.hist-dot { border-radius: 50%; flex-shrink: 0; height: 8px; margin-top: 3px; width: 8px; }
.dot-blue   { background: #1565c0; } .dot-green  { background: #388E3C; }
.dot-purple { background: #7b1fa2; } .dot-red    { background: #e53935; }
.hist-date { color: #9e9e9e; font-size: 10px; white-space: nowrap; width: 120px; flex-shrink: 0; }
.hist-txt  { color: #515151; font-size: 11px; }

.dp-foot { border-top: 1px solid #c3c6d4; display: flex; gap: 8px; justify-content: flex-end; padding: 12px 16px; flex-shrink: 0; }
.btn-outline { background: #fff; border: 1px solid #d0d3e0; border-radius: 3px; color: #515151; cursor: pointer; font-family: 'Poppins', sans-serif; font-size: 11px; padding: 6px 16px; }
.btn-outline:hover { background: #f5f5f5; }
</style>
