<template>
  <div class="sfm-root">

    <!-- TOPBAR -->
    <div class="top-bar">
      <span class="page-title">&#9632; Shop Floor Monitoring</span>
      <div class="line-tabs">
        <button class="line-tab" :class="{ active: activeTab === 'indoor'  }" @click="activeTab = 'indoor'">&#9654; FFS Packaging Line (Indoors)</button>
        <button class="line-tab" :class="{ active: activeTab === 'outdoor' }" @click="activeTab = 'outdoor'">&#9654; Batching Plant (Outdoors)</button>
      </div>
      <div class="top-bar-right">
        <span class="live-dot"></span>
        <span>LIVE &nbsp;|&nbsp; {{ clock }}</span>
      </div>
    </div>

    <!-- ══ TAB: INDOORS ══ -->
    <div class="tab-pane" v-show="activeTab === 'indoor'">
      <div class="kpi-row">
        <div class="kpi-card"><div class="kpi-label">FFS Machines</div><div class="kpi-value kpi-white">4</div><div class="kpi-sub">on floor</div></div>
        <div class="kpi-card"><div class="kpi-label">Active</div><div class="kpi-value kpi-green">3</div><div class="kpi-sub">running</div></div>
        <div class="kpi-card"><div class="kpi-label">Idle</div><div class="kpi-value kpi-yellow">0</div><div class="kpi-sub">standby</div></div>
        <div class="kpi-card"><div class="kpi-label">Fault</div><div class="kpi-value kpi-red">1</div><div class="kpi-sub">requires attention</div></div>
        <div class="kpi-card"><div class="kpi-label">Output Today</div><div class="kpi-value kpi-blue">{{ inKpi.output.toLocaleString() }}</div><div class="kpi-sub">packs produced</div></div>
        <div class="kpi-card"><div class="kpi-label">Active Alarms</div><div class="kpi-value kpi-red">2</div><div class="kpi-sub">unacknowledged</div></div>
        <div class="kpi-card"><div class="kpi-label">Avg Fill Weight</div><div class="kpi-value kpi-blue">{{ inKpi.fillWt }}</div><div class="kpi-sub">g target: 250 g</div></div>
      </div>

      <div class="main-body">
        <div class="floor-panel">
          <div class="floor-panel-title">&#9632; Floor Plan — FFS Packaging Line &nbsp;<span style="font-weight:400;color:#9e9e9e;">(click a zone to inspect)</span></div>
          <div class="floor-svg-wrap">
            <svg viewBox="0 0 700 420" xmlns="http://www.w3.org/2000/svg" style="width:100%;height:100%;">
              <rect x="10" y="10" width="680" height="400" fill="#eef0f2" stroke="#9e9e9e" stroke-width="3" rx="4"/>
              <line x1="10" y1="212" x2="690" y2="212" stroke="#bdbdbd" stroke-width="2"/>
              <!-- ZONE A -->
              <rect x="18" y="18" width="664" height="186" fill="#f1f8e9" stroke="#2e7d32" stroke-width="1.5" rx="2"/>
              <text x="28" y="33" font-size="10" fill="#2e7d32" font-weight="700" letter-spacing="0.4">ZONE A — FFS MACHINE 01 &amp; 02</text>
              <line x1="350" y1="22" x2="350" y2="198" stroke="#a5d6a7" stroke-width="1" stroke-dasharray="4,3"/>
              <!-- M01 -->
              <rect x="26" y="40" width="314" height="158" fill="#e8f5e9" stroke="#43a047" stroke-width="1" rx="2"/>
              <text x="36" y="55" font-size="9" fill="#1b5e20" font-weight="700">FFS Machine 01</text>
              <circle cx="328" cy="50" r="6" fill="#43a047"/>
              <rect x="34" y="62" width="138" height="34" fill="#f5f9f5" stroke="#a5d6a7" stroke-width="1" rx="2"/>
              <text x="42" y="74" font-size="8" fill="#7f7f7f">Fill Weight</text>
              <text x="42" y="87" font-size="13" font-weight="700" fill="#1565c0">{{ m01.fw }} g</text>
              <rect x="180" y="62" width="138" height="34" fill="#f5f9f5" stroke="#a5d6a7" stroke-width="1" rx="2"/>
              <text x="188" y="74" font-size="8" fill="#7f7f7f">Seal Temp (H-jaw)</text>
              <text x="188" y="87" font-size="13" font-weight="700" fill="#e65100">{{ m01.st }} °C</text>
              <rect x="34" y="103" width="138" height="34" fill="#f5f9f5" stroke="#a5d6a7" stroke-width="1" rx="2"/>
              <text x="42" y="115" font-size="8" fill="#7f7f7f">Screw Feed Rate</text>
              <text x="42" y="128" font-size="13" font-weight="700" fill="#1565c0">{{ m01.fr }} kg/h</text>
              <rect x="180" y="103" width="138" height="34" fill="#f5f9f5" stroke="#a5d6a7" stroke-width="1" rx="2"/>
              <text x="188" y="115" font-size="8" fill="#7f7f7f">Motor Current</text>
              <text x="188" y="128" font-size="13" font-weight="700" fill="#1565c0">{{ m01.mc }} A</text>
              <text x="36" y="152" font-size="9" fill="#388E3C">Packs today: <tspan font-weight="700">4,820</tspan></text>
              <text x="36" y="165" font-size="9" fill="#388E3C">Rejects: <tspan font-weight="700">3</tspan> &nbsp;|&nbsp; Film: <tspan font-weight="700">342 m</tspan></text>
              <text x="36" y="178" font-size="9" fill="#388E3C">Status: <tspan fill="#2e7d32" font-weight="700">RUNNING</tspan></text>
              <circle cx="328" cy="150" r="5" fill="#43a047" stroke="#fff" stroke-width="1" opacity="0.9"/>
              <circle cx="328" cy="150" r="5" fill="none" stroke="#43a047" stroke-width="1">
                <animate attributeName="r" values="5;12;5" dur="2s" repeatCount="indefinite"/>
                <animate attributeName="opacity" values="0.9;0;0.9" dur="2s" repeatCount="indefinite"/>
              </circle>
              <!-- M02 -->
              <rect x="358" y="40" width="314" height="158" fill="#e8f5e9" stroke="#43a047" stroke-width="1" rx="2"/>
              <text x="368" y="55" font-size="9" fill="#1b5e20" font-weight="700">FFS Machine 02</text>
              <circle cx="662" cy="50" r="6" fill="#43a047"/>
              <rect x="366" y="62" width="138" height="34" fill="#f5f9f5" stroke="#a5d6a7" stroke-width="1" rx="2"/>
              <text x="374" y="74" font-size="8" fill="#7f7f7f">Fill Weight</text>
              <text x="374" y="87" font-size="13" font-weight="700" fill="#1565c0">{{ m02.fw }} g</text>
              <rect x="512" y="62" width="148" height="34" fill="#f5f9f5" stroke="#a5d6a7" stroke-width="1" rx="2"/>
              <text x="520" y="74" font-size="8" fill="#7f7f7f">Seal Temp (H-jaw)</text>
              <text x="520" y="87" font-size="13" font-weight="700" fill="#e65100">{{ m02.st }} °C</text>
              <rect x="366" y="103" width="138" height="34" fill="#f5f9f5" stroke="#a5d6a7" stroke-width="1" rx="2"/>
              <text x="374" y="115" font-size="8" fill="#7f7f7f">Screw Feed Rate</text>
              <text x="374" y="128" font-size="13" font-weight="700" fill="#1565c0">{{ m02.fr }} kg/h</text>
              <rect x="512" y="103" width="148" height="34" fill="#f5f9f5" stroke="#a5d6a7" stroke-width="1" rx="2"/>
              <text x="520" y="115" font-size="8" fill="#7f7f7f">Motor Current</text>
              <text x="520" y="128" font-size="13" font-weight="700" fill="#1565c0">{{ m02.mc }} A</text>
              <text x="368" y="152" font-size="9" fill="#388E3C">Packs today: <tspan font-weight="700">4,760</tspan></text>
              <text x="368" y="165" font-size="9" fill="#388E3C">Rejects: <tspan font-weight="700">2</tspan> &nbsp;|&nbsp; Film: <tspan font-weight="700">287 m</tspan></text>
              <text x="368" y="178" font-size="9" fill="#388E3C">Status: <tspan fill="#2e7d32" font-weight="700">RUNNING</tspan></text>
              <circle cx="662" cy="150" r="5" fill="#43a047" stroke="#fff" stroke-width="1" opacity="0.9"/>
              <circle cx="662" cy="150" r="5" fill="none" stroke="#43a047" stroke-width="1">
                <animate attributeName="r" values="5;12;5" dur="2.3s" repeatCount="indefinite"/>
                <animate attributeName="opacity" values="0.9;0;0.9" dur="2.3s" repeatCount="indefinite"/>
              </circle>
              <!-- ZONE B -->
              <rect x="18" y="220" width="664" height="182" fill="#e3f2fd" stroke="#1565c0" stroke-width="1.5" rx="2"/>
              <text x="28" y="235" font-size="10" fill="#1565c0" font-weight="700" letter-spacing="0.4">ZONE B — FFS MACHINE 03 &amp; 04</text>
              <line x1="350" y1="224" x2="350" y2="396" stroke="#90caf9" stroke-width="1" stroke-dasharray="4,3"/>
              <!-- M03 -->
              <rect x="26" y="241" width="314" height="154" fill="#e3f2fd" stroke="#90caf9" stroke-width="1" rx="2"/>
              <text x="36" y="256" font-size="9" fill="#0d47a1" font-weight="700">FFS Machine 03</text>
              <circle cx="328" cy="251" r="6" fill="#43a047"/>
              <rect x="34" y="263" width="138" height="34" fill="#f5f8fd" stroke="#90caf9" stroke-width="1" rx="2"/>
              <text x="42" y="275" font-size="8" fill="#7f7f7f">Fill Weight</text>
              <text x="42" y="288" font-size="13" font-weight="700" fill="#1565c0">{{ m03.fw }} g</text>
              <rect x="180" y="263" width="138" height="34" fill="#f5f8fd" stroke="#90caf9" stroke-width="1" rx="2"/>
              <text x="188" y="275" font-size="8" fill="#7f7f7f">Seal Temp (H-jaw)</text>
              <text x="188" y="288" font-size="13" font-weight="700" fill="#e65100">{{ m03.st }} °C</text>
              <rect x="34" y="304" width="138" height="34" fill="#f5f8fd" stroke="#90caf9" stroke-width="1" rx="2"/>
              <text x="42" y="316" font-size="8" fill="#7f7f7f">Screw Feed Rate</text>
              <text x="42" y="329" font-size="13" font-weight="700" fill="#1565c0">{{ m03.fr }} kg/h</text>
              <rect x="180" y="304" width="138" height="34" fill="#f5f8fd" stroke="#90caf9" stroke-width="1" rx="2"/>
              <text x="188" y="316" font-size="8" fill="#7f7f7f">Motor Current</text>
              <text x="188" y="329" font-size="13" font-weight="700" fill="#1565c0">{{ m03.mc }} A</text>
              <text x="36" y="353" font-size="9" fill="#515151">Packs today: <tspan fill="#1565c0" font-weight="700">5,010</tspan></text>
              <text x="36" y="366" font-size="9" fill="#515151">Rejects: <tspan fill="#1565c0" font-weight="700">4</tspan> &nbsp;|&nbsp; Film: <tspan fill="#1565c0" font-weight="700">415 m</tspan></text>
              <text x="36" y="379" font-size="9" fill="#515151">Status: <tspan fill="#43a047" font-weight="700">RUNNING</tspan></text>
              <circle cx="328" cy="352" r="5" fill="#1565c0" stroke="#fff" stroke-width="1" opacity="0.9"/>
              <circle cx="328" cy="352" r="5" fill="none" stroke="#1565c0" stroke-width="1">
                <animate attributeName="r" values="5;12;5" dur="2.6s" repeatCount="indefinite"/>
                <animate attributeName="opacity" values="0.9;0;0.9" dur="2.6s" repeatCount="indefinite"/>
              </circle>
              <!-- M04 FAULT -->
              <rect x="358" y="241" width="314" height="154" fill="#ffebee" stroke="#e53935" stroke-width="1.5" rx="2"/>
              <text x="368" y="256" font-size="9" fill="#b71c1c" font-weight="700">FFS Machine 04</text>
              <circle cx="662" cy="251" r="6" fill="#e53935"/>
              <circle cx="662" cy="251" r="6" fill="none" stroke="#e53935" stroke-width="1">
                <animate attributeName="r" values="6;13;6" dur="1.5s" repeatCount="indefinite"/>
                <animate attributeName="opacity" values="0.9;0;0.9" dur="1.5s" repeatCount="indefinite"/>
              </circle>
              <rect x="366" y="263" width="138" height="34" fill="#fff5f5" stroke="#ef9a9a" stroke-width="1" rx="2"/>
              <text x="374" y="275" font-size="8" fill="#7f7f7f">Fill Weight</text>
              <text x="374" y="288" font-size="13" font-weight="700" fill="#9e9e9e">— —</text>
              <rect x="512" y="263" width="148" height="34" fill="#fff5f5" stroke="#ef9a9a" stroke-width="1" rx="2"/>
              <text x="520" y="275" font-size="8" fill="#7f7f7f">Seal Temp (H-jaw)</text>
              <text x="520" y="288" font-size="13" font-weight="700" fill="#e53935">FAULT</text>
              <rect x="366" y="304" width="138" height="34" fill="#fff5f5" stroke="#ef9a9a" stroke-width="1" rx="2"/>
              <text x="374" y="316" font-size="8" fill="#7f7f7f">Screw Feed Rate</text>
              <text x="374" y="329" font-size="13" font-weight="700" fill="#9e9e9e">— —</text>
              <rect x="512" y="304" width="148" height="34" fill="#fff5f5" stroke="#ef9a9a" stroke-width="1" rx="2"/>
              <text x="520" y="316" font-size="8" fill="#7f7f7f">Motor Current</text>
              <text x="520" y="329" font-size="13" font-weight="700" fill="#9e9e9e">0.0 A</text>
              <text x="368" y="353" font-size="9" fill="#e53935">Fault code: <tspan font-weight="700">E-42 Jaw overheat</tspan></text>
              <text x="368" y="366" font-size="9" fill="#e53935">Packs today: <tspan font-weight="700">4,050</tspan> (stopped 09:14)</text>
              <text x="368" y="379" font-size="9" fill="#e53935">Status: <tspan fill="#e53935" font-weight="700">&#9888; FAULT — STOPPED</tspan></text>
              <!-- Legend -->
              <circle cx="200" cy="412" r="4" fill="#43a047"/><text x="208" y="415" font-size="9" fill="#388E3C">Active</text>
              <circle cx="270" cy="412" r="4" fill="#f9a825"/><text x="278" y="415" font-size="9" fill="#f9a825">Idle</text>
              <circle cx="330" cy="412" r="4" fill="#e53935"/><text x="338" y="415" font-size="9" fill="#e53935">Fault</text>
              <circle cx="390" cy="412" r="4" fill="#1565c0"/><text x="398" y="415" font-size="9" fill="#1565c0">Sensor</text>
            </svg>
          </div>
        </div>

        <div class="right-panel">
          <div class="rp-card">
            <div class="rp-section-title">&#9632; Machine Status</div>
            <div class="machine-list">
              <div v-for="m in inMachines" :key="m.name" class="machine-row" :class="{ selected: inSelected === m.name }" @click="selectIn(m)">
                <div class="m-status-dot" :style="{ background: statusDot(m.status) }"></div>
                <div class="m-name">{{ m.name }}</div>
                <div class="m-zone">{{ m.zone }}</div>
                <div class="m-badge" :class="badgeClass(m.status)">{{ m.status }}</div>
              </div>
            </div>
          </div>
          <div class="rp-card" style="flex:1;overflow:hidden">
            <div class="rp-section-title">&#9632; Zone Detail</div>
            <div class="zone-detail">
              <div class="zone-detail-inner">
                <div class="zd-header">
                  <div class="zd-name">{{ inDetail.zone }} — {{ inDetail.name }}</div>
                  <div class="zd-status"><span :style="{ color: statusDot(inDetail.status), fontWeight: 700 }">&#9679; {{ inDetail.status }}</span></div>
                </div>
                <div class="zd-grid">
                  <div class="zd-metric"><div class="zd-metric-label">{{ inDetail.l1 }}</div><div class="zd-metric-val">{{ inDetail.v1 }}</div></div>
                  <div class="zd-metric"><div class="zd-metric-label">{{ inDetail.l2 }}</div><div class="zd-metric-val">{{ inDetail.v2 }}</div></div>
                  <div class="zd-metric"><div class="zd-metric-label">{{ inDetail.l3 }}</div><div class="zd-metric-val">{{ inDetail.v3 }}</div></div>
                  <div class="zd-metric"><div class="zd-metric-label">Status</div><div class="zd-metric-val">{{ inDetail.status }}</div></div>
                </div>
                <div class="event-feed">
                  <div class="event-row" v-for="(ev, i) in inEvents" :key="i">
                    <span class="ev-time">{{ ev.time }}</span>
                    <span class="ev-dot" :style="{ background: ev.color }"></span>
                    <span class="ev-text">{{ ev.text }}</span>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- ══ TAB: OUTDOORS ══ -->
    <div class="tab-pane" v-show="activeTab === 'outdoor'">
      <div class="kpi-row">
        <div class="kpi-card"><div class="kpi-label">Batching Plants</div><div class="kpi-value kpi-white">3</div><div class="kpi-sub">on site</div></div>
        <div class="kpi-card"><div class="kpi-label">Active</div><div class="kpi-value kpi-green">2</div><div class="kpi-sub">running</div></div>
        <div class="kpi-card"><div class="kpi-label">Idle</div><div class="kpi-value kpi-yellow">1</div><div class="kpi-sub">standby</div></div>
        <div class="kpi-card"><div class="kpi-label">Fault</div><div class="kpi-value kpi-red">0</div><div class="kpi-sub">requires attention</div></div>
        <div class="kpi-card"><div class="kpi-label">Batches Today</div><div class="kpi-value kpi-blue">{{ outKpi.batches }}</div><div class="kpi-sub">completed</div></div>
        <div class="kpi-card"><div class="kpi-label">Active Alarms</div><div class="kpi-value kpi-red">1</div><div class="kpi-sub">unacknowledged</div></div>
        <div class="kpi-card"><div class="kpi-label">Cement Silo</div><div class="kpi-value kpi-blue">{{ outKpi.silo }}%</div><div class="kpi-sub">fill level</div></div>
      </div>

      <div class="main-body">
        <div class="floor-panel">
          <div class="floor-panel-title">&#9632; Site Plan — Batching Plant &nbsp;<span style="font-weight:400;color:#9e9e9e;">(click a zone to inspect)</span></div>
          <div class="floor-svg-wrap">
            <svg viewBox="0 0 700 420" xmlns="http://www.w3.org/2000/svg" style="width:100%;height:100%;">
              <rect x="10" y="10" width="680" height="400" fill="#ecf0ec" stroke="#9e9e9e" stroke-width="3" rx="4"/>
              <defs>
                <pattern id="grid2" width="20" height="20" patternUnits="userSpaceOnUse">
                  <path d="M 20 0 L 0 0 0 20" fill="none" stroke="#9e9e9e" stroke-width="0.5"/>
                </pattern>
              </defs>
              <rect x="10" y="10" width="680" height="400" fill="url(#grid2)" rx="4" opacity="0.15"/>
              <rect x="10" y="352" width="680" height="58" fill="#d9d9d9" stroke="#bdbdbd" stroke-width="1"/>
              <text x="350" y="385" font-size="9" fill="#9e9e9e" text-anchor="middle" letter-spacing="2">ACCESS ROAD</text>
              <line x1="10" y1="210" x2="690" y2="210" stroke="#bdbdbd" stroke-width="2" stroke-dasharray="6,4"/>
              <!-- ZONE A -->
              <rect x="18" y="18" width="664" height="184" fill="#e8f5e9" stroke="#2e7d32" stroke-width="1.5" rx="2"/>
              <text x="28" y="33" font-size="10" fill="#2e7d32" font-weight="700" letter-spacing="0.5">ZONE A — BATCHING PLANT 01 &amp; 02</text>
              <line x1="350" y1="22" x2="350" y2="196" stroke="#a5d6a7" stroke-width="1" stroke-dasharray="4,3"/>
              <!-- BP01 -->
              <rect x="26" y="40" width="314" height="155" fill="#dcedc8" stroke="#43a047" stroke-width="1" rx="2"/>
              <text x="36" y="55" font-size="9" fill="#1b5e20" font-weight="700">Batching Plant 01</text>
              <circle cx="328" cy="50" r="6" fill="#43a047"/>
              <rect x="34" y="62" width="138" height="34" fill="#f5f9f5" stroke="#a5d6a7" stroke-width="1" rx="2"/>
              <text x="42" y="74" font-size="8" fill="#7f7f7f">Batch Weight</text>
              <text x="42" y="87" font-size="13" font-weight="700" fill="#1565c0">{{ bp01.bw.toLocaleString() }} kg</text>
              <rect x="180" y="62" width="138" height="34" fill="#f5f9f5" stroke="#a5d6a7" stroke-width="1" rx="2"/>
              <text x="188" y="74" font-size="8" fill="#7f7f7f">Mixer RPM</text>
              <text x="188" y="87" font-size="13" font-weight="700" fill="#1565c0">{{ bp01.rpm }} rpm</text>
              <rect x="34" y="103" width="138" height="34" fill="#f5f9f5" stroke="#a5d6a7" stroke-width="1" rx="2"/>
              <text x="42" y="115" font-size="8" fill="#7f7f7f">Cement Used</text>
              <text x="42" y="128" font-size="13" font-weight="700" fill="#1565c0">18,420 kg</text>
              <rect x="180" y="103" width="138" height="34" fill="#f5f9f5" stroke="#a5d6a7" stroke-width="1" rx="2"/>
              <text x="188" y="115" font-size="8" fill="#7f7f7f">Aggregate Feed</text>
              <text x="188" y="128" font-size="13" font-weight="700" fill="#1565c0">53.6 t/h</text>
              <text x="36" y="152" font-size="9" fill="#388E3C">Batches today: <tspan font-weight="700">28</tspan> &nbsp;|&nbsp; Water: <tspan font-weight="700">142 L/min</tspan></text>
              <text x="36" y="166" font-size="9" fill="#388E3C">Ambient temp: <tspan font-weight="700">28.4 °C</tspan></text>
              <text x="36" y="180" font-size="9" fill="#388E3C">Status: <tspan fill="#2e7d32" font-weight="700">RUNNING</tspan></text>
              <circle cx="328" cy="148" r="5" fill="#43a047" stroke="#fff" stroke-width="1" opacity="0.9"/>
              <circle cx="328" cy="148" r="5" fill="none" stroke="#43a047" stroke-width="1">
                <animate attributeName="r" values="5;13;5" dur="2s" repeatCount="indefinite"/>
                <animate attributeName="opacity" values="0.9;0;0.9" dur="2s" repeatCount="indefinite"/>
              </circle>
              <!-- BP02 -->
              <rect x="358" y="40" width="314" height="155" fill="#dcedc8" stroke="#43a047" stroke-width="1" rx="2"/>
              <text x="368" y="55" font-size="9" fill="#1b5e20" font-weight="700">Batching Plant 02</text>
              <circle cx="662" cy="50" r="6" fill="#43a047"/>
              <rect x="366" y="62" width="138" height="34" fill="#f5f9f5" stroke="#a5d6a7" stroke-width="1" rx="2"/>
              <text x="374" y="74" font-size="8" fill="#7f7f7f">Batch Weight</text>
              <text x="374" y="87" font-size="13" font-weight="700" fill="#1565c0">{{ bp02.bw.toLocaleString() }} kg</text>
              <rect x="512" y="62" width="148" height="34" fill="#f5f9f5" stroke="#a5d6a7" stroke-width="1" rx="2"/>
              <text x="520" y="74" font-size="8" fill="#7f7f7f">Mixer RPM</text>
              <text x="520" y="87" font-size="13" font-weight="700" fill="#1565c0">{{ bp02.rpm }} rpm</text>
              <rect x="366" y="103" width="138" height="34" fill="#f5f9f5" stroke="#a5d6a7" stroke-width="1" rx="2"/>
              <text x="374" y="115" font-size="8" fill="#7f7f7f">Cement Used</text>
              <text x="374" y="128" font-size="13" font-weight="700" fill="#1565c0">17,940 kg</text>
              <rect x="512" y="103" width="148" height="34" fill="#f5f9f5" stroke="#a5d6a7" stroke-width="1" rx="2"/>
              <text x="520" y="115" font-size="8" fill="#7f7f7f">Aggregate Feed</text>
              <text x="520" y="128" font-size="13" font-weight="700" fill="#1565c0">51.2 t/h</text>
              <text x="368" y="152" font-size="9" fill="#388E3C">Batches today: <tspan font-weight="700">19</tspan> &nbsp;|&nbsp; Water: <tspan font-weight="700">138 L/min</tspan></text>
              <text x="368" y="166" font-size="9" fill="#388E3C">Ambient temp: <tspan font-weight="700">28.4 °C</tspan></text>
              <text x="368" y="180" font-size="9" fill="#388E3C">Status: <tspan fill="#2e7d32" font-weight="700">RUNNING</tspan></text>
              <circle cx="662" cy="148" r="5" fill="#43a047" stroke="#fff" stroke-width="1" opacity="0.9"/>
              <circle cx="662" cy="148" r="5" fill="none" stroke="#43a047" stroke-width="1">
                <animate attributeName="r" values="5;13;5" dur="2.5s" repeatCount="indefinite"/>
                <animate attributeName="opacity" values="0.9;0;0.9" dur="2.5s" repeatCount="indefinite"/>
              </circle>
              <!-- ZONE B -->
              <rect x="18" y="218" width="664" height="126" fill="#fff9c4" stroke="#f9a825" stroke-width="1.5" rx="2"/>
              <text x="28" y="233" font-size="10" fill="#e65100" font-weight="700" letter-spacing="0.5">ZONE B — BATCHING PLANT 03 &amp; SUPPORT EQUIPMENT</text>
              <line x1="460" y1="222" x2="460" y2="338" stroke="#ffe082" stroke-width="1" stroke-dasharray="4,3"/>
              <!-- BP03 IDLE -->
              <rect x="26" y="240" width="424" height="96" fill="#fffde7" stroke="#ffe082" stroke-width="1" rx="2"/>
              <text x="36" y="255" font-size="9" fill="#e65100" font-weight="700">Batching Plant 03</text>
              <circle cx="440" cy="250" r="6" fill="#f9a825"/>
              <rect x="34" y="261" width="130" height="34" fill="#fffde7" stroke="#ffe082" stroke-width="1" rx="2"/>
              <text x="42" y="273" font-size="8" fill="#7f7f7f">Batch Weight</text>
              <text x="42" y="286" font-size="13" font-weight="700" fill="#9e9e9e">— —</text>
              <rect x="172" y="261" width="130" height="34" fill="#fffde7" stroke="#ffe082" stroke-width="1" rx="2"/>
              <text x="180" y="273" font-size="8" fill="#7f7f7f">Mixer RPM</text>
              <text x="180" y="286" font-size="13" font-weight="700" fill="#f9a825">0.0 rpm</text>
              <rect x="310" y="261" width="130" height="34" fill="#fffde7" stroke="#ffe082" stroke-width="1" rx="2"/>
              <text x="318" y="273" font-size="8" fill="#7f7f7f">Cement Used</text>
              <text x="318" y="286" font-size="13" font-weight="700" fill="#9e9e9e">— —</text>
              <text x="36" y="320" font-size="9" fill="#515151">Batches today: <tspan fill="#e65100" font-weight="700">0</tspan> &nbsp;|&nbsp; Awaiting work order</text>
              <text x="36" y="332" font-size="9" fill="#515151">Status: <tspan fill="#f9a825" font-weight="700">IDLE — STANDBY</tspan></text>
              <!-- Support Equipment -->
              <rect x="468" y="240" width="204" height="96" fill="#fff3e0" stroke="#ffab91" stroke-width="1" rx="2"/>
              <text x="478" y="255" font-size="9" fill="#bf360c" font-weight="700">Support Equipment</text>
              <text x="478" y="270" font-size="8" fill="#515151">Cement Silo: <tspan fill="#1565c0" font-weight="700">83%</tspan> (200 t cap)</text>
              <text x="478" y="282" font-size="8" fill="#515151">Admixture Tank: <tspan fill="#1565c0" font-weight="700">72%</tspan></text>
              <text x="478" y="294" font-size="8" fill="#515151">Water Tank: <tspan fill="#1565c0" font-weight="700">78%</tspan> (8,640 L used)</text>
              <text x="478" y="306" font-size="8" fill="#515151">Belt Conveyor: <tspan fill="#43a047" font-weight="700">RUNNING</tspan></text>
              <text x="478" y="318" font-size="8" fill="#515151">Truck Dispatch: <tspan fill="#1565c0" font-weight="700">12 loads</tspan> today</text>
              <text x="478" y="330" font-size="8" fill="#515151">Ambient Temp: <tspan fill="#1565c0" font-weight="700">28.4 °C</tspan></text>
              <!-- Legend -->
              <circle cx="200" cy="412" r="4" fill="#43a047"/><text x="208" y="415" font-size="9" fill="#388E3C">Active</text>
              <circle cx="270" cy="412" r="4" fill="#f9a825"/><text x="278" y="415" font-size="9" fill="#f9a825">Idle</text>
              <circle cx="330" cy="412" r="4" fill="#e53935"/><text x="338" y="415" font-size="9" fill="#e53935">Fault</text>
              <circle cx="390" cy="412" r="4" fill="#1565c0"/><text x="398" y="415" font-size="9" fill="#1565c0">Sensor</text>
            </svg>
          </div>
        </div>

        <div class="right-panel">
          <div class="rp-card">
            <div class="rp-section-title">&#9632; Equipment Status</div>
            <div class="machine-list">
              <div v-for="m in outMachines" :key="m.name" class="machine-row" :class="{ selected: outSelected === m.name }" @click="selectOut(m)">
                <div class="m-status-dot" :style="{ background: statusDot(m.status) }"></div>
                <div class="m-name">{{ m.name }}</div>
                <div class="m-zone">{{ m.zone }}</div>
                <div class="m-badge" :class="badgeClass(m.status)">{{ m.status }}</div>
              </div>
            </div>
          </div>
          <div class="rp-card" style="flex:1;overflow:hidden">
            <div class="rp-section-title">&#9632; Zone Detail</div>
            <div class="zone-detail">
              <div class="zone-detail-inner">
                <div class="zd-header">
                  <div class="zd-name">{{ outDetail.zone }} — {{ outDetail.name }}</div>
                  <div class="zd-status"><span :style="{ color: statusDot(outDetail.status), fontWeight: 700 }">&#9679; {{ outDetail.status }}</span></div>
                </div>
                <div class="zd-grid">
                  <div class="zd-metric"><div class="zd-metric-label">{{ outDetail.l1 }}</div><div class="zd-metric-val">{{ outDetail.v1 }}</div></div>
                  <div class="zd-metric"><div class="zd-metric-label">{{ outDetail.l2 }}</div><div class="zd-metric-val">{{ outDetail.v2 }}</div></div>
                  <div class="zd-metric"><div class="zd-metric-label">{{ outDetail.l3 }}</div><div class="zd-metric-val">{{ outDetail.v3 }}</div></div>
                  <div class="zd-metric"><div class="zd-metric-label">Status</div><div class="zd-metric-val">{{ outDetail.status }}</div></div>
                </div>
                <div class="event-feed">
                  <div class="event-row" v-for="(ev, i) in outEvents" :key="i">
                    <span class="ev-time">{{ ev.time }}</span>
                    <span class="ev-dot" :style="{ background: ev.color }"></span>
                    <span class="ev-text">{{ ev.text }}</span>
                  </div>
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
import { ref, reactive, onMounted, onBeforeUnmount } from 'vue'
import { mqttSubscribe, mqttUnsubscribe } from './src/mqttService'

