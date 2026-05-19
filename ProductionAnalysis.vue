<template>
  <div class="root">

    <!-- Topbar -->
    <div class="topbar">
      <span class="pg-title">&#9632; Production Analysis</span>
      <div class="filters">
        <button v-for="p in periods" :key="p" class="sf-btn" :class="{ 'sf-btn--active': period === p }" @click="period = p">{{ p }}</button>
        <select class="sel" v-model="lineFilter">
          <option value="All">All Lines</option>
          <option value="Line 1">Line 1</option>
          <option value="Line 2">Line 2</option>
          <option value="Batch Plant 01">Batch Plant 01</option>
          <option value="Batch Plant 02">Batch Plant 02</option>
        </select>
        <button class="btn-outline-sm"><i class="fa-solid fa-file-export"></i> Export</button>
      </div>
    </div>

    <!-- KPI Strip -->
    <div class="kpi-strip">
      <div class="kc" v-for="k in kpis" :key="k.label">
        <div class="kc-val" :class="k.cls">{{ k.val }}</div>
        <div class="kc-lbl">{{ k.label }}</div>
        <div v-if="k.sub" class="kc-sub" :class="k.subCls">{{ k.sub }}</div>
      </div>
    </div>

    <!-- Main content -->
    <div class="content">

      <!-- Left column: Output chart + Downtime -->
      <div class="col-left">

        <!-- Output vs Target Chart -->
        <div class="pc">
          <div class="ph">Daily Output vs Target <span class="ph-r">{{ period }}</span></div>
          <div class="chart-area">
            <canvas ref="chartCanvas" class="chart-canvas"></canvas>
          </div>
          <div class="chart-legend">
            <span class="leg-item"><span class="leg-dot" style="background:#1565c0"></span>Output</span>
            <span class="leg-item"><span class="leg-dot" style="background:#e53935; border-radius:0; height:2px"></span>Target</span>
          </div>
        </div>

        <!-- Downtime Analysis -->
        <div class="pc" style="margin-top:8px">
          <div class="ph">Downtime Analysis <span class="ph-r">Total: {{ totalDowntime }}h</span></div>
          <div class="tbl-wrap">
            <table class="tbl">
              <thead><tr><th>Category</th><th>Equipment / Line</th><th class="tr">Duration (h)</th><th>Root Cause</th><th>Status</th></tr></thead>
              <tbody>
                <tr v-for="d in downtimes" :key="d.id" class="tbl-row">
                  <td><span class="cat-tag" :class="d.catCls">{{ d.category }}</span></td>
                  <td>{{ d.equipment }}</td>
                  <td class="tr"><b>{{ d.hours }}</b></td>
                  <td class="cause-cell">{{ d.cause }}</td>
                  <td><span class="badge" :class="d.resCls">{{ d.resolved ? 'Resolved' : 'Open' }}</span></td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>

      <!-- Right column: Defect Pareto + Quality table -->
      <div class="col-right">

        <!-- Defect Pareto -->
        <div class="pc">
          <div class="ph">Defect Pareto <span class="ph-r">Total: {{ totalDefects }} units</span></div>
          <div class="pareto-list">
            <div v-for="(d, i) in defects" :key="d.type" class="pareto-row">
              <div class="par-rank">{{ i + 1 }}</div>
              <div class="par-info">
                <div class="par-type">{{ d.type }}</div>
                <div class="par-bar-wrap">
                  <div class="par-bar" :style="{ width: d.pct + '%', background: paretoColor(i) }"></div>
                </div>
              </div>
              <div class="par-nums">
                <span class="par-val">{{ d.units }}</span>
                <span class="par-pct">{{ d.pct }}%</span>
              </div>
            </div>
          </div>
        </div>

        <!-- Quality Summary -->
        <div class="pc" style="margin-top:8px">
          <div class="ph">Quality Summary by Line</div>
          <div class="tbl-wrap">
            <table class="tbl">
              <thead><tr><th>Line</th><th class="tr">Output</th><th class="tr">Passed</th><th class="tr">Rejected</th><th class="tr">Reject %</th><th>Grade</th></tr></thead>
              <tbody>
                <tr v-for="q in quality" :key="q.line" class="tbl-row">
                  <td>{{ q.line }}</td>
                  <td class="tr">{{ q.output.toLocaleString() }}</td>
                  <td class="tr c-green">{{ q.passed.toLocaleString() }}</td>
                  <td class="tr c-red">{{ q.rejected }}</td>
                  <td class="tr"><b :class="q.rejectPct <= 1 ? 'c-green' : 'c-red'">{{ q.rejectPct }}%</b></td>
                  <td><span class="badge" :class="gradeCls(q.grade)">{{ q.grade }}</span></td>
                </tr>
                <tr class="total-row">
                  <td><b>Total</b></td>
                  <td class="tr"><b>{{ totalOutput.toLocaleString() }}</b></td>
                  <td class="tr c-green"><b>{{ totalPassed.toLocaleString() }}</b></td>
                  <td class="tr c-red"><b>{{ totalRejected }}</b></td>
                  <td class="tr"><b :class="overallRejectPct <= 1 ? 'c-green' : 'c-red'">{{ overallRejectPct }}%</b></td>
                  <td></td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>

        <!-- OEE Mini Cards -->
        <div class="pc" style="margin-top:8px">
          <div class="ph">OEE Breakdown (Today)</div>
          <div class="oee-cards">
            <div class="oee-card" v-for="o in oeeCards" :key="o.label">
              <div class="oee-gauge">
                <svg viewBox="0 0 80 50" class="oee-svg">
                  <path :d="arcTrack" stroke="#e0e0e0" stroke-width="7" fill="none" stroke-linecap="round"/>
                  <path :d="arcPath(o.val)" :stroke="o.color" stroke-width="7" fill="none" stroke-linecap="round"/>
                </svg>
                <div class="oee-val" :style="{ color: o.color }">{{ o.val }}%</div>
              </div>
              <div class="oee-lbl">{{ o.label }}</div>
            </div>
          </div>
        </div>

      </div>
    </div>

  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onBeforeUnmount, watch } from 'vue'

