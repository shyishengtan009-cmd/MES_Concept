# Concept HIAS — Full Project Notes
**Last Updated:** 2026-05-04  
**Purpose:** Standalone UI/UX concept mockup for the HIAS MES/OEE/Fleet manufacturing dashboard module.  
**Stack:** Vue 3 (Composition API, `<script setup lang="ts">`) + Vite 5 + TypeScript. C# controllers are reference files only — not connected to a running API.  
**Rule:** All files stay inside this folder. Never copy into HIAS-AC-VUE or HIAS-AC-NET-CORE unless explicitly asked.

---

## How to Run

```bash
cd "C:\Users\Tan Shyi Sheng\OneDrive - DagangAsia Network Holding Sdn Bhd\Concept_HIAS"
npm run dev
# Open http://localhost:5173
```

Entry: `src/main.ts` → `createApp(HiasMockup).mount('#app')`

---

## Folder Structure

```
Concept_HIAS/
├── index.html                 # Loads FontAwesome 6 + Poppins/Roboto CDN, mounts #app
├── vite.config.ts             # @vitejs/plugin-vue only
├── tsconfig.json              # ESNext, bundler moduleResolution
├── package.json               # vue ^3.4, vite ^5, @vitejs/plugin-vue ^5
├── src/
│   └── main.ts                # createApp(HiasMockup).mount('#app')
├── public/                    # Static assets — img src must use leading /
│   ├── hias-logo.png
│   ├── ffs-packaging-line.jpg
│   └── batching-plant-outdoor.png
├── Controllers/               # C# Web API reference controllers
│   ├── ConceptHiasShellController.cs
│   ├── ConceptOeeMainController.cs
│   ├── ConceptOeeController.cs
│   ├── ConceptShopFloorController.cs
│   ├── ConceptPreventiveMaintenanceController.cs
│   ├── ConceptMesWesController.cs
│   ├── ConceptFleetController.cs
│   ├── ConceptInventoryController.cs
│   └── ConceptCostAnalysisController.cs
├── HiasMockup.vue             # Root shell component
├── OeeMain.vue                # Tab container (OEE / Shop Floor / PM / MES)
├── OeeDashboard.vue           # Live OEE gauges, trend chart, alarm table
├── ShopFloorMonitoring.vue    # SVG floor plan, machine list, zone detail
├── PreventiveMaintenance.vue  # PM schedule, asset health bars, activity feed
├── MesWesOverview.vue         # Work orders, quality holds, AGV status, WES flow
├── FleetDashboard.vue         # FFS Packaging fleet dashboard
├── FleetDashboard2.vue        # Batching Plant fleet dashboard
├── Inventory.vue              # Stock register, movements, category bars
└── CostAnalysis.vue           # Cost breakdown, budget vs actual, overhead table
```

---

## Component Deep Dives

---

### `HiasMockup.vue` — App Shell (Root Component)

**Mounted by:** `src/main.ts`  
**Imports:** `OeeMain`, `Inventory`, `CostAnalysis` from same folder

#### Template Structure
```
#hias-root
├── header.hias-topnav
│   ├── .topnav-left  → sidebar toggle button + logo img
│   └── .menu-section → global search input, org button, bell badge, user button
└── .hias-body  (:class { sidebar-hidden: !showSidebar })
    ├── nav.hias-sidebar
    │   └── .sidebar-menu → v-for menus → accordion children
    └── main.hias-content
        ├── <OeeMain>      v-if="activePage === 'oee-main'"
        ├── <Inventory>    v-if="activePage === 'inventory'"
        └── <CostAnalysis> v-if="activePage === 'cost-analysis'"
```

#### Reactive State
```ts
const showSidebar  = ref(true)           // sidebar visible/hidden
const searchQuery  = ref('')             // global search input
const openMenu     = ref<string | null>('MES')  // which accordion is expanded
const activeParent = ref('MES')          // bold/highlighted sidebar parent
const activeChild  = ref('Dashboard')   // underlined sidebar child
const activePage   = ref('oee-main')    // which content component to show
```

#### Key Data
```ts
// 15 sidebar menu items — matches original hias-layout.js exactly
const menus = [
  { name: 'Dashboard', icon: 'fa-solid fa-square-poll-vertical', children: [] },
  { name: 'Document',  icon: 'fa-solid fa-folder', children: ['Document List', 'Document Category'] },
  // ... 13 more
  { name: 'MES', icon: 'fa-solid fa-diagram-project', children: ['Dashboard', 'Inventory', 'Cost Analysis'] },
  { name: 'Monitoring', icon: 'fa-solid fa-eye', children: ['Typhoid Vaccination', 'Cold Truck Real-Time Monitoring', 'Alert'] },
]

// Maps sidebar child name → page key rendered in .hias-content
const pageMap: Record<string, string> = {
  'Dashboard':     'oee-main',
  'Inventory':     'inventory',
  'Cost Analysis': 'cost-analysis',
}
```

