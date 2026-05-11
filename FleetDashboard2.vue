<template>
  <div class="fd2-root">

    <!-- Page tabs -->
    <div class="page-tabs">
      <button class="page-tab" @click="goFfs">&#9654; FFS Packaging</button>
      <button class="page-tab active">&#9654; Batching Plant</button>
    </div>

    <!-- Sliders -->
    <div class="sliders-bar">
      <div class="slider-row" style="margin-top:8px">
        <span class="slider-end">0</span>
        <div class="slider-wrap">
          <div class="slider-bubble" :style="{ left: slider1 + '%' }">{{ slider1.toFixed(3) }}</div>
          <input type="range" min="0" max="100" v-model.number="slider1" class="slider-green" />
        </div>
        <span class="slider-end">100</span>
      </div>
      <div class="slider-row" style="margin-top:4px">
        <span class="slider-end">0</span>
        <div class="slider-wrap">
          <div class="slider-bubble" :style="{ left: slider2 + '%', background: '#2196f3' }">{{ slider2 }}</div>
          <input type="range" min="0" max="100" v-model.number="slider2" class="slider-blue" />
        </div>
        <span class="slider-end">100</span>
      </div>
    </div>

    <!-- 3-column main area -->
    <div class="main-area">

      <!-- LEFT -->
      <div class="left-panel">
        <div class="alarm-states-box">
          <div class="alarm-states-title">ALARM STATES</div>
          <div class="towers-row">
            <div v-html="machine1Svg"></div>
            <div style="margin-left:6px" v-html="machine2Svg"></div>
          </div>
        </div>

        <div class="gauge-box">
          <svg id="gaugeSvg" viewBox="0 0 160 135">
            <path ref="gaugeTrackRef" fill="none" stroke="#333" stroke-width="13" stroke-linecap="round" />
            <path ref="gaugeArcRef"   fill="none" stroke="#29b6f6" stroke-width="13" stroke-linecap="round" />
            <text x="29"  y="126" font-size="9" fill="#888" text-anchor="middle">0</text>
            <text x="13"  y="64"  font-size="9" fill="#888" text-anchor="middle">20</text>
            <text x="47"  y="11"  font-size="9" fill="#888" text-anchor="middle">40</text>
            <text x="113" y="11"  font-size="9" fill="#888" text-anchor="middle">60</text>
            <text x="151" y="64"  font-size="9" fill="#888" text-anchor="middle">80</text>
            <text x="131" y="126" font-size="9" fill="#888" text-anchor="middle">100</text>
          </svg>
          <div class="gauge-readout">{{ Math.round(gaugeCur) }} batch/hr</div>
          <div class="gauge-sub">Mixer Load</div>
        </div>

        <div class="mixing-box">
          <div class="mixing-inner">
            <div class="mix-scale"><span>100</span><span>80</span><span>60</span><span>40</span><span>20</span><span>0</span></div>
            <div class="mix-bar-col">
              <div class="mix-track">
                <div class="mix-fill" :style="{ height: mixPct + '%' }"></div>
              </div>
            </div>
            <div class="mix-vert-label">{{ mixLabel }}</div>
          </div>
          <div class="mix-title">Water Tank</div>
        </div>
      </div>

      <!-- CENTER -->
      <div class="center-panel">
        <div class="image-area">
          <div class="side-text">Over the last 30 years OPEN Automation Software has been at the forefront of industrial IoT connectivity and data visualization solutions.</div>
          <img src="/batching-plant-outdoor.png" alt="Concrete Batching Plant" style="width:100%;height:100%;object-fit:cover;display:block;" />
          <div class="sensor-pin" style="top:72%;left:22%;transform:translate(-50%,-50%)">
            <div class="sensor-dot" :style="{ background: dotColors[0] }"></div>
            <div class="sensor-label">
              <span class="sl-name">Aggregate Hopper</span>
              <span class="sl-val">{{ live.aggLvl }}</span><span class="sl-unit"> % full</span>
            </div>
          </div>
          <div class="sensor-pin" style="top:55%;left:32%;transform:translate(-50%,-50%)">
            <div class="sensor-dot" :style="{ background: dotColors[1] }"></div>
            <div class="sensor-label">
              <span class="sl-name">Belt Conveyor</span>
              <span class="sl-val">{{ live.belt }}</span><span class="sl-unit"> t/h</span>
            </div>
          </div>
          <div class="sensor-pin" style="top:38%;left:57%;transform:translate(-50%,-50%)">
            <div class="sensor-dot" :style="{ background: dotColors[2] }"></div>
            <div class="sensor-label">
              <span class="sl-name">Mixer RPM</span>
              <span class="sl-val">{{ live.rpm }}</span><span class="sl-unit"> rpm</span>
            </div>
          </div>
          <div class="sensor-pin" style="top:22%;left:74%;transform:translate(-50%,-50%)">
            <div class="sensor-dot" :style="{ background: dotColors[3] }"></div>
            <div class="sensor-label">
              <span class="sl-name">Cement Silo</span>
              <span class="sl-val">{{ live.cemLvl }}</span><span class="sl-unit"> % full</span>
            </div>
          </div>
        </div>
      </div>

      <!-- RIGHT -->
      <div class="right-panel">
        <div class="rp-topbar">Mixer RPM: <span>{{ live.rpm }}</span> rpm</div>

        <div class="outflow-header">
          <span class="outflow-title">&#127991; PLANT OUTPUT</span>
          <span class="outflow-gph">{{ live.batchCount }} batches/day</span>
          <button class="outflow-x">&#x2715;</button>
        </div>

        <div class="data-section">
          <div class="intake-bar">
            <div class="intake-gph">{{ live.belt }}<br>t/h</div>
            <div class="intake-label">FEED RATE</div>
          </div>
          <div class="dt-wrap">
            <table class="dt">
              <thead>
                <tr>
                  <th>&#127991; Tag</th><th></th>
                  <th>&#128337; Timestamp</th>
                  <th>&#128200; Value</th>
                  <th style="color:#aaa;font-weight:400">Unit</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="row in tableRows" :key="row.tag">
                  <td class="dt-tag">{{ row.tag }}</td>
                  <td><span class="dt-chk">&#10003;</span></td>
                  <td class="dt-ts">{{ row.ts }}</td>
                  <td>{{ row.value }}</td>
                  <td style="color:#7a8a92">{{ row.unit }}</td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>

        <div class="trend-box">
          <div class="trend-top">
            <div class="legend">
              <div class="leg-item"><div class="leg-line" style="background:#e91e63"></div><span>RAMP</span></div>
              <div class="leg-item"><div class="leg-line" style="background:#4caf50"></div><span>RANDOM</span></div>
              <div class="leg-item"><div class="leg-line" style="background:#29b6f6"></div><span>SINE</span></div>
            </div>
            <div class="tbtn-row">
              <button class="tbtn">&#9646;&#9646;</button>
              <button class="tbtn">&#8862;</button>
            </div>
          </div>
          <div class="trend-canvas-wrap">
            <canvas ref="trendCanvasRef"></canvas>
          </div>
        </div>
      </div>
    </div>

    <!-- Bottom alarm table -->
    <div class="alarm-section">
      <div class="alarm-toolbar">
        <input class="filter-alarms" type="text" placeholder="filter alarms" v-model="alarmFilter" />
      </div>
      <div class="at-wrap">
        <table class="at">
          <thead>
            <tr>
              <th>&#9873; Alarm Id</th><th>&#128337; Alarm Rate &#9660;</th>
              <th>&#128200; Value</th><th>&#9873; Text</th><th>&#8597; Type</th>
              <th>&#9711; Group</th><th>&#9711; Priority</th><th>&#9873; Ack Date</th>
              <th>&#9873; Cleared Date</th><th>&#9873; Cleared Value</th>
              <th>&#9873; Network Node</th><th>&#9873; Units</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="(row, i) in filteredAlarms" :key="row.id" :class="alarmRowClass(i)">
              <td>{{ row.id }}</td>
              <td :class="alarmTsClass(i)">{{ row.timestamp }}</td>
              <td>{{ row.value }}</td>
              <td :class="row.bold ? 'at-bold' : ''">{{ row.text }}</td>
              <td>{{ row.type }}</td>
              <td>{{ row.group }}</td>
              <td>{{ row.priority }}</td>
              <td>{{ row.ackDate }}</td>
              <td>{{ row.clearedDate }}</td>
              <td>{{ row.clearedValue }}</td>
              <td>{{ row.node }}</td>
              <td>{{ row.unit }}</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onBeforeUnmount } from 'vue'

