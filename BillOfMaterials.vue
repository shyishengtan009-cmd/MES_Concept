<template>
  <div class="root">

    <!-- Topbar -->
    <div class="topbar">
      <span class="pg-title">&#9632; Bill of Materials</span>
      <div class="topbar-right">
        <button class="btn-primary"><i class="fa-solid fa-plus"></i> New BOM</button>
        <button class="btn-outline-sm"><i class="fa-solid fa-file-import"></i> Import</button>
      </div>
    </div>

    <!-- Two-column layout -->
    <div class="body">

      <!-- Left: Product List -->
      <div class="left-panel">
        <div class="left-head">
          <div class="search-box">
            <i class="fa-solid fa-magnifying-glass"></i>
            <input v-model="prodSearch" placeholder="Search product…" />
          </div>
          <select class="sel" v-model="statusProdFilter">
            <option value="All">All Status</option>
            <option value="Active">Active</option>
            <option value="Draft">Draft</option>
            <option value="Obsolete">Obsolete</option>
          </select>
        </div>
        <div class="prod-list">
          <div
            v-for="p in filteredProds" :key="p.code"
            class="prod-row" :class="{ 'prod-row--active': selected?.code === p.code }"
            @click="selected = p"
          >
            <div class="prod-row-top">
              <span class="prod-code">{{ p.code }}</span>
              <span class="badge" :class="prodStatusCls(p.status)">{{ p.status }}</span>
            </div>
            <div class="prod-name">{{ p.name }}</div>
            <div class="prod-ver">v{{ p.version }} &nbsp;·&nbsp; {{ p.components }} components</div>
          </div>
          <div v-if="!filteredProds.length" class="empty">No products found.</div>
        </div>
      </div>

      <!-- Right: BOM Detail -->
      <div class="right-panel">
        <div v-if="!selected" class="no-select">
          <i class="fa-solid fa-sitemap no-sel-icon"></i>
          <p>Select a product from the list to view its Bill of Materials.</p>
        </div>

        <template v-else>
          <!-- BOM Header -->
          <div class="bom-head">
            <div class="bom-head-info">
              <div class="bom-title">{{ selected.name }}</div>
              <div class="bom-meta">
                <span class="mono">{{ selected.code }}</span>
                &nbsp;·&nbsp; Version {{ selected.version }}
                &nbsp;·&nbsp; <span class="badge" :class="prodStatusCls(selected.status)">{{ selected.status }}</span>
                &nbsp;·&nbsp; <span class="meta-note">Batch Size: {{ selected.batchSize.toLocaleString() }} {{ selected.batchUom }}</span>
              </div>
            </div>
            <div class="bom-head-actions">
              <button class="btn-outline-sm"><i class="fa-solid fa-pen"></i> Edit BOM</button>
              <button class="btn-outline-sm"><i class="fa-solid fa-copy"></i> Clone</button>
              <button class="btn-outline-sm" @click="exportToExcel"><i class="fa-solid fa-file-pdf"></i> Export</button>
            </div>
          </div>

          <!-- Summary KPI -->
          <div class="bom-kpi">
            <div class="bk" v-for="k in bomKpis" :key="k.label">
              <div class="bk-lbl">{{ k.label }}</div>
              <div class="bk-val" :class="k.cls">{{ k.val }}</div>
            </div>
          </div>

          <!-- Components Table -->
          <div class="comp-wrap">
            <table class="tbl">
              <thead>
                <tr>
                  <th class="tc">Seq</th>
                  <th>Component Code</th>
                  <th>Component Name</th>
                  <th class="tr">Qty / {{ selected.batchUom }}</th>
                  <th>UOM</th>
                  <th>Type</th>
                  <th>Lead Time</th>
                  <th>Supplier</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="c in getComponents(selected.code)" :key="c.code" class="comp-row" :class="'type-' + c.type.toLowerCase().replace(/\s+/g, '-').split('-')[0]">
                  <td class="tc">{{ c.seq }}</td>
                  <td class="mono">{{ c.code }}</td>
                  <td>{{ c.name }}</td>
                  <td class="tr">{{ c.qty }}</td>
                  <td>{{ c.uom }}</td>
                  <td><span class="type-tag" :class="typeCls(c.type)">{{ c.type }}</span></td>
                  <td>{{ c.lead }}</td>
                  <td class="sup-cell">{{ c.supplier }}</td>
                </tr>
              </tbody>
            </table>
          </div>

          <!-- Cost Breakdown -->
          <div class="cost-section">
            <div class="sec-title">Estimated Material Cost (per {{ selected.batchUom }})</div>
            <div class="cost-bars">
              <div v-for="cb in costBreakdown(selected.code)" :key="cb.type" class="cost-row">
                <span class="cost-lbl">{{ cb.type }}</span>
                <div class="cost-bar-wrap">
                  <div class="cost-bar" :style="{ width: cb.pct + '%', background: cb.color }"></div>
                </div>
                <span class="cost-pct">{{ cb.pct }}%</span>
                <span class="cost-val">RM {{ cb.cost.toFixed(2) }}</span>
              </div>
            </div>
          </div>
        </template>
      </div>
    </div>

  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import ExcelJS from 'exceljs'