#### Functions
```ts
function selectChild(parent: string, child: string) {
  activeParent.value = parent
  activeChild.value  = child
  activePage.value   = pageMap[child] ?? ''  // '' = no component shown
}
function toggleMenu(name: string) {
  openMenu.value = openMenu.value === name ? null : name
}
```

#### SVG Constants
```ts
const corporateFareSVG = `<svg ...>...</svg>`  // org building icon in topnav
const userAvatarSVG    = `<svg ...>...</svg>`  // user avatar circle in topnav
```

#### C# Controller: `ConceptHiasShellController.cs`
```
GET api/concept/shell/menus      → returns the 15-item menus array
GET api/concept/shell/page-map   → returns { Dashboard: "oee-main", Inventory: "inventory", "Cost Analysis": "cost-analysis" }
```

---

### `OeeMain.vue` — Tab Container

**Parent:** `HiasMockup.vue` (rendered when `activePage === 'oee-main'`)  
**Imports:** `OeeDashboard`, `ShopFloorMonitoring`, `PreventiveMaintenance`, `MesWesOverview`

#### Reactive State
```ts
const activeTab = ref<'oee' | 'shop' | 'pm' | 'mes'>('oee')
```

#### Tab Definitions (static const)
```ts
const tabs = [
  { id: 'oee',  label: 'OEE',                   icon: 'fa-solid fa-industry'          },
  { id: 'shop', label: 'Shop Floor Monitoring',  icon: 'fa-solid fa-warehouse'         },
  { id: 'pm',   label: 'Preventive Maintenance', icon: 'fa-solid fa-wrench'            },
  { id: 'mes',  label: 'MES / WES Overview',     icon: 'fa-solid fa-diagram-project'   },
] as const
```

#### Template Logic
```html
<!-- Tab renders with v-if (unmounts/remounts on switch) -->
<OeeDashboard        v-if="activeTab === 'oee'"  />
<ShopFloorMonitoring v-if="activeTab === 'shop'" />
<PreventiveMaintenance v-if="activeTab === 'pm'" />
<MesWesOverview      v-if="activeTab === 'mes'"  />
```

#### C# Controller: `ConceptOeeMainController.cs`
```
GET api/concept/oee-main/tabs → returns the 4 tab definitions
```

---

### `OeeDashboard.vue` — OEE Dashboard

**Parent:** `OeeMain.vue`  
**Lines:** ~608  
**Purpose:** Full live OEE dashboard for two production lines (FFS Packaging / Batching Plant), switched via page tabs at top.

#### Reactive State
```ts
type Line = 'ffs' | 'batch'
const line = ref<Line>('ffs')

// Arc gauge targets (what RAF eases toward)
const avlTarget  = ref(91.2)
const perfTarget = ref(83.7)
const qualTarget = ref(98.4)

// Arc gauge current (animated by RAF)
const avlCur  = ref(91.2)
const perfCur = ref(83.7)
const qualCur = ref(98.4)

// Computed OEE from the three values
const oee = computed(() => (avlCur.value/100) * (perfCur.value/100) * (qualCur.value/100) * 100)

// Output counters
const packCount    = ref(18640)   // increments every 600ms
const rejectCount  = ref(12)      // occasionally increments
const goodCount    = computed(() => packCount.value - rejectCount.value)
const currentTarget = ref(22000)
const outputPct    = computed(() => Math.min(100, (packCount.value / currentTarget.value) * 100))
const lineSpeed    = ref(62)      // p/min, randomised every 600ms
const minorStops   = ref(3)

// 4 sensor pin dots per line (green=true, red=false)
const dotFfs   = reactive([true, true, true, true])
const dotBatch = reactive([true, true, true, true])

// Live pin label values
const pins = reactive({
  ffs:   ['250.4', '182.5', '48.2', '415.1'],   // Fill Weight g / Seal Temp °C / Screw Feed kg/h / Motor Voltage V
  batch: ['72.0', '1.45', '47.3', '12.4'],       // Aggregate Hopper % / Belt Speed m/s / Mixer Drum % / Water Flow m³/h
})

// Alarm table
const alarmFilter  = ref('')
const activeAlarms = ref<Alarm[]>(ffsAlarms.map(a => ({ ...a })))
const filteredAlarms = computed(() => { /* filter by alarmFilter text */ })

// Canvas ref
const trendCanvas = ref<HTMLCanvasElement | null>(null)
```