// ── Tabs ────────────────────────────────────────────────────
function goFfs() {
  /* In a full app this would be a router.push('/fleet-dashboard') */
  console.log('navigate to FFS Packaging')
}

// ── Sliders ─────────────────────────────────────────────────
const slider1 = ref(76.830)
const slider2 = ref(100)

// ── Live data ────────────────────────────────────────────────
const live = ref({
  aggLvl:     '68.5',
  cemLvl:     '83.0',
  belt:        '53.6',
  rpm:         '18.4',
  water:       '142.0',
  batchWt:     '2450',
  admix:       '4.8',
  motorA:      '38.2',
  motorV:      '415.2',
  convA:       '12.7',
  batchCount:  47,
  cementUsed:  18420,
  aggUsed:     52180,
  waterUsed:   8640,
  energy:      5832.1,
})

const tableRows = computed(() => {
  const l = live.value
  return [
    { tag: 'Aggregate.Hopper.Level',   ts: '2026-03-26 08:10:14 am', value: l.aggLvl,            unit: '%'      },
    { tag: 'Cement.Silo.Level',        ts: '2026-03-26 08:10:15 am', value: l.cemLvl,            unit: '%'      },
    { tag: 'Belt.Feed.Rate',           ts: '2026-03-26 08:10:16 am', value: l.belt,              unit: 't/h'    },
    { tag: 'Mixer.RPM',                ts: '2026-03-26 08:10:16 am', value: l.rpm,               unit: 'rpm'    },
    { tag: 'Water.Flow.Rate',          ts: '2026-03-26 08:10:17 am', value: l.water,             unit: 'L/min'  },
    { tag: 'Batch.Weight',             ts: '2026-03-26 08:10:17 am', value: l.batchWt,           unit: 'kg'     },
    { tag: 'Admixture.Flow',           ts: '2026-03-26 08:10:18 am', value: l.admix,             unit: 'L/min'  },
    { tag: 'Mixer.Motor.Current',      ts: '2026-03-26 08:10:18 am', value: l.motorA,            unit: 'A'      },
    { tag: 'Mixer.Motor.Voltage',      ts: '2026-03-26 08:10:19 am', value: l.motorV,            unit: 'V'      },
    { tag: 'Conveyor.Motor.Current',   ts: '2026-03-26 08:10:19 am', value: l.convA,             unit: 'A'      },
    { tag: 'Batch.Count.Today',        ts: '2026-03-26 08:10:20 am', value: l.batchCount,        unit: 'batches'},
    { tag: 'Cement.Consumed.Today',    ts: '2026-03-26 08:10:20 am', value: Math.round(l.cementUsed), unit: 'kg' },
    { tag: 'Aggregate.Consumed.Today', ts: '2026-03-26 08:10:21 am', value: Math.round(l.aggUsed),    unit: 'kg' },
    { tag: 'Water.Consumed.Today',     ts: '2026-03-26 08:10:21 am', value: Math.round(l.waterUsed),  unit: 'L'  },
    { tag: 'Energy.Meter',             ts: '2026-03-26 08:10:22 am', value: l.energy.toFixed(1), unit: 'kWh'    },
    { tag: 'Alarm.Count.Active',       ts: '2026-03-26 08:10:22 am', value: '3',                 unit: '—'      },
    { tag: 'Plant.Status',             ts: '2026-03-26 08:10:23 am', value: `Running — Batch #${l.batchCount} in progress`, unit: '—' },
  ]
})

