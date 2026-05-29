<template>
  <div class="oee-root">

    <!-- Line tabs -->
    <div class="page-tabs">
      <button class="page-tab" :class="{ active: line === 'ffs'   }" @click="switchLine('ffs')">&#9654; FFS Packaging</button>
      <button class="page-tab" :class="{ active: line === 'batch' }" @click="switchLine('batch')">&#9654; Batching Plant</button>
    </div>

    <!-- KPI Banner -->
    <div class="kpi-row">
      <div class="kpi-card kpi-main">
        <div class="kpi-label">OEE</div>
        <div class="kpi-value kpi-amber">{{ oee.toFixed(1) }}<span class="kpi-unit">%</span></div>
        <div class="kpi-sub">Overall Equipment Effectiveness</div>
      </div>
      <div class="kpi-card">
        <div class="kpi-label">Availability</div>
        <div class="kpi-value kpi-green">{{ avlCur.toFixed(1) }}<span class="kpi-unit">%</span></div>
        <div class="kpi-sub">Run Time ÷ Planned Time</div>
      </div>
      <div class="kpi-card">
        <div class="kpi-label">Performance</div>
        <div class="kpi-value kpi-blue">{{ perfCur.toFixed(1) }}<span class="kpi-unit">%</span></div>
        <div class="kpi-sub">Actual Rate ÷ Ideal Rate</div>
      </div>
      <div class="kpi-card">
        <div class="kpi-label">Quality</div>
        <div class="kpi-value kpi-orange">{{ qualCur.toFixed(1) }}<span class="kpi-unit">%</span></div>
        <div class="kpi-sub">Good Count ÷ Total Count</div>
      </div>
    </div>

    <!-- Main 3-column -->
    <div class="main-area">

      <!-- LEFT — Output Actual -->
      <div class="left-panel">
        <div class="lp-section-title">OUTPUT — ACTUAL</div>
        <div class="lp-output-box">
          <div class="lp-actual-val">{{ packCount.toLocaleString() }}</div>
          <div class="lp-actual-label">{{ line === 'ffs' ? 'packs produced' : 'batches completed' }}</div>
          <div class="lp-target-row">
            <span>Target</span>
            <span>{{ currentTarget.toLocaleString() }}</span>
          </div>
          <div class="lp-bar-track">
            <div class="lp-bar-fill" :style="{ width: outputPct + '%' }"></div>
          </div>
          <div class="lp-bar-pct">{{ outputPct.toFixed(1) }}%</div>
        </div>
        <div class="lp-counts-box">
          <div class="lp-count-row good"><span class="lp-count-icon">&#10003;</span><span class="lp-count-label">Good</span><span class="lp-count-val">{{ goodCount.toLocaleString() }}</span></div>
          <div class="lp-count-row reject"><span class="lp-count-icon">&#10007;</span><span class="lp-count-label">Reject</span><span class="lp-count-val">{{ rejectCount }}</span></div>
          <div class="lp-count-row rework"><span class="lp-count-icon">&#8635;</span><span class="lp-count-label">Rework</span><span class="lp-count-val">0</span></div>
        </div>
        <div class="lp-shift-box">
          <div class="lp-shift-title">SHIFT</div>
          <div class="lp-shift-row"><span>Schedule</span><span>Morning</span></div>
          <div class="lp-shift-row"><span>Hours</span><span>06:00 – 14:00</span></div>
          <div class="lp-shift-row"><span>Elapsed</span><span>4h 23m</span></div>
          <div class="lp-shift-row"><span>Remaining</span><span>3h 37m</span></div>
          <div class="lp-shift-row"><span>{{ line === 'ffs' ? 'Line Speed' : 'Batch Rate' }}</span><span>{{ lineSpeed }} {{ line === 'ffs' ? 'p/min' : 'bat/h' }}</span></div>
        </div>
      </div>

      <!-- IMAGE PANEL -->
      <div class="image-panel">
        <div class="ip-title">{{ line === 'ffs' ? 'FFS PACKAGING LINE' : 'BATCHING PLANT' }}</div>
        <!-- FFS image area -->
        <div class="image-area" v-show="line === 'ffs'">
          <div class="side-text">Over the last 30 years OPEN Automation Software has been at the forefront of industrial IoT connectivity and data visualization solutions.</div>
          <img src="/ffs-packaging-line.jpg" alt="FFS Packaging Line" style="width:100%;height:100%;object-fit:cover;display:block;" />
          <div class="sensor-pin" style="top:32%;left:22%;transform:translate(-50%,-50%)">
            <div class="sensor-dot" :class="dotFfs[0] ? 'dot-green' : 'dot-red'"></div>
            <div class="sensor-label"><span class="sl-name">Fill Weight</span><span class="sl-val">{{ pins.ffs[0] }}</span><span class="sl-unit"> g</span></div>
          </div>
          <div class="sensor-pin" style="top:68%;left:38%;transform:translate(-50%,-50%)">
            <div class="sensor-dot" :class="dotFfs[1] ? 'dot-green' : 'dot-red'"></div>
            <div class="sensor-label"><span class="sl-name">Seal Temp (H-jaw)</span><span class="sl-val">{{ pins.ffs[1] }}</span><span class="sl-unit"> °C</span></div>
          </div>
          <div class="sensor-pin" style="top:40%;left:62%;transform:translate(-50%,-50%)">
            <div class="sensor-dot" :class="dotFfs[2] ? 'dot-green' : 'dot-red'"></div>
            <div class="sensor-label"><span class="sl-name">Screw Feed Rate</span><span class="sl-val">{{ pins.ffs[2] }}</span><span class="sl-unit"> kg/h</span></div>
          </div>
          <div class="sensor-pin" style="top:30%;left:82%;transform:translate(-50%,-50%)">
            <div class="sensor-dot" :class="dotFfs[3] ? 'dot-green' : 'dot-red'"></div>
            <div class="sensor-label"><span class="sl-name">Motor Voltage</span><span class="sl-val">{{ pins.ffs[3] }}</span><span class="sl-unit"> V</span></div>
          </div>
        </div>
        <!-- Batch image area -->
        <div class="image-area" v-show="line === 'batch'">
          <img src="/batching-plant-outdoor.png" alt="Batching Plant" style="width:100%;height:100%;object-fit:cover;display:block;" />
          <div class="sensor-pin" style="top:72%;left:22%;transform:translate(-50%,-50%)">
            <div class="sensor-dot" :class="dotBatch[0] ? 'dot-green' : 'dot-red'"></div>
            <div class="sensor-label"><span class="sl-name">Aggregate Hopper</span><span class="sl-val">{{ pins.batch[0] }}</span><span class="sl-unit">% full</span></div>
          </div>
          <div class="sensor-pin" style="top:52%;left:35%;transform:translate(-50%,-50%)">
            <div class="sensor-dot" :class="dotBatch[1] ? 'dot-green' : 'dot-red'"></div>
            <div class="sensor-label"><span class="sl-name">Belt Speed</span><span class="sl-val">{{ pins.batch[1] }}</span><span class="sl-unit"> m/s</span></div>
          </div>
          <div class="sensor-pin" style="top:38%;left:60%;transform:translate(-50%,-50%)">
            <div class="sensor-dot" :class="dotBatch[2] ? 'dot-green' : 'dot-red'"></div>
            <div class="sensor-label"><span class="sl-name">Mixer Drum Fill</span><span class="sl-val">{{ pins.batch[2] }}</span><span class="sl-unit">% full</span></div>
          </div>
          <div class="sensor-pin" style="top:28%;left:76%;transform:translate(-50%,-50%)">
            <div class="sensor-dot" :class="dotBatch[3] ? 'dot-green' : 'dot-red'"></div>
            <div class="sensor-label"><span class="sl-name">Water Flow</span><span class="sl-val">{{ pins.batch[3] }}</span><span class="sl-unit"> m³/h</span></div>
          </div>
        </div>
      </div>

      <!-- CENTER — Gauges + Trend -->
      <div class="center-panel">
        <div class="gauges-row">

          <!-- Availability gauge -->
          <div class="oee-gauge-card">
            <div class="ogc-title">Availability</div>
            <svg class="ogc-svg" viewBox="0 0 160 135">
              <path :d="avlTrackPath" fill="none" stroke="#e0e0e0" stroke-width="13" stroke-linecap="round"/>
              <path :d="avlArcPath"   fill="none" stroke="#43a047" stroke-width="13" stroke-linecap="round"/>
              <text x="29"  y="126" font-size="9" fill="#aaa" text-anchor="middle">0</text>
              <text x="13"  y="64"  font-size="9" fill="#aaa" text-anchor="middle">20</text>
              <text x="47"  y="11"  font-size="9" fill="#aaa" text-anchor="middle">40</text>
              <text x="113" y="11"  font-size="9" fill="#aaa" text-anchor="middle">60</text>
              <text x="151" y="64"  font-size="9" fill="#aaa" text-anchor="middle">80</text>
              <text x="131" y="126" font-size="9" fill="#aaa" text-anchor="middle">100</text>
            </svg>
            <div class="ogc-readout">{{ avlCur.toFixed(1) }}<span class="ogc-pct">%</span></div>
            <div class="ogc-metrics">
              <div class="ogc-metric"><span class="ogc-ml">Run Time</span><span class="ogc-mv">7h 02m</span></div>
              <div class="ogc-metric"><span class="ogc-ml">Planned DT</span><span class="ogc-mv">25 min</span></div>
              <div class="ogc-metric"><span class="ogc-ml">Unplanned DT</span><span class="ogc-mv ogc-red">13 min</span></div>
            </div>
          </div>

          <!-- Performance gauge -->
          <div class="oee-gauge-card ogc-bordered">
            <div class="ogc-title">Performance</div>
            <svg class="ogc-svg" viewBox="0 0 160 135">
              <path :d="perfTrackPath" fill="none" stroke="#e0e0e0" stroke-width="13" stroke-linecap="round"/>
              <path :d="perfArcPath"   fill="none" stroke="#1565c0" stroke-width="13" stroke-linecap="round"/>
              <text x="29"  y="126" font-size="9" fill="#aaa" text-anchor="middle">0</text>
              <text x="13"  y="64"  font-size="9" fill="#aaa" text-anchor="middle">20</text>
              <text x="47"  y="11"  font-size="9" fill="#aaa" text-anchor="middle">40</text>
              <text x="113" y="11"  font-size="9" fill="#aaa" text-anchor="middle">60</text>
              <text x="151" y="64"  font-size="9" fill="#aaa" text-anchor="middle">80</text>
              <text x="131" y="126" font-size="9" fill="#aaa" text-anchor="middle">100</text>
            </svg>
            <div class="ogc-readout">{{ perfCur.toFixed(1) }}<span class="ogc-pct">%</span></div>
            <div class="ogc-metrics">
              <div class="ogc-metric"><span class="ogc-ml">Ideal Rate</span><span class="ogc-mv">70 p/min</span></div>
              <div class="ogc-metric"><span class="ogc-ml">Actual Rate</span><span class="ogc-mv">{{ lineSpeed }} p/min</span></div>
              <div class="ogc-metric"><span class="ogc-ml">Minor Stops</span><span class="ogc-mv ogc-red">{{ minorStops }}</span></div>
            </div>
          </div>

          <!-- Quality gauge -->
          <div class="oee-gauge-card">
            <div class="ogc-title">Quality</div>
            <svg class="ogc-svg" viewBox="0 0 160 135">
              <path :d="qualTrackPath" fill="none" stroke="#e0e0e0" stroke-width="13" stroke-linecap="round"/>
              <path :d="qualArcPath"   fill="none" stroke="#f57c00" stroke-width="13" stroke-linecap="round"/>
              <text x="29"  y="126" font-size="9" fill="#aaa" text-anchor="middle">0</text>
              <text x="13"  y="64"  font-size="9" fill="#aaa" text-anchor="middle">20</text>
              <text x="47"  y="11"  font-size="9" fill="#aaa" text-anchor="middle">40</text>
              <text x="113" y="11"  font-size="9" fill="#aaa" text-anchor="middle">60</text>
              <text x="151" y="64"  font-size="9" fill="#aaa" text-anchor="middle">80</text>
              <text x="131" y="126" font-size="9" fill="#aaa" text-anchor="middle">100</text>
            </svg>
            <div class="ogc-readout">{{ qualCur.toFixed(1) }}<span class="ogc-pct">%</span></div>
            <div class="ogc-metrics">
              <div class="ogc-metric"><span class="ogc-ml">Total Count</span><span class="ogc-mv">{{ packCount.toLocaleString() }}</span></div>
              <div class="ogc-metric"><span class="ogc-ml">Good Count</span><span class="ogc-mv ogc-green">{{ goodCount.toLocaleString() }}</span></div>
              <div class="ogc-metric"><span class="ogc-ml">Defects</span><span class="ogc-mv ogc-red">{{ rejectCount }}</span></div>
            </div>
          </div>

        </div>

        <!-- OEE Trend -->
        <div class="trend-box">
          <div class="trend-top">
            <div class="legend">
              <div class="leg-item"><div class="leg-line" style="background:#1565c0"></div><span>OEE</span></div>
              <div class="leg-item"><div class="leg-line" style="background:#43a047"></div><span>AVAIL</span></div>
              <div class="leg-item"><div class="leg-line" style="background:#e91e63"></div><span>PERF</span></div>
              <div class="leg-item"><div class="leg-line" style="background:#f57c00"></div><span>QUAL</span></div>
            </div>
          </div>
          <div class="trend-canvas-wrap">
            <canvas ref="trendCanvas"></canvas>
          </div>
        </div>
      </div>

    </div>

    <!-- Alarm table -->
    <div class="alarm-section">
      <div class="alarm-toolbar">
        <input class="filter-alarms" type="text" v-model="alarmFilter" placeholder="filter alarms" />
      </div>
      <div class="at-wrap">
        <table class="at">
          <thead>
            <tr>
              <th>&#9873; Alarm Id</th>
              <th>&#128337; Alarm Rate &#9660;</th>
              <th>&#128200; Value</th>
              <th>&#9873; Text</th>
              <th>&#8597; Type</th>
              <th>&#9711; Group</th>
              <th>&#9711; Priority</th>
              <th>&#9873; Ack Date</th>
              <th>&#9873; Cleared Date</th>
              <th>&#9873; Cleared Value</th>
              <th>&#9873; Network Node</th>
              <th>&#9873; Units</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="alarm in filteredAlarms" :key="alarm.id" :class="alarmRowClass(alarm.color)">
              <td>{{ alarm.id }}</td>
              <td :class="alarmTsClass(alarm.color)">{{ alarm.timestamp }}</td>
              <td>{{ alarm.value }}</td>
              <td :class="alarm.color === 'red' || alarm.color === 'blue' ? 'at-bold' : ''">{{ alarm.text }}</td>
              <td>{{ alarm.type === 'High' ? '&#8679; High' : '&#8681; Low' }}</td>
              <td>{{ alarm.group }}</td>
              <td>{{ alarm.priority }}</td>
              <td>{{ alarm.ackDate }}</td>
              <td>{{ alarm.clearedDate }}</td>
              <td>{{ alarm.clearedVal }}</td>
              <td>{{ alarm.node }}</td>
              <td>{{ alarm.unit }}</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted, onBeforeUnmount } from 'vue'
