<template>
  <div class="page-root">

    <!-- TOPBAR — always visible -->
    <div class="topbar">
      <span class="pg-title">&#9632; Outgoing Records</span>
      <div class="topbar-right">
        <span class="live-dot"></span>
        <span>{{ clock }}</span>
      </div>
    </div>

    <!-- ══════════════════════════════════════════════
         INSPECTION VIEW  (Option C — full-screen drill-down)
         Lift <div class="insp-body"> into a tab panel for Option B
    ═══════════════════════════════════════════════════ -->
    <template v-if="inspectRow !== null">

      <!-- Inspection nav bar -->
      <div class="insp-nav">
        <button class="insp-back" @click="closeInspect">
          <i class="fa-solid fa-chevron-left"></i> Outgoing Records
        </button>
        <span class="insp-sep">›</span>
        <span class="insp-nav-title">Dispatch Inspection — Stock Out #{{ inspectRow.id }}</span>
        <span :class="['isc', 'isc--' + (inspForm.status)]" style="margin-left:10px">{{ inspStatusLabel }}</span>
        <div style="margin-left:auto;display:flex;align-items:center;gap:8px">
          <button class="insp-close-btn" @click="closeInspect" title="Close without saving">
            <i class="fa-solid fa-xmark"></i>
          </button>
        </div>
      </div>

      <!-- ── Inspection body — lift this block for Option B tab ── -->
      <div class="insp-body">

        <!-- Record Summary -->
        <div class="insp-card">
          <div class="insp-card-title">Record Summary</div>
          <div class="insp-summary-grid">
            <div class="is-field"><span class="is-label">Stock Out Type</span><span class="is-val">{{ inspectRow.stockOutType }}</span></div>
            <div class="is-field"><span class="is-label">Document</span><span class="is-val">{{ inspectRow.document }}</span></div>
            <div class="is-field"><span class="is-label">Order No.</span><span class="is-val">{{ inspectRow.orderNo || '—' }}</span></div>
            <div class="is-field"><span class="is-label">Method</span>
              <span :class="['method-badge', inspectRow.method === 'RFID' ? 'badge-rfid' : 'badge-scan']">{{ inspectRow.method }}</span>
            </div>
            <div class="is-field"><span class="is-label">Consignee</span><span class="is-val">{{ inspectRow.consignee || '—' }}</span></div>
            <div class="is-field"><span class="is-label">Goods Qty</span><span class="is-val">{{ inspectRow.goodsTotal }}</span></div>
            <div class="is-field"><span class="is-label">SKU Count</span><span class="is-val">{{ inspectRow.skuTotal }}</span></div>
            <div class="is-field"><span class="is-label">Pack Unit</span><span class="is-val">{{ inspectRow.packUnit || '—' }}</span></div>
          </div>
        </div>

        <!-- Dispatch Checklist -->
        <div class="insp-card" style="flex:1;min-height:0;display:flex;flex-direction:column">
          <div class="insp-card-title">
            Dispatch Checklist
            <span class="insp-card-hint">Verify each line item before dispatch</span>
          </div>
          <div class="lc-wrap">
            <table class="lc-table">
              <thead>
                <tr>
                  <th style="min-width:90px">SKU Code</th>
                  <th style="min-width:140px">SKU Name</th>
                  <th style="min-width:70px;text-align:right">Order Qty</th>
                  <th style="min-width:80px;text-align:right">Dispatched Qty</th>
                  <th style="min-width:110px">Packaging</th>
                  <th style="min-width:70px;text-align:center">Halal Label</th>
                  <th style="min-width:75px;text-align:center">Expiry Valid</th>
                  <th style="min-width:160px">Notes</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="(lc, idx) in inspLineChecks" :key="idx" :class="lcRowClass(lc)">
                  <td class="lc-ro">{{ lc.skuCode }}</td>
                  <td class="lc-ro">{{ lc.skuName }}</td>
                  <td class="lc-ro" style="text-align:right">{{ lc.orderQty }}</td>
                  <td style="text-align:right">
                    <input class="lc-input lc-num" type="number" v-model.number="lc.dispatchedQty" min="0" />
                  </td>
                  <td>
                    <select class="lc-select" v-model="lc.packaging">
                      <option>Intact</option>
                      <option>Damaged</option>
                      <option>Repackaged</option>
                    </select>
                  </td>
                  <td style="text-align:center">
                    <input type="checkbox" v-model="lc.halalLabel" />
                  </td>
                  <td style="text-align:center">
                    <input type="checkbox" v-model="lc.expiryValid" />
                  </td>
                  <td>
                    <input class="lc-input" type="text" v-model="lc.notes" placeholder="Remarks…" />
                  </td>
                </tr>
                <tr v-if="inspLineChecks.length === 0">
                  <td colspan="8" style="text-align:center;color:#9e9e9e;font-size:10px;padding:16px">No line items on this record.</td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>

        <!-- Sign-off -->
        <div class="insp-card">
          <div class="insp-card-title">Dispatch Sign-off</div>
          <div class="insp-signoff-grid">
            <div class="dp-field">
              <label>Inspector Name <span class="req">*</span></label>
              <input type="text" v-model="inspForm.inspector" placeholder="Full name" />
            </div>
            <div class="dp-field">
              <label>Dispatch Date</label>
              <input type="date" v-model="inspForm.dispatchedAt" />
            </div>
            <div class="dp-field" style="grid-column:1/-1">
              <label>Remarks</label>
              <input type="text" v-model="inspForm.remarks" placeholder="Overall dispatch remarks…" />
            </div>
          </div>
        </div>

      </div><!-- /insp-body -->

      <!-- Inspection footer -->
      <div class="insp-footer">
        <button class="ifp-btn ifp-back"    @click="submitInspection('pending')">
          <i class="fa-solid fa-rotate-left"></i> Send Back
        </button>
        <div style="margin-left:auto;display:flex;gap:8px">
          <button class="ifp-btn ifp-reject"  @click="submitInspection('failed')">
            <i class="fa-solid fa-circle-xmark"></i> Reject
          </button>
          <button class="ifp-btn ifp-approve" @click="submitInspection('passed')">
            <i class="fa-solid fa-circle-check"></i> Approve
          </button>
        </div>
      </div>

    </template>

    <!-- ══════════════════════════════════════════════
         LIST VIEW
    ═══════════════════════════════════════════════════ -->
    <template v-else>

      <!-- SEARCH / FILTER BAR -->
      <div class="filter-bar">
        <span class="fl">Consignee</span>
        <input class="f-search" type="text" placeholder="Enter consignee name" v-model="consigneeSearch" @keyup.enter="applySearch" style="margin-left:0; width:200px" />
        <button class="f-btn-primary" @click="applySearch"><i class="fa-solid fa-magnifying-glass"></i> Search</button>
        <button class="f-btn-outline" @click="resetSearch"><i class="fa-solid fa-rotate-right"></i> Reset</button>
        <span class="result-count" style="margin-left:auto">{{ filteredRows.length }} records</span>
      </div>

      <!-- ACTION BAR -->
      <div class="action-bar">
        <button class="act-btn act-new"    @click="openNew()"><i class="fa-solid fa-plus"></i> New</button>
        <button class="act-btn act-edit"   @click="openEditSelected()" :disabled="checkedIds.size !== 1"><i class="fa-regular fa-pen-to-square"></i> Edit</button>
        <button class="act-btn act-delete" @click="openDeleteSelected()" :disabled="checkedIds.size === 0"><i class="fa-regular fa-trash-can"></i> Delete</button>
        <button class="act-btn act-export"><i class="fa-solid fa-arrow-up-from-bracket"></i> Export</button>
        <button class="act-icon-btn" style="margin-left:auto" title="Column settings"><i class="fa-solid fa-magnifying-glass"></i></button>
      </div>

      <!-- TABLE PANEL -->
      <div class="body-full">
        <div class="pc" style="flex:1">
          <div class="ph">
            Stock Out Records
            <span class="ph-r">{{ filteredRows.length }} records</span>
          </div>
          <div class="tbl-wrap">
            <table>
              <thead>
                <tr>
                  <th class="col-chk"><input type="checkbox" v-model="allChecked" @change="toggleAll" /></th>
                  <th class="col-id">ID</th>
                  <th style="min-width:115px">Stock Out Type</th>
                  <th style="min-width:105px">Document</th>
                  <th style="min-width:145px">Order No.</th>
                  <th style="min-width:65px">Method</th>
                  <th style="min-width:145px">Consignee</th>
                  <th style="min-width:78px;text-align:right">Goods Qty</th>
                  <th style="min-width:78px;text-align:right">SKU Count</th>
                  <th style="min-width:78px">Pack Unit</th>
                  <th style="min-width:100px">Created At</th>
                  <th style="min-width:88px">Created By</th>
                  <th class="col-actions">Actions</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="row in filteredRows" :key="row.id"
                    :class="{ 'row-sel': checkedIds.has(row.id) }"
                    class="row-clickable"
                    @click="openInspect(row)">
                  <td class="col-chk" @click.stop><input type="checkbox" :checked="checkedIds.has(row.id)" @change="toggleRow(row.id)" /></td>
                  <td class="col-id">
                    {{ row.id }}
                    <span v-if="inspectionMap[row.id]" :class="['insp-dot', 'idot--' + inspectionMap[row.id]!.status]" :title="inspectionMap[row.id]!.status"></span>
                  </td>
                  <td>{{ row.stockOutType }}</td>
                  <td>{{ row.document }}</td>
                  <td :class="{ dash: !row.orderNo }">{{ row.orderNo || '—' }}</td>
                  <td><span :class="['method-badge', row.method === 'RFID' ? 'badge-rfid' : 'badge-scan']">{{ row.method }}</span></td>
                  <td :class="{ dash: !row.consignee }">{{ row.consignee || '—' }}</td>
                  <td style="text-align:right">{{ row.goodsTotal }}</td>
                  <td style="text-align:right">{{ row.skuTotal }}</td>
                  <td :class="{ dash: !row.packUnit }">{{ row.packUnit || '—' }}</td>
                  <td>{{ row.createdAt }}</td>
                  <td :class="{ dash: !row.createdBy }">{{ row.createdBy || '—' }}</td>
                  <td class="col-actions" @click.stop>
                    <div><a class="row-edit" @click.prevent="openEdit(row)"><i class="fa-regular fa-pen-to-square"></i> Edit</a></div>
                    <div><a class="row-del"  @click.prevent="openDelete(row.id)"><i class="fa-regular fa-trash-can"></i> Delete</a></div>
                  </td>
                </tr>
                <tr v-if="filteredRows.length === 0">
                  <td colspan="13" class="empty-msg">No records found.</td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>

      <!-- PAGINATION -->
      <div class="pag-bar">
        <span class="pag-info">Total {{ filteredRows.length }} records</span>
        <div class="pag-pages">
          <button class="pag-btn" disabled>&lt;</button>
          <button class="pag-btn pag-active">1</button>
          <button class="pag-btn">&gt;</button>
        </div>
      </div>

      <!-- ── DETAIL PANEL OVERLAY ── -->
      <transition name="overlay-fade">
        <div v-if="detailMode !== null" class="overlay" @click.self="closeDetail"></div>
      </transition>
      <transition name="panel-slide">
        <div v-if="detailMode !== null" class="detail-panel">

          <!-- Panel header -->
          <div class="dp-header">
            <div class="dp-header-left">
              <span class="dp-badge" :class="detailMode === 'new' ? 'badge-new' : 'badge-edit'">
                {{ detailMode === 'new' ? 'NEW RECORD' : 'EDIT RECORD' }}
              </span>
              <span class="dp-title">{{ detailMode === 'new' ? 'Stock Out — New Entry' : `Stock Out #${editForm.id}` }}</span>
            </div>
            <button class="dp-close" @click="closeDetail"><i class="fa-solid fa-xmark"></i></button>
          </div>

          <!-- Scrollable body -->
          <div class="dp-body">

            <!-- Section: Record Info -->
            <div class="dp-section-title">Record Information</div>
            <div class="dp-grid">
              <div class="dp-field">
                <label>Stock Out Type <span class="req">*</span></label>
                <select v-model="editForm.stockOutType">
                  <option>Sales Out</option>
                  <option>Transfer Out</option>
                  <option>Return Out</option>
                  <option>Shortage Out</option>
                </select>
              </div>
              <div class="dp-field">
                <label>Document Type <span class="req">*</span></label>
                <select v-model="editForm.document">
                  <option>With Order</option>
                  <option>Without Order</option>
                </select>
              </div>
              <div class="dp-field">
                <label>Order No.</label>
                <input type="text" v-model="editForm.orderNo" placeholder="e.g. SLS-2026-0001" :disabled="editForm.document === 'Without Order'" />
              </div>
              <div class="dp-field">
                <label>Method <span class="req">*</span></label>
                <select v-model="editForm.method">
                  <option>Scan</option>
                  <option>RFID</option>
                </select>
              </div>
              <div class="dp-field dp-field-full">
                <label>Consignee</label>
                <input type="text" v-model="editForm.consignee" placeholder="Enter consignee name" />
              </div>
            </div>

            <!-- Section: Quantity & Packaging -->
            <div class="dp-section-title" style="margin-top:16px">Quantity & Packaging</div>
            <div class="dp-grid">
              <div class="dp-field">
                <label>Goods Qty</label>
                <input type="number" v-model.number="editForm.goodsTotal" min="0" />
              </div>
              <div class="dp-field">
                <label>SKU Count</label>
                <input type="number" v-model.number="editForm.skuTotal" min="0" />
              </div>
              <div class="dp-field">
                <label>Pack Unit</label>
                <select v-model="editForm.packUnit">
                  <option value="">— None —</option>
                  <option>Box</option>
                  <option>Carton</option>
                  <option>Pallet</option>
                  <option>Bag</option>
                  <option>Drum</option>
                </select>
              </div>
            </div>

            <!-- Section: Line Items -->
            <div class="dp-section-title" style="margin-top:16px">
              Line Items
              <button class="li-add-btn" @click="addLineItem"><i class="fa-solid fa-plus"></i> Add SKU</button>
            </div>
            <div class="li-table-wrap">
              <table class="li-table">
                <thead>
                  <tr>
                    <th style="min-width:90px">SKU Code</th>
                    <th style="min-width:140px">SKU Name</th>
                    <th style="min-width:55px;text-align:right">Qty</th>
                    <th style="min-width:55px">Unit</th>
                    <th style="min-width:90px">Batch No.</th>
                    <th style="min-width:90px">Expiry</th>
                    <th style="width:32px"></th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="(li, idx) in editLineItems" :key="idx">
                    <td><input class="li-input" v-model="li.skuCode" placeholder="SKU-001" /></td>
                    <td><input class="li-input" v-model="li.skuName" placeholder="Product name" /></td>
                    <td><input class="li-input li-num" type="number" v-model.number="li.qty" min="0" /></td>
                    <td>
                      <select class="li-select" v-model="li.unit">
                        <option>pcs</option><option>kg</option><option>L</option><option>box</option><option>carton</option>
                      </select>
                    </td>
                    <td><input class="li-input" v-model="li.batchNo" placeholder="BT-001" /></td>
                    <td><input class="li-input" type="date" v-model="li.expiryDate" /></td>
                    <td><button class="li-del-btn" @click="removeLineItem(idx)" title="Remove"><i class="fa-solid fa-xmark"></i></button></td>
                  </tr>
                  <tr v-if="editLineItems.length === 0">
                    <td colspan="7" style="text-align:center;color:#9e9e9e;font-size:10px;padding:12px">No line items. Click + Add SKU to add one.</td>
                  </tr>
                </tbody>
              </table>
            </div>

            <!-- Section: Meta -->
            <div class="dp-section-title" style="margin-top:16px">Record Meta</div>
            <div class="dp-grid">
              <div class="dp-field">
                <label>Created At</label>
                <input type="date" v-model="editForm.createdAt" />
              </div>
              <div class="dp-field">
                <label>Created By</label>
                <input type="text" v-model="editForm.createdBy" placeholder="Operator name" />
              </div>
            </div>

          </div><!-- /dp-body -->

          <!-- Panel footer -->
          <div class="dp-footer">
            <button class="fp-btn fp-cancel" @click="closeDetail"><i class="fa-solid fa-xmark"></i> Cancel</button>
            <button class="fp-btn fp-save" @click="saveRecord"><i class="fa-solid fa-floppy-disk"></i> {{ detailMode === 'new' ? 'Create' : 'Save Changes' }}</button>
          </div>

        </div>
      </transition>

      <!-- ── DELETE CONFIRM MODAL ── -->
      <transition name="overlay-fade">
        <div v-if="showDelete" class="overlay" @click.self="showDelete = false"></div>
      </transition>
      <transition name="modal-pop">
        <div v-if="showDelete" class="del-modal">
          <div class="del-icon"><i class="fa-solid fa-triangle-exclamation"></i></div>
          <div class="del-title">Delete {{ deleteIds.length > 1 ? deleteIds.length + ' Records' : 'Record' }}?</div>
          <div class="del-msg">
            This will permanently remove
            <strong>{{ deleteIds.length > 1 ? deleteIds.length + ' stock out records' : 'stock out record #' + deleteIds[0] }}</strong>.
            This action cannot be undone.
          </div>
          <div class="del-actions">
            <button class="fp-btn fp-cancel" @click="showDelete = false">Cancel</button>
            <button class="fp-btn fp-delete" @click="confirmDelete"><i class="fa-regular fa-trash-can"></i> Delete</button>
          </div>
        </div>
      </transition>

    </template>

  </div>
