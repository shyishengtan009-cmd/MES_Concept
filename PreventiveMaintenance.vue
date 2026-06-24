<template>
  <div class="pm-root">

    <!-- TOPBAR -->
    <div class="pm-topbar">
      <span class="pm-title">&#9632; Preventive Maintenance</span>
      <SegmentedToggle :tabs="lineTabs" v-model="activeTab" />
      <div class="pm-live">
        <span class="live-dot"></span>
        LIVE | <span>{{ clock }}</span>
      </div>
    </div>

    <!-- FFS TAB -->
    <div v-show="activeTab === 'ffs'" class="tab-pane">
      <div class="kpi-row">
        <div class="kpi-card"><div class="kpi-label">Total Tasks</div><div class="kpi-val c-blue">16</div><div class="kpi-sub">this month</div></div>
        <div class="kpi-card"><div class="kpi-label">Completed</div><div class="kpi-val c-green">13</div><div class="kpi-sub">on schedule</div></div>
        <div class="kpi-card"><div class="kpi-label">In Progress</div><div class="kpi-val c-blue">1</div><div class="kpi-sub">active</div></div>
        <div class="kpi-card"><div class="kpi-label">Overdue</div><div class="kpi-val c-red">2</div><div class="kpi-sub">requires attention</div></div>
        <div class="kpi-card"><div class="kpi-label">Upcoming (7d)</div><div class="kpi-val c-yellow">1</div><div class="kpi-sub">scheduled</div></div>
        <div class="kpi-card"><div class="kpi-label">Compliance Rate</div><div class="kpi-val c-green">91%</div><div class="kpi-sub">this quarter</div></div>
      </div>
      <div class="main-body">
        <div class="pm-table-wrap">
          <div class="pm-table-title">&#9632; Maintenance Schedule — FFS Packaging Line</div>
          <div class="table-scroll">
            <table>
              <thead><tr><th>Asset</th><th>Zone</th><th>Task</th><th>Frequency</th><th>Last Done</th><th>Next Due</th><th>Status</th><th>Assigned</th></tr></thead>
              <tbody>
                <tr v-for="row in ffsSchedule" :key="row.asset + row.task">
                  <td>{{ row.asset }}</td>
                  <td>{{ row.zone }}</td>
                  <td>{{ row.task }}</td>
                  <td>{{ row.frequency }}</td>
                  <td>{{ row.lastDone }}</td>
                  <td>{{ row.nextDue }}</td>
                  <td><span class="badge" :class="statusBadge(row.status)">{{ row.status }}</span></td>
                  <td>{{ row.assigned }}</td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
        <div class="right-panel">
          <div class="rp-card" style="flex:1;">
            <div class="rp-title">&#9632; Asset Health</div>
            <div class="rp-body">
              <div v-for="a in ffsHealth" :key="a.name" class="gauge-row">
                <span class="gauge-label">{{ a.name }}</span>
                <div class="gauge-bar-wrap"><div class="gauge-bar" :style="{ width: a.health + '%', background: a.color }"></div></div>
                <span class="gauge-val">{{ a.health }}%</span>
              </div>
            </div>
          </div>
          <div class="rp-card" style="flex:1;">
            <div class="rp-title">&#9632; Recent Activity</div>
            <div class="rp-body">
              <div class="event-list">
                <div v-for="e in ffsActivity" :key="e.time" class="event-item">
                  <span class="ev-dot" :style="{ background: e.color }"></span>
                  <span class="ev-time">{{ e.time }}</span>
                  <span class="ev-text">{{ e.text }}</span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- BATCHING PLANT TAB -->
    <div v-show="activeTab === 'bp'" class="tab-pane">
      <div class="kpi-row">
        <div class="kpi-card"><div class="kpi-label">Total Tasks</div><div class="kpi-val c-blue">10</div><div class="kpi-sub">this month</div></div>
        <div class="kpi-card"><div class="kpi-label">Completed</div><div class="kpi-val c-green">7</div><div class="kpi-sub">on schedule</div></div>
        <div class="kpi-card"><div class="kpi-label">In Progress</div><div class="kpi-val c-blue">1</div><div class="kpi-sub">active</div></div>
        <div class="kpi-card"><div class="kpi-label">Overdue</div><div class="kpi-val c-red">1</div><div class="kpi-sub">requires attention</div></div>
        <div class="kpi-card"><div class="kpi-label">Upcoming (7d)</div><div class="kpi-val c-yellow">1</div><div class="kpi-sub">scheduled</div></div>
        <div class="kpi-card"><div class="kpi-label">Compliance Rate</div><div class="kpi-val c-yellow">75%</div><div class="kpi-sub">this quarter</div></div>
      </div>
      <div class="main-body">
        <div class="pm-table-wrap">
          <div class="pm-table-title">&#9632; Maintenance Schedule — Batching Plant</div>
          <div class="table-scroll">
            <table>
              <thead><tr><th>Asset</th><th>Zone</th><th>Task</th><th>Frequency</th><th>Last Done</th><th>Next Due</th><th>Status</th><th>Assigned</th></tr></thead>
              <tbody>
                <tr v-for="row in bpSchedule" :key="row.asset + row.task">
                  <td>{{ row.asset }}</td>
                  <td>{{ row.zone }}</td>
                  <td>{{ row.task }}</td>
                  <td>{{ row.frequency }}</td>
                  <td>{{ row.lastDone }}</td>
                  <td>{{ row.nextDue }}</td>
                  <td><span class="badge" :class="statusBadge(row.status)">{{ row.status }}</span></td>
                  <td>{{ row.assigned }}</td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
        <div class="right-panel">
          <div class="rp-card" style="flex:1;">
            <div class="rp-title">&#9632; Asset Health</div>
            <div class="rp-body">
              <div v-for="a in bpHealth" :key="a.name" class="gauge-row">
                <span class="gauge-label">{{ a.name }}</span>
                <div class="gauge-bar-wrap"><div class="gauge-bar" :style="{ width: a.health + '%', background: a.color }"></div></div>
                <span class="gauge-val">{{ a.health }}%</span>
              </div>
            </div>
          </div>
          <div class="rp-card" style="flex:1;">
            <div class="rp-title">&#9632; Recent Activity</div>
            <div class="rp-body">
              <div class="event-list">
                <div v-for="e in bpActivity" :key="e.time" class="event-item">
                  <span class="ev-dot" :style="{ background: e.color }"></span>
                  <span class="ev-time">{{ e.time }}</span>
                  <span class="ev-text">{{ e.text }}</span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onBeforeUnmount } from 'vue'
