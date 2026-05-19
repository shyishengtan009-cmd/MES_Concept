<template>
  <div class="root">

    <!-- Topbar -->
    <div class="topbar">
      <span class="pg-title">&#9632; Production Planning</span>
      <div class="week-nav">
        <button class="nav-btn" @click="prevWeek"><i class="fa-solid fa-chevron-left"></i></button>
        <span class="week-label">{{ weekLabel }}</span>
        <button class="nav-btn" @click="nextWeek"><i class="fa-solid fa-chevron-right"></i></button>
        <button class="btn-outline-sm" @click="goToday">Today</button>
      </div>
      <div class="topbar-right">
        <select class="sel" v-model="lineFilter">
          <option value="All">All Lines</option>
          <option v-for="l in lines" :key="l" :value="l">{{ l }}</option>
        </select>
        <button class="btn-primary"><i class="fa-solid fa-plus"></i> Schedule WO</button>
      </div>
    </div>

    <!-- KPI Strip -->
    <div class="kpi-strip">
      <div class="kc" v-for="k in kpis" :key="k.label">
        <div class="kc-val" :class="k.cls">{{ k.val }}</div>
        <div class="kc-lbl">{{ k.label }}</div>
      </div>
    </div>

    <!-- Gantt Table -->
    <div class="gantt-wrap">
      <table class="gantt-tbl">
        <thead>
          <tr>
            <th class="line-col">Production Line</th>
            <th v-for="d in weekDays" :key="d.key" class="day-col" :class="{ 'today-col': d.isToday }">
              <div class="day-name">{{ d.name }}</div>
              <div class="day-date">{{ d.date }}</div>
            </th>
          </tr>
        </thead>
        <tbody>
          <template v-for="line in visibleLines" :key="line">
            <tr class="gantt-row">
              <td class="line-cell">
                <div class="line-name">{{ line }}</div>
                <div class="line-util">Util: {{ lineUtil(line) }}%</div>
              </td>
              <td
                v-for="d in weekDays" :key="d.key"
                class="gantt-cell" :class="{ 'today-cell': d.isToday }"
              >
                <div
                  v-for="wo in getCell(line, d.key)" :key="wo.woNo"
                  class="wo-block" :class="'wo-' + wo.status"
                  @click="openDetail(wo)"
                  :title="wo.product"
                >
                  <div class="wo-no">{{ wo.woNo }}</div>
                  <div class="wo-prod">{{ wo.product }}</div>
                  <div class="wo-qty">{{ wo.qty.toLocaleString() }} pcs</div>
                </div>
                <div v-if="!getCell(line, d.key).length" class="cell-empty"></div>
              </td>
            </tr>
          </template>
        </tbody>
      </table>
    </div>

    <!-- Legend + Summary -->
    <div class="footer-row">
      <div class="legend">
        <span class="leg-item"><span class="leg-dot wo-running"></span>Running</span>
        <span class="leg-item"><span class="leg-dot wo-planned"></span>Planned</span>
        <span class="leg-item"><span class="leg-dot wo-completed"></span>Completed</span>
        <span class="leg-item"><span class="leg-dot wo-on-hold"></span>On Hold</span>
      </div>
      <div class="summary">
        <span>Week total: <b>{{ weekTotal }} WOs</b></span>
        <span>Avg utilisation: <b>{{ avgUtil }}%</b></span>
      </div>
    </div>

    <!-- WO Detail Popup -->
    <div v-if="detail" class="popup-overlay" @click="detail = null">
      <div class="popup" @click.stop>
        <div class="pop-head">
          <span class="pop-wo">{{ detail.woNo }}</span>
          <span class="badge" :class="woCls(detail.status)">{{ detail.status }}</span>
          <button class="dp-close" @click="detail = null"><i class="fa-solid fa-xmark"></i></button>
        </div>
        <div class="pop-body">
          <div class="pop-field"><span class="dfl">Product</span><span>{{ detail.product }}</span></div>
          <div class="pop-field"><span class="dfl">Line</span><span>{{ detail.line }}</span></div>
          <div class="pop-field"><span class="dfl">Qty Planned</span><span>{{ detail.qty.toLocaleString() }} pcs</span></div>
          <div class="pop-field"><span class="dfl">Scheduled Day</span><span>{{ detail.day }}</span></div>
          <div class="pop-field"><span class="dfl">SO Reference</span><span class="mono">{{ detail.soRef }}</span></div>
          <div class="pop-field" style="margin-top:8px">
            <span class="dfl">Progress</span>
            <div style="display:flex; align-items:center; gap:8px; margin-top:4px">
              <div class="prog-wrap"><div class="prog-bar" :style="{ width: detail.progress + '%' }"></div></div>
              <span>{{ detail.progress }}%</span>
            </div>
          </div>
        </div>
        <div class="pop-foot">
          <button class="btn-outline-sm" @click="detail = null">Close</button>
          <button class="btn-primary"><i class="fa-solid fa-pen"></i> Edit WO</button>
        </div>
      </div>
    </div>

  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'