#### Arc Gauge Math
```ts
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

// Track paths (full grey arc) — static, computed once
const TRACK_PATH = arcPath(START_DEG, START_DEG + TOTAL_DEG)

// Value paths — reactive computed from *Cur refs
const avlArcPath  = computed(() => gaugePath(avlCur.value))
const perfArcPath = computed(() => gaugePath(perfCur.value))
const qualArcPath = computed(() => gaugePath(qualCur.value))

function gaugePath(val: number): string {
  const span = Math.max(MIN_ARC, (Math.max(0, Math.min(100, val)) / 100) * TOTAL_DEG)
  return arcPath(START_DEG, START_DEG + span)
}
```

#### RAF Gauge Loop
```ts
let rafId: number | null = null
function gaugeLoop() {
  avlCur.value  += (avlTarget.value  - avlCur.value)  * 0.1   // exponential ease
  perfCur.value += (perfTarget.value - perfCur.value) * 0.1
  qualCur.value += (qualTarget.value - qualCur.value) * 0.1
  rafId = requestAnimationFrame(gaugeLoop)
}
```

#### Timers (all cleared in `onBeforeUnmount`)
| Timer | Interval | What it does |
|-------|----------|--------------|
| `dotTimer` | 700ms | Randomly toggles green/red on FFS and batch sensor dots |
| `sensorTimer` | 600ms | Updates pin values, lineSpeed, packCount, avlTarget, perfTarget, qualTarget |
| `trendTimer` | 40ms | Pushes new data point to 300-point circular buffers, redraws canvas chart |
| `alarmTimer` | 500ms | Randomly changes 1–2 alarm row colors (red/blue/white) |

#### Canvas Trend Chart
```ts
const POINTS = 300
const oeeData  = Array.from({ length: POINTS }, () => 0.73 + Math.random() * 0.04)
const avlData  = Array.from({ length: POINTS }, () => 0.89 + Math.random() * 0.04)
const perfData = Array.from({ length: POINTS }, () => 0.80 + Math.random() * 0.07)
const qualData = Array.from({ length: POINTS }, () => 0.97 + Math.random() * 0.025)

// Each tick: shift oldest, push current value
oeeData.shift(); oeeData.push(Math.max(0.5, Math.min(1, curOee)))
// drawChart() draws 4 lines: OEE(#1565c0), AVAIL(#43a047), PERF(#e91e63), QUAL(#f57c00)
```

#### Line Switch (`switchLine`)
```ts
function switchLine(l: Line) {
  line.value = l
  if (l === 'batch') {
    currentTarget.value = 18000; packCount.value = 14280; rejectCount.value = 24
    avlTarget.value = 88.5; perfTarget.value = 79.2; qualTarget.value = 97.1
    activeAlarms.value = batchAlarms.map(a => ({ ...a }))
  } else {
    currentTarget.value = 22000; packCount.value = 18640; rejectCount.value = 12
    avlTarget.value = 91.2; perfTarget.value = 83.7; qualTarget.value = 98.4
    activeAlarms.value = ffsAlarms.map(a => ({ ...a }))
  }
}
```

#### Alarm Data Shape
```ts
interface Alarm {
  id: string; timestamp: string; value: string; text: string
  type: string; group: string; priority: number; ackDate: string
  clearedDate: string; clearedVal: string; node: string; unit: string
  color: 'red' | 'blue' | 'white'
}
// 5 FFS alarms, 5 batch alarms — swapped when line switches
```

#### C# Controller: `ConceptOeeController.cs`
```
GET api/concept/oee?line=ffs    → OEE KPI + sensor pin + alarm data for FFS
GET api/concept/oee?line=batch  → same for Batching Plant
```

---

### `ShopFloorMonitoring.vue` — Shop Floor

**Parent:** `OeeMain.vue`  
**Lines:** ~538

#### Reactive State
```ts
const activeTab = ref<'indoor' | 'outdoor'>('indoor')
const clock     = ref('')         // live HH:MM:SS, updated every 1s
const inKpi  = reactive({ output: 18640, fillWt: '250.0' })
const outKpi = reactive({ batches: 47, silo: 83 })

// SVG text values for each machine — updated every 17s
const m01 = reactive({ fw: '250.4', st: '182.5', fr: '48.2', mc: '14.3' })
const m02 = reactive({ fw: '249.8', st: '181.0', fr: '47.9', mc: '13.8' })
const m03 = reactive({ fw: '251.2', st: '183.2', fr: '49.0', mc: '15.1' })
const bp01 = reactive({ bw: 2450, rpm: '18.4' })
const bp02 = reactive({ bw: 2380, rpm: '17.8' })
```

