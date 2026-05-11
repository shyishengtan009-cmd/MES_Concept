<template>
  <div class="inv-root">

    <!-- TOPBAR -->
    <div class="topbar">
      <span class="pg-title">&#9632; Inventory</span>
      <div class="pg-tabs">
        <button class="pg-tab" :class="{ active: activeTab === 'overview'  }" @click="activeTab = 'overview'">
          Overview
        </button>
        <button class="pg-tab" :class="{ active: activeTab === 'stock'     }" @click="activeTab = 'stock'">
          Stock Register
        </button>
        <button class="pg-tab" :class="{ active: activeTab === 'movements' }" @click="activeTab = 'movements'">
          Movements
        </button>
      </div>
      <div class="topbar-right">
        <span class="live-dot"></span>
        <span>{{ clock }}</span>
      </div>
    </div>

    <!-- ── TAB: OVERVIEW ── -->
    <div class="tab-pane" v-show="activeTab === 'overview'">
      <div class="kpi-row">
        <div class="kpi-card"><div class="kpi-label">Total SKUs</div><div class="kpi-val c-b">248</div><div class="kpi-sub">active items</div></div>
        <div class="kpi-card"><div class="kpi-label">Stock Value</div><div class="kpi-val c-d">RM 1.24M</div><div class="kpi-sub">current valuation</div></div>
        <div class="kpi-card"><div class="kpi-label">Low / Critical</div><div class="kpi-val c-r">14</div><div class="kpi-sub">below reorder point</div></div>
        <div class="kpi-card"><div class="kpi-label">Pending Receipts</div><div class="kpi-val c-y">7</div><div class="kpi-sub">awaiting GRN</div></div>
      </div>
      <div class="body-2col">
        <div class="col">
          <div class="pc" style="flex:1">
            <div class="ph">Stock by Category <span class="ph-r">by qty on hand</span></div>
            <div class="pc-body" style="padding:14px 16px">
              <div class="cat-row" v-for="cat in categories" :key="cat.name">
                <span class="cat-name">{{ cat.name }}</span>
                <div class="cat-bg"><div class="cat-fill" :style="{ width: cat.pct + '%', background: cat.color }"></div></div>
                <span class="cat-val">{{ cat.qty.toLocaleString() }}</span>
              </div>
            </div>
          </div>
        </div>
        <div class="col">
          <div class="pc" style="flex:1">
            <div class="ph">Recent Movements <span class="ph-r">last 24 hours</span></div>
            <div class="tbl-wrap">
              <table>
                <thead><tr><th>Time</th><th>Item</th><th>Movement</th><th>Qty</th><th>Ref</th></tr></thead>
                <tbody>
                  <tr v-for="(mv, i) in recentMovements" :key="i">
                    <td>{{ mv.time }}</td>
                    <td>{{ mv.item }}</td>
                    <td><span :class="mvClass(mv.movement)">{{ mvLabel(mv.movement) }}</span></td>
                    <td>{{ mv.qty }}</td>
                    <td>{{ mv.ref }}</td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- ── TAB: STOCK REGISTER ── -->
    <div class="tab-pane" v-show="activeTab === 'stock'">
      <div class="filter-bar">
        <span class="fl">Type:</span>
        <button class="f-btn" :class="{ on: stockType === 'all' }" @click="stockType = 'all'">All</button>
        <button class="f-btn" :class="{ on: stockType === 'rm'  }" @click="stockType = 'rm'">Raw Material</button>
        <button class="f-btn" :class="{ on: stockType === 'fg'  }" @click="stockType = 'fg'">Finished Goods</button>
        <button class="f-btn" :class="{ on: stockType === 'sp'  }" @click="stockType = 'sp'">Spare Parts</button>
        <span class="fl" style="margin-left:10px">Status:</span>
        <button class="f-btn" :class="{ on: stockStatus === 'all'     }" @click="stockStatus = 'all'">All</button>
        <button class="f-btn" :class="{ on: stockStatus === 'lowcrit' }" @click="stockStatus = 'lowcrit'">Low / Critical</button>
        <button class="f-btn" :class="{ on: stockStatus === 'normal'  }" @click="stockStatus = 'normal'">Normal</button>
        <input class="f-search" type="text" placeholder="Search…" v-model="stockSearch" />
        <span class="result-count">{{ filteredStock.length }} items shown</span>
      </div>
      <div class="body-full">
        <div class="pc" style="flex:1">
          <div class="ph">Stock Register <span class="ph-r">{{ filteredStock.length }} items shown</span></div>
          <div class="tbl-wrap">
            <table>
              <thead>
                <tr>
                  <th>Code</th><th>Item Name</th><th>Category</th><th>Type</th>
                  <th style="text-align:right">On Hand</th>
                  <th style="text-align:right">Reorder Pt</th>
                  <th>Status</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="row in filteredStock" :key="row.code">
                  <td>{{ row.code }}</td>
                  <td>{{ row.name }}</td>
                  <td>{{ row.category }}</td>
                  <td><span class="badge" :class="typeBadge(row.type)">{{ row.type.toUpperCase() }}</span></td>
                  <td style="text-align:right">{{ row.onHand }}</td>
                  <td style="text-align:right">{{ row.reorderPt }}</td>
                  <td><span class="badge" :class="statusBadge(row.status)">{{ row.status.charAt(0).toUpperCase() + row.status.slice(1) }}</span></td>
                </tr>
                <tr v-if="filteredStock.length === 0">
                  <td colspan="7" class="empty-msg">No items match the current filter.</td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </div>

    <!-- ── TAB: MOVEMENTS ── -->
    <div class="tab-pane" v-show="activeTab === 'movements'">
      <div class="filter-bar">
        <span class="fl">Movement:</span>
        <button class="f-btn" :class="{ on: mvType === 'all'        }" @click="mvType = 'all'">All</button>
        <button class="f-btn" :class="{ on: mvType === 'received'   }" @click="mvType = 'received'">Goods Received</button>
        <button class="f-btn" :class="{ on: mvType === 'production' }" @click="mvType = 'production'">Issued to Production</button>
        <button class="f-btn" :class="{ on: mvType === 'dispatch'   }" @click="mvType = 'dispatch'">Sales Dispatch</button>
        <button class="f-btn" :class="{ on: mvType === 'corrected'  }" @click="mvType = 'corrected'">Count Corrected</button>
        <span class="fl" style="margin-left:10px">Period:</span>
        <button class="f-btn" :class="{ on: mvPeriod === 'today' }" @click="mvPeriod = 'today'">Today</button>
        <button class="f-btn" :class="{ on: mvPeriod === 'week'  }" @click="mvPeriod = 'week'">This Week</button>
        <button class="f-btn" :class="{ on: mvPeriod === 'month' }" @click="mvPeriod = 'month'">This Month</button>
        <span class="result-count">{{ filteredMovements.length }} transactions</span>
      </div>
      <div class="body-full">
        <div class="pc" style="flex:1">
          <div class="ph">Movement Log <span class="ph-r">{{ filteredMovements.length }} transactions</span></div>
          <div class="tbl-wrap">
            <table>
              <thead>
                <tr><th>Date / Time</th><th>Ref No.</th><th>Item Name</th><th>Movement</th><th style="text-align:right">Qty</th><th>By</th></tr>
              </thead>
              <tbody>
                <tr v-for="(mv, i) in filteredMovements" :key="i">
                  <td>{{ mv.date }}</td>
                  <td>{{ mv.refNo }}</td>
                  <td>{{ mv.item }}</td>
                  <td><span :class="mvClass(mv.movement)">{{ mvLabel(mv.movement) }}</span></td>
                  <td style="text-align:right">{{ mv.qty }}</td>
                  <td>{{ mv.by }}</td>
                </tr>
                <tr v-if="filteredMovements.length === 0">
                  <td colspan="6" class="empty-msg">No movements match the current filter.</td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </div>

  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onBeforeUnmount } from 'vue'