interface WOBlock { woNo: string; product: string; qty: number; status: string; line: string; day: string; soRef: string; progress: number }

const lineFilter = ref('All')
const detail     = ref<WOBlock | null>(null)

const lines = ['Line 1', 'Line 2', 'Batch Plant 01', 'Batch Plant 02']

// Reference week starting Monday 21 Apr 2026
const baseMonday = new Date(2026, 3, 21) // Apr 21 2026
const weekOffset = ref(0)

const weekStart = computed(() => {
  const d = new Date(baseMonday)
  d.setDate(d.getDate() + weekOffset.value * 7)
  return d
})

const weekLabel = computed(() => {
  const s = weekStart.value
  const e = new Date(s); e.setDate(s.getDate() + 6)
  const fmt = (d: Date) => `${d.getDate()} ${d.toLocaleString('en-GB', { month: 'short' })} ${d.getFullYear()}`
  return `${fmt(s)} – ${fmt(e)}`
})

const weekDays = computed(() => {
  const today = new Date(2026, 3, 22) // simulate today = 22 Apr 2026
  const days = ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun']
  return days.map((name, i) => {
    const d = new Date(weekStart.value)
    d.setDate(d.getDate() + i)
    const key = `${String(d.getDate()).padStart(2,'0')}/${String(d.getMonth()+1).padStart(2,'0')}`
    const isToday = d.getDate() === today.getDate() && d.getMonth() === today.getMonth()
    return { name, date: key, key, isToday }
  })
})