// ── Alarm table ──────────────────────────────────────────────
const alarmColors = ref(['red','red','white','blue','blue','white'])
const alarmFilter = ref('')

const alarms = [
  { id: 'Aggregate.Hopper.Level_Lo', timestamp: '2026-03-26 08:09:42 am', value: '18.2', text: 'Aggregate Hopper Low Level',      type: '&#8681; Low',  group: 'Hopper',   priority: 1, ackDate: '',                        clearedDate: '',                        clearedValue: '20.0', node: 'batching-plant-02', unit: '%',     bold: true  },
  { id: 'Mixer.RPM_Hi',              timestamp: '2026-03-26 08:08:55 am', value: '28.6', text: 'Mixer Over-Speed Alarm',           type: '&#8679; High', group: 'Mixer',    priority: 1, ackDate: '',                        clearedDate: '',                        clearedValue: '25.0', node: 'batching-plant-02', unit: 'rpm',   bold: true  },
  { id: 'Cement.Silo.Level_Lo',      timestamp: '2026-03-26 08:05:11 am', value: '22.4', text: 'Cement Silo Low Level Warning',    type: '&#8681; Low',  group: 'Silo',     priority: 2, ackDate: '2026-03-26 08:06:00 am',  clearedDate: '2026-03-26 08:07:30 am',  clearedValue: '25.0', node: 'batching-plant-02', unit: '%',     bold: false },
  { id: 'Belt.Feed.Rate_Lo',         timestamp: '2026-03-26 08:03:28 am', value: '31.0', text: 'Belt Feed Rate Low',               type: '&#8681; Low',  group: 'Conveyor', priority: 2, ackDate: '2026-03-26 08:04:10 am',  clearedDate: '2026-03-26 08:05:45 am',  clearedValue: '35.0', node: 'batching-plant-02', unit: 't/h',   bold: false },
  { id: 'Mixer.Motor.Current_Hi',    timestamp: '2026-03-26 08:01:04 am', value: '52.1', text: 'Mixer Motor Overcurrent',          type: '&#8679; High', group: 'Mixer',    priority: 1, ackDate: '2026-03-26 08:02:00 am',  clearedDate: '2026-03-26 08:03:15 am',  clearedValue: '45.0', node: 'batching-plant-02', unit: 'A',     bold: false },
  { id: 'Water.Flow.Rate_Lo',        timestamp: '2026-03-26 07:58:33 am', value: '98.5', text: 'Water Flow Rate Below Setpoint',   type: '&#8681; Low',  group: 'Mixer',    priority: 2, ackDate: '2026-03-26 07:59:20 am',  clearedDate: '2026-03-26 08:00:10 am',  clearedValue: '120.0',node: 'batching-plant-02', unit: 'L/min', bold: false },
]