interface Product { code: string; name: string; version: string; status: string; components: number; batchSize: number; batchUom: string }
interface Component { seq: number; code: string; name: string; qty: number; uom: string; type: string; lead: string; supplier: string }

const prodSearch      = ref('')
const statusProdFilter = ref('All')
const selected        = ref<Product | null>(null)

const products = ref<Product[]>([
  { code:'FG-001', name:'Halal Seasoning Blend A 500g',  version:'1.2', status:'Active',   components: 9, batchSize: 2400, batchUom:'pcs' },
  { code:'FG-002', name:'Palm Olein Oil 5L',             version:'2.0', status:'Active',   components: 5, batchSize: 600,  batchUom:'btl' },
  { code:'FG-003', name:'Halal Marinade Sauce 250ml',    version:'1.0', status:'Active',   components: 7, batchSize: 1800, batchUom:'btl' },
  { code:'FG-004', name:'Halal Curry Paste 200g',        version:'1.1', status:'Active',   components: 8, batchSize: 1200, batchUom:'pcs' },
  { code:'FG-005', name:'Mixed Spice Sachet 50g',        version:'1.3', status:'Active',   components: 6, batchSize: 4800, batchUom:'pcs' },
  { code:'FG-006', name:'Spice Mix Pack 1kg',            version:'1.0', status:'Draft',    components: 7, batchSize: 600,  batchUom:'pcs' },
  { code:'FG-007', name:'Halal BBQ Sauce 300ml',         version:'0.9', status:'Draft',    components: 8, batchSize: 1200, batchUom:'btl' },
  { code:'FG-008', name:'Chilli Garlic Paste 180g',      version:'2.1', status:'Obsolete', components: 6, batchSize: 2400, batchUom:'pcs' },
])