</template>

<script setup lang="ts">
import { ref, computed, reactive, onMounted, onBeforeUnmount } from 'vue'

const clock           = ref('')
const consigneeSearch = ref('')
const activeFilter    = ref('')
const allChecked      = ref(false)
const checkedIds      = ref<Set<number>>(new Set())
let clockTimer: ReturnType<typeof setInterval> | null = null

function tick() {
  clock.value = new Date().toLocaleString('en-MY', {
    day: '2-digit', month: 'short', year: 'numeric',
    hour: '2-digit', minute: '2-digit', hour12: true,
  })
}

function applySearch() { activeFilter.value = consigneeSearch.value.trim() }
function resetSearch()  { consigneeSearch.value = ''; activeFilter.value = '' }

function toggleAll() {
  if (allChecked.value) filteredRows.value.forEach(r => checkedIds.value.add(r.id))
  else checkedIds.value.clear()
}
function toggleRow(id: number) {
  checkedIds.value = new Set(checkedIds.value)
  if (checkedIds.value.has(id)) checkedIds.value.delete(id)
  else checkedIds.value.add(id)
}

// ── Data ──────────────────────────────────────────
interface LineItem { skuCode: string; skuName: string; qty: number; unit: string; batchNo: string; expiryDate: string }
interface StockOutRow {
  id: number; stockOutType: string; document: string; orderNo: string
  method: string; consignee: string; goodsTotal: number; skuTotal: number
  packUnit: string; createdAt: string; createdBy: string
}