const filteredAlarms = computed(() => {
  const q = alarmFilter.value.toLowerCase()
  return q ? alarms.filter(a => (a.id + a.text + a.group).toLowerCase().includes(q)) : alarms
})

function alarmRowClass(i: number): string {
  const c = alarmColors.value[i] ?? 'white'
  return c === 'red' ? 'at-red' : c === 'blue' ? 'at-blue' : 'at-white'
}
function alarmTsClass(i: number): string {
  const c = alarmColors.value[i] ?? 'white'
  return c === 'red' ? 'at-ts-red' : c === 'blue' ? 'at-ts-blue' : 'at-ts-white'
}

// ── SVG Machines ─────────────────────────────────────────────
const dotStates = ref([true, true, true, true])

function dotColor(on: boolean) { return on ? '#43a047' : '#e53935' }

const machine1Svg = computed(() => {
  const [d0, d1] = dotStates.value
  return `<svg width="88" height="70" viewBox="0 0 88 70" xmlns="http://www.w3.org/2000/svg">
    <polygon points="2,28 28,28 26,46 4,46" fill="#575757" stroke="#2a2a2a" stroke-width="1.2"/>
    <polygon points="5,30 25,30 23,44 7,44" fill="#484848" stroke="none" opacity="0.6"/>
    <line x1="4"  y1="46" x2="2"  y2="62" stroke="#444" stroke-width="2"/>
    <line x1="26" y1="46" x2="28" y2="62" stroke="#444" stroke-width="2"/>
    <line x1="8"  y1="52" x2="22" y2="52" stroke="#444" stroke-width="1.5"/>
    <line x1="4"  y1="56" x2="10" y2="46" stroke="#444" stroke-width="1.2"/>
    <line x1="26" y1="56" x2="20" y2="46" stroke="#444" stroke-width="1.2"/>
    <rect x="0"  y="62" width="30" height="3" fill="#333"/>
    <polygon points="18,45 25,42 76,14 69,11" fill="#484848" stroke="#2a2a2a" stroke-width="1"/>
    <line x1="22" y1="44" x2="28" y2="36" stroke="#2a2a2a" stroke-width="0.9"/>
    <line x1="31" y1="39" x2="37" y2="31" stroke="#2a2a2a" stroke-width="0.9"/>
    <line x1="41" y1="33" x2="47" y2="25" stroke="#2a2a2a" stroke-width="0.9"/>
    <line x1="51" y1="28" x2="57" y2="20" stroke="#2a2a2a" stroke-width="0.9"/>
    <line x1="62" y1="22" x2="68" y2="14" stroke="#2a2a2a" stroke-width="0.9"/>
    <rect x="66" y="4"  width="20" height="26" fill="#575757" stroke="#2a2a2a" stroke-width="1.2" rx="1"/>
    <rect x="68" y="6"  width="16" height="10" fill="#5b7fa6" stroke="#3a5a7a" stroke-width="0.5" opacity="0.85" rx="0.5"/>
    <rect x="64" y="30" width="24" height="10" fill="#484848" stroke="#2a2a2a" stroke-width="1" rx="1"/>
    <rect x="67" y="40" width="4"  height="20" fill="#3a3a3a"/>
    <rect x="77" y="40" width="4"  height="20" fill="#3a3a3a"/>
    <rect x="63" y="58" width="24" height="3"  fill="#333"/>
    <circle cx="15" cy="36" r="5" fill="${dotColor(d0)}" stroke="#111" stroke-width="1"/>
    <circle cx="75" cy="16" r="5" fill="${dotColor(d1)}" stroke="#111" stroke-width="1"/>
  </svg>`
})

const machine2Svg = computed(() => {
  const [,, d2, d3] = dotStates.value
  return `<svg width="72" height="70" viewBox="0 0 72 70" xmlns="http://www.w3.org/2000/svg">
    <rect x="4" y="12" width="22" height="46" fill="#575757" stroke="#2a2a2a" stroke-width="1.2" rx="1"/>
    <rect x="6"  y="14" width="18" height="12" fill="#5b7fa6" stroke="#3a5a7a" stroke-width="0.5" opacity="0.85" rx="0.5"/>
    <rect x="6"  y="28" width="18" height="10" fill="#484848" stroke="#2a2a2a" stroke-width="0.5" opacity="0.8" rx="0.5"/>
    <rect x="2" y="50" width="26" height="10" fill="#484848" stroke="#2a2a2a" stroke-width="1" rx="1"/>
    <ellipse cx="15" cy="50" rx="11" ry="3" fill="#3a3a3a" stroke="#2a2a2a" stroke-width="0.8"/>
    <rect x="8" y="40" width="9" height="8" fill="#3a3a3a" rx="0.5"/>
    <circle cx="11" cy="44" r="1.5" fill="#43a047"/>
    <circle cx="14" cy="44" r="1.5" fill="#e8b400"/>
    <rect x="2"  y="60" width="28" height="4" fill="#333" rx="0.5"/>
    <rect x="26" y="22" width="12" height="4" fill="#444" stroke="#333" stroke-width="0.5"/>
    <rect x="38" y="4"  width="20" height="54" fill="#5b7fa6" stroke="#3a5a7a" stroke-width="1.5" rx="2"/>
    <ellipse cx="48" cy="4"  rx="10" ry="4" fill="#4a6e96" stroke="#3a5a7a" stroke-width="1"/>
    <rect x="38" y="18" width="20" height="2" fill="#4a6e96" opacity="0.6"/>
    <rect x="38" y="30" width="20" height="2" fill="#4a6e96" opacity="0.6"/>
    <rect x="38" y="42" width="20" height="2" fill="#4a6e96" opacity="0.6"/>
    <polygon points="40,58 56,58 54,65 42,65" fill="#484848" stroke="#2a2a2a" stroke-width="1"/>
    <rect x="38" y="65" width="4" height="4" fill="#333"/>
    <rect x="54" y="65" width="4" height="4" fill="#333"/>
    <circle cx="15" cy="35" r="5" fill="${dotColor(d2)}" stroke="#111" stroke-width="1"/>
    <circle cx="48" cy="25" r="5" fill="${dotColor(d3)}" stroke="#111" stroke-width="1"/>
  </svg>`
})