import { mqttSubscribe, mqttUnsubscribe } from './src/mqttService'

type Line = 'ffs' | 'batch'

const line = ref<Line>('ffs')

// ── Arc gauge math ──────────────────────────────────────────────
const CX = 80, CY = 75, R = 59
const START_DEG = 135, TOTAL_DEG = 270, MIN_ARC = 4

function polarXY(deg: number) {
  const r = deg * Math.PI / 180
  return { x: CX + R * Math.cos(r), y: CY + R * Math.sin(r) }
}
function arcPath(startDeg: number, endDeg: number): string {
  const s = polarXY(startDeg), e = polarXY(endDeg)
  const big = (endDeg - startDeg > 180) ? 1 : 0
  return `M ${s.x.toFixed(2)} ${s.y.toFixed(2)} A ${R} ${R} 0 ${big} 1 ${e.x.toFixed(2)} ${e.y.toFixed(2)}`
}

const TRACK_PATH = arcPath(START_DEG, START_DEG + TOTAL_DEG)
const avlTrackPath  = TRACK_PATH
const perfTrackPath = TRACK_PATH
const qualTrackPath = TRACK_PATH

// ── Gauge state ─────────────────────────────────────────────────
const avlTarget  = ref(91.2)
const perfTarget = ref(83.7)
const qualTarget = ref(98.4)
const avlCur  = ref(91.2)
const perfCur = ref(83.7)
const qualCur = ref(98.4)