const rows = ref<StockOutRow[]>([
  { id: 8, stockOutType: 'Transfer Out', document: 'With Order',    orderNo: '',                method: 'RFID', consignee: '',                   goodsTotal: 0,  skuTotal: 0, packUnit: '',       createdAt: '2026-04-14', createdBy: ''           },
  { id: 7, stockOutType: 'Transfer Out', document: 'With Order',    orderNo: '',                method: 'RFID', consignee: '',                   goodsTotal: 0,  skuTotal: 0, packUnit: '',       createdAt: '2026-04-14', createdBy: ''           },
  { id: 6, stockOutType: 'Sales Out',    document: 'With Order',    orderNo: 'CKD202512250001', method: 'Scan', consignee: 'Aeon Retail Sdn Bhd', goodsTotal: 1,  skuTotal: 2, packUnit: 'Pallet', createdAt: '2026-01-09', createdBy: ''           },
  { id: 5, stockOutType: 'Return Out',   document: 'With Order',    orderNo: 'RTN-2025-0088',   method: 'Scan', consignee: 'Kemas Packaging Sdn', goodsTotal: 3,  skuTotal: 1, packUnit: 'Box',    createdAt: '2025-12-20', createdBy: 'Operator A' },
  { id: 4, stockOutType: 'Sales Out',    document: 'With Order',    orderNo: 'SLS-2025-0342',   method: 'Scan', consignee: 'Giant Supermarket',   goodsTotal: 12, skuTotal: 4, packUnit: 'Carton', createdAt: '2025-12-15', createdBy: 'Operator B' },
  { id: 3, stockOutType: 'Transfer Out', document: 'Without Order', orderNo: '',                method: 'RFID', consignee: 'Branch Warehouse B',  goodsTotal: 20, skuTotal: 6, packUnit: 'Pallet', createdAt: '2025-11-30', createdBy: 'Operator A' },
  { id: 2, stockOutType: 'Shortage Out', document: 'Without Order', orderNo: '',                method: 'Scan', consignee: '',                   goodsTotal: 2,  skuTotal: 1, packUnit: 'Box',    createdAt: '2025-11-15', createdBy: 'Admin'      },
  { id: 1, stockOutType: 'Sales Out',    document: 'With Order',    orderNo: 'SLS-2025-0100',   method: 'Scan', consignee: 'Hero Market Sdn Bhd', goodsTotal: 50, skuTotal: 8, packUnit: 'Carton', createdAt: '2025-10-01', createdBy: 'Admin'      },
])

