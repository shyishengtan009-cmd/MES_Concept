<template>
  <div class="page-root">

    <!-- TOPBAR -->
    <div class="topbar">
      <span class="pg-title">&#9632; Incoming Records</span>
      <div class="topbar-right">
        <span class="live-dot"></span>
        <span>{{ clock }}</span>
      </div>
    </div>

    <!-- SEARCH / FILTER BAR -->
    <div class="filter-bar">
      <span class="fl">Supplier</span>
      <input class="f-search" type="text" placeholder="Enter supplier name" v-model="supplierSearch" @keyup.enter="applySearch" style="margin-left:0; width:200px" />
      <button class="f-btn-primary" @click="applySearch"><i class="fa-solid fa-magnifying-glass"></i> Search</button>
      <button class="f-btn-outline" @click="resetSearch"><i class="fa-solid fa-rotate-right"></i> Reset</button>
      <span class="result-count" style="margin-left:auto">{{ filteredRows.length }} records</span>
    </div>

    <!-- ACTION BAR -->
    <div class="action-bar">
      <button class="act-btn act-new">   <i class="fa-solid fa-plus"></i> New</button>
      <button class="act-btn act-edit">  <i class="fa-regular fa-pen-to-square"></i> Edit</button>
      <button class="act-btn act-delete"><i class="fa-regular fa-trash-can"></i> Delete</button>
      <button class="act-btn act-export"><i class="fa-solid fa-arrow-up-from-bracket"></i> Export</button>
      <button class="act-icon-btn" style="margin-left:auto" title="Column settings"><i class="fa-solid fa-magnifying-glass"></i></button>
    </div>

    <!-- TABLE PANEL -->
    <div class="body-full">
      <div class="pc" style="flex:1">
        <div class="ph">
          Stock In Records
          <span class="ph-r">{{ filteredRows.length }} records</span>
        </div>
        <div class="tbl-wrap">
          <table>
            <thead>
              <tr>
                <th class="col-chk"><input type="checkbox" v-model="allChecked" @change="toggleAll" /></th>
                <th class="col-id">ID</th>
                <th style="min-width:105px">Stock In Type</th>
                <th style="min-width:105px">Document</th>
                <th style="min-width:135px">Order No.</th>
                <th style="min-width:65px">Method</th>
                <th style="min-width:145px">SKU</th>
                <th style="min-width:78px">Pack Unit</th>
                <th style="min-width:145px">Supplier</th>
                <th style="min-width:82px">Warehouse</th>
                <th style="min-width:82px">Location</th>
                <th style="min-width:82px;text-align:right">Goods Total</th>
                <th style="min-width:100px">Created At</th>
                <th style="min-width:88px">Created By</th>
                <th class="col-actions">Actions</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="row in filteredRows" :key="row.id" :class="{ 'row-sel': checkedIds.has(row.id) }">
                <td class="col-chk"><input type="checkbox" :checked="checkedIds.has(row.id)" @change="toggleRow(row.id)" /></td>
                <td class="col-id">{{ row.id }}</td>
                <td>{{ row.stockInType }}</td>
                <td>{{ row.document }}</td>
                <td :class="{ dash: !row.orderNo }">{{ row.orderNo || '—' }}</td>
                <td>{{ row.method }}</td>
                <td :class="{ dash: !row.skuName }">{{ row.skuName || '—' }}</td>
                <td :class="{ dash: !row.packageUnit }">{{ row.packageUnit || '—' }}</td>
                <td :class="{ dash: !row.supplierName }">{{ row.supplierName || '—' }}</td>
                <td>{{ row.warehouseName }}</td>
                <td>{{ row.warehousePositionName }}</td>
                <td style="text-align:right">{{ row.goodsTotal }}</td>
                <td>{{ row.createdAt }}</td>
                <td :class="{ dash: !row.createdBy }">{{ row.createdBy || '—' }}</td>
                <td class="col-actions">
                  <div><a class="row-edit" @click.prevent><i class="fa-regular fa-pen-to-square"></i> Edit</a></div>
                  <div><a class="row-del"  @click.prevent><i class="fa-regular fa-trash-can"></i> Delete</a></div>
                </td>
              </tr>
              <tr v-if="filteredRows.length === 0">
                <td colspan="15" class="empty-msg">No records found.</td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>

    <!-- PAGINATION -->
    <div class="pag-bar">
      <span class="pag-info">Total {{ filteredRows.length }} records</span>
      <div class="pag-pages">
        <button class="pag-btn" disabled>&lt;</button>
        <button class="pag-btn pag-active">1</button>
        <button class="pag-btn">&gt;</button>
      </div>
    </div>

  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onBeforeUnmount } from 'vue'