#### Machine/Equipment Data
```ts
// Indoor FFS machines (used for machine list + zone detail panel)
const inMachines = [
  { name: 'FFS Machine 01', zone: 'Zone A', status: 'ACTIVE', l1: 'Fill Weight', v1: '250.4 g', l2: 'Seal Temp H-jaw', v2: '182.5 °C', l3: 'Screw Feed Rate', v3: '48.2 kg/h' },
  { name: 'FFS Machine 02', zone: 'Zone A', status: 'ACTIVE', ... },
  { name: 'FFS Machine 03', zone: 'Zone B', status: 'ACTIVE', ... },
  { name: 'FFS Machine 04', zone: 'Zone B', status: 'FAULT', l2: 'Seal Temp H-jaw', v2: 'FAULT E-42', ... },
]
// Outdoor batching plants
const outMachines = [
  { name: 'Batching Plant 01', zone: 'Zone A', status: 'ACTIVE', l1: 'Batch Weight', v1: '2,450 kg', l2: 'Mixer RPM', v2: '18.4 rpm', l3: 'Aggregate Feed', v3: '53.6 t/h' },
  { name: 'Batching Plant 02', zone: 'Zone A', status: 'ACTIVE', ... },
  { name: 'Batching Plant 03', zone: 'Zone B', status: 'IDLE', ... },
]
```

#### Zone Detail Selection
```ts
// Clicking a machine row calls selectIn(m) / selectOut(m)
// which Object.assign()s into inDetail / outDetail reactive objects
// The zone detail panel binds directly to inDetail / outDetail
const inDetail  = reactive({ zone, name, status, l1, v1, l2, v2, l3, v3 })
const outDetail = reactive({ zone, name, status, l1, v1, l2, v2, l3, v3 })
```

#### Event Feeds (static)
```ts
const inEvents = [
  { time: '09:14 am', color: '#e53935', text: 'FFS Machine 04 stopped — E-42 jaw overheat fault' },
  { time: '09:08 am', color: '#e53935', text: 'Fill weight deviation alarm — M01 +3.8 g over tolerance' },
  { time: '09:02 am', color: '#43a047', text: 'FFS Machine 03 resumed after film roll change' },
  { time: '08:44 am', color: '#1565c0', text: 'Shift 2 started — WO-2026-041 active on all lines' },
]
// similar outEvents for batching plant
```

#### SVG Floor Plan
Both tabs render an inline `<svg viewBox="0 0 700 420">` floor plan. Machine sensor values are bound with `{{ m01.fw }}` etc. directly inside `<text>` elements. Pulsing status circles use `<animate>` elements (no JS — CSS/SVG animation).

#### Timers
| Timer | Interval | What it does |
|-------|----------|--------------|
| `clockTimer` | 1000ms | Updates `clock` to current `HH:MM:SS` |
| `liveTimer` | 17000ms | Updates all machine sensor values (m01–m03, bp01–bp02) and KPIs |

#### C# Controller: `ConceptShopFloorController.cs`
```
GET api/concept/shop-floor?tab=indoor   → machine status, KPIs, SVG data for FFS
GET api/concept/shop-floor?tab=outdoor  → machine status, KPIs, SVG data for batching
```

---

### `PreventiveMaintenance.vue` — Preventive Maintenance

**Parent:** `OeeMain.vue`  
**Lines:** ~277

#### Reactive State
```ts
const activeTab = ref<'ffs' | 'bp'>('ffs')
const clock     = ref('')
let clockTimer: ReturnType<typeof setInterval> | null = null
```

#### PM Schedule Data Shape
```ts
// ffsSchedule: 9 rows, bpSchedule: 9 rows
{ asset, zone, task, frequency, lastDone, nextDue, status, assigned }
// status values: 'OVERDUE' | 'DUE SOON' | 'IN PROGRESS' | 'OK'
```

#### Status Badge Mapping
```ts
function statusBadge(status: string) {
  if (status === 'OVERDUE')     return 'badge-overdue'   // red
  if (status === 'DUE SOON')    return 'badge-due'       // yellow
  if (status === 'IN PROGRESS') return 'badge-progress'  // blue
  return 'badge-ok'                                       // green
}
```

#### Asset Health (right panel — bar chart rows)
```ts
// ffsHealth: 6 assets with health%, color (green/amber/red)
{ name: 'FFS Machine 04', health: 34, color: '#e53935' }  // fault → red
{ name: 'FFS Machine 01', health: 62, color: '#f9a825' }  // degraded → amber
{ name: 'FFS Machine 02', health: 81, color: '#388E3C' }  // healthy → green
// bpHealth: 5 assets
```

#### Recent Activity (right panel — event feed)
```ts
// ffsActivity: 6 events, bpActivity: 6 events
// Each: { color, time, text }
// Bound with v-for in event-list div
```

#### Timers
Only `clockTimer` (1000ms) — no live data simulation.

#### C# Controller: `ConceptPreventiveMaintenanceController.cs`
```
GET api/concept/pm?line=ffs   → schedule rows, health bars, activity for FFS
GET api/concept/pm?line=batch → same for Batching Plant
```

---

### `MesWesOverview.vue` — MES / WES Overview

**Parent:** `OeeMain.vue`  
**Lines:** ~340