const lineItemsMap: Record<number, LineItem[]> = {
  6: [
    { skuCode: 'SKU-0021', skuName: 'Palm Olein Oil (5L)',      qty: 1,  unit: 'pcs',    batchNo: 'BT-6A', expiryDate: '2027-01-01' },
    { skuCode: 'SKU-0034', skuName: 'Corrugated Box 400×300',  qty: 12, unit: 'carton', batchNo: 'BT-6B', expiryDate: '' },
  ],
  5: [{ skuCode: 'SKU-0055', skuName: 'FFS Film Roll 500mm', qty: 3, unit: 'box', batchNo: 'BT-5A', expiryDate: '2026-06-30' }],
  4: [
    { skuCode: 'SKU-0012', skuName: 'Curry Powder (1kg bag)',  qty: 8,  unit: 'carton', batchNo: 'BT-4A', expiryDate: '2026-09-15' },
    { skuCode: 'SKU-0019', skuName: 'Sodium Benzoate E211',   qty: 2,  unit: 'carton', batchNo: 'BT-4B', expiryDate: '2027-03-01' },
    { skuCode: 'SKU-0034', skuName: 'Corrugated Box 400×300', qty: 2,  unit: 'carton', batchNo: 'BT-4C', expiryDate: '' },
    { skuCode: 'SKU-0078', skuName: 'Tomato Paste (Drum)',    qty: 2,  unit: 'L',      batchNo: 'BT-4D', expiryDate: '2026-12-01' },
  ],
  1: [
    { skuCode: 'SKU-0019', skuName: 'Sodium Benzoate E211',  qty: 50, unit: 'carton', batchNo: 'BT-1A', expiryDate: '2027-06-01' },
    { skuCode: 'SKU-0021', skuName: 'Palm Olein Oil (5L)',    qty: 30, unit: 'pcs',   batchNo: 'BT-1B', expiryDate: '2027-01-15' },
    { skuCode: 'SKU-0056', skuName: 'White Sugar (50kg bag)', qty: 20, unit: 'box',   batchNo: 'BT-1C', expiryDate: '2028-01-01' },
  ],
}

const filteredRows = computed(() => {
  const q = activeFilter.value.toLowerCase()
  if (!q) return [...rows.value]
  return rows.value.filter(r => r.consignee.toLowerCase().includes(q))
})

// ── Inspection ────────────────────────────────────
interface InspLineCheck {
  skuCode: string; skuName: string; orderQty: number; dispatchedQty: number
  packaging: 'Intact' | 'Damaged' | 'Repackaged'; halalLabel: boolean; expiryValid: boolean; notes: string
}
interface InspectionData {
  status: 'pending' | 'in_progress' | 'passed' | 'failed'
  inspector: string; dispatchedAt: string; remarks: string; lineChecks: InspLineCheck[]
}

const inspectRow    = ref<StockOutRow | null>(null)
const inspLineChecks = ref<InspLineCheck[]>([])
const inspForm      = ref<InspectionData>({ status: 'in_progress', inspector: '', dispatchedAt: '', remarks: '', lineChecks: [] })
const inspectionMap = reactive<Record<number, InspectionData>>({})

const inspStatusLabel = computed(() => {
  const map: Record<string, string> = { pending: 'Pending', in_progress: 'In Progress', passed: 'Approved', failed: 'Rejected' }
  return map[inspForm.value.status] ?? inspForm.value.status
})

function buildLineChecks(row: StockOutRow): InspLineCheck[] {
  const items = lineItemsMap[row.id]
  if (items && items.length > 0) {
    return items.map(li => ({
      skuCode: li.skuCode, skuName: li.skuName,
      orderQty: li.qty, dispatchedQty: li.qty,
      packaging: 'Intact', halalLabel: true, expiryValid: true, notes: '',
    }))
  }
  return [{
    skuCode: '—', skuName: row.stockOutType,
    orderQty: row.goodsTotal, dispatchedQty: row.goodsTotal,
    packaging: 'Intact', halalLabel: true, expiryValid: true, notes: '',
  }]
}