type Tab = 'overview' | 'stock' | 'movements'

const activeTab = ref<Tab>('overview')
const clock     = ref('')
let clockTimer: ReturnType<typeof setInterval> | null = null

function tick() {
  clock.value = new Date().toLocaleString('en-MY', {
    day: '2-digit', month: 'short', year: 'numeric',
    hour: '2-digit', minute: '2-digit', hour12: true
  })
}

const categories = [
  { name: 'Packaging Material', qty: 41200, pct: 82, color: '#1565c0' },
  { name: 'Food Ingredient',    qty: 32800, pct: 65, color: '#388E3C' },
  { name: 'Finished Goods',     qty: 27600, pct: 55, color: '#7b1fa2' },
  { name: 'Food Additive',      qty: 14200, pct: 28, color: '#f9a825' },
  { name: 'Spare Parts',        qty: 9100,  pct: 18, color: '#9e9e9e' },
]

const recentMovements = [
  { time: '10:38',      item: 'Refined Palm Olein (5L)',        movement: 'received',   qty: '+100',    ref: 'GRN-00214' },
  { time: '09:55',      item: 'Chili Sauce Pouch 250g',         movement: 'dispatch',   qty: '−1,200',  ref: 'ISO-00389' },
  { time: '09:20',      item: 'FFS Film Roll 500mm',             movement: 'production', qty: '−80 kg',  ref: 'ISO-00388' },
  { time: '08:45',      item: 'Citric Acid E330',                movement: 'corrected',  qty: '−2 kg',   ref: 'ADJ-00047' },
  { time: '08:10',      item: 'Corrugated Box 400×300',          movement: 'received',   qty: '+500',    ref: 'GRN-00213' },
  { time: '07:50',      item: 'Tomato Paste Concentrate (Drum)', movement: 'received',   qty: '+20',     ref: 'GRN-00212' },
  { time: 'Yest 16:30', item: 'Curry Paste Tub 500g',            movement: 'dispatch',   qty: '−300',    ref: 'ISO-00387' },
  { time: 'Yest 15:10', item: 'Label Sticker 50×30mm',           movement: 'production', qty: '−800',    ref: 'ISO-00386' },
  { time: 'Yest 14:00', item: 'Seasoning Blend A',               movement: 'received',   qty: '+200 kg', ref: 'GRN-00211' },
]