const oee = computed(() => (avlCur.value / 100) * (perfCur.value / 100) * (qualCur.value / 100) * 100)

function gaugePath(val: number): string {
  const pct  = Math.max(0, Math.min(100, val)) / 100
  const span = Math.max(MIN_ARC, pct * TOTAL_DEG)
  return arcPath(START_DEG, START_DEG + span)
}
const avlArcPath  = computed(() => gaugePath(avlCur.value))
const perfArcPath = computed(() => gaugePath(perfCur.value))
const qualArcPath = computed(() => gaugePath(qualCur.value))

// ── Output state ────────────────────────────────────────────────
const packCount    = ref(18640)
const rejectCount  = ref(12)
const goodCount    = computed(() => packCount.value - rejectCount.value)
const currentTarget = ref(22000)
const outputPct    = computed(() => Math.min(100, (packCount.value / currentTarget.value) * 100))
const lineSpeed    = ref(62)
const minorStops   = ref(3)

// ── Sensor pins ─────────────────────────────────────────────────
const dotFfs   = reactive([true, true, true, true])
const dotBatch = reactive([true, true, true, true])
const pins = reactive({
  ffs:   ['250.4', '182.5', '48.2', '415.1'],
  batch: ['72.0', '1.45', '47.3', '12.4'],
})