const activeTab = ref<'indoor' | 'outdoor'>('indoor')
const clock     = ref('')
let clockTimer: ReturnType<typeof setInterval> | null = null
let liveTimer:  ReturnType<typeof setInterval> | null = null

function tick() {
  clock.value = new Date().toLocaleTimeString('en-GB', { hour: '2-digit', minute: '2-digit', second: '2-digit' })
}

const inKpi  = reactive({ output: 18640, fillWt: '250.0' })
const outKpi = reactive({ batches: 47, silo: 83 })

const m01 = reactive({ fw: '250.4', st: '182.5', fr: '48.2', mc: '14.3' })
const m02 = reactive({ fw: '249.8', st: '181.0', fr: '47.9', mc: '13.8' })
const m03 = reactive({ fw: '251.2', st: '183.2', fr: '49.0', mc: '15.1' })
const bp01 = reactive({ bw: 2450, rpm: '18.4' })
const bp02 = reactive({ bw: 2380, rpm: '17.8' })

function rnd(base: number, delta: number, dec: number): number {
  const v = base + (Math.random() * 2 - 1) * delta
  return dec > 0 ? parseFloat(v.toFixed(dec)) : Math.round(v)
}

function liveUpdate() {
  inKpi.output  += Math.floor(Math.random() * 8 + 2)
  inKpi.fillWt   = rnd(250.0, 1.5, 1).toFixed(1)

  m01.fw = rnd(250.4, 2, 1).toFixed(1)
  m01.st = rnd(182.5, 3, 1).toFixed(1)
  m01.fr = rnd(48.2,  1.5, 1).toFixed(1)
  m01.mc = rnd(14.3,  0.8, 1).toFixed(1)

  m02.fw = rnd(249.8, 2, 1).toFixed(1)
  m02.st = rnd(181.0, 3, 1).toFixed(1)
  m02.fr = rnd(47.9,  1.5, 1).toFixed(1)
  m02.mc = rnd(13.8,  0.8, 1).toFixed(1)

  m03.fw = rnd(251.2, 2, 1).toFixed(1)
  m03.st = rnd(183.2, 3, 1).toFixed(1)
  m03.fr = rnd(49.0,  1.5, 1).toFixed(1)
  m03.mc = rnd(15.1,  0.8, 1).toFixed(1)

  if (Math.random() > 0.5) outKpi.batches++
  outKpi.silo   = Math.max(70, Math.min(95, outKpi.silo + (Math.random() * 0.6 - 0.3)))

  bp01.bw  = Math.round(rnd(2450, 80, 0))
  bp01.rpm = rnd(18.4, 1.2, 1).toFixed(1)
  bp02.bw  = Math.round(rnd(2380, 80, 0))
  bp02.rpm = rnd(17.8, 1.2, 1).toFixed(1)
}