#### Reactive State
```ts
type View = 'both' | 'mes' | 'wes'
const view  = ref<View>('both')   // 3-way toggle in topbar
const clock = ref('')

const kpis = ref({
  activeWorkOrders: 12,
  ordersComplete:   47,
  onTimeRate:       '91%',
  warehouseTasks:   38,
  agvsActive:       '4/5',
  pickAccuracy:     '99.2%',
  exceptions:       3
})
```

#### MES Data
```ts
// Work Orders table (5 rows, updated by liveUpdate)
const workOrders = ref([
  { order: 'WO-2026-041', product: 'Pack A 500g', line: 'Line 1', qty: '2,400', progress: 82,  status: 'RUNNING' },
  { order: 'WO-2026-043', product: 'Mix Batch C', line: 'Line 2', qty: '80 bat', progress: 100, status: 'DONE'   },
  // status: 'RUNNING' | 'DONE' | 'SETUP' | 'QUEUED'
])

// Quality Holds table (3 rows)
const qualityHolds = ref([
  { lot: 'LOT-2026-088', reason: 'Fill weight deviation', qtyHeld: '240 units', raised: '09:07 am', status: 'ON HOLD' },
  // status: 'ON HOLD' | 'REVIEWING' | 'RELEASED'
])

// MES event timeline (5 events)
const mesEvents = ref([{ color, time, text }, ...])
```

#### WES Data
```ts
// Warehouse flow nodes (6 stations in 2 rows of 3)
const wesFlow = ref({ receiving: 8, storage: 2840, picking: 14, packing: 6, staging: 3, dispatch: 47 })
// Rendered as: Receiving → Storage → Picking (row 1)
//              Packing   → Staging  → Dispatch (row 2)

// AGV Status table (6 units)
const agvStatus = ref([
  { unit: 'AGV Unit 01', zone: 'Zone D',   task: 'Rack A → Staging',   battery: '88%',  status: 'ACTIVE'   },
  { unit: 'AGV Unit 04', zone: 'Charging', task: '—',                   battery: '32%',  status: 'CHARGING' },
  { unit: 'AGV Unit 05', zone: '—',        task: 'Maintenance hold',    battery: '—',    status: 'OFFLINE'  },
])

// WES event timeline (5 events)
const wesEvents = ref([...])
```

#### Live Update (every 17s)
```ts
function liveUpdate() {
  if (Math.random() > 0.5) kpis.value.ordersComplete++
  kpis.value.warehouseTasks = Math.max(20, Math.min(60, kpis.value.warehouseTasks + random(-2..+2)))
  wesFlow.value.storage = Math.round(rnd(2840, 100, 0))
  // Work order progress ticks up: wo[0] +1..4, wo[1] +1..5, wo[3] +0..3
}
```

#### Badge Helpers
```ts
statusBadge(status)  // RUNNING→blue, DONE→green, SETUP/QUEUED→yellow
holdBadge(status)    // ON HOLD→red, REVIEWING→yellow, RELEASED→green
agvBadge(status)     // ACTIVE→green, RUNNING→blue, IDLE/CHARGING→yellow, OFFLINE→red
progressColor(status) // DONE→green, RUNNING→green, SETUP→amber
```

#### C# Controller: `ConceptMesWesController.cs`
```
GET api/concept/mes-wes?view=both → all MES + WES data
GET api/concept/mes-wes?view=mes  → MES only
GET api/concept/mes-wes?view=wes  → WES only
```

---

### `FleetDashboard2.vue` — Batching Plant Fleet Dashboard

**Parent:** `HiasMockup.vue` (concept reference — not wired to a sidebar route yet)  
**Lines:** ~700+

#### Reactive State
```ts
const slider1 = ref(50)    // green slider (v-model.number)
const slider2 = ref(30)    // blue slider

const live = ref({         // updated every 600ms
  rpm: 18.4, output: 47, feedRate: 68, mixerLoad: 72,
  pin0: '72.0', pin1: '1.45', pin2: '18.4', pin3: '42.0'
})

const gaugeCur    = ref(0)         // animated by RAF loop
const gaugeTarget = ref(72)
const mixPct      = ref(64)        // Water Tank fill bar %
const mixLabel    = ref('64%')

// Dot colors for 4 sensor pins on plant photo
const dotColors = reactive(['#43a047', '#43a047', '#43a047', '#43a047'])

const alarmFilter = ref('')        // alarm table search
```

#### SVG Machine Diagrams (via `v-html`)
```ts
// computed properties return SVG strings with reactive dot colors embedded
const machine1Svg = computed(() => `
  <svg viewBox="0 0 60 120" ...>
    <circle cx="30" cy="20" r="8" fill="${dotColors[0]}" />
    ...
  </svg>
`)
const machine2Svg = computed(() => `...svg with dotColors[1]...`)
```
These are bound in template as `<div v-html="machine1Svg"></div>`.