const compMap: Record<string, Component[]> = {
  'FG-001': [
    { seq:1,  code:'RM-014', name:'Coriander Powder',       qty:150,   uom:'g',   type:'Raw Material', lead:'3 days',  supplier:'Spice World Sdn Bhd'  },
    { seq:2,  code:'RM-008', name:'Cumin Powder',           qty:80,    uom:'g',   type:'Raw Material', lead:'3 days',  supplier:'Spice World Sdn Bhd'  },
    { seq:3,  code:'RM-022', name:'Salt (Refined)',         qty:120,   uom:'g',   type:'Raw Material', lead:'1 day',   supplier:'Garam Jaya Sdn Bhd'   },
    { seq:4,  code:'RM-031', name:'Chilli Powder',          qty:50,    uom:'g',   type:'Raw Material', lead:'3 days',  supplier:'Spice World Sdn Bhd'  },
    { seq:5,  code:'RM-041', name:'Turmeric Powder',        qty:30,    uom:'g',   type:'Raw Material', lead:'3 days',  supplier:'Herba Nusantara'       },
    { seq:6,  code:'RM-009', name:'Anti-caking Agent E551', qty:5,     uom:'g',   type:'Raw Material', lead:'5 days',  supplier:'Chemfood Supply'       },
    { seq:7,  code:'PK-012', name:'Stand-up Pouch 500g',   qty:1,     uom:'Pcs', type:'Packaging',    lead:'7 days',  supplier:'Packrite Sdn Bhd'     },
    { seq:8,  code:'PK-034', name:'Sticker Label (Halal)', qty:1,     uom:'Pcs', type:'Packaging',    lead:'5 days',  supplier:'Label Express'         },
    { seq:9,  code:'PK-045', name:'Outer Carton (24 pcs)', qty:0.042, uom:'Pcs', type:'Packaging',    lead:'7 days',  supplier:'Packrite Sdn Bhd'     },
  ],
  'FG-002': [
    { seq:1,  code:'RM-051', name:'Palm Olein (Crude)',     qty:5.2,   uom:'L',   type:'Raw Material', lead:'2 days',  supplier:'Golden Palm Bhd'      },
    { seq:2,  code:'RM-052', name:'Antioxidant BHA',       qty:0.005, uom:'kg',  type:'Raw Material', lead:'5 days',  supplier:'Chemfood Supply'       },
    { seq:3,  code:'PK-061', name:'HDPE Bottle 5L',        qty:1,     uom:'Pcs', type:'Packaging',    lead:'7 days',  supplier:'Plastipak Sdn Bhd'    },
    { seq:4,  code:'PK-062', name:'Tamper-evident Cap',    qty:1,     uom:'Pcs', type:'Packaging',    lead:'7 days',  supplier:'Plastipak Sdn Bhd'    },
    { seq:5,  code:'PK-063', name:'Shrink Label 5L',       qty:1,     uom:'Pcs', type:'Packaging',    lead:'5 days',  supplier:'Label Express'         },
  ],
  'FG-003': [
    { seq:1,  code:'RM-071', name:'Chilli Paste (Halal)',  qty:90,    uom:'g',   type:'Raw Material', lead:'2 days',  supplier:'Kerabu Foods'          },
    { seq:2,  code:'RM-072', name:'Garlic Extract',        qty:20,    uom:'g',   type:'Raw Material', lead:'3 days',  supplier:'Herba Nusantara'       },
    { seq:3,  code:'RM-073', name:'Soy Sauce (Halal)',     qty:30,    uom:'g',   type:'Raw Material', lead:'2 days',  supplier:'Kerabu Foods'          },
    { seq:4,  code:'RM-022', name:'Salt (Refined)',        qty:8,     uom:'g',   type:'Raw Material', lead:'1 day',   supplier:'Garam Jaya Sdn Bhd'   },
    { seq:5,  code:'RM-074', name:'Sugar (Raw)',           qty:12,    uom:'g',   type:'Raw Material', lead:'1 day',   supplier:'MSM Malaysia'          },
    { seq:6,  code:'PK-081', name:'Glass Bottle 250ml',   qty:1,     uom:'Pcs', type:'Packaging',    lead:'10 days', supplier:'Vitro Glass Sdn Bhd'  },
    { seq:7,  code:'PK-082', name:'Metal Lid (Twist)',    qty:1,     uom:'Pcs', type:'Packaging',    lead:'7 days',  supplier:'Packrite Sdn Bhd'     },
  ],
  'FG-004': [
    { seq:1,  code:'RM-081', name:'Curry Paste Base',     qty:80,    uom:'g',   type:'Semi-Finished', lead:'1 day',   supplier:'Internal'              },
    { seq:2,  code:'RM-082', name:'Coconut Cream Powder', qty:30,    uom:'g',   type:'Raw Material',  lead:'3 days',  supplier:'Tropika Foods'         },
    { seq:3,  code:'RM-014', name:'Coriander Powder',     qty:20,    uom:'g',   type:'Raw Material',  lead:'3 days',  supplier:'Spice World Sdn Bhd'  },
    { seq:4,  code:'RM-041', name:'Turmeric Powder',      qty:15,    uom:'g',   type:'Raw Material',  lead:'3 days',  supplier:'Herba Nusantara'       },
    { seq:5,  code:'RM-022', name:'Salt (Refined)',       qty:10,    uom:'g',   type:'Raw Material',  lead:'1 day',   supplier:'Garam Jaya Sdn Bhd'   },
    { seq:6,  code:'PK-091', name:'Jar 200g (PP)',        qty:1,     uom:'Pcs', type:'Packaging',     lead:'7 days',  supplier:'Plastipak Sdn Bhd'    },
    { seq:7,  code:'PK-092', name:'Lid (Press-fit)',      qty:1,     uom:'Pcs', type:'Packaging',     lead:'7 days',  supplier:'Plastipak Sdn Bhd'    },
    { seq:8,  code:'PK-093', name:'Sticker Label (200g)', qty:1,    uom:'Pcs', type:'Packaging',     lead:'5 days',  supplier:'Label Express'         },
  ],
  'FG-005': [
    { seq:1,  code:'RM-014', name:'Coriander Powder',    qty:12,    uom:'g',   type:'Raw Material', lead:'3 days',  supplier:'Spice World Sdn Bhd'  },
    { seq:2,  code:'RM-008', name:'Cumin Powder',        qty:8,     uom:'g',   type:'Raw Material', lead:'3 days',  supplier:'Spice World Sdn Bhd'  },
    { seq:3,  code:'RM-031', name:'Chilli Powder',       qty:6,     uom:'g',   type:'Raw Material', lead:'3 days',  supplier:'Spice World Sdn Bhd'  },
    { seq:4,  code:'RM-022', name:'Salt (Refined)',      qty:10,    uom:'g',   type:'Raw Material', lead:'1 day',   supplier:'Garam Jaya Sdn Bhd'   },
    { seq:5,  code:'PK-101', name:'Sachet Film 50g',     qty:1,     uom:'Pcs', type:'Packaging',    lead:'7 days',  supplier:'Packrite Sdn Bhd'     },
    { seq:6,  code:'PK-045', name:'Outer Carton (48 pcs)',qty:0.021,uom:'Pcs', type:'Packaging',    lead:'7 days',  supplier:'Packrite Sdn Bhd'     },
  ],
}

