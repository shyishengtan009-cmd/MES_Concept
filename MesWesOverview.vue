<template>
  <div class="mw-root">

    <!-- TOPBAR -->
    <div class="mw-topbar">
      <span class="mw-title">&#9632; MES / WES Overview</span>
      <div class="view-tabs">
        <button class="view-tab" :class="{ active: view === 'both' }" @click="view = 'both'">Both</button>
        <button class="view-tab" :class="{ active: view === 'mes'  }" @click="view = 'mes'">MES Only</button>
        <button class="view-tab" :class="{ active: view === 'wes'  }" @click="view = 'wes'">WES Only</button>
      </div>
      <div class="mw-live"><span class="live-dot"></span> LIVE | {{ clock }}</div>
    </div>

    <!-- KPI ROW -->
    <div class="kpi-row">
      <div class="kpi-card"><div class="kpi-label">Work Orders</div><div class="kpi-val c-b">{{ kpis.activeWorkOrders }}</div><div class="kpi-sub">active</div></div>
      <div class="kpi-card"><div class="kpi-label">Orders Complete</div><div class="kpi-val c-g">{{ kpis.ordersComplete }}</div><div class="kpi-sub">today</div></div>
      <div class="kpi-card"><div class="kpi-label">On-Time Rate</div><div class="kpi-val c-g">{{ kpis.onTimeRate }}</div><div class="kpi-sub">last 24h</div></div>
      <div class="kpi-card"><div class="kpi-label">Warehouse Tasks</div><div class="kpi-val c-b">{{ kpis.warehouseTasks }}</div><div class="kpi-sub">queued</div></div>
      <div class="kpi-card"><div class="kpi-label">AGVs Active</div><div class="kpi-val c-g">{{ kpis.agvsActive }}</div><div class="kpi-sub">in operation</div></div>
      <div class="kpi-card"><div class="kpi-label">Pick Accuracy</div><div class="kpi-val c-g">{{ kpis.pickAccuracy }}</div><div class="kpi-sub">shift avg</div></div>
      <div class="kpi-card"><div class="kpi-label">Exceptions</div><div class="kpi-val c-r">{{ kpis.exceptions }}</div><div class="kpi-sub">unresolved</div></div>
    </div>

    <!-- BODY -->
    <div class="mw-body">

      <!-- MES COLUMN -->
      <div class="mes-col" v-show="view !== 'wes'">

        <div class="pc">
          <div class="ph">
            <span class="ph-mes">&#9632; MES — Active Work Orders</span>
            <span class="ph-right">Manufacturing Execution System</span>
          </div>
          <table class="dt">
            <thead><tr><th>Order</th><th>Product</th><th>Line</th><th>Qty</th><th>Progress</th><th>Status</th></tr></thead>
            <tbody>
              <tr v-for="wo in workOrders" :key="wo.order">
                <td>{{ wo.order }}</td>
                <td>{{ wo.product }}</td>
                <td>{{ wo.line }}</td>
                <td>{{ wo.qty }}</td>
                <td>
                  <span class="prog-wrap"><span class="prog-bar" :style="{ width: wo.progress + '%', background: progressColor(wo.status) }"></span></span>
                  {{ wo.progress }}%
                </td>
                <td><span class="badge" :class="statusBadge(wo.status)">{{ wo.status }}</span></td>
              </tr>
            </tbody>
          </table>
        </div>

        <div class="pc">
          <div class="ph"><span class="ph-mes">&#9632; MES — Quality Holds</span></div>
          <table class="dt">
            <thead><tr><th>Lot</th><th>Reason</th><th>Qty Held</th><th>Raised</th><th>Status</th></tr></thead>
            <tbody>
              <tr v-for="hold in qualityHolds" :key="hold.lot">
                <td>{{ hold.lot }}</td>
                <td>{{ hold.reason }}</td>
                <td>{{ hold.qtyHeld }}</td>
                <td>{{ hold.raised }}</td>
                <td><span class="badge" :class="holdBadge(hold.status)">{{ hold.status }}</span></td>
              </tr>
            </tbody>
          </table>
        </div>

        <div class="pc" style="flex:1">
          <div class="ph"><span class="ph-mes">&#9632; MES — Recent Events</span></div>
          <div class="timeline">
            <div class="tl-row" v-for="(ev, i) in mesEvents" :key="i">
              <span class="tl-dot" :style="{ background: ev.color }"></span>
              <span class="tl-time">{{ ev.time }}</span>
              <span class="tl-text">{{ ev.text }}</span>
            </div>
          </div>
        </div>

      </div>

      <!-- WES COLUMN -->
      <div class="wes-col" v-show="view !== 'mes'">

        <div class="pc">
          <div class="ph">
            <span class="ph-wes">&#9632; WES — Warehouse Flow</span>
            <span class="ph-right">Warehouse Execution System</span>
          </div>
          <div class="wes-flow">
            <div class="flow-row">
              <div class="flow-node"><div class="fn-name">Receiving</div><div class="fn-val c-b">{{ wesFlow.receiving }}</div><div class="fn-sub">pending putaway</div></div>
              <span class="flow-arrow">&#8594;</span>
              <div class="flow-node"><div class="fn-name">Storage</div><div class="fn-val c-g">{{ wesFlow.storage.toLocaleString() }}</div><div class="fn-sub">units on shelf</div></div>
              <span class="flow-arrow">&#8594;</span>
              <div class="flow-node"><div class="fn-name">Picking</div><div class="fn-val c-b">{{ wesFlow.picking }}</div><div class="fn-sub">tasks active</div></div>
            </div>
            <div class="flow-row">
              <div class="flow-node"><div class="fn-name">Packing</div><div class="fn-val c-b">{{ wesFlow.packing }}</div><div class="fn-sub">orders packing</div></div>
              <span class="flow-arrow">&#8594;</span>
              <div class="flow-node"><div class="fn-name">Staging</div><div class="fn-val c-y">{{ wesFlow.staging }}</div><div class="fn-sub">awaiting dispatch</div></div>
              <span class="flow-arrow">&#8594;</span>
              <div class="flow-node"><div class="fn-name">Dispatch</div><div class="fn-val c-g">{{ wesFlow.dispatch }}</div><div class="fn-sub">shipped today</div></div>
            </div>
          </div>
        </div>

        <div class="pc">
          <div class="ph"><span class="ph-wes">&#9632; WES — AGV / Equipment Status</span></div>
          <table class="dt">
            <thead><tr><th>Unit</th><th>Zone</th><th>Task</th><th>Battery</th><th>Status</th></tr></thead>
            <tbody>
              <tr v-for="agv in agvStatus" :key="agv.unit">
                <td>{{ agv.unit }}</td>
                <td>{{ agv.zone }}</td>
                <td>{{ agv.task }}</td>
                <td>{{ agv.battery }}</td>
                <td><span class="badge" :class="agvBadge(agv.status)">{{ agv.status }}</span></td>
              </tr>
            </tbody>
          </table>
        </div>

        <div class="pc" style="flex:1">
          <div class="ph"><span class="ph-wes">&#9632; WES — Recent Events</span></div>
          <div class="timeline">
            <div class="tl-row" v-for="(ev, i) in wesEvents" :key="i">
              <span class="tl-dot" :style="{ background: ev.color }"></span>
              <span class="tl-time">{{ ev.time }}</span>
              <span class="tl-text">{{ ev.text }}</span>
            </div>
          </div>
        </div>

      </div>
    </div>

  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onBeforeUnmount } from 'vue'

