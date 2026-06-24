<template>
  <div class="pg-root">
    <div class="topbar">
      <span class="pg-title">&#9632; Product / Food Cost Calculation</span>
      <div class="topbar-right"><span class="live-dot"></span><span>{{ clock }}</span></div>
    </div>

    <div class="body-2col">
      <div class="col" style="flex: 0 0 280px">
        <div class="pc" style="flex:1">
          <div class="ph">Select Product</div>
          <div class="pc-body">
            <div class="form-row">
              <label>Product</label>
              <select v-model="selected">
                <option v-for="p in products" :key="p.name" :value="p.name">{{ p.name }}</option>
              </select>
            </div>
            <div class="cost-total">
              <div class="ct-label">Cost per Unit</div>
              <div class="ct-val">RM {{ currentCost.toFixed(2) }}</div>
            </div>
          </div>
        </div>
      </div>

      <div class="col">
        <div class="pc" style="flex:1">
          <div class="ph">Ingredient Cost Breakdown <span class="ph-r">{{ selected }}</span></div>
          <div class="tbl-wrap">
            <table>
              <thead><tr><th>Ingredient</th><th style="text-align:right">Qty / Unit</th><th>UOM</th><th style="text-align:right">Unit Cost</th><th style="text-align:right">Cost / Unit</th></tr></thead>
              <tbody>
                <tr v-for="(ing, i) in currentIngredients" :key="i">
                  <td>{{ ing.name }}</td>
                  <td style="text-align:right">{{ ing.qty }}</td>
                  <td>{{ ing.uom }}</td>
                  <td style="text-align:right">RM {{ ing.unitCost.toFixed(2) }}</td>
                  <td style="text-align:right">RM {{ (ing.qty * ing.unitCost).toFixed(2) }}</td>
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
import { ref, computed, onMounted, onBeforeUnmount } from 'vue'

const clock = ref('')
let clockTimer: ReturnType<typeof setInterval> | null = null
function tick() { clock.value = new Date().toLocaleString('en-MY', { day: '2-digit', month: 'short', year: 'numeric', hour: '2-digit', minute: '2-digit', hour12: true }) }

const products = [
  {
    name: 'Chili Sauce Pouch 250g',
    ingredients: [
      { name: 'Dried Chili Paste',  qty: 0.08, uom: 'kg', unitCost: 12.50 },
      { name: 'White Refined Sugar', qty: 0.04, uom: 'kg', unitCost: 2.80 },
      { name: 'Citric Acid E330',   qty: 0.002, uom: 'kg', unitCost: 18.00 },
      { name: 'Stand-up Pouch',     qty: 1,     uom: 'pc', unitCost: 0.18 },
    ],
  },
  {
    name: 'Curry Paste Tub 500g',
    ingredients: [
      { name: 'Curry Spice Blend', qty: 0.15, uom: 'kg', unitCost: 9.20 },
      { name: 'Coconut Milk Powder', qty: 0.10, uom: 'kg', unitCost: 14.00 },
      { name: 'Refined Palm Olein', qty: 0.06, uom: 'L',  unitCost: 6.50 },
      { name: 'PET Tub 500g',      qty: 1,     uom: 'pc', unitCost: 0.42 },
    ],
  },
]

const selected = ref(products[0].name)
const currentIngredients = computed(() => products.find(p => p.name === selected.value)?.ingredients ?? [])
const currentCost = computed(() => currentIngredients.value.reduce((sum, i) => sum + i.qty * i.unitCost, 0))

onMounted(() => { tick(); clockTimer = setInterval(tick, 1000) })
onBeforeUnmount(() => { if (clockTimer) clearInterval(clockTimer) })
</script>