const inMachines = [
  { name: 'FFS Machine 01', zone: 'Zone A', status: 'ACTIVE', l1: 'Fill Weight',   v1: '250.4 g',  l2: 'Seal Temp H-jaw', v2: '182.5 °C', l3: 'Screw Feed Rate', v3: '48.2 kg/h' },
  { name: 'FFS Machine 02', zone: 'Zone A', status: 'ACTIVE', l1: 'Fill Weight',   v1: '249.8 g',  l2: 'Seal Temp H-jaw', v2: '181.0 °C', l3: 'Screw Feed Rate', v3: '47.9 kg/h' },
  { name: 'FFS Machine 03', zone: 'Zone B', status: 'ACTIVE', l1: 'Fill Weight',   v1: '251.2 g',  l2: 'Seal Temp H-jaw', v2: '183.2 °C', l3: 'Screw Feed Rate', v3: '49.0 kg/h' },
  { name: 'FFS Machine 04', zone: 'Zone B', status: 'FAULT',  l1: 'Fill Weight',   v1: '— —',      l2: 'Seal Temp H-jaw', v2: 'FAULT E-42', l3: 'Screw Feed Rate', v3: '0.0 kg/h' },
]

const outMachines = [
  { name: 'Batching Plant 01', zone: 'Zone A', status: 'ACTIVE', l1: 'Batch Weight',    v1: '2,450 kg', l2: 'Mixer RPM',   v2: '18.4 rpm', l3: 'Aggregate Feed', v3: '53.6 t/h' },
  { name: 'Batching Plant 02', zone: 'Zone A', status: 'ACTIVE', l1: 'Batch Weight',    v1: '2,380 kg', l2: 'Mixer RPM',   v2: '17.8 rpm', l3: 'Aggregate Feed', v3: '51.2 t/h' },
  { name: 'Batching Plant 03', zone: 'Zone B', status: 'IDLE',   l1: 'Batch Weight',    v1: '— —',      l2: 'Mixer RPM',   v2: '0.0 rpm',  l3: 'Aggregate Feed', v3: 'Standby'   },
]