const clock          = ref('')
const supplierSearch = ref('')
const activeFilter   = ref('')
const allChecked     = ref(false)
const checkedIds     = ref<Set<number>>(new Set())
let clockTimer: ReturnType<typeof setInterval> | null = null

function tick() {
  clock.value = new Date().toLocaleString('en-MY', {
    day: '2-digit', month: 'short', year: 'numeric',
    hour: '2-digit', minute: '2-digit', hour12: true,
  })
}

function applySearch() { activeFilter.value = supplierSearch.value.trim() }
function resetSearch()  { supplierSearch.value = ''; activeFilter.value = '' }

function toggleAll() {
  if (allChecked.value) filteredRows.value.forEach(r => checkedIds.value.add(r.id))
  else checkedIds.value.clear()
}
function toggleRow(id: number) {
  if (checkedIds.value.has(id)) checkedIds.value.delete(id)
  else checkedIds.value.add(id)
}

interface StockInRow {
  id: number
  stockInType: string           // 1=Purchase In, 2=Transfer In, 3=Return In, 4=Surplus In
  document: string              // 1=With Order, 2=Without Order
  orderNo: string
  method: string                // 1=Scan, 2=RFID
  skuName: string
  packageUnit: string
  supplierName: string
  warehouseName: string
  warehousePositionName: string
  goodsTotal: number
  createdAt: string
  createdBy: string
}

const allRows: StockInRow[] = [
  { id: 9, stockInType: 'Purchase In',  document: 'With Order',    orderNo: 'PO-2026-0041',     method: 'Scan', skuName: 'Palm Olein Oil (5L)',     packageUnit: 'Carton', supplierName: 'Mewah International',  warehouseName: 'Zone A', warehousePositionName: 'A-01-02', goodsTotal: 100, createdAt: '2026-04-15', createdBy: 'Saliza'     },
  { id: 8, stockInType: 'Transfer In',  document: 'With Order',    orderNo: '',                 method: 'RFID', skuName: '',                       packageUnit: '',       supplierName: '',                     warehouseName: 'Zone B', warehousePositionName: 'B-02-01', goodsTotal: 0,   createdAt: '2026-04-14', createdBy: ''           },
  { id: 7, stockInType: 'Transfer In',  document: 'With Order',    orderNo: '',                 method: 'RFID', skuName: '',                       packageUnit: '',       supplierName: '',                     warehouseName: 'Zone A', warehousePositionName: 'A-03-01', goodsTotal: 0,   createdAt: '2026-04-14', createdBy: ''           },
  { id: 6, stockInType: 'Purchase In',  document: 'With Order',    orderNo: 'PO-2026-0018',     method: 'Scan', skuName: 'Curry Powder (1kg bag)',  packageUnit: 'Pallet', supplierName: 'Nona Spices Sdn Bhd',  warehouseName: 'Zone A', warehousePositionName: 'A-01-01', goodsTotal: 200, createdAt: '2026-01-09', createdBy: ''           },
  { id: 5, stockInType: 'Return In',    document: 'With Order',    orderNo: 'RTN-IN-2025-0012', method: 'Scan', skuName: 'FFS Film Roll 500mm',     packageUnit: 'Box',    supplierName: 'Kemas Packaging Sdn',  warehouseName: 'Zone C', warehousePositionName: 'C-01-01', goodsTotal: 3,   createdAt: '2025-12-20', createdBy: 'Operator A' },
  { id: 4, stockInType: 'Purchase In',  document: 'With Order',    orderNo: 'PO-2025-0380',     method: 'Scan', skuName: 'Corrugated Box 400×300', packageUnit: 'Carton', supplierName: 'Kemas Packaging Sdn',  warehouseName: 'Zone B', warehousePositionName: 'B-01-03', goodsTotal: 24,  createdAt: '2025-12-15', createdBy: 'Operator B' },
  { id: 3, stockInType: 'Transfer In',  document: 'Without Order', orderNo: '',                 method: 'RFID', skuName: 'Tomato Paste (Drum)',     packageUnit: 'Pallet', supplierName: '',                     warehouseName: 'Zone A', warehousePositionName: 'A-02-02', goodsTotal: 20,  createdAt: '2025-11-30', createdBy: 'Operator A' },
  { id: 2, stockInType: 'Surplus In',   document: 'Without Order', orderNo: '',                 method: 'Scan', skuName: 'White Sugar (50kg bag)',   packageUnit: 'Box',    supplierName: '',                     warehouseName: 'Zone C', warehousePositionName: 'C-02-01', goodsTotal: 5,   createdAt: '2025-11-15', createdBy: 'Admin'      },
  { id: 1, stockInType: 'Purchase In',  document: 'With Order',    orderNo: 'PO-2025-0100',     method: 'Scan', skuName: 'Sodium Benzoate E211',    packageUnit: 'Carton', supplierName: 'Chemtek Supplies',      warehouseName: 'Zone D', warehousePositionName: 'D-01-01', goodsTotal: 80,  createdAt: '2025-10-01', createdBy: 'Admin'      },
]