<style scoped>
*, *::before, *::after { box-sizing: border-box; margin: 0; padding: 0; }
.pg-root { height: 100%; overflow: hidden; font-family: 'Poppins', sans-serif; font-size: 12px; background: #f5f5f5; color: #333; display: flex; flex-direction: column; }
.topbar { background: #fff; border-bottom: 1px solid #c3c6d4; padding: 0 16px; display: flex; align-items: center; flex-shrink: 0; box-shadow: 0 1px 3px rgba(0,0,0,.06); height: 42px; }
.pg-title { font-size: 13px; font-weight: 700; color: #515151; letter-spacing: 0.5px; text-transform: uppercase; white-space: nowrap; }
.topbar-right { margin-left: auto; display: flex; align-items: center; font-size: 10px; color: #9e9e9e; gap: 6px; }
.live-dot { width: 7px; height: 7px; border-radius: 50%; background: #43a047; animation: pulse 1.4s infinite; display: inline-block; }
@keyframes pulse { 0%,100%{ opacity:1 } 50%{ opacity:0.3 } }

.body-2col { display: flex; gap: 10px; padding: 10px; flex: 1; overflow: hidden; min-height: 0; }
.col { flex: 1; display: flex; flex-direction: column; gap: 10px; min-width: 0; overflow: hidden; }

.pc { background: #fff; border: 1px solid #c3c6d4; border-radius: 6px; overflow: hidden; display: flex; flex-direction: column; }
.ph { padding: 8px 12px; font-size: 11px; font-weight: 700; color: #515151; text-transform: uppercase; letter-spacing: 0.4px; flex-shrink: 0; background: linear-gradient(0deg, #d7d7d7 0%, #fff 100%); border-bottom: 1px solid #c3c6d4; display: flex; align-items: center; justify-content: space-between; }
.ph-r { font-size: 10px; font-weight: 400; color: #9e9e9e; text-transform: none; letter-spacing: 0; }
.pc-body { padding: 14px; overflow-y: auto; overflow-x: hidden; flex: 1; }

.form-row { display: flex; flex-direction: column; gap: 4px; margin-bottom: 14px; }
.form-row label { font-size: 10px; color: #7f7f7f; font-weight: 600; text-transform: uppercase; letter-spacing: 0.4px; }
.form-row select { border: 1px solid #c3c6d4; border-radius: 4px; padding: 6px 8px; font-family: 'Poppins', sans-serif; font-size: 12px; color: #333; outline: none; }
.form-row select:focus { border-color: #1565c0; }

.cost-total { background: linear-gradient(135deg, #e3f2fd, #f5f9ff); border: 1px solid #90caf9; border-radius: 6px; padding: 14px; text-align: center; position: relative; overflow: hidden; box-shadow: 0 2px 8px rgba(21,101,192,.08); }
.cost-total::before { content: '\f155'; font-family: 'Font Awesome 6 Free'; font-weight: 900; position: absolute; right: 8px; top: 4px; font-size: 38px; color: #1565c0; opacity: .10; pointer-events: none; }
.ct-label { font-size: 10px; color: #7f7f7f; text-transform: uppercase; letter-spacing: 0.4px; position: relative; z-index: 1; }
.ct-val { font-size: 26px; font-weight: 800; color: #1565c0; margin-top: 4px; position: relative; z-index: 1; }

.tbl-wrap { flex: 1; overflow-y: auto; overflow-x: hidden; }
table { width: 100%; border-collapse: collapse; font-size: 11px; }
th { background: linear-gradient(0deg, #d7d7d7 0%, #fff 100%); color: #515151; font-size: 10px; text-transform: uppercase; padding: 7px 10px; text-align: left; border-bottom: 2px solid #c3c6d4; position: sticky; top: 0; z-index: 1; letter-spacing: 0.3px; white-space: nowrap; }
td { padding: 7px 10px; border-bottom: 1px solid #e8e8e8; color: #515151; white-space: nowrap; }
tbody tr:nth-child(even) td { background: #f9f9f9; }
tbody tr:hover td { background: #f2f2f2 !important; }
</style>