const stockRows = [
  { code: 'RM-001', name: 'Refined Palm Olein (5L)',          category: 'Food Ingredient', type: 'rm', onHand: '12',    reorderPt: '50',    status: 'critical' },
  { code: 'RM-002', name: 'Tomato Paste Concentrate (Drum)',  category: 'Food Ingredient', type: 'rm', onHand: '220',   reorderPt: '50',    status: 'normal'   },
  { code: 'RM-003', name: 'Seasoning Blend A',                category: 'Food Ingredient', type: 'rm', onHand: '1,200', reorderPt: '300',   status: 'normal'   },
  { code: 'RM-004', name: 'Citric Acid E330',                 category: 'Food Additive',   type: 'rm', onHand: '28',    reorderPt: '40',    status: 'low'      },
  { code: 'RM-005', name: 'FFS Film Roll 500mm',              category: 'Packaging',       type: 'rm', onHand: '3,400', reorderPt: '500',   status: 'normal'   },
  { code: 'RM-006', name: 'LDPE Shrink Film 20µm',            category: 'Packaging',       type: 'rm', onHand: '80',    reorderPt: '200',   status: 'critical' },
  { code: 'RM-007', name: 'Corrugated Box 400×300',           category: 'Packaging',       type: 'rm', onHand: '140',   reorderPt: '200',   status: 'low'      },
  { code: 'RM-008', name: 'Xanthan Gum E415',                 category: 'Food Additive',   type: 'rm', onHand: '5.2',   reorderPt: '10',    status: 'low'      },
  { code: 'RM-009', name: 'Label Sticker 50×30mm',            category: 'Packaging',       type: 'rm', onHand: '1,200', reorderPt: '2,000', status: 'low'      },
  { code: 'RM-010', name: 'White Refined Sugar (50kg bag)',   category: 'Food Ingredient', type: 'rm', onHand: '85',    reorderPt: '30',    status: 'normal'   },
  { code: 'FG-001', name: 'Chili Sauce Pouch 250g',           category: 'Finished Goods',  type: 'fg', onHand: '8,400', reorderPt: '1,000', status: 'normal'   },
  { code: 'FG-002', name: 'Curry Paste Tub 500g',             category: 'Finished Goods',  type: 'fg', onHand: '5,200', reorderPt: '800',   status: 'normal'   },
  { code: 'FG-003', name: 'Sambal Belacan Jar 200g',          category: 'Finished Goods',  type: 'fg', onHand: '2,100', reorderPt: '500',   status: 'normal'   },
  { code: 'SP-001', name: 'Conveyor Belt Segment',            category: 'Spare Part',      type: 'sp', onHand: '4',     reorderPt: '2',     status: 'normal'   },
  { code: 'SP-002', name: 'Sealing Element 12mm',             category: 'Spare Part',      type: 'sp', onHand: '6',     reorderPt: '5',     status: 'normal'   },
]