const period     = ref('Today')
const lineFilter = ref('All')
const periods    = ['Today', 'This Week', 'This Month']
const chartCanvas = ref<HTMLCanvasElement | null>(null)
let trendTimer: ReturnType<typeof setInterval> | null = null

// KPI data per period
const kpiData = {
  'Today':      { output: 6840,  target: 8000,  oee: 78.2, reject: 0.8, downtime: 4.5 },
  'This Week':  { output: 34200, target: 40000, oee: 81.4, reject: 0.6, downtime: 14.2 },
  'This Month': { output: 142600,target: 160000,oee: 79.8, reject: 0.7, downtime: 52.8 },
}

const kpis = computed(() => {
  const d = kpiData[period.value as keyof typeof kpiData]
  const ach = Math.round(d.output / d.target * 100)
  return [
    { label: 'Total Output',    val: d.output.toLocaleString() + ' pcs', cls: 'c-blue',  sub: undefined,         subCls: '' },
    { label: 'Target',          val: d.target.toLocaleString() + ' pcs', cls: 'c-grey',  sub: undefined,         subCls: '' },
    { label: 'Achievement',     val: ach + '%',         cls: ach >= 90 ? 'c-green' : ach >= 75 ? 'c-amber' : 'c-red', sub: ach >= 90 ? '▲ On track' : '▼ Below target', subCls: ach >= 90 ? 'sub-green' : 'sub-red' },
    { label: 'OEE',             val: d.oee + '%',       cls: d.oee >= 85 ? 'c-green' : d.oee >= 70 ? 'c-amber' : 'c-red', sub: undefined, subCls: '' },
    { label: 'Reject Rate',     val: d.reject + '%',    cls: d.reject <= 1 ? 'c-green' : 'c-red', sub: undefined, subCls: '' },
    { label: 'Downtime (h)',    val: d.downtime,        cls: 'c-red',   sub: undefined,         subCls: '' },
  ]
})

const downtimes = [
  { id:1, category:'Equipment Failure', catCls:'cat-equip', equipment:'FFS Machine 04',  hours:2.5, cause:'E-42 jaw overheat — seal bar replaced', resolved:true,  resCls:'b-green' },
  { id:2, category:'Material Shortage', catCls:'cat-mat',   equipment:'Line 2',          hours:1.2, cause:'Palm Olein drum empty — resupply delayed', resolved:true, resCls:'b-green' },
  { id:3, category:'Changeover',        catCls:'cat-chg',   equipment:'Line 1',          hours:0.8, cause:'Product changeover WO-041 → WO-044',      resolved:true, resCls:'b-green' },
  { id:4, category:'Quality Hold',      catCls:'cat-qc',    equipment:'Batch Plant 01',  hours:3.0, cause:'Palm Olein FFA level out of spec — pending re-inspection', resolved:false, resCls:'b-red' },
]
const totalDowntime = computed(() => downtimes.reduce((a, d) => a + d.hours, 0).toFixed(1))

