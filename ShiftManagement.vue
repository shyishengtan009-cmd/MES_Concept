<template>
  <div class="root">

    <!-- Topbar -->
    <div class="topbar">
      <span class="pg-title">&#9632; Shift Management</span>
      <div class="topbar-right">
        <span class="live-clock"><i class="fa-solid fa-clock"></i> {{ clock }}</span>
        <button class="btn-primary"><i class="fa-solid fa-plus"></i> New Shift</button>
        <button class="btn-outline-sm"><i class="fa-solid fa-file-export"></i> Export</button>
      </div>
    </div>

    <!-- Shift Cards -->
    <div class="shift-cards">
      <div
        v-for="s in shifts" :key="s.id"
        class="shift-card" :class="{ 'shift-card--active': s.status === 'Active' }"
      >
        <div class="sc-top">
          <div class="sc-name">{{ s.name }}</div>
          <span class="badge" :class="shiftCls(s.status)">{{ s.status }}</span>
        </div>
        <div class="sc-time"><i class="fa-regular fa-clock"></i> {{ s.start }} – {{ s.end }}</div>
        <div class="sc-sup"><i class="fa-solid fa-user-tie"></i> {{ s.supervisor }}</div>
        <div class="sc-lines"><i class="fa-solid fa-industry"></i> {{ s.lines }}</div>
        <div class="sc-bottom">
          <div class="sc-stat">
            <div class="sc-stat-val c-blue">{{ s.operators }}</div>
            <div class="sc-stat-lbl">Operators</div>
          </div>
          <div class="sc-stat">
            <div class="sc-stat-val c-green">{{ s.present }}</div>
            <div class="sc-stat-lbl">Present</div>
          </div>
          <div class="sc-stat">
            <div class="sc-stat-val c-red">{{ s.absent }}</div>
            <div class="sc-stat-lbl">Absent</div>
          </div>
          <div class="sc-stat">
            <div class="sc-stat-val" :class="s.output > 0 ? 'c-blue' : 'c-grey'">{{ s.output > 0 ? s.output.toLocaleString() : '–' }}</div>
            <div class="sc-stat-lbl">Output (pcs)</div>
          </div>
        </div>
      </div>
    </div>

    <!-- Main content -->
    <div class="content">

      <!-- Left: Operator Roster -->
      <div class="col-left">
        <div class="pc">
          <div class="ph">
            Operator Roster — Current Shift (Morning)
            <span class="ph-r">
              <select class="sel-sm" v-model="rosterShift">
                <option>Morning</option><option>Afternoon</option><option>Night</option>
              </select>
            </span>
          </div>
          <div class="tbl-wrap">
            <table class="tbl">
              <thead>
                <tr>
                  <th>#</th>
                  <th>Employee</th>
                  <th>Role</th>
                  <th>Line</th>
                  <th>Shift Hours</th>
                  <th>Attendance</th>
                  <th>Remarks</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="(op, i) in currentRoster" :key="op.id" class="tbl-row">
                  <td>{{ i + 1 }}</td>
                  <td>
                    <div class="op-name">{{ op.name }}</div>
                    <div class="op-id">{{ op.empId }}</div>
                  </td>
                  <td><span class="role-tag" :class="roleCls(op.role)">{{ op.role }}</span></td>
                  <td>{{ op.line }}</td>
                  <td class="tc">{{ op.hours }}</td>
                  <td><span class="badge" :class="attCls(op.attendance)">{{ op.attendance }}</span></td>
                  <td class="rem-cell">{{ op.remark || '—' }}</td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>

      <!-- Right: Handover + Weekly Schedule -->
      <div class="col-right">

        <!-- Shift Handover Log -->
        <div class="pc">
          <div class="ph">Shift Handover Log <span class="ph-r">22/04/2026 — Morning → Afternoon</span></div>
          <div class="handover-body">
            <div v-for="h in handover" :key="h.line" class="ho-section">
              <div class="ho-line-header">
                <span class="ho-line">{{ h.line }}</span>
                <span class="badge" :class="h.statusCls">{{ h.statusLabel }}</span>
              </div>
              <div class="ho-notes">
                <div v-for="n in h.notes" :key="n.text" class="ho-note">
                  <span class="ho-dot" :class="n.dot"></span>
                  <span class="ho-text">{{ n.text }}</span>
                </div>
              </div>
              <div v-if="h.action" class="ho-action">
                <i class="fa-solid fa-arrow-right"></i> <b>Action required:</b> {{ h.action }}
              </div>
            </div>
          </div>
          <div class="ho-footer">
            <div class="ho-sig">
              <span class="sig-lbl">Handed over by:</span>
              <span class="sig-name">Ahmad Razif (Shift Supervisor)</span>
            </div>
            <div class="ho-sig">
              <span class="sig-lbl">Received by:</span>
              <span class="sig-name">Nurul Hana (Shift Supervisor)</span>
            </div>
          </div>
        </div>

        <!-- Weekly Shift Schedule -->
        <div class="pc" style="margin-top:8px">
          <div class="ph">Weekly Shift Schedule <span class="ph-r">21 Apr – 27 Apr 2026</span></div>
          <div class="sched-wrap">
            <table class="sched-tbl">
              <thead>
                <tr>
                  <th>Supervisor</th>
                  <th v-for="d in weekDays" :key="d">{{ d }}</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="sup in supervisors" :key="sup.name">
                  <td class="sup-cell">
                    <div class="sup-name">{{ sup.name }}</div>
                    <div class="sup-role">{{ sup.shift }}</div>
                  </td>
                  <td v-for="d in weekDays" :key="d" class="sched-cell">
                    <span v-if="sup.days.includes(d)" class="sched-dot" :class="sup.cls"></span>
                    <span v-else class="sched-off">OFF</span>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
          <div class="sched-legend">
            <span class="leg-item"><span class="sched-dot dot-morning"></span>Morning</span>
            <span class="leg-item"><span class="sched-dot dot-afternoon"></span>Afternoon</span>
            <span class="leg-item"><span class="sched-dot dot-night"></span>Night</span>
          </div>
        </div>

      </div>
    </div>

  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onBeforeUnmount } from 'vue'