const inSelected  = ref('FFS Machine 01')
const outSelected = ref('Batching Plant 01')

const inDetail  = reactive({ zone: 'Zone A', name: 'FFS Machine 01', status: 'ACTIVE', l1: 'Fill Weight', v1: '250.4 g', l2: 'Seal Temp H-jaw', v2: '182.5 °C', l3: 'Screw Feed Rate', v3: '48.2 kg/h' })
const outDetail = reactive({ zone: 'Zone A', name: 'Batching Plant 01', status: 'ACTIVE', l1: 'Batch Weight', v1: '2,450 kg', l2: 'Mixer RPM', v2: '18.4 rpm', l3: 'Aggregate Feed', v3: '53.6 t/h' })

const inEvents = [
  { time: '09:14 am', color: '#e53935', text: 'FFS Machine 04 stopped — E-42 jaw overheat fault'            },
  { time: '09:08 am', color: '#e53935', text: 'Fill weight deviation alarm — M01 +3.8 g over tolerance'     },
  { time: '09:02 am', color: '#43a047', text: 'FFS Machine 03 resumed after film roll change'               },
  { time: '08:44 am', color: '#1565c0', text: 'Shift 2 started — WO-2026-041 active on all lines'           },
]

const outEvents = [
  { time: '09:11 am', color: '#e53935', text: 'Aggregate hopper low level alarm — BP01 hopper 18.2%' },
  { time: '09:02 am', color: '#43a047', text: 'Batch #47 completed — 2,450 kg dispatched (BP01)'    },
  { time: '08:58 am', color: '#43a047', text: 'Batch #19 completed — 2,380 kg dispatched (BP02)'    },
  { time: '08:44 am', color: '#f9a825', text: 'Batching Plant 03 set to IDLE — awaiting work order'  },
]

