<template>
  <div class="ca-root">

    <!-- TOPBAR -->
    <div class="topbar">
      <span class="pg-title">&#9632; Cost Analysis</span>
      <div class="pg-tabs">
        <button class="pg-tab" :class="{ active: activeTab === 'overview'  }" @click="activeTab = 'overview'">Overview</button>
        <button class="pg-tab" :class="{ active: activeTab === 'material'  }" @click="activeTab = 'material'">Material Cost</button>
        <button class="pg-tab" :class="{ active: activeTab === 'overhead'  }" @click="activeTab = 'overhead'">Overhead</button>
      </div>
      <div class="topbar-right">
        <button v-for="p in periods" :key="p" class="per-btn" :class="{ on: period === p }" @click="period = p">{{ p }}</button>
        <button class="export-btn" @click="exportToExcel"><i class="fa-solid fa-arrow-up-from-bracket"></i> Export</button>
      </div>
    </div>

    <!-- ── TAB: OVERVIEW ── -->
    <div class="tab-pane" v-show="activeTab === 'overview'">
      <div class="kpi-row">
        <div class="kpi-card">
          <div class="kpi-label">Total Cost ({{ period }})</div>
          <div class="kpi-val c-d">{{ fmtRM(totalActual) }}</div>
          <div class="kpi-sub"><span class="t-up">▲ {{ meta.changeLabel }}</span></div>
        </div>
        <div class="kpi-card">
          <div class="kpi-label">Material Cost</div>
          <div class="kpi-val c-b">{{ fmtRM(materialCost) }}</div>
          <div class="kpi-sub">{{ (materialCost / totalActual * 100).toFixed(1) }}% of total</div>
        </div>
        <div class="kpi-card">
          <div class="kpi-label">Overhead Cost</div>
          <div class="kpi-val c-pur">{{ fmtRM(overheadCost) }}</div>
          <div class="kpi-sub">{{ (overheadCost / totalActual * 100).toFixed(1) }}% of total</div>
        </div>
        <div class="kpi-card">
          <div class="kpi-label">vs Budget</div>
          <div class="kpi-val c-r">{{ vsBudgetAmt >= 0 ? '+' : '−' }}{{ fmtRM(Math.abs(vsBudgetAmt)) }}</div>
          <div class="kpi-sub"><span class="t-up">{{ vsBudgetPct >= 0 ? '+' : '−' }}{{ Math.abs(vsBudgetPct).toFixed(1) }}% {{ vsBudgetPct >= 0 ? 'over' : 'under' }} budget</span></div>
        </div>
      </div>

      <div class="body-2col">
        <div class="col">
          <div class="pc" style="flex:1">
            <div class="ph">Cost Breakdown <span class="ph-r">{{ meta.dateLabel }}</span></div>
            <div class="pc-body" style="padding:14px 16px">
              <div class="cb-row" v-for="item in breakdown" :key="item.name">
                <div class="cb-dot" :style="{ background: item.color }"></div>
                <span class="cb-name">{{ item.name }}</span>
                <div class="cb-bg"><div class="cb-fill" :style="{ width: item.barPct + '%', background: item.color }"></div></div>
                <span class="cb-pct">{{ item.pct }}%</span>
                <span class="cb-val">{{ item.value }}</span>
              </div>
            </div>
          </div>
        </div>

        <div class="col">
          <div class="pc" style="flex:1">
            <div class="ph">Budget vs Actual <span class="ph-r">{{ meta.dateLabel }}</span></div>
            <div class="pc-body">
              <div class="bva-leg">
                <span><span class="swatch" style="background:#bbdefb"></span>Budget</span>
                <span><span class="swatch" style="background:#1565c0"></span>Actual (within)</span>
                <span><span class="swatch" style="background:#e53935"></span>Actual (over)</span>
              </div>
              <div class="bva-row" v-for="(item, i) in budgetVsActual" :key="item.name" :style="i === budgetVsActual.length - 1 ? 'margin-bottom:0' : ''">
                <div class="bva-head">
                  <span class="bva-name">{{ item.name }}</span>
                  <span class="bva-nums">
                    {{ item.actual }} / Budget {{ item.budget }}&nbsp;
                    <span :class="item.over ? 'var-pos' : 'var-neg'">{{ item.variance }}</span>
                  </span>
                </div>
                <div class="bva-track">
                  <div class="bva-b-bar" :style="{ width: item.budgetBarPct + '%' }"></div>
                  <div class="bva-a-bar" :style="{ width: item.actualBarPct + '%', background: item.over ? '#e53935' : '#1565c0', opacity: 0.7 }"></div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- ── TAB: MATERIAL COST ── -->
    <div class="tab-pane" v-show="activeTab === 'material'">
      <div class="filter-bar">
        <span class="fl">Category:</span>
        <button class="f-btn" :class="{ on: matCat === 'all'       }" @click="matCat = 'all'">All</button>
        <button class="f-btn" :class="{ on: matCat === 'food'      }" @click="matCat = 'food'">Food Ingredient</button>
        <button class="f-btn" :class="{ on: matCat === 'packaging' }" @click="matCat = 'packaging'">Packaging</button>
        <button class="f-btn" :class="{ on: matCat === 'chemical'  }" @click="matCat = 'chemical'">Chemical / Additive</button>
      </div>
      <div class="body-full">
        <div class="pc" style="flex:1">
          <div class="ph">
            Material Cost — {{ meta.dateLabel }}
            <span class="ph-r">Total: RM {{ matTotal.toLocaleString() }} — {{ filteredMaterial.length }} items</span>
          </div>
          <div class="tbl-wrap">
            <table>
              <thead>
                <tr>
                  <th>Item Name</th><th>Category</th>
                  <th style="text-align:right">Total Cost (RM)</th>
                  <th style="text-align:right">% of Total</th>
                  <th style="text-align:right">vs Last Month</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="row in filteredMaterial" :key="row.name">
                  <td>{{ row.name }}</td>
                  <td>{{ row.categoryLabel }}</td>
                  <td style="text-align:right;font-weight:600">{{ row.total.toLocaleString() }}</td>
                  <td style="text-align:right">{{ row.pct }}</td>
                  <td style="text-align:right"><span :class="row.vsPositive ? 'var-pos' : 'var-neg'">{{ row.vsLastMonth }}</span></td>
                </tr>
                <tr v-if="filteredMaterial.length === 0">
                  <td colspan="5" class="empty-msg">No items match the current filter.</td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </div>

    <!-- ── TAB: OVERHEAD ── -->
    <div class="tab-pane" v-show="activeTab === 'overhead'">
      <div class="body-full">
        <div class="pc" style="flex:1">
          <div class="ph">
            Overhead Detail — {{ meta.dateLabel }}
            <span class="ph-r">Total: RM {{ overheadTotalActual.toLocaleString() }} &nbsp;|&nbsp; Budget: RM {{ overheadTotalBudget.toLocaleString() }} &nbsp;|&nbsp; <span :class="overheadVarianceAmt >= 0 ? 'var-pos' : 'var-neg'">{{ overheadVarianceAmt >= 0 ? '+' : '−' }}RM {{ Math.abs(overheadVarianceAmt).toLocaleString() }} ({{ overheadVariancePct >= 0 ? '+' : '−' }}{{ Math.abs(overheadVariancePct).toFixed(1) }}%)</span></span>
          </div>
          <div class="tbl-wrap">
            <table>
              <thead>
                <tr>
                  <th>Category</th><th>Item</th>
                  <th style="text-align:right">Budget (RM)</th>
                  <th style="text-align:right">Actual (RM)</th>
                  <th style="text-align:right">Variance</th>
                  <th>Status</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="row in overhead" :key="row.item">
                  <td>{{ row.category }}</td>
                  <td>{{ row.item }}</td>
                  <td style="text-align:right">{{ row.budget.toLocaleString() }}</td>
                  <td style="text-align:right">{{ row.actual.toLocaleString() }}</td>
                  <td style="text-align:right"><span :class="row.over ? 'var-pos' : 'var-neg'">{{ row.variance }}</span></td>
                  <td><span class="badge" :class="overBadge(row)">{{ overLabel(row) }}</span></td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </div>

  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import ExcelJS from 'exceljs'