const filteredRows = computed(() => {
  const q = activeFilter.value.toLowerCase()
  if (!q) return [...allRows]
  return allRows.filter(r => r.supplierName.toLowerCase().includes(q))
})

onMounted(() => { tick(); clockTimer = setInterval(tick, 1000) })
onBeforeUnmount(() => { if (clockTimer) clearInterval(clockTimer) })
</script>

<style scoped>
*, *::before, *::after { box-sizing: border-box; margin: 0; padding: 0; }

.page-root { height: 100%; overflow: hidden; font-family: 'Poppins', sans-serif; font-size: 12px; background: #f5f5f5; color: #333; display: flex; flex-direction: column; }

/* ── Topbar ── */
.topbar { background: #fff; border-bottom: 1px solid #c3c6d4; padding: 0 16px; display: flex; align-items: center; flex-shrink: 0; box-shadow: 0 1px 3px rgba(0,0,0,.06); height: 42px; }
.pg-title { font-size: 13px; font-weight: 700; color: #515151; letter-spacing: 0.5px; text-transform: uppercase; }
.topbar-right { margin-left: auto; display: flex; align-items: center; font-size: 10px; color: #9e9e9e; gap: 6px; }
.live-dot { width: 7px; height: 7px; border-radius: 50%; background: #43a047; animation: pulse 1.4s infinite; display: inline-block; }
@keyframes pulse { 0%,100%{ opacity:1 } 50%{ opacity:0.3 } }

/* ── Filter / search bar ── */
.filter-bar { background: #fff; border-bottom: 1px solid #e8e8e8; padding: 6px 16px; display: flex; align-items: center; gap: 8px; flex-shrink: 0; flex-wrap: wrap; }
.fl { font-size: 10px; color: #9e9e9e; font-weight: 600; text-transform: uppercase; letter-spacing: 0.4px; }
.f-search { border: 1px solid #c3c6d4; border-radius: 3px; font-family: 'Poppins', sans-serif; font-size: 11px; padding: 4px 8px; color: #515151; outline: none; height: 28px; }
.f-search:focus { border-color: #1565c0; }
.f-btn-primary { background: #1565c0; border: none; border-radius: 3px; color: #fff; cursor: pointer; font-family: 'Poppins', sans-serif; font-size: 11px; font-weight: 600; height: 28px; padding: 0 12px; display: flex; align-items: center; gap: 5px; }
.f-btn-primary:hover { background: #1976d2; }
.f-btn-outline { background: #fff; border: 1px solid #c3c6d4; border-radius: 3px; color: #515151; cursor: pointer; font-family: 'Poppins', sans-serif; font-size: 11px; height: 28px; padding: 0 12px; display: flex; align-items: center; gap: 5px; }
.f-btn-outline:hover { background: #f5f5f5; }
.result-count { font-size: 10px; color: #9e9e9e; }

/* ── Action bar ── */
.action-bar { background: #fff; border-bottom: 1px solid #e8e8e8; padding: 6px 16px; display: flex; align-items: center; gap: 6px; flex-shrink: 0; }
.act-btn { background: #f5f5f5; border: 1px solid #c3c6d4; border-radius: 3px; cursor: pointer; font-family: 'Poppins', sans-serif; font-size: 11px; font-weight: 600; height: 28px; padding: 0 12px; display: flex; align-items: center; gap: 5px; transition: background .12s; color: #515151; }
.act-new    { background: #e8f5e9; border-color: #a5d6a7; color: #388E3C; }
.act-new:hover    { background: #c8e6c9; }
.act-edit   { background: #e3f2fd; border-color: #90caf9; color: #1565c0; }
.act-edit:hover   { background: #bbdefb; }
.act-delete { background: #ffebee; border-color: #ef9a9a; color: #e53935; }
.act-delete:hover { background: #ffcdd2; }
.act-export { background: #fff9c4; border-color: #fdd835; color: #f9a825; }
.act-export:hover { background: #fff59d; }
.act-icon-btn { background: #f5f5f5; border: 1px solid #c3c6d4; border-radius: 3px; color: #757575; cursor: pointer; height: 28px; width: 28px; display: flex; align-items: center; justify-content: center; font-size: 11px; }
.act-icon-btn:hover { background: #e3f2fd; color: #1565c0; border-color: #90caf9; }

/* ── Body / panel ── */
.body-full { display: flex; flex-direction: column; padding: 10px; flex: 1; overflow: hidden; min-height: 0; }
.pc { background: #fff; border: 1px solid #c3c6d4; border-radius: 6px; overflow: hidden; display: flex; flex-direction: column; }
.ph { padding: 8px 12px; font-size: 11px; font-weight: 700; color: #515151; text-transform: uppercase; letter-spacing: 0.4px; flex-shrink: 0; background: linear-gradient(0deg, #d7d7d7 0%, #fff 100%); border-bottom: 1px solid #c3c6d4; display: flex; align-items: center; justify-content: space-between; }
.ph-r { font-size: 10px; font-weight: 400; color: #9e9e9e; text-transform: none; letter-spacing: 0; }

/* ── Table ── */
.tbl-wrap { flex: 1; overflow: auto; }
.tbl-wrap::-webkit-scrollbar { width: 4px; height: 4px; }
.tbl-wrap::-webkit-scrollbar-thumb { background: #c3c6d4; border-radius: 2px; }
table { width: 100%; border-collapse: collapse; font-size: 11px; min-width: 1100px; }
th { background: linear-gradient(0deg, #d7d7d7 0%, #fff 100%); color: #515151; font-size: 10px; text-transform: uppercase; padding: 7px 10px; text-align: left; border-bottom: 2px solid #c3c6d4; position: sticky; top: 0; z-index: 1; letter-spacing: 0.3px; white-space: nowrap; font-weight: 700; }
td { padding: 7px 10px; border-bottom: 1px solid #e8e8e8; color: #515151; white-space: nowrap; font-size: 11px; }
tbody tr:nth-child(even) td { background: #f9f9f9; }
tbody tr:hover td { background: #e3f2fd !important; }
tbody tr.row-sel td { background: #e3f2fd; }

.col-chk     { width: 36px; text-align: center; }
.col-id      { width: 48px; text-align: center; font-weight: 700; color: #515151; }
.col-actions { width: 80px; text-align: center; }
.dash        { color: #c3c6d4; }

/* ── Per-row actions ── */
.row-edit, .row-del { cursor: pointer; font-size: 10px; font-weight: 600; display: inline-flex; align-items: center; gap: 3px; line-height: 1.8; }
.row-edit { color: #1565c0; }
.row-edit:hover { text-decoration: underline; }
.row-del  { color: #e53935; }
.row-del:hover  { text-decoration: underline; }

/* ── Pagination ── */
.pag-bar { background: #fff; border-top: 1px solid #c3c6d4; padding: 6px 16px; display: flex; align-items: center; justify-content: space-between; flex-shrink: 0; }
.pag-info { font-size: 10px; color: #9e9e9e; }
.pag-pages { display: flex; gap: 4px; }
.pag-btn { background: #f5f5f5; border: 1px solid #c3c6d4; border-radius: 3px; color: #515151; cursor: pointer; font-family: 'Poppins', sans-serif; font-size: 10px; font-weight: 600; height: 24px; min-width: 24px; padding: 0 6px; }
.pag-btn:hover:not(:disabled) { background: #e3f2fd; border-color: #90caf9; color: #1565c0; }
.pag-active { background: #1565c0; border-color: #1565c0; color: #fff; cursor: default; }
.pag-btn:disabled { opacity: 0.4; cursor: not-allowed; }

.empty-msg { padding: 32px; text-align: center; color: #9e9e9e; font-size: 11px; }
</style>