// Static schedule data keyed by [line][day]
const schedule: Record<string, Record<string, WOBlock[]>> = {
  'Line 1': {
    '21/04': [{ woNo:'WO-2026-039', product:'Mixed Spice Sachet 50g',       qty:4800, status:'completed', line:'Line 1',        day:'21/04', soRef:'SO-2026-0407', progress:100 }],
    '22/04': [{ woNo:'WO-2026-041', product:'Halal Seasoning Blend A 500g', qty:2400, status:'running',   line:'Line 1',        day:'22/04', soRef:'SO-2026-0412', progress:82  }],
    '24/04': [{ woNo:'WO-2026-044', product:'Halal Marinade Sauce 250ml',   qty:2400, status:'planned',   line:'Line 1',        day:'24/04', soRef:'SO-2026-0418', progress:0   }],
    '25/04': [{ woNo:'WO-2026-046', product:'Halal Curry Paste 200g',       qty:2400, status:'planned',   line:'Line 1',        day:'25/04', soRef:'SO-2026-0420', progress:0   }],
    '27/04': [{ woNo:'WO-2026-050', product:'Halal Seasoning Blend A 500g', qty:2400, status:'planned',   line:'Line 1',        day:'27/04', soRef:'SO-2026-0425', progress:0   }],
  },
  'Line 2': {
    '21/04': [{ woNo:'WO-2026-038', product:'Halal Seasoning Blend A 500g', qty:2400, status:'completed', line:'Line 2',        day:'21/04', soRef:'SO-2026-0407', progress:100 }],
    '23/04': [{ woNo:'WO-2026-042', product:'Halal Marinade Sauce 250ml',   qty:1800, status:'planned',   line:'Line 2',        day:'23/04', soRef:'SO-2026-0412', progress:0   }],
    '26/04': [{ woNo:'WO-2026-049', product:'Palm Olein Oil 5L',            qty:600,  status:'planned',   line:'Line 2',        day:'26/04', soRef:'SO-2026-0423', progress:0   }],
  },
  'Batch Plant 01': {
    '20/04': [{ woNo:'WO-2026-043', product:'Halal Curry Paste 200g',       qty:600,  status:'completed', line:'Batch Plant 01',day:'20/04', soRef:'SO-2026-0415', progress:100 }],
    '21/04': [{ woNo:'WO-2026-040', product:'Palm Olein Oil 5L',            qty:600,  status:'on-hold',   line:'Batch Plant 01',day:'21/04', soRef:'SO-2026-0410', progress:0   }],
    '25/04': [{ woNo:'WO-2026-048', product:'Spice Mix Pack 1kg',           qty:600,  status:'planned',   line:'Batch Plant 01',day:'25/04', soRef:'SO-2026-0422', progress:0   }],
  },
  'Batch Plant 02': {
    '22/04': [{ woNo:'WO-2026-045', product:'Spice Mix Pack 1kg',           qty:600,  status:'running',   line:'Batch Plant 02',day:'22/04', soRef:'SO-2026-0418', progress:60  }],
    '25/04': [{ woNo:'WO-2026-047', product:'Spice Mix Pack 1kg',           qty:600,  status:'planned',   line:'Batch Plant 02',day:'25/04', soRef:'SO-2026-0420', progress:0   }],
    '27/04': [{ woNo:'WO-2026-051', product:'Mixed Spice Sachet 50g',       qty:4800, status:'planned',   line:'Batch Plant 02',day:'27/04', soRef:'SO-2026-0424', progress:0   }],
  },
}

const visibleLines = computed(() =>
  lineFilter.value === 'All' ? lines : lines.filter(l => l === lineFilter.value)
)

function getCell(line: string, day: string): WOBlock[] {
  return schedule[line]?.[day] ?? []
}

function lineUtil(line: string): number {
  const total = weekDays.value.reduce((acc, d) => acc + getCell(line, d.key).length, 0)
  return Math.min(100, Math.round(total / 7 * 100))
}

const weekTotal = computed(() =>
  visibleLines.value.reduce((acc, l) => acc + weekDays.value.reduce((a, d) => a + getCell(l, d.key).length, 0), 0)
)

const avgUtil = computed(() => {
  const utils = visibleLines.value.map(l => lineUtil(l))
  return utils.length ? Math.round(utils.reduce((a, b) => a + b, 0) / utils.length) : 0
})

const kpis = computed(() => [
  { label: 'Scheduled WOs',  val: weekTotal.value,                                   cls: 'c-blue'   },
  { label: 'Running',        val: visibleLines.value.reduce((acc, l) => acc + weekDays.value.reduce((a, d) => a + getCell(l,d.key).filter(w=>w.status==='running').length, 0), 0), cls: 'c-blue' },
  { label: 'Completed',      val: visibleLines.value.reduce((acc, l) => acc + weekDays.value.reduce((a, d) => a + getCell(l,d.key).filter(w=>w.status==='completed').length, 0), 0), cls: 'c-green' },
  { label: 'On Hold',        val: visibleLines.value.reduce((acc, l) => acc + weekDays.value.reduce((a, d) => a + getCell(l,d.key).filter(w=>w.status==='on-hold').length, 0), 0), cls: 'c-red' },
  { label: 'Avg Utilisation', val: avgUtil.value + '%',                              cls: avgUtil.value >= 70 ? 'c-green' : avgUtil.value >= 40 ? 'c-amber' : 'c-red' },
])