#### Arc Gauge (SVG refs + RAF)
```ts
const gaugeTrackRef = ref<SVGPathElement | null>(null)
const gaugeArcRef   = ref<SVGPathElement | null>(null)

// RAF loop sets .setAttribute('d', arcPath(...)) directly on SVG DOM elements
// Uses same polarXY / arcPath math as OeeDashboard
// gaugeTarget changes every 3000ms via gaugeTargetTimer
```

#### Data Table (17 rows via computed)
```ts
const tableRows = computed(() => [
  { label: 'Plant Output',      value: `${live.value.output} batches`,    unit: 'today' },
  { label: 'Mixer RPM',         value: `${live.value.rpm.toFixed(1)}`,    unit: 'rpm' },
  { label: 'Feed Rate',         value: `${live.value.feedRate}`,          unit: 't/h' },
  // ... 14 more rows
])
```

#### Canvas Trend Chart
```ts
// 3 modes supported: RAMP, RANDOM, SINE
// 300-point buffer, redrawn every 40ms via trendTimer
// drawTrend() draws a single white line on dark background
```

#### Timers
| Timer | Interval | What it does |
|-------|----------|--------------|
| `dotTimer` | 700ms | Randomly toggles dot colors (green/red) on machine diagrams and sensor pins |
| `sensorTimer` | 600ms | Updates `live` values (rpm, feedRate, pin values), `mixPct`, `mixLabel` |
| `trendTimer` | 40ms | Pushes data to canvas chart buffer, redraws chart |
| `alarmTimer` | 500ms | Random color cycling on alarm table rows (red/blue/white) |
| `gaugeTargetTimer` | 3000ms | Changes `gaugeTarget` to new random value for gauge animation |

All 5 timers + RAF are cancelled in `onBeforeUnmount`.

#### Alarm Data
```ts
// 8 alarm rows, same shape as OeeDashboard alarms
// Color cycles: 'red' | 'blue' | 'white'
```

#### C# Controller: `ConceptFleetController.cs`
```
GET api/concept/fleet?line=batch → batching plant fleet data
GET api/concept/fleet?line=ffs   → FFS packaging fleet data (for FleetDashboard.vue)
```

---

### `Inventory.vue` — Inventory Management

**Parent:** `HiasMockup.vue` (rendered when `activePage === 'inventory'`)  
**Lines:** ~377  
**Tabs:** Overview | Stock Register | Movements

#### Reactive State
```ts
type Tab = 'overview' | 'stock' | 'movements'
const activeTab   = ref<Tab>('overview')
const clock       = ref('')

// Stock Register filters
const stockType   = ref<string>('all')    // 'all' | 'rm' | 'fg' | 'sp'
const stockStatus = ref<string>('all')    // 'all' | 'lowcrit' | 'normal'
const stockSearch = ref('')               // text search on code+name+category

// Movements filters
const mvType   = ref<string>('all')       // 'all' | 'received' | 'production' | 'dispatch' | 'corrected'
const mvPeriod = ref<string>('today')     // 'today' | 'week' | 'month'
```

#### Computed Filters
```ts
const filteredStock = computed(() => {
  // typeOk: checks r.type ('rm'/'fg'/'sp')
  // statusOk: 'lowcrit' matches status 'low' OR 'critical'
  // searchOk: searches code + name + category
  return stockRows.filter(r => typeOk && statusOk && searchOk)
})

const filteredMovements = computed(() => {
  // periodOk: 'week' includes 'today', 'month' includes all
  // typeOk: matches movement type
  return movementRows.filter(r => periodOk && typeOk)
})
```

#### Stock Register Data (15 rows, static)
```ts
{ code: 'RM-001', name: 'Palm Olein Oil (5L)', category: 'Food Ingredient', type: 'rm',
  onHand: '12', reorderPt: '50', status: 'critical' }
// status: 'critical' | 'low' | 'normal'
// type:   'rm' (raw material) | 'fg' (finished goods) | 'sp' (spare parts)
```

#### Movement Data (16 rows, tagged by period)
```ts
{ date: '15/04 10:38', refNo: 'GRN-00214', item: 'Palm Olein Oil (5L)',
  movement: 'received', qty: '+100 unit', by: 'Amirul', period: 'today' }
// movement: 'received' | 'production' | 'dispatch' | 'corrected'
// period:   'today' | 'week' | 'month'
```

#### Overview — Category Bars (5 rows, static)
```ts
const categories = [
  { name: 'Packaging Material',  qty: 41200, pct: 82, color: '#1565c0' },
  { name: 'Food Ingredients',    qty: 32800, pct: 65, color: '#388E3C' },
  // ...
]
```