const defects = [
  { type:'Fill Weight Deviation', units:82, pct:42 },
  { type:'Seal Defect',           units:54, pct:28 },
  { type:'Label Misalignment',    units:32, pct:16 },
  { type:'Wrong Batch Code',      units:18, pct: 9 },
  { type:'Other',                 units:10, pct: 5 },
]
const totalDefects = defects.reduce((a, d) => a + d.units, 0)
const paretoColors = ['#e53935','#ef9a9a','#f9a825','#1565c0','#9e9e9e']
function paretoColor(i: number) { return paretoColors[i] ?? '#9e9e9e' }

const quality = [
  { line:'Line 1',        output:3600, passed:3568, rejected:32, rejectPct:0.9, grade:'A' },
  { line:'Line 2',        output:1800, passed:1800, rejected:0,  rejectPct:0.0, grade:'A+'},
  { line:'Batch Plant 01',output:600,  passed:564,  rejected:36, rejectPct:6.0, grade:'C' },
  { line:'Batch Plant 02',output:840,  passed:824,  rejected:16, rejectPct:1.9, grade:'B' },
]
const totalOutput   = computed(() => quality.reduce((a, q) => a + q.output, 0))
const totalPassed   = computed(() => quality.reduce((a, q) => a + q.passed, 0))
const totalRejected = computed(() => quality.reduce((a, q) => a + q.rejected, 0))
const overallRejectPct = computed(() => +(totalRejected.value / totalOutput.value * 100).toFixed(1))

function gradeCls(g: string) {
  return { 'b-green': g==='A+' || g==='A', 'b-blue': g==='B', 'b-amber': g==='C', 'b-red': g==='D' }
}

// OEE mini SVG arcs
const CX = 40, CY = 45, R = 30, START = 210, SPAN = 120
function polar(deg: number) {
  const r = deg * Math.PI / 180
  return { x: CX + R * Math.cos(r), y: CY + R * Math.sin(r) }
}
function arcPath(val: number): string {
  const end = START + (val / 100) * SPAN
  const s = polar(START), e = polar(end)
  const big = end - START > 180 ? 1 : 0
  return `M ${s.x.toFixed(1)} ${s.y.toFixed(1)} A ${R} ${R} 0 ${big} 1 ${e.x.toFixed(1)} ${e.y.toFixed(1)}`
}
const arcTrack = arcPath(100)

const oeeCards = [
  { label:'Availability', val:91.2, color:'#388E3C' },
  { label:'Performance',  val:83.7, color:'#1565c0' },
  { label:'Quality',      val:99.1, color:'#7b1fa2' },
  { label:'OEE',          val:78.2, color:'#f9a825' },
]

// Canvas bar chart
const barData = ref([
  { day:'Mon', output:7200, target:8000 },
  { day:'Tue', output:7800, target:8000 },
  { day:'Wed', output:6500, target:8000 },
  { day:'Thu', output:8100, target:8000 },
  { day:'Fri', output:7600, target:8000 },
  { day:'Sat', output:5800, target:8000 },
  { day:'Sun', output:6840, target:8000 },
])