function selectIn(m: typeof inMachines[0]) {
  inSelected.value = m.name
  Object.assign(inDetail, { zone: m.zone, name: m.name, status: m.status, l1: m.l1, v1: m.v1, l2: m.l2, v2: m.v2, l3: m.l3, v3: m.v3 })
}

function selectOut(m: typeof outMachines[0]) {
  outSelected.value = m.name
  Object.assign(outDetail, { zone: m.zone, name: m.name, status: m.status, l1: m.l1, v1: m.v1, l2: m.l2, v2: m.v2, l3: m.l3, v3: m.v3 })
}

function statusDot(status: string): string {
  if (status === 'ACTIVE'  || status === 'RUNNING') return '#43a047'
  if (status === 'IDLE')    return '#f9a825'
  if (status === 'FAULT')   return '#e53935'
  return '#333'
}

function badgeClass(status: string): string {
  if (status === 'ACTIVE' || status === 'RUNNING') return 'badge-active'
  if (status === 'IDLE')    return 'badge-idle'
  if (status === 'FAULT')   return 'badge-fault'
  return 'badge-idle'
}

// ── MQTT handlers ────────────────────────────────────────────────
function onM01fw(v: string)  { m01.fw = parseFloat(v).toFixed(1) }
function onM01st(v: string)  { m01.st = parseFloat(v).toFixed(1) }
function onM02fw(v: string)  { m02.fw = parseFloat(v).toFixed(1) }
function onM02st(v: string)  { m02.st = parseFloat(v).toFixed(1) }
function onM03fw(v: string)  { m03.fw = parseFloat(v).toFixed(1) }
function onM03st(v: string)  { m03.st = parseFloat(v).toFixed(1) }
function onBp01bw(v: string) { bp01.bw = parseInt(v) }
function onBp01rpm(v: string){ bp01.rpm = parseFloat(v).toFixed(1) }
function onBp02bw(v: string) { bp02.bw = parseInt(v) }
function onBp02rpm(v: string){ bp02.rpm = parseFloat(v).toFixed(1) }