const costData: Record<string, Array<{ type: string; pct: number; cost: number; color: string }>> = {
  'FG-001': [{ type:'Raw Material', pct:62, cost:2.98, color:'#1565c0'}, { type:'Packaging', pct:28, cost:1.34, color:'#42a5f5'}, { type:'Labour', pct:7, cost:0.34, color:'#ef9a9a'}, { type:'Overhead', pct:3, cost:0.14, color:'#7b1fa2'}],
  'FG-002': [{ type:'Raw Material', pct:74, cost:9.18, color:'#1565c0'}, { type:'Packaging', pct:18, cost:2.23, color:'#42a5f5'}, { type:'Labour', pct:5, cost:0.62, color:'#ef9a9a'}, { type:'Overhead', pct:3, cost:0.37, color:'#7b1fa2'}],
  'FG-003': [{ type:'Raw Material', pct:55, cost:3.96, color:'#1565c0'}, { type:'Packaging', pct:32, cost:2.30, color:'#42a5f5'}, { type:'Labour', pct:9, cost:0.65, color:'#ef9a9a'}, { type:'Overhead', pct:4, cost:0.29, color:'#7b1fa2'}],
  'FG-004': [{ type:'Raw Material', pct:58, cost:3.25, color:'#1565c0'}, { type:'Packaging', pct:26, cost:1.46, color:'#42a5f5'}, { type:'Labour', pct:10, cost:0.56, color:'#ef9a9a'}, { type:'Overhead', pct:6, cost:0.34, color:'#7b1fa2'}],
  'FG-005': [{ type:'Raw Material', pct:48, cost:0.77, color:'#1565c0'}, { type:'Packaging', pct:38, cost:0.61, color:'#42a5f5'}, { type:'Labour', pct:9, cost:0.14, color:'#ef9a9a'}, { type:'Overhead', pct:5, cost:0.08, color:'#7b1fa2'}],
}