type View = 'both' | 'mes' | 'wes'

const view  = ref<View>('both')
const clock = ref('')

let clockTimer: ReturnType<typeof setInterval> | null = null
let liveTimer:  ReturnType<typeof setInterval> | null = null

function tick() {
  clock.value = new Date().toLocaleTimeString('en-GB', { hour: '2-digit', minute: '2-digit', second: '2-digit' })
}

const kpis = ref({
  activeWorkOrders: 12,
  ordersComplete:   47,
  onTimeRate:       '91%',
  warehouseTasks:   38,
  agvsActive:       '4/5',
  pickAccuracy:     '99.2%',
  exceptions:       3
})

const workOrders = ref([
  { order: 'WO-2026-041', product: 'Pack A 500g', line: 'Line 1', qty: '2,400',  progress: 82,  status: 'RUNNING' },
  { order: 'WO-2026-042', product: 'Pack B 250g', line: 'Line 1', qty: '1,800',  progress: 55,  status: 'RUNNING' },
  { order: 'WO-2026-043', product: 'Mix Batch C', line: 'Line 2', qty: '80 bat', progress: 100, status: 'DONE'    },
  { order: 'WO-2026-044', product: 'Pack A 1kg',  line: 'Line 2', qty: '600',    progress: 20,  status: 'SETUP'   },
  { order: 'WO-2026-045', product: 'Mix Batch D', line: 'Line 2', qty: '120 bat',progress: 0,   status: 'QUEUED'  },
])