const clock      = ref('')
const rosterShift = ref('Morning')
let clockTimer: ReturnType<typeof setInterval> | null = null

onMounted(() => {
  function tick() {
    clock.value = new Date().toLocaleTimeString('en-GB', { hour:'2-digit', minute:'2-digit', second:'2-digit' })
  }
  tick()
  clockTimer = setInterval(tick, 1000)
})
onBeforeUnmount(() => { if (clockTimer) clearInterval(clockTimer) })

const shifts = [
  { id:1, name:'Morning Shift',   status:'Active',   start:'06:00', end:'14:00', supervisor:'Ahmad Razif',     lines:'Line 1, Line 2',             operators:12, present:11, absent:1, output:3960 },
  { id:2, name:'Afternoon Shift', status:'Upcoming', start:'14:00', end:'22:00', supervisor:'Nurul Hana',      lines:'Line 1, Batch Plant 01',     operators:10, present:10, absent:0, output:0    },
  { id:3, name:'Night Shift',     status:'Upcoming', start:'22:00', end:'06:00', supervisor:'Hafiz Zulkifli', lines:'Batch Plant 02',              operators:8,  present:8,  absent:0, output:0    },
]

const rosterMap: Record<string, Array<{ id:number; name:string; empId:string; role:string; line:string; hours:string; attendance:string; remark:string }>> = {
  Morning: [
    { id:1,  name:'Ahmad bin Kadir',     empId:'E-0142', role:'Operator',     line:'Line 1',        hours:'06:00–14:00', attendance:'Present', remark:''                    },
    { id:2,  name:'Siti Norzila',        empId:'E-0195', role:'QC Inspector', line:'Line 1',        hours:'06:00–14:00', attendance:'Present', remark:''                    },
    { id:3,  name:'Rajan Krishnan',      empId:'E-0208', role:'Operator',     line:'Line 1',        hours:'06:00–14:00', attendance:'Present', remark:''                    },
    { id:4,  name:'Mohd Faizal',         empId:'E-0114', role:'Technician',   line:'Line 1',        hours:'06:00–14:00', attendance:'Present', remark:''                    },
    { id:5,  name:'Lee Chun Hao',        empId:'E-0231', role:'Operator',     line:'Line 2',        hours:'06:00–14:00', attendance:'Present', remark:''                    },
    { id:6,  name:'Halimah Bte Yusof',   empId:'E-0167', role:'Operator',     line:'Line 2',        hours:'06:00–14:00', attendance:'Present', remark:''                    },
    { id:7,  name:'Selvam Ramasamy',     empId:'E-0188', role:'Operator',     line:'Line 2',        hours:'06:00–14:00', attendance:'Present', remark:''                    },
    { id:8,  name:'Nurul Ain',           empId:'E-0204', role:'QC Inspector', line:'Line 2',        hours:'06:00–14:00', attendance:'Present', remark:''                    },
    { id:9,  name:'Zulkifli Hamdan',     empId:'E-0119', role:'Operator',     line:'Batch Plant 01',hours:'06:00–14:00', attendance:'Present', remark:''                    },
    { id:10, name:'Chong Wei Liang',     empId:'E-0176', role:'Operator',     line:'Batch Plant 01',hours:'06:00–14:00', attendance:'Present', remark:''                    },
    { id:11, name:'Suriani Mokhtar',     empId:'E-0133', role:'Operator',     line:'Batch Plant 02',hours:'06:00–14:00', attendance:'Absent',  remark:'MC (medical cert)'   },
    { id:12, name:'Ahmad Razif',         empId:'E-0102', role:'Supervisor',   line:'All Lines',     hours:'06:00–14:00', attendance:'Present', remark:'Shift Supervisor'    },
  ],
  Afternoon: [
    { id:13, name:'Nurul Hana',          empId:'E-0109', role:'Supervisor',   line:'All Lines',     hours:'14:00–22:00', attendance:'Scheduled', remark:'Incoming Supervisor' },
    { id:14, name:'Azman Othman',        empId:'E-0147', role:'Operator',     line:'Line 1',        hours:'14:00–22:00', attendance:'Scheduled', remark:'' },
    { id:15, name:'Krishnaveni D/O Ramu',empId:'E-0213', role:'QC Inspector', line:'Line 1',        hours:'14:00–22:00', attendance:'Scheduled', remark:'' },
    { id:16, name:'Tan Boon Seng',       empId:'E-0186', role:'Operator',     line:'Line 2',        hours:'14:00–22:00', attendance:'Scheduled', remark:'' },
    { id:17, name:'Mazlinda Ramlan',     empId:'E-0220', role:'Operator',     line:'Batch Plant 01',hours:'14:00–22:00', attendance:'Scheduled', remark:'' },
  ],
  Night: [
    { id:18, name:'Hafiz Zulkifli',      empId:'E-0128', role:'Supervisor',   line:'All Lines',     hours:'22:00–06:00', attendance:'Scheduled', remark:'Night Shift Supervisor' },
    { id:19, name:'Bahari Ismail',       empId:'E-0155', role:'Operator',     line:'Batch Plant 02',hours:'22:00–06:00', attendance:'Scheduled', remark:'' },
    { id:20, name:'Rohani Mansor',       empId:'E-0162', role:'QC Inspector', line:'Batch Plant 02',hours:'22:00–06:00', attendance:'Scheduled', remark:'' },
  ],
}