type Tab = 'overview' | 'material' | 'overhead'

const activeTab = ref<Tab>('overview')
const period    = ref('MTD')
const periods   = ['MTD', 'This Month', 'Q1 2026', 'YTD']

interface CatRow { name: string; color: string; budget: number; actual: number }

const periodCategories: Record<string, CatRow[]> = {
  'MTD': [
    { name: 'Raw Materials', color: '#1565c0', budget: 142000, actual: 149200 },
    { name: 'Packaging',     color: '#42a5f5', budget: 48000,  actual: 47200  },
    { name: 'Utilities',     color: '#7b1fa2', budget: 30000,  actual: 33600  },
    { name: 'Labour',        color: '#ef9a9a', budget: 28000,  actual: 27900  },
    { name: 'Maintenance',   color: '#f9a825', budget: 15000,  actual: 17900  },
    { name: 'Other',         color: '#9e9e9e', budget: 9000,   actual: 8900   },
  ],
  'This Month': [
    { name: 'Raw Materials', color: '#1565c0', budget: 165600, actual: 174000 },
    { name: 'Packaging',     color: '#42a5f5', budget: 56000,  actual: 55100  },
    { name: 'Utilities',     color: '#7b1fa2', budget: 35000,  actual: 39200  },
    { name: 'Labour',        color: '#ef9a9a', budget: 32700,  actual: 32500  },
    { name: 'Maintenance',   color: '#f9a825', budget: 17500,  actual: 20900  },
    { name: 'Other',         color: '#9e9e9e', budget: 10500,  actual: 10300  },
  ],
  'Q1 2026': [
    { name: 'Raw Materials', color: '#1565c0', budget: 419000, actual: 440200 },
    { name: 'Packaging',     color: '#42a5f5', budget: 141600, actual: 139400 },
    { name: 'Utilities',     color: '#7b1fa2', budget: 88500,  actual: 99100  },
    { name: 'Labour',        color: '#ef9a9a', budget: 82600,  actual: 82300  },
    { name: 'Maintenance',   color: '#f9a825', budget: 44300,  actual: 52900  },
    { name: 'Other',         color: '#9e9e9e', budget: 26600,  actual: 26100  },
  ],
  'YTD': [
    { name: 'Raw Materials', color: '#1565c0', budget: 573600, actual: 602600 },
    { name: 'Packaging',     color: '#42a5f5', budget: 193900, actual: 190900 },
    { name: 'Utilities',     color: '#7b1fa2', budget: 121200, actual: 135700 },
    { name: 'Labour',        color: '#ef9a9a', budget: 113100, actual: 112700 },
    { name: 'Maintenance',   color: '#f9a825', budget: 60600,  actual: 72500  },
    { name: 'Other',         color: '#9e9e9e', budget: 36400,  actual: 35600  },
  ],
}