const filteredProds = computed(() => {
  const q = prodSearch.value.toLowerCase()
  return products.value.filter(p => {
    const sOk = statusProdFilter.value === 'All' || p.status === statusProdFilter.value
    const qOk = !q || p.name.toLowerCase().includes(q) || p.code.toLowerCase().includes(q)
    return sOk && qOk
  })
})

const bomKpis = computed(() => {
  if (!selected.value) return []
  const comps = getComponents(selected.value.code)
  const rm   = comps.filter(c => c.type === 'Raw Material').length
  const pk   = comps.filter(c => c.type === 'Packaging').length
  const sf   = comps.filter(c => c.type === 'Semi-Finished').length
  return [
    { label: 'Total Components', val: comps.length,    cls: 'c-blue'   },
    { label: 'Raw Materials',    val: rm,               cls: 'c-green'  },
    { label: 'Packaging',        val: pk,               cls: 'c-amber'  },
    { label: 'Semi-Finished',    val: sf,               cls: 'c-purple' },
    { label: 'Batch Size',       val: selected.value.batchSize.toLocaleString() + ' ' + selected.value.batchUom, cls: 'c-blue' },
  ]
})

function getComponents(code: string): Component[] { return compMap[code] ?? [] }
function costBreakdown(code: string) { return costData[code] ?? [] }