// ── Alarm table ─────────────────────────────────────────────────
const ALARM_COLORS = ['red', 'blue', 'white'] as const
type AlarmColor = typeof ALARM_COLORS[number]

interface Alarm {
  id: string; timestamp: string; value: string; text: string
  type: string; group: string; priority: number; ackDate: string
  clearedDate: string; clearedVal: string; node: string; unit: string
  color: AlarmColor
}

const ffsAlarms: Alarm[] = [
  { id: 'Seal.Temp.Horizontal_Hi', timestamp: '2026-03-26 09:08:52 am', value: '198.4', text: 'Horizontal Seal Temp Over Limit',  type: 'High', group: 'Sealing',  priority: 1, ackDate: '',                      clearedDate: '',                      clearedVal: '190.0', node: 'packaging-line-01', unit: '°C',  color: 'red'   },
  { id: 'Fill.Weight.Deviation_Hi', timestamp: '2026-03-26 09:07:31 am', value: '3.8',  text: 'Fill Weight Out of Tolerance',    type: 'High', group: 'Dosing',   priority: 1, ackDate: '',                      clearedDate: '',                      clearedVal: '2.0',   node: 'packaging-line-01', unit: 'g',   color: 'red'   },
  { id: 'Film.Roll.Remaining_Lo',   timestamp: '2026-03-26 09:04:18 am', value: '95.0', text: 'Film Roll Running Low',           type: 'Low',  group: 'Film',     priority: 2, ackDate: '2026-03-26 09:05:00 am', clearedDate: '2026-03-26 09:06:30 am', clearedVal: '100.0', node: 'packaging-line-01', unit: 'm',   color: 'white' },
  { id: 'Reject.Rate_Hi',           timestamp: '2026-03-26 09:02:45 am', value: '4.2',  text: 'Pack Reject Rate High',          type: 'High', group: 'Quality',  priority: 2, ackDate: '2026-03-26 09:03:10 am', clearedDate: '2026-03-26 09:04:50 am', clearedVal: '2.0',   node: 'packaging-line-01', unit: '%',   color: 'blue'  },
  { id: 'Screw.Feed.Rate_Lo',       timestamp: '2026-03-26 09:00:12 am', value: '28.5', text: 'Screw Conveyor Feed Rate Low',   type: 'Low',  group: 'Conveyor', priority: 2, ackDate: '2026-03-26 09:01:00 am', clearedDate: '2026-03-26 09:02:20 am', clearedVal: '35.0',  node: 'packaging-line-01', unit: '%',   color: 'blue'  },
]

const batchAlarms: Alarm[] = [
  { id: 'Aggregate.Hopper.Level_Lo', timestamp: '2026-03-26 08:09:42 am', value: '18.2', text: 'Aggregate Hopper Low Level',     type: 'Low',  group: 'Hopper',   priority: 1, ackDate: '', clearedDate: '', clearedVal: '25.0', node: 'batching-plant-02', unit: '%',    color: 'red'   },
  { id: 'Mixer.RPM_Hi',              timestamp: '2026-03-26 08:08:55 am', value: '28.6', text: 'Mixer Over-Speed Alarm',         type: 'High', group: 'Mixer',    priority: 1, ackDate: '', clearedDate: '', clearedVal: '20.0', node: 'batching-plant-02', unit: 'rpm',  color: 'red'   },
  { id: 'Cement.Silo.Level_Lo',      timestamp: '2026-03-26 08:05:11 am', value: '22.4', text: 'Cement Silo Low Level Warning', type: 'Low',  group: 'Silo',     priority: 2, ackDate: '2026-03-26 08:06:00 am', clearedDate: '', clearedVal: '30.0', node: 'batching-plant-02', unit: '%',    color: 'white' },
  { id: 'Belt.Feed.Rate_Lo',         timestamp: '2026-03-26 08:03:28 am', value: '31.0', text: 'Belt Feed Rate Low',            type: 'Low',  group: 'Conveyor', priority: 2, ackDate: '2026-03-26 08:04:00 am', clearedDate: '2026-03-26 08:05:30 am', clearedVal: '40.0', node: 'batching-plant-02', unit: 't/h',  color: 'blue'  },
  { id: 'Mixer.Motor.Current_Hi',    timestamp: '2026-03-26 08:01:04 am', value: '52.1', text: 'Mixer Motor Overcurrent',       type: 'High', group: 'Mixer',    priority: 1, ackDate: '2026-03-26 08:02:00 am', clearedDate: '2026-03-26 08:03:10 am', clearedVal: '40.0', node: 'batching-plant-02', unit: 'A',    color: 'blue'  },
]