const qualityHolds = ref([
  { lot: 'LOT-2026-088', reason: 'Fill weight deviation', qtyHeld: '240 units', raised: '09:07 am', status: 'ON HOLD'   },
  { lot: 'LOT-2026-085', reason: 'Seal integrity check',  qtyHeld: '80 units',  raised: '08:50 am', status: 'REVIEWING' },
  { lot: 'LOT-2026-081', reason: 'Label placement',       qtyHeld: '150 units', raised: '07:30 am', status: 'RELEASED'  },
])

const mesEvents = ref([
  { color: '#e53935', time: '09:14 am', text: 'WO-2026-044: setup delay — awaiting raw material from Zone B' },
  { color: '#388E3C', time: '09:02 am', text: 'WO-2026-043 completed — 80 batches dispatched to Zone D'     },
  { color: '#f9a825', time: '08:57 am', text: 'LOT-2026-088: quality hold raised on Line 1'                  },
  { color: '#1565c0', time: '08:44 am', text: 'Shift 2 started — WO-2026-041 resumed on Line 1'              },
  { color: '#388E3C', time: '08:30 am', text: 'LOT-2026-081 released — 150 units cleared by QC'              },
])

const wesFlow = ref({ receiving: 8, storage: 2840, picking: 14, packing: 6, staging: 3, dispatch: 47 })

const agvStatus = ref([
  { unit: 'AGV Unit 01', zone: 'Zone D',   task: 'Rack A → Staging',   battery: '88%',  status: 'ACTIVE'   },
  { unit: 'Forklift 01', zone: 'Zone D',   task: 'Receiving → Rack C', battery: '—',    status: 'RUNNING'  },
  { unit: 'AGV Unit 02', zone: 'Zone B',   task: 'Batch → Zone D',     battery: '61%',  status: 'ACTIVE'   },
  { unit: 'AGV Unit 03', zone: '—',        task: 'Idle at dock',        battery: '100%', status: 'IDLE'     },
  { unit: 'AGV Unit 04', zone: 'Charging', task: '—',                   battery: '32%',  status: 'CHARGING' },
  { unit: 'AGV Unit 05', zone: '—',        task: 'Maintenance hold',    battery: '—',    status: 'OFFLINE'  },
])

const wesEvents = ref([
  { color: '#e53935', time: '09:11 am', text: 'AGV Unit 05 offline — maintenance hold flagged'   },
  { color: '#388E3C', time: '09:05 am', text: 'Order #ORD-882 dispatched — 480 units shipped'    },
  { color: '#1565c0', time: '08:58 am', text: 'AGV Unit 02 rerouted — Zone B congestion'         },
  { color: '#388E3C', time: '08:44 am', text: 'Rack C replenished — 320 units putaway complete'  },
  { color: '#f9a825', time: '08:30 am', text: 'AGV Unit 04 low battery — returning to dock'      },
])

function rnd(base: number, delta: number, dec: number): number {
  const v = base + (Math.random() * 2 - 1) * delta
  return dec > 0 ? parseFloat(v.toFixed(dec)) : Math.round(v)
}

function liveUpdate() {
  if (Math.random() > 0.5) kpis.value.ordersComplete++
  kpis.value.warehouseTasks = Math.max(20, Math.min(60, kpis.value.warehouseTasks + Math.floor(Math.random() * 5) - 2))
  kpis.value.onTimeRate   = Math.min(99, Math.max(85, rnd(91, 2, 0))) + '%'
  kpis.value.pickAccuracy = Math.min(100, Math.max(97, rnd(99.2, 0.3, 1))) + '%'

  wesFlow.value.dispatch  = kpis.value.ordersComplete
  wesFlow.value.storage   = Math.round(rnd(2840, 100, 0))
  wesFlow.value.picking   = Math.max(0, rnd(14, 4, 0))
  wesFlow.value.packing   = Math.max(0, rnd(6, 2, 0))
  wesFlow.value.staging   = Math.max(0, rnd(3, 2, 0))

  const wo = workOrders.value
  wo[0].progress = Math.min(100, wo[0].progress + Math.floor(Math.random() * 4 + 1))
  wo[1].progress = Math.min(100, wo[1].progress + Math.floor(Math.random() * 5 + 1))
  wo[3].progress = Math.min(100, wo[3].progress + Math.floor(Math.random() * 3 + 1))
}