const movementRows = [
  { date: '15/04 10:38', refNo: 'GRN-00214', item: 'Refined Palm Olein (5L)',          movement: 'received',   qty: '+100 unit',  by: 'Amirul',     period: 'today' },
  { date: '15/04 09:55', refNo: 'ISO-00389', item: 'Chili Sauce Pouch 250g',           movement: 'dispatch',   qty: '−1,200 pcs', by: 'Farah',      period: 'today' },
  { date: '15/04 09:20', refNo: 'ISO-00388', item: 'FFS Film Roll 500mm',               movement: 'production', qty: '−80 kg',     by: 'Amirul',     period: 'today' },
  { date: '15/04 08:45', refNo: 'ADJ-00047', item: 'Citric Acid E330',                  movement: 'corrected',  qty: '−2 kg',      by: 'Supervisor', period: 'today' },
  { date: '15/04 08:10', refNo: 'GRN-00213', item: 'Corrugated Box 400×300',            movement: 'received',   qty: '+500 unit',  by: 'Saliza',     period: 'today' },
  { date: '15/04 07:50', refNo: 'GRN-00212', item: 'Tomato Paste Concentrate (Drum)',   movement: 'received',   qty: '+20 unit',   by: 'Saliza',     period: 'today' },
  { date: '14/04 16:30', refNo: 'ISO-00387', item: 'Curry Paste Tub 500g',              movement: 'dispatch',   qty: '−300 pcs',   by: 'Farah',      period: 'week'  },
  { date: '14/04 15:10', refNo: 'ISO-00386', item: 'Label Sticker 50×30mm',              movement: 'production', qty: '−800 pcs',   by: 'Amirul',     period: 'week'  },
  { date: '14/04 14:00', refNo: 'GRN-00211', item: 'Seasoning Blend A',                  movement: 'received',   qty: '+200 kg',    by: 'Saliza',     period: 'week'  },
  { date: '14/04 11:20', refNo: 'ADJ-00046', item: 'Sambal Belacan Jar 200g',            movement: 'corrected',  qty: '+50 pcs',    by: 'Supervisor', period: 'week'  },
  { date: '14/04 10:05', refNo: 'GRN-00210', item: 'White Refined Sugar (50kg bag)',    movement: 'received',   qty: '+10 bag',    by: 'Saliza',     period: 'week'  },
  { date: '14/04 09:30', refNo: 'ISO-00385', item: 'Seasoning Blend A',                  movement: 'production', qty: '−150 kg',    by: 'Amirul',     period: 'week'  },
  { date: '10/04 09:00', refNo: 'GRN-00208', item: 'LDPE Shrink Film 20µm',              movement: 'received',   qty: '+50 roll',   by: 'Saliza',     period: 'month' },
  { date: '09/04 14:20', refNo: 'ISO-00382', item: 'Sambal Belacan Jar 200g',            movement: 'dispatch',   qty: '−600 pcs',   by: 'Farah',      period: 'month' },
  { date: '08/04 11:00', refNo: 'ISO-00380', item: 'Refined Palm Olein (5L)',             movement: 'production', qty: '−60 unit',   by: 'Amirul',     period: 'month' },
  { date: '07/04 10:30', refNo: 'ADJ-00044', item: 'FFS Film Roll 500mm',                movement: 'corrected',  qty: '+5 kg',      by: 'Supervisor', period: 'month' },
]

const stockType   = ref<string>('all')
const stockStatus = ref<string>('all')
const stockSearch = ref('')

const filteredStock = computed(() => {
  const q = stockSearch.value.toLowerCase()
  return stockRows.filter(r => {
    const typeOk   = stockType.value === 'all' || r.type === stockType.value
    const statusOk = stockStatus.value === 'all'
      || (stockStatus.value === 'lowcrit' && (r.status === 'low' || r.status === 'critical'))
      || r.status === stockStatus.value
    const searchOk = !q || (r.code + r.name + r.category).toLowerCase().includes(q)
    return typeOk && statusOk && searchOk
  })
})

const mvType   = ref<string>('all')
const mvPeriod = ref<string>('today')