function openInspect(row: StockOutRow) {
  closeDetail()
  inspectRow.value = row
  const existing = inspectionMap[row.id]
  if (existing) {
    inspLineChecks.value = JSON.parse(JSON.stringify(existing.lineChecks))
    inspForm.value = { ...existing, status: existing.status === 'pending' ? 'in_progress' : existing.status, lineChecks: [] }
  } else {
    inspLineChecks.value = buildLineChecks(row)
    inspForm.value = {
      status: 'in_progress', inspector: '', dispatchedAt: new Date().toISOString().slice(0, 10), remarks: '', lineChecks: [],
    }
  }
}

function closeInspect() { inspectRow.value = null }

function submitInspection(status: InspectionData['status']) {
  if (!inspectRow.value) return
  const saved: InspectionData = {
    ...JSON.parse(JSON.stringify(inspForm.value)),
    status,
    lineChecks: JSON.parse(JSON.stringify(inspLineChecks.value)),
  }
  inspectionMap[inspectRow.value.id] = saved
  closeInspect()
}

function lcRowClass(lc: InspLineCheck): string {
  if (lc.packaging !== 'Intact' || !lc.halalLabel) return 'lc-rejected'
  if (!lc.expiryValid || lc.dispatchedQty < lc.orderQty) return 'lc-warn'
  return ''
}

// ── Detail panel ──────────────────────────────────
type DetailMode = 'edit' | 'new' | null
const detailMode    = ref<DetailMode>(null)
const editForm      = ref<StockOutRow>({ id: 0, stockOutType: 'Sales Out', document: 'With Order', orderNo: '', method: 'Scan', consignee: '', goodsTotal: 0, skuTotal: 0, packUnit: '', createdAt: '', createdBy: '' })
const editLineItems = ref<LineItem[]>([])

function openEdit(row: StockOutRow) {
  editForm.value      = { ...row }
  editLineItems.value = JSON.parse(JSON.stringify(lineItemsMap[row.id] ?? []))
  detailMode.value    = 'edit'
}
function openNew() {
  const today = new Date().toISOString().slice(0, 10)
  editForm.value = { id: 0, stockOutType: 'Sales Out', document: 'With Order', orderNo: '', method: 'Scan', consignee: '', goodsTotal: 0, skuTotal: 0, packUnit: '', createdAt: today, createdBy: '' }
  editLineItems.value = []
  detailMode.value = 'new'
}
function openEditSelected() {
  const id = [...checkedIds.value][0]
  const row = rows.value.find(r => r.id === id)
  if (row) openEdit(row)
}
function closeDetail() { detailMode.value = null }

function saveRecord() {
  if (detailMode.value === 'new') {
    const newId = Math.max(0, ...rows.value.map(r => r.id)) + 1
    editForm.value.id = newId
    rows.value.unshift({ ...editForm.value })
    lineItemsMap[newId] = JSON.parse(JSON.stringify(editLineItems.value))
  } else {
    const idx = rows.value.findIndex(r => r.id === editForm.value.id)
    if (idx !== -1) {
      rows.value[idx] = { ...editForm.value }
      lineItemsMap[editForm.value.id] = JSON.parse(JSON.stringify(editLineItems.value))
    }
  }
  closeDetail()
}

function addLineItem() {
  editLineItems.value.push({ skuCode: '', skuName: '', qty: 1, unit: 'pcs', batchNo: '', expiryDate: '' })
}
function removeLineItem(idx: number) { editLineItems.value.splice(idx, 1) }

// ── Delete ────────────────────────────────────────
const showDelete = ref(false)
const deleteIds  = ref<number[]>([])

function openDelete(id: number) { deleteIds.value = [id]; showDelete.value = true }
function openDeleteSelected()   { deleteIds.value = [...checkedIds.value]; showDelete.value = true }
function confirmDelete() {
  rows.value = rows.value.filter(r => !deleteIds.value.includes(r.id))
  checkedIds.value = new Set([...checkedIds.value].filter(id => !deleteIds.value.includes(id)))
  showDelete.value = false
}

onMounted(() => { tick(); clockTimer = setInterval(tick, 1000) })
onBeforeUnmount(() => { if (clockTimer) clearInterval(clockTimer) })
</script>

<style scoped>
*, *::before, *::after { box-sizing: border-box; margin: 0; padding: 0; }