function progressColor(status: string): string {
  if (status === 'DONE')    return '#388E3C'
  if (status === 'RUNNING') return status === 'RUNNING' ? '#388E3C' : '#1565c0'
  if (status === 'SETUP')   return '#f9a825'
  return '#1565c0'
}

function statusBadge(status: string): string {
  if (status === 'RUNNING') return 'b-blue'
  if (status === 'DONE')    return 'b-green'
  if (status === 'SETUP')   return 'b-yellow'
  if (status === 'QUEUED')  return 'b-yellow'
  return 'b-blue'
}

function holdBadge(status: string): string {
  if (status === 'ON HOLD')   return 'b-red'
  if (status === 'REVIEWING') return 'b-yellow'
  if (status === 'RELEASED')  return 'b-green'
  return 'b-blue'
}

function agvBadge(status: string): string {
  if (status === 'ACTIVE')   return 'b-green'
  if (status === 'RUNNING')  return 'b-blue'
  if (status === 'IDLE')     return 'b-yellow'
  if (status === 'CHARGING') return 'b-yellow'
  if (status === 'OFFLINE')  return 'b-red'
  return 'b-blue'
}

onMounted(() => {
  tick()
  clockTimer = setInterval(tick, 1000)
  liveTimer  = setInterval(liveUpdate, 17000)
})

onBeforeUnmount(() => {
  if (clockTimer) clearInterval(clockTimer)
  if (liveTimer)  clearInterval(liveTimer)
})
</script>

<style scoped>
*, *::before, *::after { box-sizing: border-box; margin: 0; padding: 0; }