const activeAlarms = ref<Alarm[]>(ffsAlarms.map(a => ({ ...a })))
const alarmFilter  = ref('')

const filteredAlarms = computed(() => {
  const q = alarmFilter.value.toLowerCase()
  if (!q) return activeAlarms.value
  return activeAlarms.value.filter(a =>
    a.id.toLowerCase().includes(q) || a.text.toLowerCase().includes(q) || a.group.toLowerCase().includes(q)
  )
})

function alarmRowClass(color: AlarmColor) {
  return { 'at-red': color === 'red', 'at-blue': color === 'blue', 'at-white': color === 'white' }
}
function alarmTsClass(color: AlarmColor) {
  return color === 'red' ? 'at-ts-red' : color === 'blue' ? 'at-ts-blue' : 'at-ts-white'
}

// ── Trend canvas ─────────────────────────────────────────────────
const trendCanvas = ref<HTMLCanvasElement | null>(null)
const POINTS = 300

const oeeData  = Array.from({ length: POINTS }, () => 0.73 + Math.random() * 0.04)
const avlData  = Array.from({ length: POINTS }, () => 0.89 + Math.random() * 0.04)
const perfData = Array.from({ length: POINTS }, () => 0.80 + Math.random() * 0.07)
const qualData = Array.from({ length: POINTS }, () => 0.97 + Math.random() * 0.025)

function resizeCanvas() {
  const c = trendCanvas.value
  if (!c) return
  const wrap = c.parentElement
  if (!wrap) return
  c.width  = wrap.clientWidth
  c.height = wrap.clientHeight
}

function drawChart() {
  const c = trendCanvas.value
  if (!c) return
  const ctx = c.getContext('2d')
  if (!ctx) return
  const W = c.width, H = c.height
  ctx.clearRect(0, 0, W, H)
  ctx.fillStyle = '#fff'
  ctx.fillRect(0, 0, W, H)
  ctx.strokeStyle = '#e0e0e0'; ctx.lineWidth = 1
  ;[0.25, 0.5, 0.75].forEach(y => {
    ctx.beginPath(); ctx.moveTo(0, y * H); ctx.lineTo(W, y * H); ctx.stroke()
  })
  const step = W / (POINTS - 1)
  function drawLine(data: number[], color: string, width: number, alpha: number) {
    ctx.beginPath()
    ctx.strokeStyle = color; ctx.lineWidth = width; ctx.globalAlpha = alpha
    data.forEach((v, i) => {
      const x = i * step, y = (1 - v) * H * 0.9 + H * 0.05
      i === 0 ? ctx.moveTo(x, y) : ctx.lineTo(x, y)
    })
    ctx.stroke(); ctx.globalAlpha = 1
  }
  drawLine(oeeData,  '#1565c0', 2.5, 0.9)
  drawLine(avlData,  '#43a047', 1.5, 0.7)
  drawLine(perfData, '#e91e63', 1.5, 0.7)
  drawLine(qualData, '#f57c00', 1.5, 0.7)
}

// ── Animation frame ──────────────────────────────────────────────
let rafId: number | null = null

function gaugeLoop() {
  avlCur.value  += (avlTarget.value  - avlCur.value)  * 0.1
  perfCur.value += (perfTarget.value - perfCur.value) * 0.1
  qualCur.value += (qualTarget.value - qualCur.value) * 0.1
  rafId = requestAnimationFrame(gaugeLoop)
}

// ── Intervals ───────────────────────────────────────────────────
let dotTimer:    ReturnType<typeof setInterval> | null = null
let sensorTimer: ReturnType<typeof setInterval> | null = null
let trendTimer:  ReturnType<typeof setInterval> | null = null
let alarmTimer:  ReturnType<typeof setInterval> | null = null

function rnd(base: number, delta: number, dec: number) {
  const v = base + (Math.random() * 2 - 1) * delta
  return dec > 0 ? v.toFixed(dec) : String(Math.round(v))
}