.page-root { height: 100%; overflow: hidden; font-family: 'Poppins', sans-serif; font-size: 12px; background: #f5f5f5; color: #333; display: flex; flex-direction: column; }

/* ── Topbar ── */
.topbar { background: #fff; border-bottom: 1px solid #c3c6d4; padding: 0 16px; display: flex; align-items: center; flex-shrink: 0; box-shadow: 0 1px 3px rgba(0,0,0,.06); height: 42px; }
.pg-title { font-size: 13px; font-weight: 700; color: #515151; letter-spacing: 0.5px; text-transform: uppercase; }
.topbar-right { margin-left: auto; display: flex; align-items: center; font-size: 10px; color: #9e9e9e; gap: 6px; }
.live-dot { width: 7px; height: 7px; border-radius: 50%; background: #43a047; animation: pulse 1.4s infinite; display: inline-block; }
@keyframes pulse { 0%,100%{ opacity:1 } 50%{ opacity:0.3 } }

/* ── Filter bar ── */
.filter-bar { background: #fff; border-bottom: 1px solid #e8e8e8; padding: 6px 16px; display: flex; align-items: center; gap: 8px; flex-shrink: 0; flex-wrap: wrap; }
.fl { font-size: 10px; color: #9e9e9e; font-weight: 600; text-transform: uppercase; letter-spacing: 0.4px; }
.f-search { border: 1px solid #c3c6d4; border-radius: 3px; font-family: 'Poppins', sans-serif; font-size: 11px; padding: 4px 8px; color: #515151; outline: none; height: 28px; }
.f-search:focus { border-color: #1565c0; }
.f-btn-primary { background: #1565c0; border: none; border-radius: 3px; color: #fff; cursor: pointer; font-family: 'Poppins', sans-serif; font-size: 11px; font-weight: 600; height: 28px; padding: 0 12px; display: flex; align-items: center; gap: 5px; }
.f-btn-primary:hover { background: #1976d2; }
.f-btn-outline { background: #fff; border: 1px solid #c3c6d4; border-radius: 3px; color: #515151; cursor: pointer; font-family: 'Poppins', sans-serif; font-size: 11px; height: 28px; padding: 0 12px; display: flex; align-items: center; gap: 5px; }
.f-btn-outline:hover { background: #f5f5f5; }
.result-count { font-size: 10px; color: #9e9e9e; }

/* ── Action bar ── */
.action-bar { background: #fff; border-bottom: 1px solid #e8e8e8; padding: 6px 16px; display: flex; align-items: center; gap: 6px; flex-shrink: 0; }
.act-btn { background: #f5f5f5; border: 1px solid #c3c6d4; border-radius: 3px; cursor: pointer; font-family: 'Poppins', sans-serif; font-size: 11px; font-weight: 600; height: 28px; padding: 0 12px; display: flex; align-items: center; gap: 5px; transition: background .12s; color: #515151; }
.act-btn:disabled { opacity: 0.4; cursor: not-allowed; }
.act-new    { background: #e8f5e9; border-color: #a5d6a7; color: #388E3C; }
.act-new:hover:not(:disabled)    { background: #c8e6c9; }
.act-edit   { background: #e3f2fd; border-color: #90caf9; color: #1565c0; }
.act-edit:hover:not(:disabled)   { background: #bbdefb; }
.act-delete { background: #ffebee; border-color: #ef9a9a; color: #e53935; }
.act-delete:hover:not(:disabled) { background: #ffcdd2; }
.act-export { background: #fff9c4; border-color: #fdd835; color: #f9a825; }
.act-export:hover { background: #fff59d; }
.act-icon-btn { background: #f5f5f5; border: 1px solid #c3c6d4; border-radius: 3px; color: #757575; cursor: pointer; height: 28px; width: 28px; display: flex; align-items: center; justify-content: center; font-size: 11px; }
.act-icon-btn:hover { background: #e3f2fd; color: #1565c0; border-color: #90caf9; }

/* ── Body / panel ── */
.body-full { display: flex; flex-direction: column; padding: 10px; flex: 1; overflow: hidden; min-height: 0; }
.pc { background: #fff; border: 1px solid #c3c6d4; border-radius: 6px; overflow: hidden; display: flex; flex-direction: column; }
.ph { padding: 8px 12px; font-size: 11px; font-weight: 700; color: #515151; text-transform: uppercase; letter-spacing: 0.4px; flex-shrink: 0; background: linear-gradient(0deg, #d7d7d7 0%, #fff 100%); border-bottom: 1px solid #c3c6d4; display: flex; align-items: center; justify-content: space-between; }
.ph-r { font-size: 10px; font-weight: 400; color: #9e9e9e; text-transform: none; letter-spacing: 0; }

/* ── Table ── */
.tbl-wrap { flex: 1; overflow: auto; }
.tbl-wrap::-webkit-scrollbar { width: 4px; height: 4px; }
.tbl-wrap::-webkit-scrollbar-thumb { background: #c3c6d4; border-radius: 2px; }
table { width: 100%; border-collapse: collapse; font-size: 11px; min-width: 900px; }
th { background: linear-gradient(0deg, #d7d7d7 0%, #fff 100%); color: #515151; font-size: 10px; text-transform: uppercase; padding: 7px 10px; text-align: left; border-bottom: 2px solid #c3c6d4; position: sticky; top: 0; z-index: 1; letter-spacing: 0.3px; white-space: nowrap; font-weight: 700; }
td { padding: 7px 10px; border-bottom: 1px solid #e8e8e8; color: #515151; white-space: nowrap; font-size: 11px; }
tbody tr:nth-child(even) td { background: #f9f9f9; }
tbody tr:hover td { background: #e3f2fd !important; }
tbody tr.row-sel td { background: #e3f2fd; }
.row-clickable { cursor: pointer; }
.col-chk  { width: 36px; text-align: center; }
.col-id   { width: 56px; text-align: center; font-weight: 700; }
.col-actions { width: 80px; text-align: center; }
.dash { color: #c3c6d4; }
.row-edit, .row-del { cursor: pointer; font-size: 10px; font-weight: 600; display: inline-flex; align-items: center; gap: 3px; line-height: 1.8; }
.row-edit { color: #1565c0; } .row-edit:hover { text-decoration: underline; }
.row-del  { color: #e53935; } .row-del:hover  { text-decoration: underline; }

/* Method badges */
.method-badge { font-size: 9px; font-weight: 700; padding: 2px 6px; border-radius: 3px; letter-spacing: 0.3px; }
.badge-scan { background: #e3f2fd; color: #1565c0; }
.badge-rfid { background: #f3e5f5; color: #7b1fa2; }

/* Inspection status dot (in ID cell) */
.insp-dot { display: inline-block; width: 7px; height: 7px; border-radius: 50%; margin-left: 5px; vertical-align: middle; }
.idot--pending     { background: #9e9e9e; }
.idot--in_progress { background: #f9a825; }
.idot--passed      { background: #43a047; }
.idot--failed      { background: #e53935; }

/* ── Pagination ── */
.pag-bar { background: #fff; border-top: 1px solid #c3c6d4; padding: 6px 16px; display: flex; align-items: center; justify-content: space-between; flex-shrink: 0; }
.pag-info { font-size: 10px; color: #9e9e9e; }
.pag-pages { display: flex; gap: 4px; }
.pag-btn { background: #f5f5f5; border: 1px solid #c3c6d4; border-radius: 3px; color: #515151; cursor: pointer; font-family: 'Poppins', sans-serif; font-size: 10px; font-weight: 600; height: 24px; min-width: 24px; padding: 0 6px; }
.pag-btn:hover:not(:disabled) { background: #e3f2fd; border-color: #90caf9; color: #1565c0; }
.pag-active { background: #1565c0; border-color: #1565c0; color: #fff; cursor: default; }
.pag-btn:disabled { opacity: 0.4; cursor: not-allowed; }
.empty-msg { padding: 32px; text-align: center; color: #9e9e9e; font-size: 11px; }

/* ── Overlay ── */
.overlay { position: fixed; inset: 0; background: rgba(0,0,0,.35); z-index: 100; }
.overlay-fade-enter-active, .overlay-fade-leave-active { transition: opacity .2s ease; }
.overlay-fade-enter-from, .overlay-fade-leave-to { opacity: 0; }

/* ── Detail panel ── */
.detail-panel {
  position: fixed; top: 0; right: 0; bottom: 0; width: 560px; max-width: 95vw;
  background: #fff; border-left: 1px solid #c3c6d4; z-index: 101;
  display: flex; flex-direction: column; box-shadow: -4px 0 20px rgba(0,0,0,.12);
}
.panel-slide-enter-active, .panel-slide-leave-active { transition: transform .25s ease; }
.panel-slide-enter-from, .panel-slide-leave-to { transform: translateX(100%); }

.dp-header { padding: 0 16px; height: 52px; border-bottom: 1px solid #c3c6d4; display: flex; align-items: center; justify-content: space-between; flex-shrink: 0; background: linear-gradient(0deg, #d7d7d7 0%, #fff 100%); }
.dp-header-left { display: flex; align-items: center; gap: 10px; }
.dp-badge { font-size: 9px; font-weight: 700; letter-spacing: 0.5px; padding: 3px 8px; border-radius: 3px; }
.badge-edit { background: #e3f2fd; color: #1565c0; }
.badge-new  { background: #e8f5e9; color: #388E3C; }
.dp-title { font-size: 13px; font-weight: 700; color: #515151; }
.dp-close { background: none; border: 1px solid #c3c6d4; border-radius: 3px; color: #757575; cursor: pointer; font-size: 13px; height: 26px; width: 26px; display: flex; align-items: center; justify-content: center; }
.dp-close:hover { background: #ffebee; border-color: #ef9a9a; color: #e53935; }

.dp-body { flex: 1; overflow-y: auto; padding: 16px; }
.dp-body::-webkit-scrollbar { width: 4px; }
.dp-body::-webkit-scrollbar-thumb { background: #c3c6d4; border-radius: 2px; }

.dp-section-title { font-size: 10px; font-weight: 700; color: #9e9e9e; text-transform: uppercase; letter-spacing: 0.5px; margin-bottom: 10px; padding-bottom: 6px; border-bottom: 1px solid #e8e8e8; display: flex; align-items: center; justify-content: space-between; }

.dp-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 10px; }
.dp-field { display: flex; flex-direction: column; gap: 4px; }
.dp-field-full { grid-column: 1 / -1; }
.dp-field label { font-size: 10px; font-weight: 600; color: #515151; }
.req { color: #e53935; }
.dp-field input, .dp-field select {
  border: 1px solid #c3c6d4; border-radius: 3px; font-family: 'Poppins', sans-serif;
  font-size: 11px; color: #515151; padding: 5px 8px; height: 30px; outline: none; background: #fff;
}
.dp-field input:focus, .dp-field select:focus { border-color: #1565c0; box-shadow: 0 0 0 2px rgba(21,101,192,.1); }
.dp-field input:disabled { background: #f5f5f5; color: #9e9e9e; }

/* Line items table */
.li-add-btn { background: #e8f5e9; border: 1px solid #a5d6a7; border-radius: 3px; color: #388E3C; cursor: pointer; font-family: 'Poppins', sans-serif; font-size: 9px; font-weight: 700; height: 22px; padding: 0 8px; display: flex; align-items: center; gap: 4px; }
.li-add-btn:hover { background: #c8e6c9; }
.li-table-wrap { border: 1px solid #e8e8e8; border-radius: 4px; overflow: auto; max-height: 200px; }
.li-table-wrap::-webkit-scrollbar { width: 3px; height: 3px; }
.li-table-wrap::-webkit-scrollbar-thumb { background: #c3c6d4; border-radius: 2px; }
.li-table { width: 100%; border-collapse: collapse; font-size: 10px; min-width: 500px; }
.li-table th { background: #f5f5f5; color: #9e9e9e; font-size: 9px; text-transform: uppercase; padding: 5px 8px; border-bottom: 1px solid #e8e8e8; font-weight: 700; letter-spacing: 0.3px; white-space: nowrap; text-align: left; }
.li-table td { padding: 4px 4px; border-bottom: 1px solid #f0f0f0; }
.li-input { border: 1px solid #e0e0e0; border-radius: 3px; font-family: 'Poppins', sans-serif; font-size: 10px; color: #515151; padding: 3px 6px; height: 24px; outline: none; width: 100%; }
.li-input:focus { border-color: #1565c0; }
.li-num { text-align: right; }
.li-select { border: 1px solid #e0e0e0; border-radius: 3px; font-family: 'Poppins', sans-serif; font-size: 10px; color: #515151; padding: 3px 4px; height: 24px; outline: none; width: 100%; }
.li-del-btn { background: none; border: none; color: #bdbdbd; cursor: pointer; font-size: 11px; padding: 2px 4px; border-radius: 3px; }
.li-del-btn:hover { background: #ffebee; color: #e53935; }

/* ── Panel footer ── */
.dp-footer { padding: 10px 16px; border-top: 1px solid #c3c6d4; display: flex; justify-content: flex-end; gap: 8px; flex-shrink: 0; background: #fafafa; }
.fp-btn { border-radius: 3px; cursor: pointer; font-family: 'Poppins', sans-serif; font-size: 11px; font-weight: 600; height: 30px; padding: 0 14px; display: flex; align-items: center; gap: 5px; border: 1px solid transparent; }
.fp-cancel { background: #f5f5f5; border-color: #c3c6d4; color: #515151; }
.fp-cancel:hover { background: #e8e8e8; }
.fp-save   { background: #1565c0; color: #fff; }
.fp-save:hover { background: #1976d2; }
.fp-delete { background: #e53935; color: #fff; }
.fp-delete:hover { background: #c62828; }

/* ── Delete modal ── */
.del-modal {
  position: fixed; top: 50%; left: 50%; transform: translate(-50%, -50%);
  background: #fff; border: 1px solid #c3c6d4; border-radius: 8px;
  padding: 28px 24px; width: 360px; z-index: 102; text-align: center;
  box-shadow: 0 8px 32px rgba(0,0,0,.18);
}
.modal-pop-enter-active, .modal-pop-leave-active { transition: all .2s ease; }
.modal-pop-enter-from, .modal-pop-leave-to { opacity: 0; transform: translate(-50%, -48%) scale(.95); }
.del-icon { font-size: 32px; color: #f9a825; margin-bottom: 10px; }
.del-title { font-size: 14px; font-weight: 700; color: #515151; margin-bottom: 8px; }
.del-msg { font-size: 11px; color: #757575; line-height: 1.6; margin-bottom: 20px; }
.del-actions { display: flex; justify-content: center; gap: 10px; }

/* ══════════════════════════════════════════════════
   INSPECTION VIEW STYLES
══════════════════════════════════════════════════ */

/* Nav bar */
.insp-nav {
  background: #fff; border-bottom: 1px solid #c3c6d4; padding: 0 16px;
  height: 42px; display: flex; align-items: center; gap: 8px; flex-shrink: 0;
  box-shadow: 0 1px 3px rgba(0,0,0,.06);
}
.insp-back { background: none; border: 1px solid #c3c6d4; border-radius: 3px; color: #1565c0; cursor: pointer; font-family: 'Poppins', sans-serif; font-size: 11px; font-weight: 600; height: 26px; padding: 0 10px; display: flex; align-items: center; gap: 5px; }
.insp-back:hover { background: #e3f2fd; }
.insp-sep { color: #9e9e9e; font-size: 12px; }
.insp-nav-title { font-size: 12px; font-weight: 700; color: #515151; }
.insp-close-btn { background: none; border: 1px solid #c3c6d4; border-radius: 3px; color: #757575; cursor: pointer; font-size: 13px; height: 26px; width: 26px; display: flex; align-items: center; justify-content: center; }
.insp-close-btn:hover { background: #ffebee; border-color: #ef9a9a; color: #e53935; }

/* Status chip */
.isc { font-size: 9px; font-weight: 700; letter-spacing: 0.5px; padding: 2px 8px; border-radius: 10px; text-transform: uppercase; }
.isc--pending     { background: #f5f5f5; color: #757575; border: 1px solid #bdbdbd; }
.isc--in_progress { background: #fff9c4; color: #f57f17; border: 1px solid #f9a825; }
.isc--passed      { background: #e8f5e9; color: #2e7d32; border: 1px solid #a5d6a7; }
.isc--failed      { background: #ffebee; color: #c62828; border: 1px solid #ef9a9a; }

/* Body */
.insp-body {
  flex: 1; overflow-y: auto; padding: 12px 16px; display: flex; flex-direction: column; gap: 12px; min-height: 0;
}
.insp-body::-webkit-scrollbar { width: 4px; }
.insp-body::-webkit-scrollbar-thumb { background: #c3c6d4; border-radius: 2px; }

/* Cards */
.insp-card { background: #fff; border: 1px solid #c3c6d4; border-radius: 6px; overflow: hidden; flex-shrink: 0; }
.insp-card:last-child { flex-shrink: 0; }
.insp-card-title {
  padding: 8px 12px; font-size: 10px; font-weight: 700; color: #515151; text-transform: uppercase;
  letter-spacing: 0.4px; background: linear-gradient(0deg, #d7d7d7 0%, #fff 100%);
  border-bottom: 1px solid #c3c6d4; display: flex; align-items: center; justify-content: space-between;
}
.insp-card-hint { font-size: 10px; font-weight: 400; color: #9e9e9e; text-transform: none; letter-spacing: 0; }

/* Summary grid */
.insp-summary-grid { display: grid; grid-template-columns: repeat(4, 1fr); gap: 0; }
.is-field { padding: 8px 12px; border-right: 1px solid #f0f0f0; border-bottom: 1px solid #f0f0f0; }
.is-field:nth-child(4n) { border-right: none; }
.is-label { display: block; font-size: 9px; font-weight: 700; color: #9e9e9e; text-transform: uppercase; letter-spacing: 0.4px; margin-bottom: 2px; }
.is-val { font-size: 12px; font-weight: 600; color: #515151; }

/* Checklist table */
.lc-wrap { overflow: auto; flex: 1; min-height: 120px; }
.lc-wrap::-webkit-scrollbar { width: 4px; height: 4px; }
.lc-wrap::-webkit-scrollbar-thumb { background: #c3c6d4; border-radius: 2px; }
.lc-table { width: 100%; border-collapse: collapse; font-size: 11px; }
.lc-table th { background: linear-gradient(0deg, #d7d7d7 0%, #fff 100%); color: #515151; font-size: 10px; text-transform: uppercase; padding: 7px 10px; text-align: left; border-bottom: 2px solid #c3c6d4; position: sticky; top: 0; z-index: 1; letter-spacing: 0.3px; white-space: nowrap; font-weight: 700; }
.lc-table td { padding: 5px 8px; border-bottom: 1px solid #f0f0f0; vertical-align: middle; }
.lc-ro { color: #757575; font-size: 11px; white-space: nowrap; }
.lc-input { border: 1px solid #e0e0e0; border-radius: 3px; font-family: 'Poppins', sans-serif; font-size: 11px; color: #515151; padding: 3px 6px; height: 26px; outline: none; width: 100%; min-width: 60px; }
.lc-input:focus { border-color: #1565c0; }
.lc-num { text-align: right; }
.lc-select { border: 1px solid #e0e0e0; border-radius: 3px; font-family: 'Poppins', sans-serif; font-size: 11px; color: #515151; padding: 3px 6px; height: 26px; outline: none; width: 100%; }
.lc-rejected td { background: #fff8f8 !important; }
.lc-warn     td { background: #fffde7 !important; }

/* Sign-off grid */
.insp-signoff-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 10px; padding: 14px 12px; }

/* Footer */
.insp-footer {
  background: #fff; border-top: 1px solid #c3c6d4; padding: 10px 16px;
  display: flex; align-items: center; flex-shrink: 0;
  box-shadow: 0 -1px 4px rgba(0,0,0,.06);
}
.ifp-btn { border-radius: 3px; cursor: pointer; font-family: 'Poppins', sans-serif; font-size: 11px; font-weight: 600; height: 32px; padding: 0 16px; display: flex; align-items: center; gap: 6px; border: 1px solid transparent; }
.ifp-back    { background: #f5f5f5; border-color: #c3c6d4; color: #515151; }
.ifp-back:hover { background: #e8e8e8; }
.ifp-reject  { background: #ffebee; border-color: #ef9a9a; color: #c62828; }
.ifp-reject:hover { background: #ffcdd2; }
.ifp-approve { background: #1565c0; color: #fff; }
.ifp-approve:hover { background: #1976d2; }
</style>