function drawChart() {
  const canvas = chartCanvas.value
  if (!canvas) return
  const ctx = canvas.getContext('2d')
  if (!ctx) return
  const W = canvas.width, H = canvas.height
  const PAD = { t:20, r:16, b:32, l:50 }
  const cW = W - PAD.l - PAD.r, cH = H - PAD.t - PAD.b
  const maxVal = Math.max(...barData.value.map(d => Math.max(d.output, d.target))) * 1.1

  ctx.clearRect(0, 0, W, H)

  const n = barData.value.length
  const barW = Math.floor(cW / n * 0.55)
  const gap   = cW / n

  // Grid lines
  ctx.strokeStyle = '#e0e0e0'
  ctx.lineWidth = 1
  for (let i = 0; i <= 4; i++) {
    const y = PAD.t + cH - (i / 4) * cH
    ctx.beginPath(); ctx.moveTo(PAD.l, y); ctx.lineTo(PAD.l + cW, y); ctx.stroke()
    ctx.fillStyle = '#9e9e9e'
    ctx.font = '9px Poppins,sans-serif'
    ctx.textAlign = 'right'
    ctx.fillText(String(Math.round(maxVal * i / 4)), PAD.l - 4, y + 3)
  }

  barData.value.forEach((d, i) => {
    const x = PAD.l + i * gap + gap / 2 - barW / 2
    const barH = (d.output / maxVal) * cH
    const y = PAD.t + cH - barH

    // Bar
    ctx.fillStyle = '#1565c0'
    ctx.fillRect(x, y, barW, barH)

    // Value label
    ctx.fillStyle = '#333'
    ctx.font = '9px Poppins,sans-serif'
    ctx.textAlign = 'center'
    ctx.fillText(String(Math.round(d.output / 100) * 100), x + barW / 2, y - 4)

    // Day label
    ctx.fillStyle = '#757575'
    ctx.fillText(d.day, x + barW / 2, PAD.t + cH + 14)
  })

  // Target line
  const tY = PAD.t + cH - (barData.value[0].target / maxVal) * cH
  ctx.strokeStyle = '#e53935'
  ctx.lineWidth = 1.5
  ctx.setLineDash([5, 4])
  ctx.beginPath()
  ctx.moveTo(PAD.l, tY); ctx.lineTo(PAD.l + cW, tY)
  ctx.stroke()
  ctx.setLineDash([])
  ctx.fillStyle = '#e53935'
  ctx.font = '9px Poppins,sans-serif'
  ctx.textAlign = 'left'
  ctx.fillText('Target', PAD.l + cW + 2, tY + 3)
}

function resizeCanvas() {
  const canvas = chartCanvas.value
  if (!canvas) return
  const parent = canvas.parentElement
  if (!parent) return
  canvas.width  = parent.clientWidth
  canvas.height = parent.clientHeight
  drawChart()
}

onMounted(() => {
  requestAnimationFrame(resizeCanvas)
  trendTimer = setInterval(() => {
    barData.value = barData.value.map((d, i) => ({
      ...d,
      output: i < 6 ? d.output : Math.max(4000, Math.min(8400, d.output + (Math.random() * 200 - 100)))
    }))
    drawChart()
  }, 2000)
})

onBeforeUnmount(() => { if (trendTimer) clearInterval(trendTimer) })
watch(period, () => requestAnimationFrame(drawChart))
</script>