function startIntervals() {
  dotTimer = setInterval(() => {
    if (Math.random() < 0.2) dotFfs[Math.floor(Math.random() * 4)]   = !dotFfs[Math.floor(Math.random() * 4)]
    if (Math.random() < 0.2) dotBatch[Math.floor(Math.random() * 4)] = !dotBatch[Math.floor(Math.random() * 4)]
  }, 700)

  sensorTimer = setInterval(() => {
    const speed = Math.round(56 + Math.random() * 12)
    lineSpeed.value = speed
    packCount.value += Math.round(Math.random() * 3)
    if (Math.random() < 0.04) rejectCount.value++
    avlTarget.value  = Math.max(85, Math.min(96, avlTarget.value  + (Math.random() - 0.5) * 0.4))
    perfTarget.value = (speed / 70) * 100
    qualTarget.value = ((packCount.value - rejectCount.value) / packCount.value) * 100
    if (Math.random() < 0.04) minorStops.value++

    if (line.value === 'ffs') {
      pins.ffs[0] = (248 + Math.random() * 5).toFixed(1)
      pins.ffs[1] = (180 + Math.random() * 6).toFixed(1)
      pins.ffs[2] = (44  + Math.random() * 10).toFixed(1)
      pins.ffs[3] = (413 + Math.random() * 4).toFixed(1)
    } else {
      pins.batch[0] = (62  + Math.random() * 12).toFixed(1)
      pins.batch[1] = (1.2 + Math.random() * 0.6).toFixed(2)
      pins.batch[2] = (42  + Math.random() * 10).toFixed(1)
      pins.batch[3] = (10  + Math.random() * 5).toFixed(1)
    }
  }, 600)

  trendTimer = setInterval(() => {
    const curOee = (avlCur.value / 100) * (perfCur.value / 100) * (qualCur.value / 100)
    oeeData.shift();  oeeData.push(Math.max(0.5, Math.min(1, curOee)))
    avlData.shift();  avlData.push(Math.max(0.5, Math.min(1, avlCur.value  / 100)))
    perfData.shift(); perfData.push(Math.max(0.5, Math.min(1, perfCur.value / 100)))
    qualData.shift(); qualData.push(Math.max(0.5, Math.min(1, qualCur.value / 100)))
    drawChart()
  }, 40)

  alarmTimer = setInterval(() => {
    const alarms = activeAlarms.value
    const n = 1 + Math.floor(Math.random() * 2)
    for (let i = 0; i < n; i++) {
      const idx = Math.floor(Math.random() * alarms.length)
      alarms[idx].color = ALARM_COLORS[Math.floor(Math.random() * ALARM_COLORS.length)]
    }
  }, 500)
}

function switchLine(l: Line) {
  line.value = l
  if (l === 'batch') {
    currentTarget.value = 18000
    avlTarget.value  = 88.5; perfTarget.value = 79.2; qualTarget.value = 97.1
    packCount.value  = 14280; rejectCount.value = 24; minorStops.value = 5
    activeAlarms.value = batchAlarms.map(a => ({ ...a }))
  } else {
    currentTarget.value = 22000
    avlTarget.value  = 91.2; perfTarget.value = 83.7; qualTarget.value = 98.4
    packCount.value  = 18640; rejectCount.value = 12; minorStops.value = 3
    activeAlarms.value = ffsAlarms.map(a => ({ ...a }))
  }
}

// ── MQTT handlers ───────────────────────────────────────────────
function onFfsAvl(v: string)   { avlTarget.value  = parseFloat(v) }
function onFfsPerf(v: string)  { perfTarget.value = parseFloat(v) }
function onFfsQual(v: string)  { qualTarget.value = parseFloat(v) }
function onFfsPack(v: string)  { packCount.value  = parseInt(v) }
function onFfsSpeed(v: string) { lineSpeed.value  = parseInt(v) }
function onBatchAvl(v: string)  { if (line.value === 'batch') avlTarget.value  = parseFloat(v) }
function onBatchPerf(v: string) { if (line.value === 'batch') perfTarget.value = parseFloat(v) }
function onBatchQual(v: string) { if (line.value === 'batch') qualTarget.value = parseFloat(v) }

onMounted(() => {
  resizeCanvas()
  window.addEventListener('resize', resizeCanvas)
  gaugeLoop()
  startIntervals()
  // MQTT subscriptions — override timer values when broker data arrives
  mqttSubscribe('hias/oee/ffs/availability', onFfsAvl)
  mqttSubscribe('hias/oee/ffs/performance',  onFfsPerf)
  mqttSubscribe('hias/oee/ffs/quality',      onFfsQual)
  mqttSubscribe('hias/oee/ffs/packcount',    onFfsPack)
  mqttSubscribe('hias/oee/ffs/linespeed',    onFfsSpeed)
  mqttSubscribe('hias/oee/batch/availability', onBatchAvl)
  mqttSubscribe('hias/oee/batch/performance',  onBatchPerf)
  mqttSubscribe('hias/oee/batch/quality',      onBatchQual)
})

onBeforeUnmount(() => {
  if (rafId !== null) cancelAnimationFrame(rafId)
  if (dotTimer)    clearInterval(dotTimer)
  if (sensorTimer) clearInterval(sensorTimer)
  if (trendTimer)  clearInterval(trendTimer)
  if (alarmTimer)  clearInterval(alarmTimer)
  window.removeEventListener('resize', resizeCanvas)
  mqttUnsubscribe('hias/oee/ffs/availability', onFfsAvl)
  mqttUnsubscribe('hias/oee/ffs/performance',  onFfsPerf)
  mqttUnsubscribe('hias/oee/ffs/quality',      onFfsQual)
  mqttUnsubscribe('hias/oee/ffs/packcount',    onFfsPack)
  mqttUnsubscribe('hias/oee/ffs/linespeed',    onFfsSpeed)
  mqttUnsubscribe('hias/oee/batch/availability', onBatchAvl)
  mqttUnsubscribe('hias/oee/batch/performance',  onBatchPerf)
  mqttUnsubscribe('hias/oee/batch/quality',      onBatchQual)
})
</script>

<style scoped>
*, *::before, *::after { box-sizing: border-box; margin: 0; padding: 0; }