onMounted(() => {
  tick()
  clockTimer = setInterval(tick, 1000)
  liveTimer  = setInterval(liveUpdate, 17000)
  mqttSubscribe('hias/shopfloor/m01/fillweight',    onM01fw)
  mqttSubscribe('hias/shopfloor/m01/sealtemp',      onM01st)
  mqttSubscribe('hias/shopfloor/m02/fillweight',    onM02fw)
  mqttSubscribe('hias/shopfloor/m02/sealtemp',      onM02st)
  mqttSubscribe('hias/shopfloor/m03/fillweight',    onM03fw)
  mqttSubscribe('hias/shopfloor/m03/sealtemp',      onM03st)
  mqttSubscribe('hias/shopfloor/bp01/batchweight',  onBp01bw)
  mqttSubscribe('hias/shopfloor/bp01/rpm',          onBp01rpm)
  mqttSubscribe('hias/shopfloor/bp02/batchweight',  onBp02bw)
  mqttSubscribe('hias/shopfloor/bp02/rpm',          onBp02rpm)
})

onBeforeUnmount(() => {
  if (clockTimer) clearInterval(clockTimer)
  if (liveTimer)  clearInterval(liveTimer)
  mqttUnsubscribe('hias/shopfloor/m01/fillweight',   onM01fw)
  mqttUnsubscribe('hias/shopfloor/m01/sealtemp',     onM01st)
  mqttUnsubscribe('hias/shopfloor/m02/fillweight',   onM02fw)
  mqttUnsubscribe('hias/shopfloor/m02/sealtemp',     onM02st)
  mqttUnsubscribe('hias/shopfloor/m03/fillweight',   onM03fw)
  mqttUnsubscribe('hias/shopfloor/m03/sealtemp',     onM03st)
  mqttUnsubscribe('hias/shopfloor/bp01/batchweight', onBp01bw)
  mqttUnsubscribe('hias/shopfloor/bp01/rpm',         onBp01rpm)
  mqttUnsubscribe('hias/shopfloor/bp02/batchweight', onBp02bw)
  mqttUnsubscribe('hias/shopfloor/bp02/rpm',         onBp02rpm)
})
</script>