const filteredMovements = computed(() => {
  return movementRows.filter(r => {
    const periodOk = mvPeriod.value === 'month'
      || (mvPeriod.value === 'week'  && (r.period === 'today' || r.period === 'week'))
      || (mvPeriod.value === 'today' && r.period === 'today')
    const typeOk = mvType.value === 'all' || r.movement === mvType.value
    return periodOk && typeOk
  })
})

function mvClass(movement: string): string {
  const map: Record<string, string> = {
    received:   'mv-received',
    production: 'mv-production',
    dispatch:   'mv-dispatch',
    corrected:  'mv-corrected',
  }
  return map[movement] ?? ''
}

function mvLabel(movement: string): string {
  const map: Record<string, string> = {
    received:   '▲ Goods Received',
    production: '→ Issued to Production',
    dispatch:   '▼ Sales Dispatch',
    corrected:  '● Count Corrected',
  }
  return map[movement] ?? movement
}

function typeBadge(type: string): string {
  if (type === 'rm') return 'b-b'
  if (type === 'fg') return 'b-g'
  return 'b-grey'
}

function statusBadge(status: string): string {
  if (status === 'critical') return 'b-r'
  if (status === 'low')      return 'b-y'
  return 'b-g'
}

onMounted(() => {
  tick()
  clockTimer = setInterval(tick, 1000)
})

onBeforeUnmount(() => {
  if (clockTimer) clearInterval(clockTimer)
})
</script>

<style scoped>
*, *::before, *::after { box-sizing: border-box; margin: 0; padding: 0; }