import SegmentedToggle from './SegmentedToggle.vue'

const activeTab = ref<'ffs' | 'bp'>('ffs')
const clock     = ref('')

const lineTabs = [
  { id: 'ffs', label: '▶ FFS Packaging Line (Indoors)' },
  { id: 'bp',  label: '▶ Batching Plant (Outdoors)' },
] as const

let clockTimer: ReturnType<typeof setInterval> | null = null

function tick() {
  clock.value = new Date().toLocaleTimeString('en-GB', { hour: '2-digit', minute: '2-digit', second: '2-digit' })
}

onMounted(() => { tick(); clockTimer = setInterval(tick, 1000) })
onBeforeUnmount(() => { if (clockTimer) clearInterval(clockTimer) })

function statusBadge(status: string) {
  if (status === 'OVERDUE')     return 'badge-overdue'
  if (status === 'DUE SOON')    return 'badge-due'
  if (status === 'IN PROGRESS') return 'badge-progress'
  return 'badge-ok'
}

const ffsSchedule = [
  { asset: 'FFS Machine 01',  zone: 'Zone A', task: 'Seal jaw inspection',         frequency: 'Weekly',  lastDone: '29 Mar 2026', nextDue: '05 Apr 2026', status: 'OVERDUE',     assigned: 'Ali B.' },
  { asset: 'FFS Machine 04',  zone: 'Zone B', task: 'Horizontal jaw calibration',  frequency: 'Weekly',  lastDone: '25 Mar 2026', nextDue: '01 Apr 2026', status: 'OVERDUE',     assigned: 'Raj T.' },
  { asset: 'FFS Machine 02',  zone: 'Zone A', task: 'Film roll guide lubrication', frequency: 'Weekly',  lastDone: '02 Apr 2026', nextDue: '09 Apr 2026', status: 'DUE SOON',    assigned: 'Ali B.' },
  { asset: 'FFS Machine 01',  zone: 'Zone A', task: 'Drive belt inspection',       frequency: 'Monthly', lastDone: '05 Mar 2026', nextDue: '05 Apr 2026', status: 'IN PROGRESS', assigned: 'Ali B.' },
  { asset: 'FFS Machine 03',  zone: 'Zone A', task: 'Sealing wire replacement',    frequency: 'Monthly', lastDone: '10 Mar 2026', nextDue: '10 Apr 2026', status: 'OK',          assigned: 'Ali B.' },
  { asset: 'FFS Machine 02',  zone: 'Zone A', task: 'Dosing hopper cleaning',      frequency: 'Weekly',  lastDone: '04 Apr 2026', nextDue: '11 Apr 2026', status: 'OK',          assigned: 'Ali B.' },
  { asset: 'FFS Machine 03',  zone: 'Zone A', task: 'Motor current check',         frequency: 'Monthly', lastDone: '06 Mar 2026', nextDue: '06 Apr 2026', status: 'OK',          assigned: 'Raj T.' },
  { asset: 'Conveyor Belt A', zone: 'Zone A', task: 'Belt tension & alignment',    frequency: 'Monthly', lastDone: '07 Mar 2026', nextDue: '07 Apr 2026', status: 'OK',          assigned: 'Ali B.' },
  { asset: 'Inspection Unit', zone: 'Zone A', task: 'Camera lens clean',           frequency: 'Weekly',  lastDone: '03 Apr 2026', nextDue: '10 Apr 2026', status: 'OK',          assigned: 'Raj T.' },
]