<style scoped>
*, *::before, *::after { box-sizing: border-box; margin: 0; padding: 0; }

.sfm-root { height: 100%; overflow: hidden; font-family: 'Poppins', sans-serif; font-size: 12px; background: #f5f5f5; color: #333; display: flex; flex-direction: column; }

.top-bar { background: #ffffff; border-bottom: 1px solid #c3c6d4; padding: 8px 16px; display: flex; align-items: center; gap: 12px; flex-shrink: 0; box-shadow: 0 1px 3px rgba(0,0,0,.06); }
.page-title { font-size: 13px; font-weight: 700; color: #515151; letter-spacing: 0.5px; text-transform: uppercase; }
.line-tabs  { display: flex; gap: 0; margin-left: 16px; border-bottom: 2px solid #c3c6d4; align-self: stretch; }
.line-tab   { background: #fff; border: none; border-right: 1px solid #e0e0e0; color: #7f7f7f; cursor: pointer; font-family: 'Poppins', sans-serif; font-size: 12px; font-weight: 600; padding: 6px 18px; transition: background .15s, color .15s; letter-spacing: 0.4px; }
.line-tab:hover  { background: #f2f2f2; color: #333; }
.line-tab.active { background: #fff; color: #333; border-bottom: 2px solid #1565c0; margin-bottom: -2px; }
.top-bar-right { display: flex; align-items: center; gap: 8px; font-size: 11px; color: #7f7f7f; margin-left: auto; }
.live-dot { width: 8px; height: 8px; border-radius: 50%; background: #43a047; display: inline-block; animation: blink 1.4s ease-in-out infinite; }
@keyframes blink { 0%,100%{ opacity:1 } 50%{ opacity:.2 } }

.tab-pane { display: flex; flex-direction: column; flex: 1; overflow: hidden; min-height: 0; }

.kpi-row  { display: flex; gap: 1px; background: #c3c6d4; border-bottom: 2px solid #c3c6d4; flex-shrink: 0; }
.kpi-card { flex: 1; background: #ffffff; padding: 8px 14px; display: flex; flex-direction: column; gap: 2px; }
.kpi-label { font-size: 10px; color: #7f7f7f; text-transform: uppercase; letter-spacing: 0.6px; }
.kpi-value { font-size: 22px; font-weight: 700; line-height: 1.1; }
.kpi-sub   { font-size: 10px; color: #9e9e9e; }
.kpi-green  { color: #388E3C; } .kpi-blue { color: #1565c0; } .kpi-red { color: #e53935; } .kpi-yellow { color: #f9a825; } .kpi-white { color: #333; }

.main-body { display: flex; flex: 1; overflow: hidden; min-height: 0; gap: 10px; padding: 10px; }

.floor-panel { flex: 1; display: flex; flex-direction: column; background: #ffffff; border: 1px solid #c3c6d4; border-radius: 6px; overflow: hidden; }
.floor-panel-title { padding: 8px 12px; font-size: 11px; font-weight: 700; color: #515151; text-transform: uppercase; letter-spacing: 0.4px; flex-shrink: 0; background: linear-gradient(0deg, #d7d7d7 0%, #ffffff 100%); border-bottom: 1px solid #c3c6d4; }
.floor-svg-wrap { flex: 1; overflow: hidden; background: #f0f0f0; display: flex; align-items: center; justify-content: center; padding: 12px; }

.right-panel { width: 360px; flex-shrink: 0; display: flex; flex-direction: column; gap: 10px; overflow: hidden; }
.rp-card { background: #ffffff; border: 1px solid #c3c6d4; border-radius: 6px; overflow: hidden; display: flex; flex-direction: column; }
.rp-section-title { padding: 8px 12px; font-size: 11px; font-weight: 700; color: #515151; text-transform: uppercase; letter-spacing: 0.4px; flex-shrink: 0; background: linear-gradient(0deg, #d7d7d7 0%, #ffffff 100%); border-bottom: 1px solid #c3c6d4; }

.machine-list { overflow-y: auto; max-height: 210px; }
.machine-list::-webkit-scrollbar { width: 4px; }
.machine-list::-webkit-scrollbar-thumb { background: #c3c6d4; border-radius: 2px; }
.machine-row { display: flex; align-items: center; gap: 8px; padding: 6px 10px; border-bottom: 1px solid #e0e0e0; cursor: pointer; transition: background .1s; }
.machine-row:hover    { background: #f2f2f2; }
.machine-row.selected { background: #e8f5e9; border-left: 3px solid #388E3C; }
.m-status-dot { width: 9px; height: 9px; border-radius: 50%; flex-shrink: 0; }
.m-name  { flex: 1; font-size: 12px; color: #333; font-weight: 600; }
.m-zone  { font-size: 10px; color: #7f7f7f; min-width: 50px; text-align: right; }
.m-badge { font-size: 10px; font-weight: 700; padding: 2px 7px; border-radius: 10px; min-width: 52px; text-align: center; }
.badge-active { background: #e8f5e9; color: #388E3C; border: 1px solid #a5d6a7; }
.badge-idle   { background: #fff9c4; color: #f9a825; border: 1px solid #fdd835; }
.badge-fault  { background: #ffebee; color: #e53935; border: 1px solid #ef9a9a; }

.zone-detail { flex: 1; overflow-y: auto; min-height: 0; }
.zone-detail::-webkit-scrollbar { width: 4px; }
.zone-detail::-webkit-scrollbar-thumb { background: #c3c6d4; border-radius: 2px; }
.zone-detail-inner { padding: 10px; display: flex; flex-direction: column; gap: 8px; }
.zd-header  { display: flex; align-items: center; justify-content: space-between; padding-bottom: 6px; border-bottom: 1px solid #e0e0e0; }
.zd-name    { font-size: 13px; font-weight: 700; color: #1565c0; }
.zd-status  { font-size: 11px; }
.zd-grid    { display: grid; grid-template-columns: 1fr 1fr; gap: 6px; }
.zd-metric  { background: #f5f5f5; border-radius: 3px; padding: 6px 8px; border: 1px solid #e0e0e0; }
.zd-metric-label { font-size: 9px; color: #7f7f7f; text-transform: uppercase; letter-spacing: 0.5px; }
.zd-metric-val   { font-size: 15px; font-weight: 700; color: #333; margin-top: 1px; }

.event-feed { max-height: 120px; overflow-y: auto; border-top: 1px solid #e0e0e0; }
.event-feed::-webkit-scrollbar { width: 4px; }
.event-feed::-webkit-scrollbar-thumb { background: #c3c6d4; border-radius: 2px; }
.event-row  { display: flex; align-items: flex-start; gap: 6px; padding: 4px 10px; border-bottom: 1px solid #f0f0f0; font-size: 11px; }
.ev-time { color: #7f7f7f; white-space: nowrap; min-width: 75px; }
.ev-dot  { width: 7px; height: 7px; border-radius: 50%; margin-top: 3px; flex-shrink: 0; }
.ev-text { color: #515151; }
</style>