const currentRoster = computed(() => rosterMap[rosterShift.value] ?? [])

const handover = [
  {
    line:'Line 1', statusLabel:'Running — WO in progress', statusCls:'b-blue',
    notes:[
      { text:'WO-2026-041 (Halal Seasoning Blend A) at 82% — 1,960 / 2,400 pcs produced', dot:'dot-blue'   },
      { text:'Fill weight deviation alarm raised at 09:14 — calibration adjusted, cleared at 09:36', dot:'dot-amber'  },
      { text:'Machine 04 jaw fault (E-42) — repaired, back online at 11:02', dot:'dot-red'   },
      { text:'No material shortage — sufficient stock for remaining 440 pcs', dot:'dot-green' },
    ],
    action: 'Complete WO-041 first (440 pcs remaining), then set up Line 1 for WO-044 (Marinade Sauce).',
  },
  {
    line:'Line 2', statusLabel:'Idle — Awaiting Material', statusCls:'b-amber',
    notes:[
      { text:'WO-2026-042 (Marinade Sauce) pending — Palm Olein drum exhausted at 11:45', dot:'dot-red'   },
      { text:'Resupply drum ETA 14:30 — confirmed with warehouse (Ref: GRN-00228)', dot:'dot-blue' },
      { text:'Machine pre-cleaned and set up for product changeover', dot:'dot-green' },
    ],
    action: 'Receive and inspect GRN-00228 on arrival. Start WO-042 after material clearance.',
  },
  {
    line:'Batch Plant 01', statusLabel:'QC Hold', statusCls:'b-red',
    notes:[
      { text:'WO-2026-040 (Palm Olein Oil) on HOLD since 21/04 11:15', dot:'dot-red'   },
      { text:'FFA level 0.38% — spec max 0.25%. Re-sampling submitted to lab at 08:30', dot:'dot-red'   },
      { text:'Supplier (Golden Palm Bhd) notified — awaiting disposition decision', dot:'dot-amber' },
    ],
    action: 'Do NOT release WO-040 until lab result received. Escalate to QA Manager if no result by 16:00.',
  },
]