#### Movement Label/Class Helpers
```ts
function mvLabel(movement: string) {
  // 'received'   → '▲ Goods Received'
  // 'production' → '→ Issued to Production'
  // 'dispatch'   → '▼ Sales Dispatch'
  // 'corrected'  → '● Count Corrected'
}
function mvClass(movement: string) {
  // returns CSS class: mv-received (green) / mv-production (blue) / mv-dispatch (red) / mv-corrected (amber)
}
```

#### C# Controller: `ConceptInventoryController.cs`
```
GET api/concept/inventory/overview    → category totals, recent movements
GET api/concept/inventory/stock       → full stock register rows
GET api/concept/inventory/movements  → movement log rows
```

---

### `CostAnalysis.vue` — Cost Analysis

**Parent:** `HiasMockup.vue` (rendered when `activePage === 'cost-analysis'`)  
**Lines:** ~325  
**Tabs:** Overview | Material Cost | Overhead  
**No timers** — all data is static.

#### Reactive State
```ts
type Tab = 'overview' | 'material' | 'overhead'
const activeTab = ref<Tab>('overview')
const period    = ref('MTD')
const periods   = ['MTD', 'This Month', 'Q1 2026', 'YTD']  // period buttons in topbar

const matCat = ref<string>('all')  // 'all' | 'food' | 'packaging' | 'chemical'
```

#### Computed Filters
```ts
const filteredMaterial = computed(() =>
  matCat.value === 'all' ? materialRows : materialRows.filter(r => r.category === matCat.value)
)

// Category totals for the "Total: RM X" header
const catTotals: Record<string, number> = { all: 196400, food: 88800, packaging: 50940, chemical: 2208 }
const matTotal = computed(() => catTotals[matCat.value] ?? 196400)
```

#### Overview — Cost Breakdown (6 rows, static)
```ts
const breakdown = [
  { name: 'Raw Materials', pct: 52.4, barPct: 100, value: 'RM 149,200', color: '#1565c0' },
  { name: 'Packaging',     pct: 16.6, barPct: 44,  value: 'RM 47,200',  color: '#42a5f5' },
  { name: 'Utilities',     pct: 11.8, barPct: 30,  value: 'RM 33,600',  color: '#7b1fa2' },
  { name: 'Labour',        pct: 9.8,  barPct: 26,  value: 'RM 27,900',  color: '#ef9a9a' },
  { name: 'Maintenance',   pct: 6.3,  barPct: 17,  value: 'RM 17,900',  color: '#f9a825' },
  { name: 'Other',         pct: 3.1,  barPct: 11,  value: 'RM 8,900',   color: '#9e9e9e' },
]
```

#### Overview — Budget vs Actual (6 rows, static)
```ts
const budgetVsActual = [
  { name: 'Raw Materials', budget: 'RM 142,000', actual: 'RM 149,200', variance: '+5.1%',
    over: true, budgetBarPct: 100, actualBarPct: 105 },
  // over: true → red bar + var-pos class; over: false → blue bar + var-neg class
]
```

#### Material Cost Tab (10 rows)
```ts
{ name: 'Halal Seasoning Blend A', category: 'food', categoryLabel: 'Food Ingredient',
  total: 38200, pct: '19.5%', vsLastMonth: '+1,400', vsPositive: true }
// vsPositive: true → var-pos (red = cost went up), false → var-neg (green = cost down)
```

#### Overhead Tab (12 rows)
```ts
{ category: 'Maintenance', item: 'Corrective Repairs',
  budget: 4000, actual: 6400, variance: '+2,400', over: true }

function overBadge(row) {
  if (!row.over) return 'b-g'                           // Under → green
  const absVal = parseInt(row.variance.replace(/[^0-9]/g, ''))
  return absVal >= 1000 ? 'b-r' : 'b-y'                // Over big → red, small → yellow
}
function overLabel(row) {
  if (!row.over) return 'Under'
  return parseInt(...) >= 1000 ? 'Over' : 'Slight'
}
```

#### C# Controller: `ConceptCostAnalysisController.cs`
```
GET api/concept/cost-analysis/overview   → breakdown, budgetVsActual
GET api/concept/cost-analysis/material   → materialRows
GET api/concept/cost-analysis/overhead   → overheadRows
```

---

## Shared Patterns Across All Components

### Layout Pattern
Every component root uses:
```css
.root { height: 100%; overflow: hidden; font-family: 'Poppins', sans-serif;
        font-size: 12px; background: #f5f5f5; color: #333;
        display: flex; flex-direction: column; }
```
This fills the content area from `HiasMockup.vue` fully, with flex children using `flex: 1; overflow: hidden; min-height: 0` to allow inner scroll without page overflow.

### Tab/Topbar Pattern
All components with tabs use the same structure:
```html
<div class="topbar">
  <span class="pg-title">■ Title</span>
  <div class="pg-tabs">
    <button class="pg-tab" :class="{ active: activeTab === 'x' }" @click="activeTab = 'x'">Tab</button>
  </div>
</div>
<div class="tab-pane" v-show="activeTab === 'x'"> ... </div>
```
`v-show` (not `v-if`) is used on tab panes to preserve scroll position.