const ffsHealth = [
  { name: 'FFS Machine 01',  health: 62, color: '#f9a825' },
  { name: 'FFS Machine 02',  health: 81, color: '#388E3C' },
  { name: 'FFS Machine 03',  health: 79, color: '#388E3C' },
  { name: 'FFS Machine 04',  health: 34, color: '#e53935' },
  { name: 'Conveyor Belt A', health: 71, color: '#388E3C' },
  { name: 'Inspection Unit', health: 85, color: '#388E3C' },
]

const ffsActivity = [
  { color: '#e53935', time: '09:14 am', text: 'FFS M04 stopped — E-42 jaw overheat fault' },
  { color: '#e53935', time: '09:08 am', text: 'FFS M01 — seal jaw inspection overdue by 2 days' },
  { color: '#1565c0', time: '08:55 am', text: 'FFS M01 — drive belt inspection started by Ali B.' },
  { color: '#f9a825', time: '08:10 am', text: 'FFS M02 — film roll lubrication due in 3 days' },
  { color: '#388E3C', time: '07:45 am', text: 'Conveyor Belt A — tension inspection completed ✓' },
  { color: '#388E3C', time: '07:20 am', text: 'FFS M03 — sealing wire replacement completed ✓' },
]

const bpSchedule = [
  { asset: 'Batching Plant 01', zone: 'Zone B', task: 'Mixer blade wear check',          frequency: 'Monthly',   lastDone: '01 Mar 2026', nextDue: '01 Apr 2026', status: 'OVERDUE',     assigned: 'Kumar S.' },
  { asset: 'Batching Plant 01', zone: 'Zone B', task: 'Cement silo sensor calibration',  frequency: 'Monthly',   lastDone: '06 Mar 2026', nextDue: '06 Apr 2026', status: 'IN PROGRESS', assigned: 'Kumar S.' },
  { asset: 'Batching Plant 02', zone: 'Zone B', task: 'Belt conveyor inspection',        frequency: 'Bi-weekly', lastDone: '28 Mar 2026', nextDue: '11 Apr 2026', status: 'DUE SOON',    assigned: 'Kumar S.' },
  { asset: 'Mixer Unit 01',     zone: 'Zone B', task: 'Drive belt tension check',        frequency: 'Bi-weekly', lastDone: '24 Mar 2026', nextDue: '07 Apr 2026', status: 'OK',          assigned: 'Kumar S.' },
  { asset: 'Mixer Unit 02',     zone: 'Zone B', task: 'Drive belt tension check',        frequency: 'Bi-weekly', lastDone: '24 Mar 2026', nextDue: '07 Apr 2026', status: 'OK',          assigned: 'Kumar S.' },
  { asset: 'Batching Plant 01', zone: 'Zone B', task: 'Weigh hopper calibration',        frequency: 'Monthly',   lastDone: '05 Mar 2026', nextDue: '05 Apr 2026', status: 'OK',          assigned: 'Kumar S.' },
  { asset: 'Batching Plant 02', zone: 'Zone B', task: 'Water flow meter calibration',    frequency: 'Monthly',   lastDone: '10 Mar 2026', nextDue: '10 Apr 2026', status: 'OK',          assigned: 'Kumar S.' },
  { asset: 'Batching Plant 03', zone: 'Zone B', task: 'Aggregate hopper cleaning',       frequency: 'Weekly',    lastDone: '04 Apr 2026', nextDue: '11 Apr 2026', status: 'OK',          assigned: 'Kumar S.' },
  { asset: 'Batching Plant 01', zone: 'Zone B', task: 'Admixture pump inspection',       frequency: 'Monthly',   lastDone: '08 Mar 2026', nextDue: '08 Apr 2026', status: 'OK',          assigned: 'Kumar S.' },
]