const periodMeta: Record<string, { dateLabel: string; changeLabel: string }> = {
  'MTD':        { dateLabel: 'Apr 2026',       changeLabel: '+4.2% vs Mar 2026' },
  'This Month': { dateLabel: 'Apr 2026',       changeLabel: '+5.8% vs Mar 2026' },
  'Q1 2026':    { dateLabel: 'Jan – Mar 2026', changeLabel: '+8.1% vs Q4 2025' },
  'YTD':        { dateLabel: 'Jan – Apr 2026', changeLabel: '+6.4% vs YTD 2025' },
}

function fmtRM(n: number): string {
  return `RM ${Math.round(n).toLocaleString()}`
}

const categories  = computed(() => periodCategories[period.value])
const meta        = computed(() => periodMeta[period.value])
const totalActual = computed(() => categories.value.reduce((s, c) => s + c.actual, 0))
const totalBudget = computed(() => categories.value.reduce((s, c) => s + c.budget, 0))
const materialCost = computed(() => (categories.value[0]?.actual ?? 0) + (categories.value[1]?.actual ?? 0))
const overheadCost = computed(() => totalActual.value - materialCost.value)
const vsBudgetAmt   = computed(() => totalActual.value - totalBudget.value)
const vsBudgetPct   = computed(() => totalBudget.value ? (vsBudgetAmt.value / totalBudget.value) * 100 : 0)

const breakdown = computed(() => {
  const max = Math.max(...categories.value.map(c => c.actual))
  return categories.value.map(c => ({
    name: c.name,
    color: c.color,
    value: fmtRM(c.actual),
    pct: totalActual.value ? +(c.actual / totalActual.value * 100).toFixed(1) : 0,
    barPct: max ? Math.round((c.actual / max) * 100) : 0,
  }))
})