const dotColors = computed(() => dotStates.value.map(on => on ? '#43a047' : '#e53935'))

// ── Gauge (SVG arc + RAF) ────────────────────────────────────
const gaugeTrackRef = ref<SVGPathElement | null>(null)
const gaugeArcRef   = ref<SVGPathElement | null>(null)
const gaugeCur      = ref(72)
const mixPct        = ref(47)
const mixLabel      = ref('47.0 m³')

let gaugeTarget = 72
let rafId = 0

const CX = 80, CY = 75, R = 59, START_DEG = 135, TOTAL_DEG = 270

function polarXY(deg: number) {
  const r = deg * Math.PI / 180
  return { x: CX + R * Math.cos(r), y: CY + R * Math.sin(r) }
}
function arcPath(startDeg: number, endDeg: number): string {
  const s = polarXY(startDeg), e = polarXY(endDeg)
  const big = (endDeg - startDeg > 180) ? 1 : 0
  return `M ${s.x.toFixed(2)} ${s.y.toFixed(2)} A ${R} ${R} 0 ${big} 1 ${e.x.toFixed(2)} ${e.y.toFixed(2)}`
}

function gaugeLoop() {
  gaugeCur.value += (gaugeTarget - gaugeCur.value) * 0.12
  const pct  = Math.max(0, Math.min(100, gaugeCur.value)) / 100
  const span = Math.max(4, pct * TOTAL_DEG)
  if (gaugeArcRef.value) gaugeArcRef.value.setAttribute('d', arcPath(START_DEG, START_DEG + span))
  const mp = 20 + (gaugeCur.value / 100) * 65
  mixPct.value  = mp
  mixLabel.value = mp.toFixed(1) + ' m³'
  rafId = requestAnimationFrame(gaugeLoop)
}

// ── Canvas trend chart ────────────────────────────────────────
const trendCanvasRef = ref<HTMLCanvasElement | null>(null)
const POINTS = 300
const sineData   = Array.from({ length: POINTS }, (_, i) => 0.5 + 0.45 * Math.sin(i * 0.08 + 1.2))
const rampData   = Array.from({ length: POINTS }, (_, i) => (i % 80) / 80)
const randomData = Array.from({ length: POINTS }, () => Math.random())
let trendTick = 0

function resizeCanvas() {
  const cv = trendCanvasRef.value
  if (!cv) return
  const wrap = cv.parentElement
  if (wrap) { cv.width = wrap.clientWidth; cv.height = wrap.clientHeight }
}

function drawChart() {
  const cv = trendCanvasRef.value
  if (!cv) return
  const ctx = cv.getContext('2d')!
  const W = cv.width, H = cv.height
  ctx.clearRect(0, 0, W, H)
  ctx.fillStyle = '#fff'; ctx.fillRect(0, 0, W, H)
  ctx.strokeStyle = '#e0e0e0'; ctx.lineWidth = 1
  ;[0.25, 0.5, 0.75].forEach(y => { ctx.beginPath(); ctx.moveTo(0, y * H); ctx.lineTo(W, y * H); ctx.stroke() })
  const step = W / (POINTS - 1)
  function drawLine(data: number[], color: string, width: number, alpha: number) {
    ctx.beginPath(); ctx.strokeStyle = color; ctx.lineWidth = width; ctx.globalAlpha = alpha
    data.forEach((v, i) => {
      const x = i * step, y = (1 - v) * H * 0.9 + H * 0.05
      i === 0 ? ctx.moveTo(x, y) : ctx.lineTo(x, y)
    })
    ctx.stroke(); ctx.globalAlpha = 1
  }
  drawLine(sineData, '#29b6f6', 2, 0.85)
  drawLine(rampData, '#e91e63', 1.5, 0.9)
  drawLine(randomData, '#4caf50', 1, 0.7)
  trendTick++
}