function prevWeek() { weekOffset.value-- }
function nextWeek() { weekOffset.value++ }
function goToday()  { weekOffset.value = 0 }
function openDetail(wo: WOBlock) { detail.value = wo }

function woCls(s: string) {
  return { 'b-blue': s==='running', 'b-green': s==='completed', 'b-amber': s==='planned', 'b-red': s==='on-hold' }
}
</script>

<style scoped>
*, *::before, *::after { box-sizing: border-box; }
.root { height: 100%; display: flex; flex-direction: column; font-family: 'Poppins', sans-serif; font-size: 12px; background: #f5f5f5; overflow: hidden; }

.topbar { background: #fff; border-bottom: 1px solid #c3c6d4; padding: 8px 14px; display: flex; align-items: center; gap: 10px; flex-shrink: 0; flex-wrap: wrap; }
.pg-title { font-size: 12px; font-weight: 700; text-transform: uppercase; color: #515151; flex-shrink: 0; }
.topbar-right { display: flex; gap: 6px; margin-left: auto; align-items: center; }

.week-nav { align-items: center; display: flex; gap: 6px; }
.week-label { font-size: 12px; font-weight: 600; color: #333; min-width: 200px; text-align: center; }
.nav-btn { background: #f5f5f5; border: 1px solid #d0d3e0; border-radius: 3px; color: #515151; cursor: pointer; font-size: 11px; padding: 5px 10px; }
.nav-btn:hover { background: #e3f2fd; border-color: #1565c0; color: #1565c0; }

.sel { border: 1px solid #d0d3e0; border-radius: 3px; font-family: 'Poppins', sans-serif; font-size: 11px; padding: 4px 8px; color: #515151; background: #fff; }
.btn-primary { background: #1565c0; border: none; border-radius: 3px; color: #fff; cursor: pointer; font-family: 'Poppins', sans-serif; font-size: 11px; gap: 5px; display: flex; align-items: center; padding: 5px 12px; }
.btn-primary:hover { background: #1976d2; }
.btn-outline-sm { background: #fff; border: 1px solid #d0d3e0; border-radius: 3px; color: #515151; cursor: pointer; font-family: 'Poppins', sans-serif; font-size: 11px; padding: 5px 10px; }
.btn-outline-sm:hover { background: #f5f5f5; }

.kpi-strip { background: #c3c6d4; display: flex; gap: 1px; flex-shrink: 0; }
.kc { background: #fff; flex: 1; padding: 8px 12px; }
.kc-val { font-size: 20px; font-weight: 700; }
.kc-lbl { font-size: 10px; color: #757575; margin-top: 1px; }
.c-blue   { color: #1565c0; } .c-green { color: #388E3C; }
.c-amber  { color: #f9a825; } .c-red   { color: #e53935; }

/* Gantt */
.gantt-wrap { flex: 1; overflow: auto; }
.gantt-tbl  { border-collapse: collapse; min-width: 900px; width: 100%; }

.gantt-tbl thead th { background: #f5f5f5; border-bottom: 2px solid #c3c6d4; border-right: 1px solid #e0e0e0; font-size: 10px; font-weight: 600; padding: 6px 8px; position: sticky; text-align: center; text-transform: uppercase; top: 0; z-index: 2; }
.gantt-tbl thead .line-col { text-align: left; width: 160px; z-index: 3; }
.today-col { background: #fff8e1 !important; }

.day-name { font-size: 11px; font-weight: 700; color: #515151; }
.day-date  { font-size: 10px; color: #9e9e9e; }

.gantt-row { border-bottom: 1px solid #e0e0e0; }
.line-cell { background: #fafafa; border-right: 2px solid #c3c6d4; padding: 10px 12px; position: sticky; left: 0; z-index: 1; vertical-align: top; min-width: 160px; }
.line-name { font-size: 11px; font-weight: 600; color: #333; }
.line-util { color: #9e9e9e; font-size: 10px; margin-top: 2px; }

.gantt-cell { border-right: 1px solid #e0e0e0; min-height: 60px; padding: 4px; vertical-align: top; min-width: 110px; }
.today-cell { background: #fffde7; }

.wo-block { border-radius: 4px; cursor: pointer; margin-bottom: 3px; padding: 5px 7px; transition: opacity .15s; }
.wo-block:hover { opacity: .85; }
.wo-block.wo-running   { background: #bbdefb; border-left: 3px solid #1565c0; }
.wo-block.wo-planned   { background: #fff3e0; border-left: 3px solid #f9a825; }
.wo-block.wo-completed { background: #c8e6c9; border-left: 3px solid #388E3C; }
.wo-block.wo-on-hold   { background: #ffebee; border-left: 3px solid #e53935; }
.wo-no   { font-family: monospace; font-size: 10px; color: #333; font-weight: 700; }
.wo-prod { font-size: 10px; color: #515151; margin-top: 1px; overflow: hidden; text-overflow: ellipsis; white-space: nowrap; max-width: 100px; }
.wo-qty  { color: #757575; font-size: 10px; margin-top: 1px; }
.cell-empty { height: 50px; }

.footer-row { align-items: center; background: #fff; border-top: 1px solid #c3c6d4; display: flex; flex-shrink: 0; gap: 16px; justify-content: space-between; padding: 8px 14px; }
.legend { display: flex; gap: 12px; }
.leg-item { align-items: center; display: flex; font-size: 11px; gap: 5px; color: #515151; }
.leg-dot { border-radius: 3px; height: 12px; width: 16px; }
.leg-dot.wo-running   { background: #bbdefb; border-left: 3px solid #1565c0; }
.leg-dot.wo-planned   { background: #fff3e0; border-left: 3px solid #f9a825; }
.leg-dot.wo-completed { background: #c8e6c9; border-left: 3px solid #388E3C; }
.leg-dot.wo-on-hold   { background: #ffebee; border-left: 3px solid #e53935; }
.summary { color: #515151; font-size: 11px; display: flex; gap: 16px; }

/* Popup */
.popup-overlay { align-items: center; background: rgba(0,0,0,.3); display: flex; inset: 0; justify-content: center; position: fixed; z-index: 300; }
.popup { background: #fff; border-radius: 6px; box-shadow: 0 8px 32px rgba(0,0,0,.2); min-width: 360px; overflow: hidden; }
.pop-head { align-items: center; background: linear-gradient(0deg,#d7d7d7,#fff); border-bottom: 1px solid #c3c6d4; display: flex; gap: 8px; padding: 12px 16px; }
.pop-wo  { font-size: 14px; font-weight: 700; color: #1565c0; flex: 1; font-family: monospace; }
.dp-close { background: transparent; border: none; color: #757575; cursor: pointer; font-size: 15px; padding: 2px 6px; }
.dp-close:hover { color: #e53935; }
.pop-body { display: flex; flex-direction: column; gap: 8px; padding: 14px 16px; }
.pop-field { display: flex; flex-direction: column; gap: 2px; }
.dfl { color: #9e9e9e; font-size: 10px; text-transform: uppercase; }
.mono { font-family: monospace; }
.prog-wrap { background: #e0e0e0; border-radius: 3px; flex: 1; height: 8px; overflow: hidden; }
.prog-bar  { background: #1565c0; border-radius: 3px; height: 100%; transition: width .3s; }
.pop-foot { border-top: 1px solid #c3c6d4; display: flex; gap: 6px; justify-content: flex-end; padding: 10px 16px; }

.badge { border-radius: 3px; font-size: 10px; font-weight: 600; padding: 2px 8px; white-space: nowrap; }
.b-blue   { background: #e3f2fd; color: #1565c0; }
.b-green  { background: #e8f5e9; color: #388E3C; }
.b-amber  { background: #fff8e1; color: #f9a825; }
.b-red    { background: #ffebee; color: #e53935; }
</style>