async function exportToExcel() {
  const wb = new ExcelJS.Workbook()
  wb.creator = 'HIAS'
  const ws = wb.addWorksheet(`Cost Breakdown (${period.value})`)

  ws.columns = [
    { header: 'Category', key: 'name',  width: 18 },
    { header: '% of Total', key: 'pct', width: 12 },
    { header: 'Value',    key: 'value', width: 16 },
  ]

  const headerRow = ws.getRow(1)
  headerRow.height = 22
  headerRow.eachCell(cell => {
    cell.fill = { type: 'pattern', pattern: 'solid', fgColor: { argb: 'FF1565C0' } }
    cell.font = { bold: true, color: { argb: 'FFFFFFFF' }, size: 11, name: 'Calibri' }
    cell.alignment = { vertical: 'middle', horizontal: 'left' }
    cell.border = { bottom: { style: 'medium', color: { argb: 'FF0D47A1' } } }
  })

  breakdown.value.forEach((b, i) => {
    const row = ws.addRow({ name: b.name, pct: b.pct, value: b.value })
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
  a.download = `Cost_Analysis_${new Date().toISOString().slice(0, 10)}.xlsx`
  a.click()
  URL.revokeObjectURL(url)
}

const budgetVsActual = computed(() => categories.value.map(c => {
  const variancePct = c.budget ? ((c.actual - c.budget) / c.budget) * 100 : 0
  const over = variancePct >= 0
  return {
    name: c.name === 'Other' ? 'Other Overhead' : c.name,
    budget: fmtRM(c.budget),
    actual: fmtRM(c.actual),
    variance: `${over ? '+' : '−'}${Math.abs(variancePct).toFixed(1)}%`,
    over,
    budgetBarPct: 100,
    actualBarPct: c.budget ? Math.round((c.actual / c.budget) * 100) : 0,
  }
}))

interface MaterialRowRaw { name: string; category: string; categoryLabel: string; total: number; vsLastMonth: number; vsPositive: boolean }

const periodMaterialRows: Record<string, MaterialRowRaw[]> = {
  'MTD': [
    { name: 'Halal Seasoning Blend A', category: 'food',      categoryLabel: 'Food Ingredient',     total: 38200, vsLastMonth: 1400,  vsPositive: true  },
    { name: 'FFS Film Roll 500mm',     category: 'packaging', categoryLabel: 'Packaging',           total: 28600, vsLastMonth: -320,  vsPositive: false },
    { name: 'Tomato Paste (Drum)',     category: 'food',      categoryLabel: 'Food Ingredient',     total: 24800, vsLastMonth: 2100,  vsPositive: true  },
    { name: 'Palm Olein Oil (5L)',     category: 'food',      categoryLabel: 'Food Ingredient',     total: 18900, vsLastMonth: -600,  vsPositive: false },
    { name: 'Corrugated Box 400×300',  category: 'packaging', categoryLabel: 'Packaging',           total: 17000, vsLastMonth: 800,   vsPositive: true  },
    { name: 'White Sugar (50kg bag)',  category: 'food',      categoryLabel: 'Food Ingredient',     total: 6900,  vsLastMonth: -300,  vsPositive: false },
    { name: 'Label Sticker 50×30mm',   category: 'packaging', categoryLabel: 'Packaging',           total: 3360,  vsLastMonth: 160,   vsPositive: true  },
    { name: 'LDPE Shrink Film 20µm',   category: 'packaging', categoryLabel: 'Packaging',           total: 1980,  vsLastMonth: -220,  vsPositive: false },
    { name: 'Sodium Benzoate E211',    category: 'chemical',  categoryLabel: 'Chemical / Additive', total: 1296,  vsLastMonth: 96,    vsPositive: true  },
    { name: 'Vitamin C Ascorbic Acid', category: 'chemical',  categoryLabel: 'Chemical / Additive', total: 912,   vsLastMonth: -38,   vsPositive: false },
  ],
  'This Month': [
    { name: 'Halal Seasoning Blend A', category: 'food',      categoryLabel: 'Food Ingredient',     total: 44500, vsLastMonth: 1630,  vsPositive: true  },
    { name: 'FFS Film Roll 500mm',     category: 'packaging', categoryLabel: 'Packaging',           total: 33400, vsLastMonth: -370,  vsPositive: false },
    { name: 'Tomato Paste (Drum)',     category: 'food',      categoryLabel: 'Food Ingredient',     total: 28900, vsLastMonth: 2450,  vsPositive: true  },
    { name: 'Palm Olein Oil (5L)',     category: 'food',      categoryLabel: 'Food Ingredient',     total: 22000, vsLastMonth: -700,  vsPositive: false },
    { name: 'Corrugated Box 400×300',  category: 'packaging', categoryLabel: 'Packaging',           total: 19800, vsLastMonth: 930,   vsPositive: true  },
    { name: 'White Sugar (50kg bag)',  category: 'food',      categoryLabel: 'Food Ingredient',     total: 8050,  vsLastMonth: -350,  vsPositive: false },
    { name: 'Label Sticker 50×30mm',   category: 'packaging', categoryLabel: 'Packaging',           total: 3920,  vsLastMonth: 190,   vsPositive: true  },
    { name: 'LDPE Shrink Film 20µm',   category: 'packaging', categoryLabel: 'Packaging',           total: 2310,  vsLastMonth: -260,  vsPositive: false },
    { name: 'Sodium Benzoate E211',    category: 'chemical',  categoryLabel: 'Chemical / Additive', total: 1510,  vsLastMonth: 110,   vsPositive: true  },
    { name: 'Vitamin C Ascorbic Acid', category: 'chemical',  categoryLabel: 'Chemical / Additive', total: 1060,  vsLastMonth: -40,   vsPositive: false },
  ],
  'Q1 2026': [
    { name: 'Halal Seasoning Blend A', category: 'food',      categoryLabel: 'Food Ingredient',     total: 112700, vsLastMonth: 4130,  vsPositive: true  },
    { name: 'FFS Film Roll 500mm',     category: 'packaging', categoryLabel: 'Packaging',           total: 84400,  vsLastMonth: -940,  vsPositive: false },
    { name: 'Tomato Paste (Drum)',     category: 'food',      categoryLabel: 'Food Ingredient',     total: 73200,  vsLastMonth: 6200,  vsPositive: true  },
    { name: 'Palm Olein Oil (5L)',     category: 'food',      categoryLabel: 'Food Ingredient',     total: 55800,  vsLastMonth: -1770, vsPositive: false },
    { name: 'Corrugated Box 400×300',  category: 'packaging', categoryLabel: 'Packaging',           total: 50200,  vsLastMonth: 2360,  vsPositive: true  },
    { name: 'White Sugar (50kg bag)',  category: 'food',      categoryLabel: 'Food Ingredient',     total: 20400,  vsLastMonth: -890,  vsPositive: false },
    { name: 'Label Sticker 50×30mm',   category: 'packaging', categoryLabel: 'Packaging',           total: 9910,   vsLastMonth: 470,   vsPositive: true  },
    { name: 'LDPE Shrink Film 20µm',   category: 'packaging', categoryLabel: 'Packaging',           total: 5840,   vsLastMonth: -650,  vsPositive: false },
    { name: 'Sodium Benzoate E211',    category: 'chemical',  categoryLabel: 'Chemical / Additive', total: 3820,   vsLastMonth: 280,   vsPositive: true  },
    { name: 'Vitamin C Ascorbic Acid', category: 'chemical',  categoryLabel: 'Chemical / Additive', total: 2690,   vsLastMonth: -110,  vsPositive: false },
  ],
  'YTD': [
    { name: 'Halal Seasoning Blend A', category: 'food',      categoryLabel: 'Food Ingredient',     total: 154300, vsLastMonth: 5660,  vsPositive: true  },
    { name: 'FFS Film Roll 500mm',     category: 'packaging', categoryLabel: 'Packaging',           total: 115500, vsLastMonth: -1290, vsPositive: false },
    { name: 'Tomato Paste (Drum)',     category: 'food',      categoryLabel: 'Food Ingredient',     total: 100200, vsLastMonth: 8480,  vsPositive: true  },
    { name: 'Palm Olein Oil (5L)',     category: 'food',      categoryLabel: 'Food Ingredient',     total: 76300,  vsLastMonth: -2420, vsPositive: false },
    { name: 'Corrugated Box 400×300',  category: 'packaging', categoryLabel: 'Packaging',           total: 68700,  vsLastMonth: 3230,  vsPositive: true  },
    { name: 'White Sugar (50kg bag)',  category: 'food',      categoryLabel: 'Food Ingredient',     total: 27900,  vsLastMonth: -1210, vsPositive: false },
    { name: 'Label Sticker 50×30mm',   category: 'packaging', categoryLabel: 'Packaging',           total: 13600,  vsLastMonth: 650,   vsPositive: true  },
    { name: 'LDPE Shrink Film 20µm',   category: 'packaging', categoryLabel: 'Packaging',           total: 8000,   vsLastMonth: -890,  vsPositive: false },
    { name: 'Sodium Benzoate E211',    category: 'chemical',  categoryLabel: 'Chemical / Additive', total: 5240,   vsLastMonth: 390,   vsPositive: true  },
    { name: 'Vitamin C Ascorbic Acid', category: 'chemical',  categoryLabel: 'Chemical / Additive', total: 3680,   vsLastMonth: -150,  vsPositive: false },
  ],
}

const matCat = ref<string>('all')

const materialRowsForPeriod = computed(() => periodMaterialRows[period.value])
const materialTotalForPeriod = computed(() => materialRowsForPeriod.value.reduce((s, r) => s + r.total, 0))

const filteredMaterial = computed(() => {
  const rows = matCat.value === 'all'
    ? materialRowsForPeriod.value
    : materialRowsForPeriod.value.filter(r => r.category === matCat.value)
  const total = materialTotalForPeriod.value
  return rows.map(r => ({
    ...r,
    pct: total ? `${(r.total / total * 100).toFixed(1)}%` : '0%',
    vsLastMonth: `${r.vsLastMonth >= 0 ? '+' : '−'}${Math.abs(r.vsLastMonth).toLocaleString()}`,
  }))
})

const matTotal = computed(() => filteredMaterial.value.reduce((s, r) => s + r.total, 0))

interface OverheadRowRaw { category: string; item: string; budget: number; actual: number }

const periodOverhead: Record<string, OverheadRowRaw[]> = {
  'MTD': [
    { category: 'Utilities',   item: 'Electricity — Production',    budget: 19100, actual: 21400 },
    { category: 'Utilities',   item: 'Electricity — Cooling',       budget: 6800,  actual: 7000  },
    { category: 'Utilities',   item: 'Natural Gas',                 budget: 3600,  actual: 3400  },
    { category: 'Utilities',   item: 'Water',                       budget: 1900,  actual: 1800  },
    { category: 'Labour',      item: 'Direct Labour (Production)',  budget: 22000, actual: 21600 },
    { category: 'Labour',      item: 'Overtime Allowance',          budget: 4000,  actual: 4800  },
    { category: 'Labour',      item: 'Contractor Labour',           budget: 2000,  actual: 1500  },
    { category: 'Maintenance', item: 'Preventive Maintenance',      budget: 8000,  actual: 9200  },
    { category: 'Maintenance', item: 'Corrective Repairs',          budget: 4000,  actual: 6400  },
    { category: 'Maintenance', item: 'Consumables / Lubrication',   budget: 3000,  actual: 2300  },
    { category: 'Other',       item: 'Logistics / Inbound Freight', budget: 5000,  actual: 5200  },
    { category: 'Other',       item: 'Admin & General',             budget: 4000,  actual: 3700  },
  ],
  'This Month': [
    { category: 'Utilities',   item: 'Electricity — Production',    budget: 22300, actual: 25000 },
    { category: 'Utilities',   item: 'Electricity — Cooling',       budget: 7900,  actual: 8200  },
    { category: 'Utilities',   item: 'Natural Gas',                 budget: 4200,  actual: 4000  },
    { category: 'Utilities',   item: 'Water',                       budget: 2200,  actual: 2100  },
    { category: 'Labour',      item: 'Direct Labour (Production)',  budget: 25700, actual: 25200 },
    { category: 'Labour',      item: 'Overtime Allowance',          budget: 4700,  actual: 5600  },
    { category: 'Labour',      item: 'Contractor Labour',           budget: 2300,  actual: 1750  },
    { category: 'Maintenance', item: 'Preventive Maintenance',      budget: 9300,  actual: 10700 },
    { category: 'Maintenance', item: 'Corrective Repairs',          budget: 4700,  actual: 7500  },
    { category: 'Maintenance', item: 'Consumables / Lubrication',   budget: 3500,  actual: 2700  },
    { category: 'Other',       item: 'Logistics / Inbound Freight', budget: 5800,  actual: 6100  },
    { category: 'Other',       item: 'Admin & General',             budget: 4700,  actual: 4300  },
  ],
  'Q1 2026': [
    { category: 'Utilities',   item: 'Electricity — Production',    budget: 56400, actual: 63100 },
    { category: 'Utilities',   item: 'Electricity — Cooling',       budget: 20100, actual: 20700 },
    { category: 'Utilities',   item: 'Natural Gas',                 budget: 10600, actual: 10000 },
    { category: 'Utilities',   item: 'Water',                       budget: 5600,  actual: 5300  },
    { category: 'Labour',      item: 'Direct Labour (Production)',  budget: 64900, actual: 63700 },
    { category: 'Labour',      item: 'Overtime Allowance',          budget: 11800, actual: 14200 },
    { category: 'Labour',      item: 'Contractor Labour',           budget: 5900,  actual: 4400  },
    { category: 'Maintenance', item: 'Preventive Maintenance',      budget: 23600, actual: 27100 },
    { category: 'Maintenance', item: 'Corrective Repairs',          budget: 11800, actual: 18900 },
    { category: 'Maintenance', item: 'Consumables / Lubrication',   budget: 8900,  actual: 6800  },
    { category: 'Other',       item: 'Logistics / Inbound Freight', budget: 14800, actual: 15300 },
    { category: 'Other',       item: 'Admin & General',             budget: 11800, actual: 10900 },
  ],
  'YTD': [
    { category: 'Utilities',   item: 'Electricity — Production',    budget: 77200, actual: 86400 },
    { category: 'Utilities',   item: 'Electricity — Cooling',       budget: 27500, actual: 28300 },
    { category: 'Utilities',   item: 'Natural Gas',                 budget: 14500, actual: 13700 },
    { category: 'Utilities',   item: 'Water',                       budget: 7700,  actual: 7300  },
    { category: 'Labour',      item: 'Direct Labour (Production)',  budget: 88900, actual: 87300 },
    { category: 'Labour',      item: 'Overtime Allowance',          budget: 16200, actual: 19400 },
    { category: 'Labour',      item: 'Contractor Labour',           budget: 8100,  actual: 6100  },
    { category: 'Maintenance', item: 'Preventive Maintenance',      budget: 32300, actual: 37200 },
    { category: 'Maintenance', item: 'Corrective Repairs',          budget: 16200, actual: 25900 },
    { category: 'Maintenance', item: 'Consumables / Lubrication',   budget: 12100, actual: 9300  },
    { category: 'Other',       item: 'Logistics / Inbound Freight', budget: 20200, actual: 21000 },
    { category: 'Other',       item: 'Admin & General',             budget: 16200, actual: 14900 },
  ],
}

const overhead = computed(() => periodOverhead[period.value].map(r => {
  const diff = r.actual - r.budget
  const over = diff >= 0
  return {
    ...r,
    over,
    variance: `${over ? '+' : '−'}${Math.abs(diff).toLocaleString()}`,
  }
}))

const overheadTotalActual  = computed(() => overhead.value.reduce((s, r) => s + r.actual, 0))
const overheadTotalBudget  = computed(() => overhead.value.reduce((s, r) => s + r.budget, 0))
const overheadVarianceAmt  = computed(() => overheadTotalActual.value - overheadTotalBudget.value)
const overheadVariancePct  = computed(() => overheadTotalBudget.value ? (overheadVarianceAmt.value / overheadTotalBudget.value) * 100 : 0)

function overBadge(row: { over: boolean; variance: string }): string {
  if (!row.over) return 'b-g'
  const absVal = parseInt(row.variance.replace(/[^0-9]/g, ''))
  return absVal >= 1000 ? 'b-r' : 'b-y'
}

function overLabel(row: { over: boolean; variance: string }): string {
  if (!row.over) return 'Under'
  const absVal = parseInt(row.variance.replace(/[^0-9]/g, ''))
  return absVal >= 1000 ? 'Over' : 'Slight'
}
</script>

<style scoped>
*, *::before, *::after { box-sizing: border-box; margin: 0; padding: 0; }

.ca-root { height: 100%; overflow: hidden; font-family: 'Poppins', sans-serif; font-size: 12px; background: #f5f5f5; color: #333; display: flex; flex-direction: column; }

.topbar { background: #fff; border-bottom: 1px solid #c3c6d4; padding: 0 16px; display: flex; align-items: stretch; flex-shrink: 0; box-shadow: 0 1px 3px rgba(0,0,0,.06); height: 42px; }
.pg-title { font-size: 13px; font-weight: 700; color: #515151; letter-spacing: 0.5px; text-transform: uppercase; white-space: nowrap; display: flex; align-items: center; }
.pg-tabs  { display: flex; margin-left: 16px; align-self: stretch; }
.pg-tab   { background: transparent; border: none; border-bottom: 3px solid transparent; color: #7f7f7f; cursor: pointer; font-family: 'Poppins', sans-serif; font-size: 11px; font-weight: 600; padding: 0 18px; transition: color .15s, border-color .15s; }
.pg-tab:hover  { color: #333; background: #f5f5f5; }
.pg-tab.active { color: #1565c0; border-bottom-color: #1565c0; }
.topbar-right { margin-left: auto; display: flex; align-items: center; gap: 0; }
.per-btn { background: #f5f5f5; border: 1px solid #c3c6d4; border-right: none; color: #515151; cursor: pointer; font-family: 'Poppins', sans-serif; font-size: 10px; font-weight: 600; padding: 4px 11px; }
.per-btn:first-child { border-radius: 3px 0 0 3px; }
.per-btn:last-child  { border-right: 1px solid #c3c6d4; border-radius: 0 3px 3px 0; }
.per-btn.on          { background: #1565c0; border-color: #1565c0; color: #fff; }
.export-btn { margin-left: 10px; background: #1565c0; color: #fff; border: none; border-radius: 4px; padding: 5px 12px; font-family: 'Poppins', sans-serif; font-size: 10px; font-weight: 700; cursor: pointer; display: flex; align-items: center; gap: 6px; transition: background .12s; }
.export-btn:hover { background: #0d47a1; }
.per-btn:hover:not(.on) { background: #e3f2fd; color: #1565c0; }

.tab-pane { display: flex; flex-direction: column; flex: 1; overflow: hidden; min-height: 0; }

.kpi-row  { display: flex; gap: 1px; background: #c3c6d4; border-bottom: 2px solid #c3c6d4; flex-shrink: 0; }
.kpi-card { flex: 1; background: #fff; padding: 9px 16px; display: flex; flex-direction: column; gap: 3px; position: relative; overflow: hidden; border-left: 3px solid #1565c0; transition: transform .15s ease, box-shadow .15s ease; }
.kpi-card:hover { transform: translateY(-2px); box-shadow: 0 6px 14px rgba(0,0,0,.12); z-index: 2; }
.kpi-card::before { content: '\f201'; font-family: 'Font Awesome 6 Free'; font-weight: 900; position: absolute; right: 6px; top: 2px; font-size: 36px; color: #1565c0; opacity: .08; pointer-events: none; }
.kpi-card::after { content: ''; position: absolute; left: 0; right: 0; bottom: 0; height: 3px; background: linear-gradient(90deg, transparent, #1565c0, transparent); background-size: 200% 100%; animation: kpi-shimmer 2.5s linear infinite; opacity: .35; }
@keyframes kpi-shimmer { 0% { background-position: 200% 0; } 100% { background-position: -200% 0; } }
.kpi-label{ font-size: 10px; color: #7f7f7f; text-transform: uppercase; letter-spacing: 0.5px; position: relative; z-index: 1; }
.kpi-val  { font-size: 22px; font-weight: 800; line-height: 1.1; position: relative; z-index: 1; }
.kpi-sub  { font-size: 10px; color: #9e9e9e; display: flex; align-items: center; gap: 3px; }
.c-g   { color: #388E3C; } .c-b { color: #1565c0; } .c-y { color: #f9a825; } .c-r { color: #e53935; } .c-d { color: #515151; } .c-pur { color: #7b1fa2; }
.t-up  { color: #e53935; } .t-dn { color: #388E3C; }

.filter-bar { background: #fff; border-bottom: 1px solid #e8e8e8; padding: 5px 16px; display: flex; align-items: center; gap: 6px; flex-shrink: 0; }
.fl    { font-size: 10px; color: #9e9e9e; font-weight: 600; text-transform: uppercase; letter-spacing: 0.4px; }
.f-btn { background: #f5f5f5; border: 1px solid #c3c6d4; border-radius: 3px; color: #515151; cursor: pointer; font-family: 'Poppins', sans-serif; font-size: 10px; font-weight: 600; padding: 2px 10px; }
.f-btn:hover { background: #e3f2fd; border-color: #90caf9; color: #1565c0; }
.f-btn.on    { background: #1565c0; border-color: #1565c0; color: #fff; }

.body-2col { display: flex; gap: 10px; padding: 10px; flex: 1; overflow: hidden; min-height: 0; }
.body-full  { display: flex; flex-direction: column; padding: 10px; flex: 1; overflow: hidden; min-height: 0; }
.col { flex: 1; display: flex; flex-direction: column; gap: 10px; min-width: 0; overflow: hidden; }

.pc { background: #fff; border: 1px solid #c3c6d4; border-radius: 6px; overflow: hidden; display: flex; flex-direction: column; }
.ph { padding: 8px 12px; font-size: 11px; font-weight: 700; color: #515151; text-transform: uppercase; letter-spacing: 0.4px; flex-shrink: 0; background: linear-gradient(0deg, #d7d7d7 0%, #fff 100%); border-bottom: 1px solid #c3c6d4; display: flex; align-items: center; justify-content: space-between; }
.ph-r { font-size: 10px; font-weight: 400; color: #9e9e9e; text-transform: none; letter-spacing: 0; }
.pc-body { padding: 10px 12px; overflow-y: auto; overflow-x: hidden; flex: 1; }
.pc-body::-webkit-scrollbar { width: 4px; }
.pc-body::-webkit-scrollbar-thumb { background: #c3c6d4; border-radius: 2px; }

.tbl-wrap { flex: 1; overflow-y: auto; overflow-x: hidden; }
.tbl-wrap::-webkit-scrollbar { width: 4px; }
.tbl-wrap::-webkit-scrollbar-thumb { background: #c3c6d4; border-radius: 2px; }
table { width: 100%; border-collapse: collapse; font-size: 11px; }
th { background: linear-gradient(0deg, #d7d7d7 0%, #fff 100%); color: #515151; font-size: 10px; text-transform: uppercase; padding: 7px 10px; text-align: left; border-bottom: 2px solid #c3c6d4; position: sticky; top: 0; z-index: 1; letter-spacing: 0.3px; white-space: nowrap; }
td { padding: 7px 10px; border-bottom: 1px solid #e8e8e8; color: #515151; white-space: nowrap; }
tbody tr:nth-child(even) td { background: #f9f9f9; }
tbody tr:hover td { background: #f2f2f2 !important; }

.badge { display: inline-block; padding: 1px 7px; border-radius: 3px; font-size: 9px; font-weight: 700; }
.b-g   { background: #e8f5e9; color: #388E3C; border: 1px solid #a5d6a7; }
.b-y   { background: #fff9c4; color: #f9a825; border: 1px solid #fdd835; }
.b-r   { background: #ffebee; color: #e53935; border: 1px solid #ef9a9a; }

.cb-row  { display: flex; align-items: center; gap: 8px; margin-bottom: 10px; }
.cb-dot  { width: 10px; height: 10px; border-radius: 2px; flex-shrink: 0; }
.cb-name { flex: 1; font-size: 11px; color: #515151; }
.cb-bg   { width: 110px; background: #e0e0e0; border-radius: 3px; height: 8px; flex-shrink: 0; }
.cb-fill { height: 8px; border-radius: 3px; }
.cb-pct  { font-size: 11px; font-weight: 600; color: #333; width: 38px; text-align: right; flex-shrink: 0; }
.cb-val  { font-size: 10px; color: #9e9e9e; width: 72px; text-align: right; flex-shrink: 0; }

.bva-row   { margin-bottom: 13px; }
.bva-head  { display: flex; justify-content: space-between; align-items: baseline; margin-bottom: 4px; }
.bva-name  { font-size: 11px; font-weight: 600; color: #515151; }
.bva-nums  { font-size: 10px; color: #9e9e9e; }
.bva-track { background: #e0e0e0; border-radius: 3px; height: 10px; position: relative; overflow: visible; }
.bva-b-bar { height: 10px; border-radius: 3px; background: #bbdefb; position: absolute; top: 0; left: 0; }
.bva-a-bar { height: 10px; border-radius: 3px; position: absolute; top: 0; left: 0; }
.bva-leg   { display: flex; gap: 14px; font-size: 10px; color: #9e9e9e; margin-bottom: 12px; }
.swatch    { display: inline-block; width: 10px; height: 10px; border-radius: 2px; margin-right: 4px; vertical-align: middle; }
.var-pos   { color: #e53935; font-weight: 600; }
.var-neg   { color: #388E3C; font-weight: 600; }
.empty-msg { padding: 24px; text-align: center; color: #9e9e9e; font-size: 11px; }
</style>