const weekDays = ['Mon','Tue','Wed','Thu','Fri','Sat','Sun']
const supervisors = [
  { name:'Ahmad Razif',     shift:'Morning (06:00–14:00)',   days:['Mon','Tue','Wed','Thu','Fri'],        cls:'dot-morning'   },
  { name:'Nurul Hana',      shift:'Afternoon (14:00–22:00)', days:['Mon','Tue','Wed','Thu','Fri','Sat'],  cls:'dot-afternoon' },
  { name:'Hafiz Zulkifli',  shift:'Night (22:00–06:00)',     days:['Mon','Tue','Wed','Fri','Sat','Sun'],  cls:'dot-night'     },
  { name:'Roslinda Bt Said',shift:'Morning (06:00–14:00)',   days:['Sat','Sun'],                          cls:'dot-morning'   },
]

function shiftCls(s: string) { return { 'b-green': s==='Active', 'b-blue': s==='Upcoming', 'b-grey': s==='Completed' } }
function roleCls(r: string)  { return { 'rt-sup': r==='Supervisor', 'rt-qc': r==='QC Inspector', 'rt-tech': r==='Technician', 'rt-op': r==='Operator' } }
function attCls(a: string)   { return { 'b-green': a==='Present', 'b-red': a==='Absent', 'b-blue': a==='Scheduled' } }
</script>