const bpHealth = [
  { name: 'Batching Plant 01', health: 48, color: '#f9a825' },
  { name: 'Batching Plant 02', health: 72, color: '#388E3C' },
  { name: 'Batching Plant 03', health: 65, color: '#f9a825' },
  { name: 'Mixer Unit 01',     health: 80, color: '#388E3C' },
  { name: 'Mixer Unit 02',     health: 76, color: '#388E3C' },
]

const bpActivity = [
  { color: '#e53935', time: '09:11 am', text: 'BP01 — aggregate hopper low level alarm (18.2%)' },
  { color: '#e53935', time: '09:00 am', text: 'BP01 — mixer blade check overdue by 13 days' },
  { color: '#1565c0', time: '08:50 am', text: 'BP01 — cement silo calibration started by Kumar S.' },
  { color: '#388E3C', time: '08:02 am', text: 'BP02 — batch #47 completed (2,450 kg dispatched)' },
  { color: '#388E3C', time: '07:55 am', text: 'Mixer Unit 02 — belt tension within spec ✓' },
  { color: '#f9a825', time: '07:44 am', text: 'BP03 set to IDLE — awaiting work order' },
]
</script>

<style scoped>
*, *::before, *::after { box-sizing: border-box; margin: 0; padding: 0; }
.pm-root { height: 100%; overflow: hidden; font-family: 'Poppins', sans-serif; font-size: 12px; background: #f5f5f5; color: #333; display: flex; flex-direction: column; }

.pm-topbar { background: #ffffff; border-bottom: 1px solid #c3c6d4; padding: 0 16px; height: 42px; display: flex; align-items: center; gap: 0; flex-shrink: 0; box-shadow: 0 1px 3px rgba(0,0,0,.06); }
.pm-title  { font-size: 13px; font-weight: 700; color: #515151; letter-spacing: 0.5px; text-transform: uppercase; white-space: nowrap; }
.pm-live { display: flex; align-items: center; gap: 6px; font-size: 10px; color: #9e9e9e; margin-left: auto; }
.live-dot { width: 7px; height: 7px; border-radius: 50%; background: #43a047; animation: pulse 1.4s infinite; display: inline-block; }
@keyframes pulse { 0%,100%{opacity:1} 50%{opacity:0.3} }

.tab-pane { display: flex; flex-direction: column; flex: 1; overflow: hidden; min-height: 0; }

.kpi-row  { display: flex; gap: 1px; background: #c3c6d4; border-bottom: 2px solid #c3c6d4; flex-shrink: 0; }
.kpi-card { flex: 1; background: #ffffff; padding: 8px 14px; display: flex; flex-direction: column; gap: 2px; position: relative; overflow: hidden; border-left: 3px solid #1565c0; transition: transform .15s ease, box-shadow .15s ease; }
.kpi-card:hover { transform: translateY(-2px); box-shadow: 0 6px 14px rgba(0,0,0,.12); z-index: 2; }
.kpi-card::before { content: '\f201'; font-family: 'Font Awesome 6 Free'; font-weight: 900; position: absolute; right: 6px; top: 2px; font-size: 36px; color: #1565c0; opacity: .08; pointer-events: none; }
.kpi-card::after { content: ''; position: absolute; left: 0; right: 0; bottom: 0; height: 3px; background: linear-gradient(90deg, transparent, #1565c0, transparent); background-size: 200% 100%; animation: kpi-shimmer 2.5s linear infinite; opacity: .35; }
@keyframes kpi-shimmer { 0% { background-position: 200% 0; } 100% { background-position: -200% 0; } }
.kpi-label{ font-size: 10px; color: #7f7f7f; text-transform: uppercase; letter-spacing: 0.6px; position: relative; z-index: 1; }
.kpi-val  { font-size: 22px; font-weight: 800; line-height: 1.1; position: relative; z-index: 1; }
.kpi-sub  { font-size: 10px; color: #9e9e9e; }
.c-green  { color: #388E3C; } .c-yellow { color: #f9a825; } .c-red { color: #e53935; } .c-blue { color: #1565c0; }

.main-body { display: flex; gap: 10px; padding: 10px; flex: 1; overflow: hidden; min-height: 0; }

.pm-table-wrap   { flex: 2; background: #ffffff; border: 1px solid #c3c6d4; border-radius: 6px; overflow: hidden; display: flex; flex-direction: column; }
.pm-table-title  { padding: 8px 12px; font-size: 11px; font-weight: 700; color: #515151; text-transform: uppercase; letter-spacing: 0.4px; flex-shrink: 0; background: linear-gradient(0deg,#d7d7d7 0%,#ffffff 100%); border-bottom: 1px solid #c3c6d4; }
.table-scroll    { flex: 1; overflow-y: auto; }
.table-scroll::-webkit-scrollbar { width: 4px; }
.table-scroll::-webkit-scrollbar-thumb { background: #c3c6d4; border-radius: 2px; }
table  { width: 100%; border-collapse: collapse; font-size: 11px; }
th     { background: linear-gradient(0deg,#d7d7d7 0%,#ffffff 100%); color: #515151; font-size: 10px; text-transform: uppercase; padding: 7px 10px; text-align: left; border-bottom: 2px solid #c3c6d4; position: sticky; top: 0; z-index: 1; white-space: nowrap; }
td     { padding: 7px 10px; border-bottom: 1px solid #e0e0e0; color: #515151; white-space: nowrap; }
tr:hover td { background: #f2f2f2; }
.badge           { display: inline-block; padding: 2px 7px; border-radius: 3px; font-size: 9px; font-weight: 700; }
.badge-overdue   { background: #ffebee; color: #e53935; border: 1px solid #ef9a9a; }
.badge-due       { background: #fff9c4; color: #f9a825; border: 1px solid #fdd835; }
.badge-ok        { background: #e8f5e9; color: #388E3C; border: 1px solid #a5d6a7; }
.badge-progress  { background: #e3f2fd; color: #1565c0; border: 1px solid #90caf9; }

.right-panel { flex: 1; display: flex; flex-direction: column; gap: 10px; overflow: hidden; min-height: 0; }
.rp-card  { background: #ffffff; border: 1px solid #c3c6d4; border-radius: 6px; overflow: hidden; display: flex; flex-direction: column; }
.rp-title { padding: 8px 12px; font-size: 11px; font-weight: 700; color: #515151; text-transform: uppercase; letter-spacing: 0.4px; flex-shrink: 0; background: linear-gradient(0deg,#d7d7d7 0%,#ffffff 100%); border-bottom: 1px solid #c3c6d4; }
.rp-body  { padding: 10px 12px; overflow-y: auto; overflow-x: hidden; flex: 1; }
.rp-body::-webkit-scrollbar { width: 4px; }
.rp-body::-webkit-scrollbar-thumb { background: #c3c6d4; border-radius: 2px; }

.gauge-row   { display: flex; align-items: center; gap: 8px; margin-bottom: 7px; }
.gauge-label { font-size: 11px; color: #515151; width: 120px; flex-shrink: 0; }
.gauge-bar-wrap { flex: 1; background: #e0e0e0; border-radius: 3px; height: 7px; }
.gauge-bar   { height: 7px; border-radius: 3px; }
.gauge-val   { font-size: 11px; color: #333; width: 34px; text-align: right; flex-shrink: 0; font-weight: 600; }

.event-list  { display: flex; flex-direction: column; gap: 6px; }
.event-item  { display: flex; align-items: flex-start; gap: 8px; font-size: 11px; }
.ev-dot  { width: 7px; height: 7px; border-radius: 50%; margin-top: 3px; flex-shrink: 0; }
.ev-time { color: #7f7f7f; min-width: 64px; font-size: 10px; white-space: nowrap; }
.ev-text { color: #515151; }
</style>