<style scoped>
*, *::before, *::after { box-sizing: border-box; }
.root { height: 100%; display: flex; flex-direction: column; font-family: 'Poppins', sans-serif; font-size: 12px; background: #f5f5f5; overflow: hidden; }

.topbar { background: #fff; border-bottom: 1px solid #c3c6d4; padding: 8px 14px; display: flex; align-items: center; gap: 10px; flex-shrink: 0; flex-wrap: wrap; }
.pg-title { font-size: 12px; font-weight: 700; text-transform: uppercase; color: #515151; flex-shrink: 0; }
.filters  { display: flex; align-items: center; gap: 6px; flex-wrap: wrap; margin-left: auto; }
.sf-btn { background: #f5f5f5; border: 1px solid #d0d3e0; border-radius: 3px; color: #515151; cursor: pointer; font-family: 'Poppins', sans-serif; font-size: 10px; padding: 4px 10px; }
.sf-btn:hover { background: #e3f2fd; border-color: #1565c0; color: #1565c0; }
.sf-btn--active { background: #1565c0; border-color: #1565c0; color: #fff; }
.sel { border: 1px solid #d0d3e0; border-radius: 3px; font-family: 'Poppins', sans-serif; font-size: 11px; padding: 4px 8px; color: #515151; background: #fff; }
.btn-outline-sm { background: #fff; border: 1px solid #d0d3e0; border-radius: 3px; color: #515151; cursor: pointer; font-family: 'Poppins', sans-serif; font-size: 11px; gap: 5px; display: flex; align-items: center; padding: 5px 10px; }
.btn-outline-sm:hover { background: #f5f5f5; }

.kpi-strip { background: #c3c6d4; display: flex; gap: 1px; flex-shrink: 0; }
.kc { background: #fff; flex: 1; padding: 8px 12px; }
.kc-val { font-size: 18px; font-weight: 700; line-height: 1.1; }
.kc-lbl { font-size: 10px; color: #757575; margin-top: 2px; }
.kc-sub { font-size: 10px; margin-top: 1px; font-weight: 600; }
.sub-green { color: #388E3C; } .sub-red { color: #e53935; }
.c-blue   { color: #1565c0; } .c-green { color: #388E3C; }
.c-amber  { color: #f9a825; } .c-red   { color: #e53935; }
.c-grey   { color: #757575; }

.content { flex: 1; display: flex; gap: 8px; overflow: hidden; padding: 8px; }
.col-left  { flex: 3; display: flex; flex-direction: column; overflow: hidden; }
.col-right { flex: 2; display: flex; flex-direction: column; overflow: hidden; }

.pc { background: #fff; border: 1px solid #c3c6d4; border-radius: 3px; display: flex; flex-direction: column; overflow: hidden; }
.ph { background: linear-gradient(0deg,#d7d7d7,#fff); border-bottom: 1px solid #c3c6d4; font-size: 11px; font-weight: 700; padding: 6px 10px; flex-shrink: 0; display: flex; justify-content: space-between; }
.ph-r { color: #9e9e9e; font-weight: 400; }

.chart-area   { flex: 1; position: relative; min-height: 140px; }
.chart-canvas { display: block; height: 100%; width: 100%; }
.chart-legend { border-top: 1px solid #f0f0f0; display: flex; gap: 14px; padding: 5px 10px; flex-shrink: 0; }
.leg-item { align-items: center; display: flex; font-size: 10px; gap: 5px; color: #515151; }
.leg-dot  { height: 10px; width: 14px; border-radius: 2px; }

.tbl-wrap { flex: 1; overflow: auto; }
.tbl { border-collapse: collapse; width: 100%; }
.tbl thead th { background: #f5f5f5; border-bottom: 1px solid #c3c6d4; color: #515151; font-size: 10px; font-weight: 600; padding: 6px 8px; position: sticky; text-align: left; text-transform: uppercase; top: 0; white-space: nowrap; }
.tbl tbody td { border-bottom: 1px solid #f0f0f0; color: #333; font-size: 11px; padding: 6px 8px; }
.tbl-row:hover { background: #f5f5f5; }
.total-row td { background: #f5f5f5; font-size: 11px; padding: 6px 8px; border-top: 1px solid #c3c6d4; }
.tr { text-align: right; }
.cause-cell { color: #757575; font-size: 10px; max-width: 160px; }

.cat-tag { border-radius: 3px; font-size: 10px; font-weight: 600; padding: 2px 6px; white-space: nowrap; }
.cat-equip { background: #ffebee; color: #e53935; }
.cat-mat   { background: #fff3e0; color: #f9a825; }
.cat-chg   { background: #e3f2fd; color: #1565c0; }
.cat-qc    { background: #f3e5f5; color: #7b1fa2; }

.badge { border-radius: 3px; font-size: 10px; font-weight: 600; padding: 2px 8px; }
.b-green { background: #e8f5e9; color: #388E3C; }
.b-red   { background: #ffebee; color: #e53935; }
.b-blue  { background: #e3f2fd; color: #1565c0; }
.b-amber { background: #fff8e1; color: #f9a825; }

.pareto-list { display: flex; flex-direction: column; gap: 6px; flex: 1; overflow-y: auto; padding: 8px 10px; }
.pareto-row  { align-items: center; display: flex; gap: 8px; }
.par-rank    { background: #f5f5f5; border-radius: 50%; color: #515151; flex-shrink: 0; font-size: 10px; font-weight: 700; height: 20px; line-height: 20px; text-align: center; width: 20px; }
.par-info    { flex: 1; }
.par-type    { color: #333; font-size: 11px; margin-bottom: 3px; }
.par-bar-wrap{ background: #f0f0f0; border-radius: 3px; height: 8px; overflow: hidden; }
.par-bar     { border-radius: 3px; height: 100%; transition: width .4s; }
.par-nums    { display: flex; flex-direction: column; align-items: flex-end; gap: 1px; flex-shrink: 0; width: 44px; }
.par-val     { color: #333; font-size: 11px; font-weight: 700; }
.par-pct     { color: #9e9e9e; font-size: 10px; }

.oee-cards { display: flex; gap: 1px; background: #c3c6d4; flex-shrink: 0; }
.oee-card  { background: #fff; flex: 1; display: flex; flex-direction: column; align-items: center; padding: 6px 4px; }
.oee-gauge { position: relative; width: 80px; height: 50px; }
.oee-svg   { display: block; width: 80px; height: 50px; }
.oee-val   { position: absolute; bottom: 2px; left: 50%; transform: translateX(-50%); font-size: 11px; font-weight: 700; white-space: nowrap; }
.oee-lbl   { font-size: 10px; color: #757575; text-align: center; margin-top: 2px; }
</style>