<style scoped>
*, *::before, *::after { box-sizing: border-box; }
.root { height: 100%; display: flex; flex-direction: column; font-family: 'Poppins', sans-serif; font-size: 12px; background: #f5f5f5; overflow: hidden; }

.topbar { background: #fff; border-bottom: 1px solid #c3c6d4; padding: 8px 14px; display: flex; align-items: center; flex-shrink: 0; }
.pg-title { font-size: 12px; font-weight: 700; text-transform: uppercase; color: #515151; }
.topbar-right { display: flex; align-items: center; gap: 8px; margin-left: auto; }
.live-clock { color: #1565c0; font-size: 12px; font-weight: 600; display: flex; align-items: center; gap: 5px; }
.btn-primary { background: #1565c0; border: none; border-radius: 3px; color: #fff; cursor: pointer; font-family: 'Poppins', sans-serif; font-size: 11px; gap: 5px; display: flex; align-items: center; padding: 5px 12px; }
.btn-primary:hover { background: #1976d2; }
.btn-outline-sm { background: #fff; border: 1px solid #d0d3e0; border-radius: 3px; color: #515151; cursor: pointer; font-family: 'Poppins', sans-serif; font-size: 11px; gap: 5px; display: flex; align-items: center; padding: 5px 10px; }
.btn-outline-sm:hover { background: #f5f5f5; }

/* Shift Cards */
.shift-cards { background: #c3c6d4; display: flex; gap: 1px; flex-shrink: 0; }
.shift-card { background: #fff; flex: 1; padding: 12px 14px; border-top: 3px solid transparent; }
.shift-card--active { border-top-color: #1565c0; background: #f0f7ff; }
.sc-top  { align-items: center; display: flex; gap: 8px; margin-bottom: 6px; }
.sc-name { font-size: 13px; font-weight: 700; color: #333; flex: 1; }
.sc-time, .sc-sup, .sc-lines { color: #515151; font-size: 11px; margin-bottom: 3px; display: flex; align-items: center; gap: 5px; }
.sc-time i, .sc-sup i, .sc-lines i { color: #9e9e9e; font-size: 11px; }
.sc-bottom { background: #c3c6d4; display: flex; gap: 1px; margin-top: 10px; }
.sc-stat { background: #f5f5f5; flex: 1; padding: 6px 8px; text-align: center; }
.shift-card--active .sc-stat { background: #e3f2fd; }
.sc-stat-val { font-size: 16px; font-weight: 700; }
.sc-stat-lbl { font-size: 9px; color: #757575; margin-top: 1px; }
.c-blue  { color: #1565c0; } .c-green { color: #388E3C; }
.c-red   { color: #e53935; } .c-grey  { color: #9e9e9e; }

/* Main content */
.content  { flex: 1; display: flex; gap: 8px; overflow: hidden; padding: 8px; }
.col-left  { flex: 3; display: flex; flex-direction: column; overflow: hidden; }
.col-right { flex: 2; display: flex; flex-direction: column; overflow: hidden; }

.pc { background: #fff; border: 1px solid #c3c6d4; border-radius: 3px; display: flex; flex-direction: column; overflow: hidden; flex: 1; }
.ph { background: linear-gradient(0deg,#d7d7d7,#fff); border-bottom: 1px solid #c3c6d4; font-size: 11px; font-weight: 700; padding: 6px 10px; flex-shrink: 0; display: flex; align-items: center; justify-content: space-between; }
.ph-r { color: #9e9e9e; font-weight: 400; font-size: 10px; }
.sel-sm { border: 1px solid #d0d3e0; border-radius: 3px; font-family: 'Poppins', sans-serif; font-size: 10px; padding: 2px 6px; color: #515151; }

.tbl-wrap { flex: 1; overflow: auto; }
.tbl { border-collapse: collapse; width: 100%; }
.tbl thead th { background: #f5f5f5; border-bottom: 1px solid #c3c6d4; color: #515151; font-size: 10px; font-weight: 600; padding: 6px 8px; position: sticky; text-align: left; text-transform: uppercase; top: 0; white-space: nowrap; }
.tbl tbody td { border-bottom: 1px solid #f0f0f0; color: #333; font-size: 11px; padding: 6px 8px; }
.tbl-row:hover { background: #f5f5f5; }
.tc { text-align: center; }
.op-name { font-weight: 600; color: #333; }
.op-id   { color: #9e9e9e; font-size: 10px; font-family: monospace; }
.rem-cell { color: #757575; font-size: 10px; }

.role-tag { border-radius: 3px; font-size: 10px; font-weight: 600; padding: 2px 6px; white-space: nowrap; }
.rt-sup  { background: #f3e5f5; color: #7b1fa2; }
.rt-qc   { background: #fff8e1; color: #f9a825; }
.rt-tech { background: #e3f2fd; color: #1565c0; }
.rt-op   { background: #f5f5f5; color: #515151; }

.badge { border-radius: 3px; font-size: 10px; font-weight: 600; padding: 2px 8px; white-space: nowrap; }
.b-green { background: #e8f5e9; color: #388E3C; }
.b-blue  { background: #e3f2fd; color: #1565c0; }
.b-amber { background: #fff8e1; color: #f9a825; }
.b-red   { background: #ffebee; color: #e53935; }
.b-grey  { background: #f5f5f5; color: #757575; }

/* Handover */
.handover-body { flex: 1; overflow-y: auto; padding: 10px 12px; display: flex; flex-direction: column; gap: 12px; }
.ho-section { border: 1px solid #e0e0e0; border-radius: 4px; overflow: hidden; }
.ho-line-header { align-items: center; background: #f5f5f5; border-bottom: 1px solid #e0e0e0; display: flex; gap: 8px; padding: 6px 10px; }
.ho-line { font-size: 11px; font-weight: 700; color: #333; flex: 1; }
.ho-notes { display: flex; flex-direction: column; gap: 5px; padding: 8px 10px; }
.ho-note  { align-items: flex-start; display: flex; gap: 6px; }
.ho-dot   { border-radius: 50%; flex-shrink: 0; height: 7px; margin-top: 4px; width: 7px; }
.dot-blue  { background: #1565c0; } .dot-green { background: #388E3C; }
.dot-red   { background: #e53935; } .dot-amber { background: #f9a825; }
.ho-text  { color: #515151; font-size: 11px; }
.ho-action { background: #fff8e1; border-top: 1px solid #f9a825; color: #515151; font-size: 11px; padding: 6px 10px; }
.ho-footer { border-top: 1px solid #c3c6d4; display: flex; gap: 20px; flex-shrink: 0; padding: 8px 12px; background: #fafafa; }
.ho-sig   { display: flex; flex-direction: column; gap: 2px; }
.sig-lbl  { color: #9e9e9e; font-size: 10px; text-transform: uppercase; }
.sig-name { color: #333; font-size: 11px; font-weight: 600; }

/* Weekly Schedule */
.col-right .pc:last-child { flex: 0 0 auto; }
.sched-wrap { overflow: auto; }
.sched-tbl  { border-collapse: collapse; width: 100%; }
.sched-tbl thead th { background: #f5f5f5; border-bottom: 1px solid #c3c6d4; font-size: 10px; font-weight: 600; padding: 6px 8px; text-align: center; text-transform: uppercase; white-space: nowrap; }
.sched-tbl thead th:first-child { text-align: left; }
.sched-tbl tbody td { border-bottom: 1px solid #f0f0f0; font-size: 11px; padding: 6px 8px; text-align: center; }
.sup-cell { text-align: left !important; }
.sup-name { font-weight: 600; color: #333; font-size: 11px; }
.sup-role { color: #9e9e9e; font-size: 10px; }
.sched-dot { border-radius: 50%; display: inline-block; height: 12px; width: 12px; }
.dot-morning   { background: #1565c0; }
.dot-afternoon { background: #388E3C; }
.dot-night     { background: #7b1fa2; }
.sched-off { color: #d0d3e0; font-size: 10px; }
.sched-legend { border-top: 1px solid #f0f0f0; display: flex; gap: 14px; padding: 6px 10px; flex-shrink: 0; }
.leg-item { align-items: center; display: flex; gap: 5px; font-size: 10px; color: #515151; }
</style>