.mw-root { height: 100%; overflow: hidden; font-family: 'Poppins', sans-serif; font-size: 12px; background: #f5f5f5; color: #333; display: flex; flex-direction: column; }

.mw-topbar { background: #ffffff; border-bottom: 1px solid #c3c6d4; padding: 8px 16px; display: flex; align-items: center; gap: 12px; flex-shrink: 0; box-shadow: 0 1px 3px rgba(0,0,0,.06); }
.mw-title  { font-size: 13px; font-weight: 700; color: #515151; text-transform: uppercase; letter-spacing: 0.5px; }
.view-tabs { display: flex; gap: 0; margin-left: 16px; border-bottom: 2px solid #c3c6d4; align-self: stretch; }
.view-tab  { background: #fff; border: none; border-right: 1px solid #e0e0e0; color: #7f7f7f; cursor: pointer; font-family: 'Poppins', sans-serif; font-size: 12px; font-weight: 600; padding: 6px 18px; transition: background .15s, color .15s; letter-spacing: 0.4px; }
.view-tab:hover  { background: #f2f2f2; color: #333; }
.view-tab.active { background: #fff; color: #333; border-bottom: 2px solid #1565c0; margin-bottom: -2px; }
.mw-live   { display: flex; align-items: center; gap: 6px; font-size: 11px; color: #7f7f7f; margin-left: auto; }
.live-dot  { width: 8px; height: 8px; border-radius: 50%; background: #43a047; animation: pulse 1.4s infinite; display: inline-block; }
@keyframes pulse { 0%,100%{ opacity:1 } 50%{ opacity:0.3 } }

.kpi-row  { display: flex; gap: 8px; padding: 10px 16px; flex-shrink: 0; }
.kpi-card { background: #ffffff; border: 1px solid #c3c6d4; border-radius: 6px; padding: 10px 14px; flex: 1; }
.kpi-label{ font-size: 10px; color: #7f7f7f; text-transform: uppercase; letter-spacing: 0.5px; margin-bottom: 4px; }
.kpi-val  { font-size: 22px; font-weight: 700; line-height: 1.1; }
.kpi-sub  { font-size: 10px; color: #9e9e9e; margin-top: 2px; }
.c-g { color: #388E3C; } .c-b { color: #1565c0; } .c-y { color: #f9a825; } .c-r { color: #e53935; } .c-w { color: #515151; }

.mw-body { display: flex; gap: 10px; padding: 0 16px 10px; flex: 1; overflow: hidden; min-height: 0; }

.pc { background: #ffffff; border: 1px solid #c3c6d4; border-radius: 6px; overflow: hidden; display: flex; flex-direction: column; }
.ph { padding: 8px 12px; font-size: 11px; font-weight: 700; color: #515151; text-transform: uppercase; letter-spacing: 0.4px; flex-shrink: 0; background: linear-gradient(0deg, #d7d7d7 0%, #ffffff 100%); border-bottom: 1px solid #c3c6d4; display: flex; align-items: center; justify-content: space-between; }
.ph-right { font-size: 10px; font-weight: 400; color: #9e9e9e; text-transform: none; letter-spacing: 0; }
.ph-mes   { color: #1565c0; }
.ph-wes   { color: #388E3C; }

.mes-col { flex: 1; display: flex; flex-direction: column; gap: 10px; overflow: auto; }
.mes-col::-webkit-scrollbar { width: 4px; } .mes-col::-webkit-scrollbar-thumb { background: #c3c6d4; border-radius: 2px; }
.wes-col { flex: 1; display: flex; flex-direction: column; gap: 10px; overflow: auto; }
.wes-col::-webkit-scrollbar { width: 4px; } .wes-col::-webkit-scrollbar-thumb { background: #c3c6d4; border-radius: 2px; }

table.dt { width: 100%; border-collapse: collapse; font-size: 11px; }
table.dt th { background: linear-gradient(0deg, #d7d7d7 0%, #ffffff 100%); padding: 5px 7px; text-align: left; color: #515151; font-weight: 600; border-bottom: 2px solid #c3c6d4; position: sticky; top: 0; z-index: 1; font-size: 10px; text-transform: uppercase; letter-spacing: 0.3px; }
table.dt tbody tr:nth-child(odd)  td { background: #ffffff; }
table.dt tbody tr:nth-child(even) td { background: #f8f8f8; }
table.dt td { padding: 6px 8px; border-bottom: 1px solid #e0e0e0; vertical-align: middle; color: #515151; font-size: 11px; }
table.dt tr:hover td { background: #f2f2f2 !important; }

.badge    { display: inline-block; padding: 1px 8px; border-radius: 3px; font-size: 9px; font-weight: 700; }
.b-green  { background: #e8f5e9; color: #388E3C; border: 1px solid #a5d6a7; }
.b-blue   { background: #e3f2fd; color: #1565c0; border: 1px solid #90caf9; }
.b-yellow { background: #fff9c4; color: #f9a825; border: 1px solid #fdd835; }
.b-red    { background: #ffebee; color: #e53935; border: 1px solid #ef9a9a; }

.prog-wrap { background: #e0e0e0; border-radius: 2px; height: 6px; width: 80px; display: inline-block; vertical-align: middle; margin-right: 4px; }
.prog-bar  { height: 6px; border-radius: 2px; display: block; }

.wes-flow  { padding: 10px 12px; display: flex; flex-direction: column; gap: 8px; }
.flow-row  { display: flex; align-items: center; gap: 6px; }
.flow-node { background: #f5f5f5; border: 1px solid #c3c6d4; border-radius: 4px; padding: 7px 10px; font-size: 10px; flex: 1; text-align: center; }
.fn-name   { color: #515151; font-weight: 600; font-size: 10px; }
.fn-val    { font-size: 14px; font-weight: 700; margin-top: 2px; }
.fn-sub    { font-size: 9px; color: #9e9e9e; margin-top: 1px; }
.flow-arrow{ color: #9e9e9e; font-size: 14px; flex-shrink: 0; }

.timeline { padding: 8px 12px; display: flex; flex-direction: column; gap: 7px; }
.tl-row   { display: flex; gap: 8px; align-items: flex-start; font-size: 10px; }
.tl-dot   { width: 7px; height: 7px; border-radius: 50%; margin-top: 3px; flex-shrink: 0; }
.tl-time  { color: #7f7f7f; min-width: 64px; }
.tl-text  { color: #515151; }
</style>