async function exportToExcel() {
  if (!selected.value) return
  const wb = new ExcelJS.Workbook()
  wb.creator = 'HIAS'
  const ws = wb.addWorksheet(`BOM ${selected.value.code}`)

  ws.columns = [
    { header: 'Seq',            key: 'seq',      width: 6  },
    { header: 'Component Code', key: 'code',     width: 14 },
    { header: 'Component Name', key: 'name',     width: 26 },
    { header: `Qty / ${selected.value.batchUom}`, key: 'qty', width: 12 },
    { header: 'UOM',            key: 'uom',      width: 8  },
    { header: 'Type',           key: 'type',     width: 16 },
    { header: 'Lead Time',      key: 'lead',     width: 12 },
    { header: 'Supplier',       key: 'supplier', width: 24 },
  ]

  const headerRow = ws.getRow(1)
  headerRow.height = 22
  headerRow.eachCell(cell => {
    cell.fill = { type: 'pattern', pattern: 'solid', fgColor: { argb: 'FF1565C0' } }
    cell.font = { bold: true, color: { argb: 'FFFFFFFF' }, size: 11, name: 'Calibri' }
    cell.alignment = { vertical: 'middle', horizontal: 'left' }
    cell.border = { bottom: { style: 'medium', color: { argb: 'FF0D47A1' } } }
  })

  getComponents(selected.value.code).forEach((c, i) => {
    const row = ws.addRow(c)
    row.height = 18
    const fillColor = i % 2 === 0 ? 'FFFFFFFF' : 'FFF3F7FB'
    row.eachCell(cell => {
      cell.fill = { type: 'pattern', pattern: 'solid', fgColor: { argb: fillColor } }
      cell.font = { size: 10, name: 'Calibri', color: { argb: 'FF333333' } }
      cell.alignment = { vertical: 'middle', horizontal: 'left' }
      cell.border = { bottom: { style: 'thin', color: { argb: 'FFE0E0E0' } } }
    })
  })

  const buf  = await wb.xlsx.writeBuffer()
  const blob = new Blob([buf], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' })
  const url  = URL.createObjectURL(blob)
  const a    = document.createElement('a')
  a.href     = url
  a.download = `BOM_${selected.value.code}_${new Date().toISOString().slice(0, 10)}.xlsx`
  a.click()
  URL.revokeObjectURL(url)
}

function prodStatusCls(s: string) {
  return { 'b-green': s==='Active', 'b-amber': s==='Draft', 'b-grey': s==='Obsolete' }
}
function typeCls(t: string) {
  return { 'tt-rm': t==='Raw Material', 'tt-pk': t==='Packaging', 'tt-sf': t==='Semi-Finished' }
}
</script>

<style scoped>
*, *::before, *::after { box-sizing: border-box; }
.root { height: 100%; display: flex; flex-direction: column; font-family: 'Poppins', sans-serif; font-size: 12px; background: #f5f5f5; overflow: hidden; }

.topbar { background: #fff; border-bottom: 1px solid #c3c6d4; padding: 8px 14px; display: flex; align-items: center; flex-shrink: 0; box-shadow: 0 1px 3px rgba(0,0,0,.06); }
.pg-title { font-size: 13px; font-weight: 700; color: #515151; letter-spacing: 0.5px; text-transform: uppercase; }
.topbar-right { display: flex; gap: 6px; margin-left: auto; }
.btn-primary { background: linear-gradient(135deg, #1976d2, #0d47a1); border: none; border-radius: 3px; color: #fff; cursor: pointer; font-family: 'Poppins', sans-serif; font-size: 11px; gap: 5px; display: flex; align-items: center; padding: 5px 12px; transition: transform .12s ease, box-shadow .12s ease; }
.btn-primary:hover { transform: translateY(-1px); box-shadow: 0 4px 10px rgba(13,71,161,.4); }
.btn-outline-sm { background: #fff; border: 1px solid #d0d3e0; border-radius: 3px; color: #515151; cursor: pointer; font-family: 'Poppins', sans-serif; font-size: 11px; gap: 5px; display: flex; align-items: center; padding: 5px 10px; }
.btn-outline-sm:hover { background: #e3f2fd; border-color: #1565c0; color: #1565c0; }

.body { flex: 1; display: flex; overflow: hidden; }

/* Left Panel */
.left-panel { width: 280px; background: #fff; border-right: 1px solid #c3c6d4; display: flex; flex-direction: column; flex-shrink: 0; }
.left-head { border-bottom: 1px solid #f0f0f0; display: flex; flex-direction: column; gap: 6px; padding: 10px; }
.search-box { align-items: center; background: #f5f5f5; border: 1px solid #d0d3e0; border-radius: 3px; display: flex; gap: 6px; padding: 5px 8px; }
.search-box i { color: #9e9e9e; font-size: 11px; }
.search-box input { background: transparent; border: none; font-family: 'Poppins', sans-serif; font-size: 11px; outline: none; width: 100%; color: #333; }
.sel { border: 1px solid #d0d3e0; border-radius: 3px; font-family: 'Poppins', sans-serif; font-size: 11px; padding: 4px 8px; color: #515151; background: #fff; width: 100%; }
.prod-list { flex: 1; overflow-y: auto; }

.prod-row { border-bottom: 1px solid #f0f0f0; cursor: pointer; padding: 10px 12px; transition: background .1s; }
.prod-row:hover { background: #f5f5f5; }
.prod-row--active { background: #e3f2fd; border-left: 3px solid #1565c0; }
.prod-row-top { align-items: center; display: flex; gap: 6px; margin-bottom: 3px; }
.prod-code { color: #9e9e9e; font-family: monospace; font-size: 11px; }
.prod-name { color: #333; font-size: 11px; font-weight: 600; margin-bottom: 2px; }
.prod-ver  { color: #9e9e9e; font-size: 10px; }
.empty { color: #9e9e9e; padding: 20px; text-align: center; }

/* Right Panel */
.right-panel { flex: 1; display: flex; flex-direction: column; overflow: hidden; }
.no-select { align-items: center; color: #9e9e9e; display: flex; flex-direction: column; gap: 12px; justify-content: center; height: 100%; }
.no-sel-icon { font-size: 48px; color: #d0d3e0; }

.bom-head { align-items: flex-start; background: linear-gradient(0deg,#d7d7d7,#fff); border-bottom: 1px solid #c3c6d4; display: flex; gap: 12px; padding: 12px 14px; flex-shrink: 0; }
.bom-head-info { flex: 1; }
.bom-title { font-size: 13px; font-weight: 700; color: #333; margin-bottom: 4px; }
.bom-meta  { align-items: center; display: flex; flex-wrap: wrap; gap: 4px; font-size: 11px; color: #757575; }
.mono { font-family: monospace; }
.meta-note { color: #515151; }
.bom-head-actions { display: flex; gap: 6px; flex-shrink: 0; }

.bom-kpi { background: #c3c6d4; display: flex; gap: 1px; flex-shrink: 0; }
.bk { background: #fff; flex: 1; padding: 8px 12px; display: flex; flex-direction: column; gap: 2px; position: relative; overflow: hidden; border-left: 3px solid #1565c0; transition: transform .15s ease, box-shadow .15s ease; }
.bk:hover { transform: translateY(-2px); box-shadow: 0 6px 14px rgba(0,0,0,.12); z-index: 2; }
.bk::before { content: '\f201'; font-family: 'Font Awesome 6 Free'; font-weight: 900; position: absolute; right: 4px; top: 0px; font-size: 26px; color: #1565c0; opacity: .08; pointer-events: none; }
.bk::after { content: ''; position: absolute; left: 0; right: 0; bottom: 0; height: 3px; background: linear-gradient(90deg, transparent, #1565c0, transparent); background-size: 200% 100%; animation: kpi-shimmer 2.5s linear infinite; opacity: .35; }
@keyframes kpi-shimmer { 0% { background-position: 200% 0; } 100% { background-position: -200% 0; } }
.bk-lbl { font-size: 10px; color: #7f7f7f; text-transform: uppercase; letter-spacing: 0.5px; position: relative; z-index: 1; }
.bk-val { font-size: 18px; font-weight: 800; position: relative; z-index: 1; }
.c-blue   { color: #1565c0; } .c-green  { color: #388E3C; }
.c-amber  { color: #f9a825; } .c-purple { color: #7b1fa2; }

.comp-wrap { flex: 1; overflow: auto; }
.tbl { border-collapse: collapse; width: 100%; }
.tbl thead th { background: #f5f5f5; border-bottom: 1px solid #c3c6d4; color: #515151; font-size: 10px; font-weight: 600; padding: 8px 10px; position: sticky; text-align: left; text-transform: uppercase; top: 0; white-space: nowrap; }
.tbl tbody td { border-bottom: 1px solid #f0f0f0; color: #333; font-size: 11px; padding: 8px 10px; }
.comp-row { transition: background .1s; }
.comp-row:hover { background: #f5f5f5; }
.tc { text-align: center; } .tr { text-align: right; }
.sup-cell { color: #757575; font-size: 10px; }

.type-tag { border-radius: 3px; font-size: 10px; font-weight: 600; padding: 2px 7px; }
.tt-rm { background: #e3f2fd; color: #1565c0; }
.tt-pk { background: #fff8e1; color: #f9a825; }
.tt-sf { background: #f3e5f5; color: #7b1fa2; }

.badge { border-radius: 3px; font-size: 10px; font-weight: 600; padding: 2px 8px; white-space: nowrap; }
.b-green  { background: #e8f5e9; color: #388E3C; }
.b-amber  { background: #fff8e1; color: #f9a825; }
.b-grey   { background: #f5f5f5; color: #757575; }
.b-blue   { background: #e3f2fd; color: #1565c0; }

.cost-section { border-top: 1px solid #c3c6d4; padding: 12px 14px; flex-shrink: 0; background: #fff; }
.sec-title { color: #515151; font-size: 11px; font-weight: 700; margin-bottom: 8px; text-transform: uppercase; }
.cost-bars { display: flex; flex-direction: column; gap: 6px; }
.cost-row  { align-items: center; display: flex; gap: 8px; }
.cost-lbl  { color: #515151; font-size: 11px; width: 110px; flex-shrink: 0; }
.cost-bar-wrap { background: #f0f0f0; border-radius: 3px; flex: 1; height: 10px; overflow: hidden; }
.cost-bar  { border-radius: 3px; height: 100%; transition: width .4s; }
.cost-pct  { color: #9e9e9e; font-size: 10px; width: 30px; text-align: right; flex-shrink: 0; }
.cost-val  { color: #333; font-size: 11px; font-weight: 600; width: 60px; text-align: right; flex-shrink: 0; }
</style>