.inv-root { height: 100%; overflow: hidden; font-family: 'Poppins', sans-serif; font-size: 12px; background: #f5f5f5; color: #333; display: flex; flex-direction: column; }

.topbar { background: #fff; border-bottom: 1px solid #c3c6d4; padding: 0 16px; display: flex; align-items: stretch; flex-shrink: 0; box-shadow: 0 1px 3px rgba(0,0,0,.06); height: 42px; }
.pg-title { font-size: 13px; font-weight: 700; color: #515151; letter-spacing: 0.5px; text-transform: uppercase; white-space: nowrap; display: flex; align-items: center; }
.pg-tabs  { display: flex; margin-left: 16px; align-self: stretch; }
.pg-tab   { background: transparent; border: none; border-bottom: 3px solid transparent; color: #7f7f7f; cursor: pointer; font-family: 'Poppins', sans-serif; font-size: 11px; font-weight: 600; padding: 0 18px; transition: color .15s, border-color .15s; }
.pg-tab:hover  { color: #333; background: #f5f5f5; }
.pg-tab.active { color: #1565c0; border-bottom-color: #1565c0; }
.topbar-right { margin-left: auto; display: flex; align-items: center; font-size: 10px; color: #9e9e9e; gap: 6px; }
.live-dot { width: 7px; height: 7px; border-radius: 50%; background: #43a047; animation: pulse 1.4s infinite; display: inline-block; }
@keyframes pulse { 0%,100%{ opacity:1 } 50%{ opacity:0.3 } }

.tab-pane { display: flex; flex-direction: column; flex: 1; overflow: hidden; min-height: 0; }

.kpi-row  { display: flex; gap: 1px; background: #c3c6d4; border-bottom: 2px solid #c3c6d4; flex-shrink: 0; }
.kpi-card { flex: 1; background: #fff; padding: 9px 16px; display: flex; flex-direction: column; gap: 3px; }
.kpi-label{ font-size: 10px; color: #7f7f7f; text-transform: uppercase; letter-spacing: 0.5px; }
.kpi-val  { font-size: 22px; font-weight: 700; line-height: 1.1; }
.kpi-sub  { font-size: 10px; color: #9e9e9e; }
.c-g { color: #388E3C; } .c-b { color: #1565c0; } .c-y { color: #f9a825; } .c-r { color: #e53935; } .c-d { color: #515151; }

.filter-bar { background: #fff; border-bottom: 1px solid #e8e8e8; padding: 5px 16px; display: flex; align-items: center; gap: 6px; flex-shrink: 0; flex-wrap: wrap; }
.fl    { font-size: 10px; color: #9e9e9e; font-weight: 600; text-transform: uppercase; letter-spacing: 0.4px; }
.f-btn { background: #f5f5f5; border: 1px solid #c3c6d4; border-radius: 3px; color: #515151; cursor: pointer; font-family: 'Poppins', sans-serif; font-size: 10px; font-weight: 600; padding: 2px 10px; transition: background .12s; }
.f-btn:hover { background: #e3f2fd; border-color: #90caf9; color: #1565c0; }
.f-btn.on    { background: #1565c0; border-color: #1565c0; color: #fff; }
.f-search    { border: 1px solid #c3c6d4; border-radius: 3px; font-family: 'Poppins', sans-serif; font-size: 11px; padding: 3px 8px; color: #515151; outline: none; width: 180px; margin-left: auto; }
.f-search:focus { border-color: #1565c0; }
.result-count { font-size: 10px; color: #9e9e9e; margin-left: 6px; }

.body-2col { display: flex; gap: 10px; padding: 10px; flex: 1; overflow: hidden; min-height: 0; }
.body-full  { display: flex; flex-direction: column; padding: 10px; flex: 1; overflow: hidden; min-height: 0; }
.col { flex: 1; display: flex; flex-direction: column; gap: 10px; min-width: 0; overflow: hidden; }

.pc { background: #fff; border: 1px solid #c3c6d4; border-radius: 6px; overflow: hidden; display: flex; flex-direction: column; }
.ph { padding: 8px 12px; font-size: 11px; font-weight: 700; color: #515151; text-transform: uppercase; letter-spacing: 0.4px; flex-shrink: 0; background: linear-gradient(0deg, #d7d7d7 0%, #fff 100%); border-bottom: 1px solid #c3c6d4; display: flex; align-items: center; justify-content: space-between; }
.ph-r { font-size: 10px; font-weight: 400; color: #9e9e9e; text-transform: none; letter-spacing: 0; }
.pc-body { padding: 10px 12px; overflow-y: auto; flex: 1; }
.pc-body::-webkit-scrollbar { width: 4px; }
.pc-body::-webkit-scrollbar-thumb { background: #c3c6d4; border-radius: 2px; }

.tbl-wrap { flex: 1; overflow-y: auto; }
.tbl-wrap::-webkit-scrollbar { width: 4px; }
.tbl-wrap::-webkit-scrollbar-thumb { background: #c3c6d4; border-radius: 2px; }
table { width: 100%; border-collapse: collapse; font-size: 11px; }
th { background: linear-gradient(0deg, #d7d7d7 0%, #fff 100%); color: #515151; font-size: 10px; text-transform: uppercase; padding: 7px 10px; text-align: left; border-bottom: 2px solid #c3c6d4; position: sticky; top: 0; z-index: 1; letter-spacing: 0.3px; white-space: nowrap; }
td { padding: 7px 10px; border-bottom: 1px solid #e8e8e8; color: #515151; white-space: nowrap; }
tbody tr:nth-child(even) td { background: #f9f9f9; }
tbody tr:hover td { background: #f2f2f2 !important; }

.badge  { display: inline-block; padding: 1px 7px; border-radius: 3px; font-size: 9px; font-weight: 700; }
.b-g    { background: #e8f5e9; color: #388E3C; border: 1px solid #a5d6a7; }
.b-b    { background: #e3f2fd; color: #1565c0; border: 1px solid #90caf9; }
.b-y    { background: #fff9c4; color: #f9a825; border: 1px solid #fdd835; }
.b-r    { background: #ffebee; color: #e53935; border: 1px solid #ef9a9a; }
.b-grey { background: #f5f5f5; color: #757575; border: 1px solid #bdbdbd; }

.cat-row  { display: flex; align-items: center; gap: 8px; margin-bottom: 10px; }
.cat-name { width: 140px; font-size: 11px; color: #515151; flex-shrink: 0; }
.cat-bg   { flex: 1; background: #e0e0e0; border-radius: 3px; height: 8px; }
.cat-fill { height: 8px; border-radius: 3px; }
.cat-val  { font-size: 11px; font-weight: 600; color: #333; width: 54px; text-align: right; flex-shrink: 0; }

.mv-received   { color: #388E3C; font-weight: 700; }
.mv-production { color: #1565c0; font-weight: 700; }
.mv-dispatch   { color: #e53935; font-weight: 700; }
.mv-corrected  { color: #f9a825; font-weight: 700; }

.empty-msg { padding: 24px; text-align: center; color: #9e9e9e; font-size: 11px; }
</style>