.oee-root { height: 100%; overflow: hidden; font-family: 'Poppins', sans-serif; font-size: 12px; background: #f5f5f5; color: #333; display: flex; flex-direction: column; }

/* Page tabs */
.page-tabs { display: flex; gap: 0; flex-shrink: 0; background: #ffffff; border-bottom: 2px solid #c3c6d4; }
.page-tab  { padding: 6px 22px; font-size: 12px; font-weight: 600; color: #7f7f7f; background: #fff; border: none; cursor: pointer; border-right: 1px solid #e0e0e0; letter-spacing: 0.4px; transition: background .15s, color .15s; }
.page-tab:hover  { background: #f2f2f2; color: #333; }
.page-tab.active { background: #fff; color: #333; border-bottom: 2px solid #1565c0; margin-bottom: -2px; }

/* KPI Banner */
.kpi-row  { display: flex; gap: 1px; flex-shrink: 0; background: #c3c6d4; border-bottom: 2px solid #c3c6d4; }
.kpi-card { flex: 1; background: #ffffff; padding: 4px 12px; display: flex; flex-direction: column; gap: 0; }
.kpi-main { flex: 1.3; border-right: 2px solid #c3c6d4; }
.kpi-label { font-size: 9px; color: #9e9e9e; text-transform: uppercase; letter-spacing: 0.5px; }
.kpi-value { font-size: 20px; font-weight: 700; line-height: 1.15; }
.kpi-unit  { font-size: 11px; font-weight: 400; color: #b0b0b0; }
.kpi-sub   { font-size: 9px; color: #b0b0b0; margin-top: 1px; }
.kpi-green  { color: #388E3C; } .kpi-blue { color: #1565c0; } .kpi-amber { color: #f9a825; } .kpi-orange { color: #f57c00; }

/* Main layout */
.main-area { display: flex; flex: 1; overflow: hidden; min-height: 0; }

/* Left panel */
.left-panel { width: 145px; flex-shrink: 0; background: #f8f8f8; border-right: 1px solid #c3c6d4; display: flex; flex-direction: column; overflow: hidden; }
.lp-section-title { text-align: center; font-weight: 700; font-size: 11px; text-transform: uppercase; letter-spacing: 0.5px; color: #515151; background: linear-gradient(0deg, #d7d7d7 0%, #ffffff 100%); border-bottom: 1px solid #c3c6d4; padding: 5px 4px; flex-shrink: 0; }
.lp-output-box { margin: 8px 8px 4px; padding: 8px 8px 6px; background: #fff; border: 1px solid #c3c6d4; display: flex; flex-direction: column; align-items: center; flex-shrink: 0; }
.lp-actual-val { font-size: 26px; font-weight: 700; color: #333; line-height: 1; text-align: center; }
.lp-actual-label { font-size: 10px; color: #7f7f7f; margin-top: 2px; }
.lp-target-row { width: 100%; display: flex; justify-content: space-between; font-size: 10px; color: #515151; margin-top: 6px; }
.lp-bar-track  { width: 100%; height: 8px; background: #e0e0e0; border-radius: 4px; margin-top: 4px; overflow: hidden; }
.lp-bar-fill   { height: 100%; background: #1565c0; border-radius: 4px; transition: width 0.5s ease; }
.lp-bar-pct    { font-size: 10px; color: #7f7f7f; margin-top: 2px; align-self: flex-end; }
.lp-counts-box { margin: 0 8px 4px; padding: 4px 8px; background: #fff; border: 1px solid #c3c6d4; flex-shrink: 0; }
.lp-count-row  { display: flex; align-items: center; gap: 6px; padding: 4px 0; border-bottom: 1px solid #f0f0f0; font-size: 12px; }
.lp-count-row:last-child { border-bottom: none; }
.lp-count-icon  { font-size: 13px; min-width: 14px; text-align: center; }
.lp-count-label { flex: 1; color: #515151; font-size: 11px; }
.lp-count-val   { font-weight: 700; font-size: 12px; color: #333; }
.lp-count-row.good   .lp-count-icon { color: #43a047; }
.lp-count-row.reject .lp-count-icon { color: #e53935; }
.lp-count-row.rework .lp-count-icon { color: #1565c0; }
.lp-shift-box   { margin: 0 8px 8px; padding: 6px 8px; background: #fff; border: 1px solid #c3c6d4; flex: 1; overflow: hidden; }
.lp-shift-title { font-size: 10px; font-weight: 700; text-transform: uppercase; letter-spacing: 0.5px; color: #515151; margin-bottom: 4px; }
.lp-shift-row   { display: flex; justify-content: space-between; font-size: 10px; color: #333; padding: 3px 0; border-bottom: 1px solid #f5f5f5; }
.lp-shift-row:last-child { border-bottom: none; }
.lp-shift-row span:first-child { color: #7f7f7f; }

/* Image panel */
.image-panel { flex: 1; display: flex; flex-direction: column; border-right: 1px solid #c3c6d4; overflow: hidden; }
.ip-title { background: linear-gradient(0deg, #d7d7d7 0%, #ffffff 100%); border-bottom: 1px solid #c3c6d4; padding: 5px 10px; font-size: 11px; font-weight: 700; color: #515151; letter-spacing: 0.4px; text-align: center; flex-shrink: 0; }
.image-area { flex: 1; background: #e8e8e8; position: relative; overflow: hidden; }
.side-text  { position: absolute; left: 0; top: 0; bottom: 0; writing-mode: vertical-rl; transform: rotate(180deg); font-size: 9px; color: #bbb; padding: 8px 2px; background: #f0f0f0; border-right: 1px solid #e0e0e0; line-height: 1.3; overflow: hidden; z-index: 1; }

/* Sensor pins */
.sensor-pin { position: absolute; display: flex; align-items: center; gap: 6px; cursor: default; }
.sensor-dot { width: 14px; height: 14px; border-radius: 50%; border: 2px solid #555; flex-shrink: 0; }
.sensor-dot.dot-green { background: #43a047; animation: pulse-green 1.6s ease-out infinite; }
.sensor-dot.dot-red   { background: #e53935; animation: pulse-red  1.6s ease-out infinite; }
@keyframes pulse-green { 0%{ box-shadow: 0 0 0 0 rgba(67,160,71,.85); } 70%{ box-shadow: 0 0 0 9px rgba(67,160,71,0); } 100%{ box-shadow: 0 0 0 0 rgba(67,160,71,0); } }
@keyframes pulse-red   { 0%{ box-shadow: 0 0 0 0 rgba(229,57,53,.85); } 70%{ box-shadow: 0 0 0 9px rgba(229,57,53,0); } 100%{ box-shadow: 0 0 0 0 rgba(229,57,53,0); } }
.sensor-label { background: rgba(255,255,255,.95); color: #333; font-size: 10px; padding: 3px 7px; border-radius: 4px; white-space: nowrap; border: 1px solid rgba(0,0,0,.15); line-height: 1.5; pointer-events: none; box-shadow: 0 1px 4px rgba(0,0,0,.12); }
.sensor-label .sl-name { font-weight: 700; font-size: 9px; color: #1565c0; text-transform: uppercase; letter-spacing: 0.5px; display: block; }
.sensor-label .sl-val  { font-size: 11px; color: #333; }
.sensor-label .sl-unit { font-size: 9px; color: #7f7f7f; }

/* Center panel */
.center-panel { flex: 1; display: flex; flex-direction: column; background: #f5f5f5; overflow: hidden; }
.gauges-row   { display: flex; flex: 1; min-height: 0; background: #c3c6d4; gap: 1px; }
.oee-gauge-card { flex: 1; background: #fff; display: flex; flex-direction: column; align-items: center; padding: 8px 6px 6px; overflow: hidden; }
.oee-gauge-card.ogc-bordered { border-left: 1px solid #c3c6d4; border-right: 1px solid #c3c6d4; }
.ogc-title   { font-size: 11px; font-weight: 700; text-transform: uppercase; letter-spacing: 0.5px; color: #515151; margin-bottom: 2px; text-align: center; }
.ogc-svg     { width: 90%; max-width: 170px; height: auto; }
.ogc-readout { font-size: 22px; font-weight: 700; color: #333; text-align: center; margin-top: 1px; letter-spacing: 0.5px; }
.ogc-pct     { font-size: 12px; font-weight: 400; color: #9e9e9e; }
.ogc-metrics { width: 100%; margin-top: 4px; }
.ogc-metric  { display: flex; justify-content: space-between; align-items: center; padding: 2px 6px; border-bottom: 1px solid #f5f5f5; font-size: 10px; }
.ogc-metric:last-child { border-bottom: none; }
.ogc-ml      { color: #9e9e9e; } .ogc-mv { font-weight: 600; color: #333; }
.ogc-green   { color: #388E3C !important; } .ogc-red { color: #e53935 !important; }

/* Trend box */
.trend-box { height: 110px; flex-shrink: 0; display: flex; flex-direction: column; border-top: 1px solid #c3c6d4; background: #fff; overflow: hidden; }
.trend-top { display: flex; justify-content: space-between; align-items: center; padding: 5px 10px; background: #f8f8f8; border-bottom: 1px solid #e0e0e0; flex-shrink: 0; }
.legend    { display: flex; gap: 6px; }
.leg-item  { display: flex; align-items: center; gap: 5px; font-size: 11px; color: #333; font-weight: 700; letter-spacing: 0.5px; }
.leg-line  { width: 36px; height: 16px; border-radius: 2px; }
.trend-canvas-wrap { flex: 1; overflow: hidden; position: relative; }
.trend-canvas-wrap canvas { width: 100%; height: 100%; display: block; }

/* Alarm section */
.alarm-section { height: 100px; flex-shrink: 0; background: #fff; color: #333; display: flex; flex-direction: column; border-top: 2px solid #c3c6d4; overflow: hidden; }
.alarm-toolbar { display: flex; justify-content: flex-end; align-items: center; padding: 3px 8px; background: #f0f0f0; border-bottom: 1px solid #c3c6d4; flex-shrink: 0; }
.filter-alarms { height: 22px; border: 1px solid #c3c6d4; border-radius: 3px; padding: 0 8px; font-size: 11px; outline: none; width: 150px; background: #fff; color: #333; font-family: 'Poppins', sans-serif; }
.at-wrap { flex: 1; overflow-y: auto; }
table.at { width: 100%; border-collapse: collapse; font-size: 11px; }
table.at th { background: linear-gradient(0deg, #d7d7d7 0%, #ffffff 100%); color: #515151; padding: 3px 5px; text-align: left; font-weight: 600; border-bottom: 1px solid #c3c6d4; position: sticky; top: 0; z-index: 1; white-space: nowrap; font-size: 10px; }
table.at td { padding: 3px 5px; white-space: nowrap; border-bottom: 1px solid rgba(0,0,0,.06); color: #333; }
.at-red   td { background: #e53935 !important; color: #fff; }
.at-blue  td { background: #1e88e5 !important; color: #fff; }
.at-white td { background: #fff    !important; color: #333; }
.at-ts-red   { color: #ffcdd2 !important; }
.at-ts-blue  { color: #bbdefb !important; }
.at-ts-white { color: #c62828; font-weight: 600; }
.at-bold { font-weight: bold; }
</style>