### KPI Banner Pattern
```html
<div class="kpi-row">
  <div class="kpi-card">
    <div class="kpi-label">Label</div>
    <div class="kpi-val c-blue">Value</div>
    <div class="kpi-sub">subtext</div>
  </div>
</div>
```
`kpi-row` uses `display: flex; gap: 1px; background: #c3c6d4` so the 1px gap shows the border color.

### Panel Card Pattern
```html
<div class="pc">
  <div class="ph">Title <span class="ph-r">right note</span></div>
  <div class="tbl-wrap"> <!-- or pc-body -->
    ...
  </div>
</div>
```
`.ph` uses `background: linear-gradient(0deg, #d7d7d7 0%, #ffffff 100%)` for the gradient header.

### Badge Pattern
```html
<span class="badge b-green">ACTIVE</span>
<!-- Classes: b-green / b-blue / b-yellow / b-red / b-grey -->
```

### Timer Cleanup Pattern (all components)
```ts
let timer: ReturnType<typeof setInterval> | null = null
onMounted(() => { timer = setInterval(fn, ms) })
onBeforeUnmount(() => { if (timer) clearInterval(timer) })
```

### Live Clock Pattern (ShopFloorMonitoring, PreventiveMaintenance, MesWesOverview)
```ts
const clock = ref('')
function tick() {
  clock.value = new Date().toLocaleTimeString('en-GB', { hour: '2-digit', minute: '2-digit', second: '2-digit' })
}
onMounted(() => { tick(); clockTimer = setInterval(tick, 1000) })
```

### Random Value Helper (used in OeeDashboard, ShopFloorMonitoring, MesWesOverview)
```ts
function rnd(base: number, delta: number, dec: number): number {
  const v = base + (Math.random() * 2 - 1) * delta
  return dec > 0 ? parseFloat(v.toFixed(dec)) : Math.round(v)
}
```

---

## C# Controllers Summary

All in `namespace ConceptHias.Controllers`, all `[ApiController]`. Reference only — not in a `.csproj`.

| File | Route | Key Endpoints |
|------|-------|---------------|
| `ConceptHiasShellController.cs` | `api/concept/shell` | `GET /menus`, `GET /page-map` |
| `ConceptOeeMainController.cs` | `api/concept/oee-main` | `GET /tabs` |
| `ConceptOeeController.cs` | `api/concept/oee` | `GET ?line=ffs\|batch` |
| `ConceptShopFloorController.cs` | `api/concept/shop-floor` | `GET ?tab=indoor\|outdoor` |
| `ConceptPreventiveMaintenanceController.cs` | `api/concept/pm` | `GET ?line=ffs\|batch` |
| `ConceptMesWesController.cs` | `api/concept/mes-wes` | `GET ?view=both\|mes\|wes` |
| `ConceptFleetController.cs` | `api/concept/fleet` | `GET ?line=ffs\|batch` |
| `ConceptInventoryController.cs` | `api/concept/inventory` | `GET /overview`, `/stock`, `/movements` |
| `ConceptCostAnalysisController.cs` | `api/concept/cost-analysis` | `GET /overview`, `/material`, `/overhead` |

---

## Conversion History

| Original | Converted To | Date |
|----------|-------------|------|
| `hias-mockup.html` + `hias-layout.js` | `HiasMockup.vue` + `ConceptHiasShellController.cs` | 2026-05-04 |
| `oee-main.html` | `OeeMain.vue` + `ConceptOeeMainController.cs` | 2026-05-04 |
| `fleet-dashboard-2.html` | `FleetDashboard2.vue` (reuses `ConceptFleetController.cs`) | 2026-05-04 |
| `oee-dashboard.html` | `OeeDashboard.vue` + `ConceptOeeController.cs` | Prior |
| `factory-floor-monitoring.html` | `ShopFloorMonitoring.vue` + `ConceptShopFloorController.cs` | Prior |
| `preventive-maintenance.html` | `PreventiveMaintenance.vue` + `ConceptPreventiveMaintenanceController.cs` | Prior |
| `mes-wes-overview.html` | `MesWesOverview.vue` + `ConceptMesWesController.cs` | Prior |
| `fleet-dashboard.html` | `FleetDashboard.vue` + `ConceptFleetController.cs` | Prior |
| `inventory.html` | `Inventory.vue` + `ConceptInventoryController.cs` | Prior |
| `cost-analysis.html` | `CostAnalysis.vue` + `ConceptCostAnalysisController.cs` | Prior |
| `hias-shell.css` + `hias-dark.css` | Absorbed into each `<style scoped>` | 2026-05-04 |