// ── Timers ────────────────────────────────────────────────────
let dotTimer = 0, sensorTimer = 0, trendTimer = 0, alarmTimer = 0, gaugeTargetTimer = 0

onMounted(() => {
  // Init gauge track
  if (gaugeTrackRef.value) gaugeTrackRef.value.setAttribute('d', arcPath(START_DEG, START_DEG + TOTAL_DEG))
  rafId = requestAnimationFrame(gaugeLoop)

  // Trend chart
  resizeCanvas()
  window.addEventListener('resize', resizeCanvas)
  trendTimer = window.setInterval(() => {
    const t = trendTick * 0.18 + 1.2
    sineData.shift();   sineData.push(0.5 + 0.45 * Math.sin(t))
    rampData.shift();   rampData.push((trendTick % 80) / 80)
    randomData.shift(); randomData.push(Math.random())
    drawChart()
  }, 40)

  // Dot flicker
  dotTimer = window.setInterval(() => {
    const i = Math.floor(Math.random() * 4)
    dotStates.value[i] = !dotStates.value[i]
  }, 700)

  // Live sensors
  sensorTimer = window.setInterval(() => {
    const l = live.value
    l.aggLvl  = (60 + Math.random() * 20).toFixed(1)
    l.cemLvl  = (75 + Math.random() * 15).toFixed(1)
    l.belt    = (45 + Math.random() * 15).toFixed(1)
    l.rpm     = (15 + Math.random() * 8).toFixed(1)
    l.water   = (130 + Math.random() * 25).toFixed(1)
    l.batchWt = String(Math.round(2400 + Math.random() * 100))
    l.admix   = (4 + Math.random() * 2).toFixed(1)
    l.motorA  = (30 + Math.random() * 15).toFixed(1)
    l.motorV  = (413 + Math.random() * 4).toFixed(1)
    l.convA   = (10 + Math.random() * 6).toFixed(1)
    if (Math.random() < 0.02) l.batchCount++
    l.cementUsed += 12 + Math.random() * 5
    l.aggUsed    += 35 + Math.random() * 10
    l.waterUsed  += 6  + Math.random() * 3
    l.energy     += 0.4 + Math.random() * 0.3
  }, 600)

  // Gauge target
  gaugeTargetTimer = window.setInterval(() => {
    gaugeTarget = Math.random() < 0.25 ? Math.random() * 8 : 5 + Math.random() * 92
  }, 900)

  // Alarm color cycling
  const COLORS = ['red','blue','white']
  alarmTimer = window.setInterval(() => {
    const n = 2 + Math.floor(Math.random() * 3)
    for (let i = 0; i < n; i++) {
      alarmColors.value[Math.floor(Math.random() * alarmColors.value.length)] =
        COLORS[Math.floor(Math.random() * 3)]
    }
  }, 500)
})

onBeforeUnmount(() => {
  cancelAnimationFrame(rafId)
  clearInterval(dotTimer); clearInterval(sensorTimer)
  clearInterval(trendTimer); clearInterval(alarmTimer); clearInterval(gaugeTargetTimer)
  window.removeEventListener('resize', resizeCanvas)
})
</script>

<style scoped>
*, *::before, *::after { box-sizing: border-box; margin: 0; padding: 0; }

.fd2-root {
  height: 100%; overflow-x: auto; overflow-y: hidden;
  font-family: 'Poppins', sans-serif; font-size: 12px;
  background: #f5f5f5; color: #333;
  display: flex; flex-direction: column;
}

/* Page tabs */
.page-tabs { display: flex; gap: 0; flex-shrink: 0; background: #ffffff; border-bottom: 2px solid #c3c6d4; }
.page-tab  { padding: 6px 22px; font-size: 12px; font-weight: 600; color: #7f7f7f; background: #fff; border: none; cursor: pointer; border-right: 1px solid #e0e0e0; letter-spacing: 0.4px; transition: background .15s, color .15s; }
.page-tab:hover { background: #f2f2f2; color: #333; }
.page-tab.active { background: #fff; color: #333; border-bottom: 2px solid #1565c0; margin-bottom: -2px; }

/* Sliders */
.sliders-bar  { background: #f0f0f0; padding: 1px 12px; flex-shrink: 0; border-bottom: 1px solid #c3c6d4; }
.slider-row   { display: flex; align-items: center; gap: 6px; margin-bottom: 1px; position: relative; }
.slider-end   { font-size: 10px; color: #515151; min-width: 20px; }
.slider-wrap  { flex: 1; position: relative; }
.slider-bubble { position: absolute; top: -16px; background: #e0e0e0; color: #333; font-size: 9px; padding: 1px 4px; border-radius: 3px; transform: translateX(-50%); white-space: nowrap; pointer-events: none; }
input[type=range] { width: 100%; height: 3px; cursor: default; -webkit-appearance: none; appearance: none; background: #c3c6d4; border-radius: 2px; outline: none; }
input[type=range]::-webkit-slider-thumb { -webkit-appearance: none; width: 11px; height: 11px; border-radius: 50%; cursor: pointer; }
.slider-green::-webkit-slider-thumb { background: #4caf50; }
.slider-blue::-webkit-slider-thumb  { background: #1565c0; }

/* 3-col layout */
.main-area { display: flex; flex: 1; overflow: hidden; min-height: 0; }

/* Left panel */
.left-panel { width: 180px; flex-shrink: 0; background: #f8f8f8; border-right: 1px solid #c3c6d4; display: flex; flex-direction: column; overflow: hidden; }
.alarm-states-box { border: 2px solid #c3c6d4; background: #f0f0f0; margin: 8px 8px 4px; padding: 6px 4px 8px; }
.alarm-states-title { text-align: center; font-weight: bold; font-size: 12px; text-decoration: underline; color: #515151; margin-bottom: 6px; }
.towers-row { display: flex; justify-content: center; align-items: flex-end; }
.gauge-box  { margin: 4px 8px; background: #fff; border: 1px solid #c3c6d4; padding: 8px 4px 4px; display: flex; flex-direction: column; align-items: center; }
#gaugeSvg   { width: 145px; height: 122px; }
.gauge-readout { font-size: 18px; font-weight: bold; color: #333; text-align: center; margin-top: 2px; letter-spacing: 1px; }
.gauge-sub     { font-size: 10px; color: #7f7f7f; text-align: center; }
.mixing-box    { margin: 4px 8px 8px; background: #fff; border: 1px solid #c3c6d4; flex: 1; min-height: 0; display: flex; flex-direction: column; overflow: hidden; }
.mixing-inner  { flex: 1; display: flex; overflow: hidden; min-height: 0; }
.mix-scale     { width: 26px; display: flex; flex-direction: column; justify-content: space-between; padding: 4px 2px; font-size: 9px; color: #7f7f7f; text-align: right; }
.mix-bar-col   { flex: 1; display: flex; align-items: flex-end; padding: 4px 6px; }
.mix-track     { width: 22px; height: 100%; background: #f0f0f0; border: 1px solid #c3c6d4; display: flex; align-items: flex-end; }
.mix-fill      { width: 100%; background: #1565c0; transition: height 1s ease; }
.mix-vert-label { writing-mode: vertical-rl; transform: rotate(180deg); font-size: 9px; color: #7f7f7f; align-self: center; padding: 0 3px; white-space: nowrap; }
.mix-title     { text-align: center; font-size: 11px; font-weight: bold; color: #333; padding: 4px; border-top: 1px solid #c3c6d4; flex-shrink: 0; }

/* Center panel */
.center-panel { flex: 1; display: flex; flex-direction: column; background: #f5f5f5; overflow: hidden; position: relative; }
.image-area   { flex: 1; background: #e8e8e8; border: none; position: relative; overflow: hidden; }
.side-text    { position: absolute; left: 0; top: 0; bottom: 0; writing-mode: vertical-rl; transform: rotate(180deg); font-size: 9px; color: #bbb; padding: 8px 2px; background: #f0f0f0; border-right: 1px solid #e0e0e0; line-height: 1.3; overflow: hidden; }

/* Sensor pins */
.sensor-pin   { position: absolute; display: flex; align-items: center; gap: 6px; cursor: default; }
.sensor-dot   { width: 14px; height: 14px; border-radius: 50%; border: 2px solid #555; flex-shrink: 0; }
.sensor-label { background: rgba(255,255,255,.95); color: #333; font-size: 10px; padding: 3px 7px; border-radius: 4px; white-space: nowrap; border: 1px solid rgba(0,0,0,.15); line-height: 1.5; pointer-events: none; box-shadow: 0 1px 4px rgba(0,0,0,.12); }
.sl-name { font-weight: 700; font-size: 9px; color: #1565c0; text-transform: uppercase; letter-spacing: 0.5px; display: block; }
.sl-val  { font-size: 11px; color: #333; }
.sl-unit { font-size: 9px; color: #7f7f7f; }

/* Right panel */
.right-panel   { width: 440px; flex-shrink: 0; display: flex; flex-direction: column; border-left: 1px solid #c3c6d4; background: #fff; color: #333; overflow: hidden; }
.rp-topbar     { background: #f8f8f8; padding: 3px 12px; text-align: center; font-size: 12px; color: #e53935; border-bottom: 1px solid #e0e0e0; flex-shrink: 0; }
.outflow-header { display: flex; align-items: center; justify-content: space-between; background: #e3f0ff; padding: 5px 10px; border-bottom: 1px solid #bbdefb; flex-shrink: 0; }
.outflow-title  { font-size: 14px; font-weight: bold; color: #0d1b2a; }
.outflow-gph    { font-size: 14px; font-weight: bold; color: #0d1b2a; }
.outflow-x      { background: none; border: none; font-size: 15px; cursor: pointer; color: #555; }
.data-section   { display: flex; flex: 1; min-height: 0; overflow: hidden; }
.intake-bar     { width: 30px; flex-shrink: 0; background: #fff; border-right: 1px solid #c3c6d4; display: flex; flex-direction: column; align-items: center; }
.intake-gph     { font-size: 9px; font-weight: bold; color: #fff; text-align: center; padding: 3px 0; border-bottom: 1px solid #90caf9; width: 100%; background: #1565c0; }
.intake-label   { writing-mode: vertical-rl; transform: rotate(180deg); font-size: 10px; font-weight: bold; color: #fff; letter-spacing: 1px; flex: 1; display: flex; align-items: center; justify-content: center; background: #1565c0; width: 100%; }
.dt-wrap { flex: 1; overflow-y: auto; background: #fafafa; }
.dt-wrap::-webkit-scrollbar { width: 4px; } .dt-wrap::-webkit-scrollbar-thumb { background: #c3c6d4; border-radius: 2px; }
table.dt { width: 100%; border-collapse: collapse; font-size: 11px; }
table.dt th { background: linear-gradient(0deg,#d7d7d7 0%,#fff 100%); padding: 5px 7px; text-align: left; color: #515151; font-weight: 600; border-bottom: 2px solid #c3c6d4; position: sticky; top: 0; z-index: 1; }
table.dt tbody tr:nth-child(odd)  td { background: #fff; }
table.dt tbody tr:nth-child(even) td { background: #f8f8f8; }
table.dt td { padding: 5px 7px; border-bottom: 1px solid #e0e0e0; vertical-align: top; color: #515151; }
table.dt tr:hover td { background: #f2f2f2 !important; }
.dt-tag { color: #1565c0; font-weight: 700; white-space: nowrap; }
.dt-chk { display: inline-flex; align-items: center; justify-content: center; background: #43a047; color: #fff; width: 16px; height: 16px; min-width: 16px; font-size: 12px; font-weight: bold; border-radius: 2px; line-height: 1; }
.dt-ts  { color: #7f7f7f; font-size: 11px; white-space: nowrap; }

/* Trend */
.trend-box        { height: 110px; flex-shrink: 0; display: flex; flex-direction: column; border-top: 1px solid #c3c6d4; background: #fff; overflow: hidden; }
.trend-top        { display: flex; justify-content: space-between; align-items: center; padding: 5px 10px; background: #f8f8f8; border-bottom: 1px solid #e0e0e0; flex-shrink: 0; }
.legend           { display: flex; gap: 6px; }
.leg-item         { display: flex; align-items: center; gap: 5px; font-size: 11px; color: #333; font-weight: 700; letter-spacing: 0.5px; }
.leg-line         { width: 36px; height: 16px; border-radius: 2px; }
.tbtn-row         { display: flex; gap: 3px; }
.tbtn             { background: #f0f0f0; border: 1px solid #c3c6d4; border-radius: 2px; width: 20px; height: 20px; font-size: 11px; cursor: pointer; display: flex; align-items: center; justify-content: center; color: #7f7f7f; }
.trend-canvas-wrap { flex: 1; overflow: hidden; position: relative; }
.trend-canvas-wrap canvas { width: 100%; height: 100%; display: block; }

/* Alarm table */
.alarm-section  { height: 120px; flex-shrink: 0; background: #fff; color: #333; display: flex; flex-direction: column; border-top: 2px solid #c3c6d4; overflow: hidden; }
.alarm-toolbar  { display: flex; justify-content: flex-end; align-items: center; padding: 3px 8px; background: #f0f0f0; border-bottom: 1px solid #c3c6d4; flex-shrink: 0; }
.filter-alarms  { height: 22px; border: 1px solid #c3c6d4; border-radius: 3px; padding: 0 8px; font-size: 11px; outline: none; width: 150px; background: #fff; color: #333; font-family: 'Poppins', sans-serif; }
.at-wrap { flex: 1; overflow-y: auto; }
table.at { width: 100%; border-collapse: collapse; font-size: 11px; }
table.at th { background: linear-gradient(0deg,#d7d7d7 0%,#fff 100%); color: #515151; padding: 3px 5px; text-align: left; font-weight: 600; border-bottom: 1px solid #c3c6d4; position: sticky; top: 0; z-index: 1; white-space: nowrap; font-size: 10px; }
table.at td { padding: 3px 5px; white-space: nowrap; border-bottom: 1px solid rgba(0,0,0,.06); color: #333; }
.at-red  td { background: #e53935 !important; color: #fff; }
.at-blue td { background: #1e88e5 !important; color: #fff; }
.at-white td { background: #fff    !important; color: #333; }
.at-ts-red   { color: #ffcdd2 !important; }
.at-ts-blue  { color: #bbdefb !important; }
.at-ts-white { color: #c62828; font-weight: 600; }
.at-bold { font-weight: bold; }
</style>
