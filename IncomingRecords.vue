<template>
  <div class="page-root">

    <!-- TOPBAR -->
    <div class="topbar">
      <span class="pg-title">&#9632; Incoming Records</span>
      <div class="topbar-right">
        <span class="live-dot"></span>
        <span>{{ clock }}</span>
      </div>
    </div>

    <!-- ══ INSPECTION VIEW ══ -->
    <template v-if="inspectRow !== null">

      <!-- Inspection nav bar -->
      <div class="insp-nav">
        <button class="insp-back" @click="closeInspect">
          <i class="fa-solid fa-arrow-left"></i> Back to Records
        </button>
        <div class="insp-nav-mid">
          <span class="insp-type-badge">{{ inspectRow.stockInType }}</span>
          <span class="insp-record-id">Stock In #{{ inspectRow.id }}</span>
        </div>
        <span :class="['insp-status-chip', `isc--${inspectionForm.status}`]">
          {{ statusLabel(inspectionForm.status) }}
        </span>
      </div>

      <!-- ── Inspection content ── -->
      <div class="insp-body">

        <!-- Record Summary -->
        <div class="insp-card">
          <div class="insp-card-title">Record Summary</div>
          <div class="insp-summary-grid">
            <div class="isf"><span class="isf-lbl">Stock In Type</span><span class="isf-val">{{ inspectRow.stockInType }}</span></div>
            <div class="isf"><span class="isf-lbl">Document</span><span class="isf-val">{{ inspectRow.document }}</span></div>
            <div class="isf"><span class="isf-lbl">Order No.</span><span class="isf-val">{{ inspectRow.orderNo || '—' }}</span></div>
            <div class="isf"><span class="isf-lbl">Method</span><span class="isf-val">{{ inspectRow.method }}</span></div>
            <div class="isf"><span class="isf-lbl">Supplier</span><span class="isf-val">{{ inspectRow.supplierName || '—' }}</span></div>
            <div class="isf"><span class="isf-lbl">Warehouse</span><span class="isf-val">{{ inspectRow.warehouseName }}</span></div>
            <div class="isf"><span class="isf-lbl">Location</span><span class="isf-val">{{ inspectRow.warehousePositionName || '—' }}</span></div>
            <div class="isf"><span class="isf-lbl">Goods Total</span><span class="isf-val">{{ inspectRow.goodsTotal }}</span></div>
          </div>
        </div>

        <!-- Inspection Checklist -->
        <div class="insp-card">
          <div class="insp-card-title">
            Inspection Checklist
            <span class="insp-card-sub">Verify each line item against the delivery note</span>
            <span v-if="hasTempSensitive" class="temp-notice">
              <i class="fa-solid fa-temperature-half"></i> Temperature-sensitive items detected — core temp required
            </span>
          </div>
          <div class="insp-tbl-wrap">
            <table class="insp-tbl">
              <thead>
                <tr>
                  <th>SKU Code</th>
                  <th style="min-width:130px">SKU Name</th>
                  <th style="text-align:right;min-width:68px">Expected</th>
                  <th style="text-align:right;min-width:80px">Verified Qty <span class="qc-col-dot" title="Less than expected triggers a warning"></span></th>
                  <th style="min-width:110px">Condition <span class="qc-col-dot" title="Rejected = hard fail · Damaged = warning"></span></th>
                  <th style="text-align:center;min-width:72px">Halal Label <span class="qc-col-dot" title="Unchecked = hard fail (compliance breach)"></span></th>
                  <th style="text-align:center;min-width:68px">Expiry OK <span class="qc-col-dot" title="Unchecked = QC failure (expired goods)"></span></th>
                  <th v-if="hasTempSensitive" style="min-width:120px">Core Temp (°C) <span class="qc-col-dot" title="Outside acceptable range = food safety fail"></span></th>
                  <th style="min-width:120px">Notes</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="(lc, idx) in inspectionForm.lineChecks" :key="idx"
                    :class="[lcRowClass(lc), { 'lc-temp-fail': lc.isTempSensitive && isTempFail(lc) }]">
                  <td class="ic-ro">{{ lc.skuCode || '—' }}</td>
                  <td class="ic-ro">
                    {{ lc.skuName || '—' }}
                    <span v-if="lc.isTempSensitive" class="temp-tag">
                      <i class="fa-solid fa-temperature-half"></i> Temp-sensitive
                    </span>
                  </td>
                  <td class="ic-ro" style="text-align:right">{{ lc.expectedQty }}</td>
                  <td><input class="ic-input ic-num" type="number" v-model.number="lc.verifiedQty" min="0" /></td>
                  <td>
                    <select class="ic-select" v-model="lc.condition"
                      :class="{ 'cond-damaged': lc.condition === 'Damaged', 'cond-rejected': lc.condition === 'Rejected' }">
                      <option>Good</option>
                      <option>Damaged</option>
                      <option>Rejected</option>
                    </select>
                  </td>
                  <td style="text-align:center"><input type="checkbox" v-model="lc.halalLabel" class="ic-chk" /></td>
                  <td style="text-align:center"><input type="checkbox" v-model="lc.expiryOk"   class="ic-chk" /></td>
                  <td v-if="hasTempSensitive">
                    <template v-if="lc.isTempSensitive">
                      <div class="temp-input-wrap">
                        <input class="ic-input ic-num" :class="{ 'temp-fail-input': isTempFail(lc) }"
                          type="number" step="0.1" v-model.number="lc.coreTemp" placeholder="°C" />
                        <span class="temp-range">{{ lc.tempMin }}–{{ lc.tempMax }} °C</span>
                        <span v-if="isTempFail(lc)" class="temp-fail-label">
                          <i class="fa-solid fa-triangle-exclamation"></i> Out of range
                        </span>
                      </div>
                    </template>
                    <span v-else class="ic-ro" style="color:#c3c6d4">N/A</span>
                  </td>
                  <td><input class="ic-input" v-model="lc.notes" placeholder="—" /></td>
                </tr>
                <tr v-if="inspectionForm.lineChecks.length === 0">
                  <td :colspan="hasTempSensitive ? 9 : 8" style="text-align:center;color:#9e9e9e;font-size:11px;padding:20px">No line items on this record.</td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>

        <!-- Inspector Sign-off -->
        <div class="insp-card">
          <div class="insp-card-title">Inspector Sign-off</div>
          <div class="insp-signoff-grid">
            <div class="isf isf-col">
              <label class="isf-lbl">Inspector Name <span class="req">*</span> <span class="qc-hint" title="Required — inspection is invalid without a named inspector">Required</span></label>
              <input class="ic-input-lg" v-model="inspectionForm.inspector" placeholder="Enter inspector name" />
            </div>
            <div class="isf isf-col">
              <label class="isf-lbl">Inspection Date</label>
              <input class="ic-input-lg" type="date" v-model="inspectionForm.inspectedAt" />
            </div>
            <div class="isf isf-col isf-full">
              <label class="isf-lbl">Remarks</label>
              <textarea class="ic-textarea" v-model="inspectionForm.remarks" rows="3" placeholder="Enter inspection remarks, discrepancies, or notes..."></textarea>
            </div>
          </div>
        </div>

        <!-- Stock Accepted Notice (shown when status is passed) -->
        <div v-if="inspectionForm.status === 'passed'" class="insp-card insp-card-stock-ok">
          <div class="insp-card-title stock-ok-title">
            <i class="fa-solid fa-circle-check"></i> Stock Accepted — Items Added to Stock Pool
          </div>
          <div class="stock-ok-body">
            <table class="stock-ok-tbl">
              <thead>
                <tr>
                  <th>SKU Code</th><th>SKU Name</th><th style="text-align:right">Qty</th><th>Unit</th><th>Batch No.</th><th>Expiry</th><th>Warehouse</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="(lc, idx) in inspectionForm.lineChecks" :key="idx">
                  <td>{{ lc.skuCode || '—' }}</td>
                  <td>{{ lc.skuName }}</td>
                  <td style="text-align:right">{{ lc.verifiedQty }}</td>
                  <td>{{ (lineItemsMap[inspectRow.id] ?? []).find(l => l.skuCode === lc.skuCode)?.unit ?? 'pcs' }}</td>
                  <td>{{ (lineItemsMap[inspectRow.id] ?? []).find(l => l.skuCode === lc.skuCode)?.batchNo ?? '—' }}</td>
                  <td>{{ (lineItemsMap[inspectRow.id] ?? []).find(l => l.skuCode === lc.skuCode)?.expiryDate || '—' }}</td>
                  <td>{{ inspectRow.warehouseName }}</td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>

        <!-- QC Failure — Correction Notice (shown only when status is failed) -->
        <div v-if="inspectionForm.status === 'failed'" class="insp-card insp-card-fail-notice">
          <div class="insp-card-title fail-title">
            <i class="fa-solid fa-circle-xmark"></i> QC Failed — Action Required
          </div>
          <div class="fail-notice-body">
            <p>This receiving record has <strong>failed QC inspection</strong>. Please review the issues identified above.</p>
            <p>You may correct the original record data and re-submit for inspection.</p>
            <button class="insp-btn insp-correct" @click="openCorrectRecord">
              <i class="fa-solid fa-pen-to-square"></i> Edit & Correct Record
            </button>
          </div>
        </div>

      </div><!-- /insp-body -->

      <!-- Inspection action footer -->
      <div class="insp-footer">
        <button class="insp-btn insp-cancel" @click="closeInspect">
          <i class="fa-solid fa-xmark"></i> Cancel
        </button>
        <div class="insp-footer-right">
          <button class="insp-btn insp-reject" @click="submitInspection('failed')">
            <i class="fa-solid fa-xmark"></i> Reject
          </button>
          <button class="insp-btn insp-approve" @click="submitInspection('passed')">
            <i class="fa-solid fa-check"></i> Approve
          </button>
        </div>
      </div>

    </template><!-- /inspection view -->

    <!-- ══ LIST VIEW ══ -->
    <template v-else>

      <!-- SEARCH / FILTER BAR -->
      <div class="filter-bar">
        <span class="fl">Supplier</span>
        <input class="f-search" type="text" placeholder="Enter supplier name" v-model="supplierSearch" @keyup.enter="applySearch" style="margin-left:0; width:200px" />
        <span class="fl" style="margin-left:8px">QC Status</span>
        <select class="f-select" v-model="qcFilter">
          <option value="">All</option>
          <option value="none">Not Inspected</option>
          <option value="in_progress">In Progress</option>
          <option value="passed">Pass</option>
          <option value="failed">Fail</option>
        </select>
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
            Stock In Records
            <span class="ph-r">{{ filteredRows.length }} records</span>
          </div>
          <div class="tbl-wrap">
            <table>
              <thead>
                <tr>
                  <th class="col-chk"><input type="checkbox" v-model="allChecked" @change="toggleAll" /></th>
                  <th class="col-id">ID</th>
                  <th style="min-width:105px">Stock In Type</th>
                  <th style="min-width:105px">Document</th>
                  <th style="min-width:135px">Order No.</th>
                  <th style="min-width:65px">Method</th>
                  <th style="min-width:145px">SKU</th>
                  <th style="min-width:78px">Pack Unit</th>
                  <th style="min-width:145px">Supplier</th>
                  <th style="min-width:82px">Warehouse</th>
                  <th style="min-width:82px">Location</th>
                  <th style="min-width:82px;text-align:right">Goods Total</th>
                  <th style="min-width:100px">Created At</th>
                  <th style="min-width:88px">Created By</th>
                  <th style="min-width:90px;text-align:center">QC Status</th>
                  <th class="col-actions">Actions</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="row in filteredRows" :key="row.id"
                  :class="{ 'row-sel': checkedIds.has(row.id), 'row-fail': inspectionMap[row.id]?.status === 'failed' }"
                  class="row-clickable"
                  @click="openInspect(row)">
                  <td class="col-chk"><input type="checkbox" :checked="checkedIds.has(row.id)" @change="toggleRow(row.id)" @click.stop /></td>
                  <td class="col-id">{{ row.id }}</td>
                  <td>{{ row.stockInType }}</td>
                  <td>{{ row.document }}</td>
                  <td :class="{ dash: !row.orderNo }">{{ row.orderNo || '—' }}</td>
                  <td><span :class="['method-badge', row.method === 'RFID' ? 'badge-rfid' : 'badge-scan']">{{ row.method }}</span></td>
                  <td :class="{ dash: !row.skuName }">{{ row.skuName || '—' }}</td>
                  <td :class="{ dash: !row.packageUnit }">{{ row.packageUnit || '—' }}</td>
                  <td :class="{ dash: !row.supplierName }">{{ row.supplierName || '—' }}</td>
                  <td>{{ row.warehouseName }}</td>
                  <td>{{ row.warehousePositionName }}</td>
                  <td style="text-align:right">{{ row.goodsTotal }}</td>
                  <td>{{ row.createdAt }}</td>
                  <td :class="{ dash: !row.createdBy }">{{ row.createdBy || '—' }}</td>
                  <td style="text-align:center" @click.stop>
                    <span v-if="!inspectionMap[row.id]" class="qc-badge qc-na">—</span>
                    <span v-else :class="['qc-badge', `qc-${inspectionMap[row.id].status}`]">
                      {{ qcStatusLabel(inspectionMap[row.id].status) }}
                    </span>
                  </td>
                  <td class="col-actions" @click.stop>
                    <div><a class="row-edit" @click.prevent="openEdit(row)"><i class="fa-regular fa-pen-to-square"></i> Edit</a></div>
                    <div v-if="inspectionMap[row.id]?.status === 'failed'">
                      <a class="row-correct" @click.prevent="openEdit(row)">
                        <i class="fa-solid fa-circle-exclamation"></i> Correct
                      </a>
                    </div>
                    <div><a class="row-del" @click.prevent="openDelete(row.id)"><i class="fa-regular fa-trash-can"></i> Delete</a></div>
                  </td>
                </tr>
                <tr v-if="filteredRows.length === 0">
                  <td colspan="16" class="empty-msg">No records found.</td>
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
          <div class="dp-header">
            <div class="dp-header-left">
              <span class="dp-badge" :class="detailMode === 'new' ? 'badge-new' : (isCorrectionMode ? 'badge-correct' : 'badge-edit')">
                {{ detailMode === 'new' ? 'NEW RECORD' : (isCorrectionMode ? 'CORRECTION' : 'EDIT RECORD') }}
              </span>
              <span class="dp-title">{{ detailMode === 'new' ? 'Stock In — New Entry' : `Stock In #${editForm.id}` }}</span>
            </div>
            <button class="dp-close" @click="closeDetail"><i class="fa-solid fa-xmark"></i></button>
          </div>
          <div v-if="isCorrectionMode" class="dp-correction-banner">
            <i class="fa-solid fa-triangle-exclamation"></i>
            This record failed QC inspection. Correct the data below and re-save. A new inspection will be required.
          </div>
          <div class="dp-body">
            <div class="dp-section-title">Record Information</div>
            <div class="dp-grid">
              <div class="dp-field">
                <label>Stock In Type <span class="req">*</span></label>
                <select v-model="editForm.stockInType">
                  <option>Purchase In</option><option>Transfer In</option><option>Return In</option><option>Surplus In</option>
                </select>
              </div>
              <div class="dp-field">
                <label>Document Type <span class="req">*</span></label>
                <select v-model="editForm.document"><option>With Order</option><option>Without Order</option></select>
              </div>
              <div class="dp-field">
                <label>Order No.</label>
                <input type="text" v-model="editForm.orderNo" placeholder="e.g. PO-2026-0001" :disabled="editForm.document === 'Without Order'" />
              </div>
              <div class="dp-field">
                <label>Method <span class="req">*</span></label>
                <select v-model="editForm.method"><option>Scan</option><option>RFID</option></select>
              </div>
              <div class="dp-field dp-field-full">
                <label>Supplier Name</label>
                <input type="text" v-model="editForm.supplierName" placeholder="Enter supplier name" :disabled="editForm.stockInType === 'Transfer In'" />
              </div>
            </div>
            <div class="dp-section-title" style="margin-top:16px">SKU & Packaging</div>
            <div class="dp-grid">
              <div class="dp-field dp-field-full">
                <label>SKU Name <span class="qc-hint" title="Determines temperature sensitivity during QC inspection">QC</span></label>
                <input type="text" v-model="editForm.skuName" placeholder="Enter SKU name" />
              </div>
              <div class="dp-field">
                <label>Pack Unit</label>
                <select v-model="editForm.packageUnit">
                  <option value="">— None —</option><option>Box</option><option>Carton</option><option>Pallet</option><option>Bag</option><option>Drum</option>
                </select>
              </div>
              <div class="dp-field">
                <label>Goods Total <span class="qc-hint" title="Used as expected quantity when no line items are defined">QC</span></label>
                <input type="number" v-model.number="editForm.goodsTotal" min="0" />
              </div>
            </div>
            <div class="dp-section-title" style="margin-top:16px">Warehouse & Location</div>
            <div class="dp-grid">
              <div class="dp-field">
                <label>Warehouse <span class="req">*</span> <span class="qc-hint qc-hint-stock" title="Attributed to stock pool entry when QC passes">Stock</span></label>
                <select v-model="editForm.warehouseName"><option>Zone A</option><option>Zone B</option><option>Zone C</option><option>Zone D</option></select>
              </div>
              <div class="dp-field">
                <label>Position</label>
                <input type="text" v-model="editForm.warehousePositionName" placeholder="e.g. A-01-01" />
              </div>
            </div>
            <div class="dp-section-title" style="margin-top:16px">
              <span style="display:flex;align-items:center;gap:6px">
                Line Items
                <span class="li-qc-notice" title="SKU Name, Qty and Expiry in this table directly affect QC inspection results"><i class="fa-solid fa-flask-vial"></i> QC-linked fields</span>
              </span>
              <button class="li-add-btn" @click="addLineItem"><i class="fa-solid fa-plus"></i> Add SKU</button>
            </div>
            <div class="li-table-wrap li-table-qc-accent">
              <table class="li-table">
                <thead>
                  <tr>
                    <th style="min-width:90px">SKU Code</th>
                    <th style="min-width:140px">SKU Name <span class="qc-col-dot" title="Determines temp-sensitivity check in QC"></span></th>
                    <th style="min-width:55px;text-align:right">Qty <span class="qc-col-dot" title="Used as expected quantity in QC checklist"></span></th>
                    <th style="min-width:55px">Unit</th>
                    <th style="min-width:90px">Batch No. <span class="qc-col-dot qc-col-dot-stock" title="Stored in stock pool when QC passes"></span></th>
                    <th style="min-width:90px">Expiry <span class="qc-col-dot" title="Pre-fills expiry check in QC"></span></th>
                    <th style="width:32px"></th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="(li, idx) in editLineItems" :key="idx">
                    <td><input class="li-input" v-model="li.skuCode" placeholder="SKU-001" /></td>
                    <td><input class="li-input" v-model="li.skuName" placeholder="Product name" /></td>
                    <td><input class="li-input li-num" type="number" v-model.number="li.qty" min="0" /></td>
                    <td><select class="li-select" v-model="li.unit"><option>pcs</option><option>kg</option><option>L</option><option>box</option><option>carton</option></select></td>
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
            <div class="dp-section-title" style="margin-top:16px">Record Meta</div>
            <div class="dp-grid">
              <div class="dp-field"><label>Created At</label><input type="date" v-model="editForm.createdAt" /></div>
              <div class="dp-field"><label>Created By</label><input type="text" v-model="editForm.createdBy" placeholder="Operator name" /></div>
            </div>
          </div>
          <div class="dp-footer">
            <button class="fp-btn fp-cancel" @click="closeDetail"><i class="fa-solid fa-xmark"></i> Cancel</button>
            <button class="fp-btn fp-save" @click="saveRecord">
              <i class="fa-solid fa-floppy-disk"></i>
              {{ detailMode === 'new' ? 'Create' : (isCorrectionMode ? 'Save & Reset QC' : 'Save Changes') }}
            </button>
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
            <strong>{{ deleteIds.length > 1 ? deleteIds.length + ' stock in records' : 'stock in record #' + deleteIds[0] }}</strong>.
            This action cannot be undone.
          </div>
          <div class="del-actions">
            <button class="fp-btn fp-cancel" @click="showDelete = false">Cancel</button>
            <button class="fp-btn fp-delete" @click="confirmDelete"><i class="fa-regular fa-trash-can"></i> Delete</button>
          </div>
        </div>
      </transition>

    </template><!-- /list view -->

  </div>
</template>

<script setup lang="ts">
import { ref, computed, reactive, onMounted, onBeforeUnmount } from 'vue'
import { addToStockPool, removeFromStockPool } from './stock'

const clock          = ref('')
const supplierSearch = ref('')
const activeFilter   = ref('')
const qcFilter       = ref('')
const activeQcFilter = ref('')
const allChecked     = ref(false)
const checkedIds     = ref<Set<number>>(new Set())
let clockTimer: ReturnType<typeof setInterval> | null = null

function tick() {
  clock.value = new Date().toLocaleString('en-MY', {
    day: '2-digit', month: 'short', year: 'numeric',
    hour: '2-digit', minute: '2-digit', hour12: true,
  })
}
function applySearch() {
  activeFilter.value   = supplierSearch.value.trim()
  activeQcFilter.value = qcFilter.value
}
function resetSearch() {
  supplierSearch.value = ''
  activeFilter.value   = ''
  qcFilter.value       = ''
  activeQcFilter.value = ''
}

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
interface StockInRow {
  id: number; stockInType: string; document: string; orderNo: string; method: string
  skuName: string; packageUnit: string; supplierName: string; warehouseName: string
  warehousePositionName: string; goodsTotal: number; createdAt: string; createdBy: string
}

const rows = ref<StockInRow[]>([
  { id: 9, stockInType: 'Purchase In',  document: 'With Order',    orderNo: 'PO-2026-0041',     method: 'Scan', skuName: 'Palm Olein Oil (5L)',     packageUnit: 'Carton', supplierName: 'Mewah International',  warehouseName: 'Zone A', warehousePositionName: 'A-01-02', goodsTotal: 100, createdAt: '2026-04-15', createdBy: 'Saliza'     },
  { id: 8, stockInType: 'Transfer In',  document: 'With Order',    orderNo: '',                 method: 'RFID', skuName: '',                       packageUnit: '',       supplierName: '',                     warehouseName: 'Zone B', warehousePositionName: 'B-02-01', goodsTotal: 0,   createdAt: '2026-04-14', createdBy: ''           },
  { id: 7, stockInType: 'Transfer In',  document: 'With Order',    orderNo: '',                 method: 'RFID', skuName: '',                       packageUnit: '',       supplierName: '',                     warehouseName: 'Zone A', warehousePositionName: 'A-03-01', goodsTotal: 0,   createdAt: '2026-04-14', createdBy: ''           },
  { id: 6, stockInType: 'Purchase In',  document: 'With Order',    orderNo: 'PO-2026-0018',     method: 'Scan', skuName: 'Curry Powder (1kg bag)',  packageUnit: 'Pallet', supplierName: 'Nona Spices Sdn Bhd',  warehouseName: 'Zone A', warehousePositionName: 'A-01-01', goodsTotal: 200, createdAt: '2026-01-09', createdBy: ''           },
  { id: 5, stockInType: 'Return In',    document: 'With Order',    orderNo: 'RTN-IN-2025-0012', method: 'Scan', skuName: 'FFS Film Roll 500mm',     packageUnit: 'Box',    supplierName: 'Kemas Packaging Sdn',  warehouseName: 'Zone C', warehousePositionName: 'C-01-01', goodsTotal: 3,   createdAt: '2025-12-20', createdBy: 'Operator A' },
  { id: 4, stockInType: 'Purchase In',  document: 'With Order',    orderNo: 'PO-2025-0380',     method: 'Scan', skuName: 'Corrugated Box 400×300', packageUnit: 'Carton', supplierName: 'Kemas Packaging Sdn',  warehouseName: 'Zone B', warehousePositionName: 'B-01-03', goodsTotal: 24,  createdAt: '2025-12-15', createdBy: 'Operator B' },
  { id: 3, stockInType: 'Transfer In',  document: 'Without Order', orderNo: '',                 method: 'RFID', skuName: 'Tomato Paste (Drum)',     packageUnit: 'Pallet', supplierName: '',                     warehouseName: 'Zone A', warehousePositionName: 'A-02-02', goodsTotal: 20,  createdAt: '2025-11-30', createdBy: 'Operator A' },
  { id: 2, stockInType: 'Surplus In',   document: 'Without Order', orderNo: '',                 method: 'Scan', skuName: 'White Sugar (50kg bag)',   packageUnit: 'Box',    supplierName: '',                     warehouseName: 'Zone C', warehousePositionName: 'C-02-01', goodsTotal: 5,   createdAt: '2025-11-15', createdBy: 'Admin'      },
  { id: 1, stockInType: 'Purchase In',  document: 'With Order',    orderNo: 'PO-2025-0100',     method: 'Scan', skuName: 'Sodium Benzoate E211',    packageUnit: 'Carton', supplierName: 'Chemtek Supplies',      warehouseName: 'Zone D', warehousePositionName: 'D-01-01', goodsTotal: 80,  createdAt: '2025-10-01', createdBy: 'Admin'      },
])

const lineItemsMap: Record<number, LineItem[]> = {
  9: [{ skuCode: 'SKU-0021', skuName: 'Palm Olein Oil (5L)',     qty: 100, unit: 'pcs',    batchNo: 'BT-9A', expiryDate: '2027-04-15' }],
  6: [{ skuCode: 'SKU-0012', skuName: 'Curry Powder (1kg bag)',  qty: 200, unit: 'carton', batchNo: 'BT-6A', expiryDate: '2026-09-01' }],
  5: [{ skuCode: 'SKU-0055', skuName: 'FFS Film Roll 500mm',     qty: 3,   unit: 'box',    batchNo: 'BT-5A', expiryDate: '2026-06-30' }],
  4: [
    { skuCode: 'SKU-0034', skuName: 'Corrugated Box 400×300', qty: 20, unit: 'carton', batchNo: 'BT-4A', expiryDate: '' },
    { skuCode: 'SKU-0034', skuName: 'Corrugated Box 400×300', qty: 4,  unit: 'carton', batchNo: 'BT-4B', expiryDate: '' },
  ],
  3: [{ skuCode: 'SKU-0078', skuName: 'Tomato Paste (Drum)',     qty: 20, unit: 'pcs',   batchNo: 'BT-3A', expiryDate: '2026-12-01' }],
  1: [{ skuCode: 'SKU-0019', skuName: 'Sodium Benzoate E211',    qty: 80, unit: 'carton', batchNo: 'BT-1A', expiryDate: '2027-06-01' }],
}

const filteredRows = computed(() => {
  const q  = activeFilter.value.toLowerCase()
  const qc = activeQcFilter.value
  return rows.value.filter(r => {
    const supplierMatch = !q || r.supplierName.toLowerCase().includes(q)
    let qcMatch = true
    if (qc === 'none')       qcMatch = !inspectionMap[r.id]
    else if (qc)             qcMatch = inspectionMap[r.id]?.status === qc
    return supplierMatch && qcMatch
  })
})

// ── Detail panel (Edit / New / Correct) ──────────────────────
type DetailMode = 'edit' | 'new' | null
const detailMode      = ref<DetailMode>(null)
const isCorrectionMode = ref(false)
const editForm        = ref<StockInRow>({ id: 0, stockInType: 'Purchase In', document: 'With Order', orderNo: '', method: 'Scan', skuName: '', packageUnit: '', supplierName: '', warehouseName: 'Zone A', warehousePositionName: '', goodsTotal: 0, createdAt: '', createdBy: '' })
const editLineItems   = ref<LineItem[]>([])

function openEdit(row: StockInRow, correction = false) {
  editForm.value       = { ...row }
  editLineItems.value  = JSON.parse(JSON.stringify(lineItemsMap[row.id] ?? []))
  detailMode.value     = 'edit'
  isCorrectionMode.value = correction
}
function openNew() {
  editForm.value = { id: 0, stockInType: 'Purchase In', document: 'With Order', orderNo: '', method: 'Scan', skuName: '', packageUnit: '', supplierName: '', warehouseName: 'Zone A', warehousePositionName: '', goodsTotal: 0, createdAt: new Date().toISOString().slice(0, 10), createdBy: '' }
  editLineItems.value    = []
  detailMode.value       = 'new'
  isCorrectionMode.value = false
}
function openEditSelected() {
  const id  = [...checkedIds.value][0]
  const row = rows.value.find(r => r.id === id)
  if (row) openEdit(row)
}
function closeDetail() { detailMode.value = null; isCorrectionMode.value = false }

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
    if (isCorrectionMode.value) {
      delete inspectionMap[editForm.value.id]
      removeFromStockPool(editForm.value.id)
    }
  }
  closeDetail()
}
function addLineItem()               { editLineItems.value.push({ skuCode: '', skuName: '', qty: 1, unit: 'pcs', batchNo: '', expiryDate: '' }) }
function removeLineItem(idx: number) { editLineItems.value.splice(idx, 1) }

// ── Delete ────────────────────────────────────────
const showDelete = ref(false)
const deleteIds  = ref<number[]>([])
function openDelete(id: number)  { deleteIds.value = [id]; showDelete.value = true }
function openDeleteSelected()    { deleteIds.value = [...checkedIds.value]; showDelete.value = true }
function confirmDelete() {
  rows.value = rows.value.filter(r => !deleteIds.value.includes(r.id))
  checkedIds.value = new Set([...checkedIds.value].filter(id => !deleteIds.value.includes(id)))
  showDelete.value = false
}

// ── Inspection ────────────────────────────────────
interface InspLineCheck {
  skuCode: string
  skuName: string
  expectedQty: number
  verifiedQty: number
  condition: 'Good' | 'Damaged' | 'Rejected'
  halalLabel: boolean
  expiryOk: boolean
  notes: string
  // Temperature check
  isTempSensitive?: boolean
  tempMin?: number
  tempMax?: number
  coreTemp?: number | null
}
interface InspectionData {
  status: 'in_progress' | 'passed' | 'failed'
  inspector: string
  inspectedAt: string
  remarks: string
  lineChecks: InspLineCheck[]
}

const inspectRow     = ref<StockInRow | null>(null)
const inspectionMap  = reactive<Record<number, InspectionData>>({})
const inspectionForm = ref<InspectionData>({ status: 'in_progress', inspector: '', inspectedAt: '', remarks: '', lineChecks: [] })

function getTempProfile(skuName: string): { isTempSensitive: boolean; tempMin?: number; tempMax?: number } {
  const n = skuName.toLowerCase()
  if (n.includes('oil') || n.includes('olein'))            return { isTempSensitive: true, tempMin: 15, tempMax: 30 }
  if (n.includes('paste') || n.includes('tomato'))         return { isTempSensitive: true, tempMin: 2,  tempMax: 25 }
  if (n.includes('cream') || n.includes('dairy') || n.includes('yogurt')) return { isTempSensitive: true, tempMin: 2, tempMax: 8 }
  if (n.includes('powder') || n.includes('curry') || n.includes('spice')) return { isTempSensitive: true, tempMin: 10, tempMax: 35 }
  if (n.includes('benzoate') || n.includes('additive') || n.includes('acid')) return { isTempSensitive: true, tempMin: 15, tempMax: 30 }
  return { isTempSensitive: false }
}

function buildLineChecks(row: StockInRow): InspLineCheck[] {
  const items = lineItemsMap[row.id] ?? []
  if (items.length > 0) {
    return items.map(li => {
      const temp = getTempProfile(li.skuName)
      return {
        skuCode: li.skuCode, skuName: li.skuName,
        expectedQty: li.qty, verifiedQty: li.qty,
        condition: 'Good', halalLabel: true, expiryOk: !!li.expiryDate,
        notes: '',
        ...temp, coreTemp: null,
      }
    })
  }
  const temp = getTempProfile(row.skuName)
  return [{
    skuCode: '', skuName: row.skuName || '—',
    expectedQty: row.goodsTotal, verifiedQty: row.goodsTotal,
    condition: 'Good', halalLabel: true, expiryOk: true,
    notes: '',
    ...temp, coreTemp: null,
  }]
}

const hasTempSensitive = computed(() =>
  inspectionForm.value.lineChecks.some(lc => lc.isTempSensitive)
)

function isTempFail(lc: InspLineCheck): boolean {
  if (!lc.isTempSensitive || lc.coreTemp == null) return false
  return lc.coreTemp < (lc.tempMin ?? -Infinity) || lc.coreTemp > (lc.tempMax ?? Infinity)
}

function openInspect(row: StockInRow) {
  detailMode.value = null
  inspectRow.value = row
  const existing = inspectionMap[row.id]
  inspectionForm.value = existing
    ? { ...existing, lineChecks: JSON.parse(JSON.stringify(existing.lineChecks)) }
    : { status: 'in_progress', inspector: '', inspectedAt: new Date().toISOString().slice(0, 10), remarks: '', lineChecks: buildLineChecks(row) }
}
function closeInspect() { inspectRow.value = null }

function submitInspection(status: InspectionData['status']) {
  inspectionForm.value.status = status
  inspectionMap[inspectRow.value!.id] = JSON.parse(JSON.stringify(inspectionForm.value))
  if (status === 'passed') {
    const row = inspectRow.value!
    const stockItems = inspectionForm.value.lineChecks.map(lc => {
      const li = (lineItemsMap[row.id] ?? []).find(l => l.skuCode === lc.skuCode)
      return {
        skuCode: lc.skuCode || '',
        skuName: lc.skuName,
        qty: lc.verifiedQty,
        unit: li?.unit ?? 'pcs',
        batchNo: li?.batchNo ?? '',
        expiryDate: li?.expiryDate ?? '',
        warehouseName: row.warehouseName,
      }
    })
    addToStockPool(row.id, stockItems)
  }
  closeInspect()
}

function openCorrectRecord() {
  const row = inspectRow.value!
  closeInspect()
  openEdit(row, true)
}

function statusLabel(s: string): string {
  return ({ in_progress: 'In Progress', passed: 'Pass', failed: 'Fail' } as Record<string, string>)[s] ?? s
}

function qcStatusLabel(s: string): string {
  return ({ in_progress: 'In Progress', passed: 'Pass', failed: 'Fail' } as Record<string, string>)[s] ?? s
}

function lcRowClass(lc: InspLineCheck) {
  if (lc.condition === 'Rejected' || !lc.halalLabel) return 'lc-rejected'
  if (lc.condition === 'Damaged' || !lc.expiryOk || lc.verifiedQty < lc.expectedQty) return 'lc-warn'
  return ''
}

onMounted(() => {
  tick()
  clockTimer = setInterval(tick, 1000)
  // Pre-seed sample QC statuses for concept demo
  inspectionMap[1] = { status: 'passed',      inspector: 'Amirul Hassan', inspectedAt: '2025-10-02', remarks: 'All items in good condition.', lineChecks: buildLineChecks(rows.value.find(r => r.id === 1)!) }
  inspectionMap[4] = { status: 'passed',      inspector: 'Saliza Rina',   inspectedAt: '2025-12-16', remarks: 'Qty verified, packaging intact.', lineChecks: buildLineChecks(rows.value.find(r => r.id === 4)!) }
  inspectionMap[5] = { status: 'failed',       inspector: 'Operator A',    inspectedAt: '2025-12-21', remarks: 'Film roll outer wrap damaged, moisture exposure possible.', lineChecks: buildLineChecks(rows.value.find(r => r.id === 5)!) }
  inspectionMap[6] = { status: 'passed',      inspector: 'Saliza Rina',   inspectedAt: '2026-01-10', remarks: 'Core temp within range, labels OK.', lineChecks: buildLineChecks(rows.value.find(r => r.id === 6)!) }
  inspectionMap[9] = { status: 'passed',      inspector: 'Amirul Hassan', inspectedAt: '2026-04-15', remarks: 'Palm olein core temp 22°C — within range. Halal label present.', lineChecks: buildLineChecks(rows.value.find(r => r.id === 9)!) }
  inspectionMap[3] = { status: 'in_progress', inspector: 'Operator A',    inspectedAt: '2025-11-30', remarks: '', lineChecks: buildLineChecks(rows.value.find(r => r.id === 3)!) }
})
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

/* ── Inspection view ── */
.insp-nav { background: #fff; border-bottom: 1px solid #c3c6d4; padding: 0 16px; height: 46px; display: flex; align-items: center; gap: 12px; flex-shrink: 0; }
.insp-back { background: none; border: 1px solid #c3c6d4; border-radius: 4px; color: #515151; cursor: pointer; font-family: 'Poppins', sans-serif; font-size: 11px; font-weight: 600; height: 28px; padding: 0 10px; display: flex; align-items: center; gap: 6px; }
.insp-back:hover { background: #f5f5f5; }
.insp-nav-mid { display: flex; align-items: center; gap: 8px; }
.insp-type-badge { background: #e3f2fd; color: #1565c0; font-size: 9px; font-weight: 700; letter-spacing: 0.5px; padding: 3px 8px; border-radius: 3px; }
.insp-record-id { font-size: 13px; font-weight: 700; color: #515151; }
.insp-status-chip { margin-left: auto; font-size: 10px; font-weight: 700; padding: 4px 10px; border-radius: 12px; letter-spacing: 0.3px; }
.isc--in_progress { background: #fff3e0; color: #e65100; border: 1px solid #ffcc02; }
.isc--passed      { background: #e8f5e9; color: #2e7d32; border: 1px solid #a5d6a7; }
.isc--failed      { background: #ffebee; color: #c62828; border: 1px solid #ef9a9a; }

.insp-body { flex: 1; overflow-y: auto; padding: 12px 16px; display: flex; flex-direction: column; gap: 12px; }
.insp-body::-webkit-scrollbar { width: 4px; }
.insp-body::-webkit-scrollbar-thumb { background: #c3c6d4; border-radius: 2px; }

.insp-card { background: #fff; border: 1px solid #c3c6d4; border-radius: 6px; overflow: hidden; }
.insp-card-title { padding: 8px 14px; font-size: 10px; font-weight: 700; color: #515151; text-transform: uppercase; letter-spacing: 0.4px; background: linear-gradient(0deg, #d7d7d7 0%, #fff 100%); border-bottom: 1px solid #c3c6d4; display: flex; align-items: center; gap: 10px; flex-wrap: wrap; }
.insp-card-sub { font-size: 9px; font-weight: 400; color: #9e9e9e; text-transform: none; letter-spacing: 0; }
.temp-notice { margin-left: auto; font-size: 9px; font-weight: 600; color: #e65100; background: #fff3e0; border: 1px solid #ffb74d; border-radius: 3px; padding: 2px 8px; text-transform: none; letter-spacing: 0; display: flex; align-items: center; gap: 4px; }

.insp-summary-grid { display: grid; grid-template-columns: repeat(4, 1fr); gap: 0; }
.isf { padding: 10px 14px; border-right: 1px solid #f0f0f0; border-bottom: 1px solid #f0f0f0; }
.isf:nth-child(4n) { border-right: none; }
.isf-lbl { display: block; font-size: 9px; font-weight: 600; color: #9e9e9e; text-transform: uppercase; letter-spacing: 0.3px; margin-bottom: 3px; }
.isf-val { font-size: 11px; font-weight: 500; color: #515151; }

.insp-tbl-wrap { overflow-x: auto; }
.insp-tbl-wrap::-webkit-scrollbar { height: 4px; }
.insp-tbl-wrap::-webkit-scrollbar-thumb { background: #c3c6d4; border-radius: 2px; }
.insp-tbl { width: 100%; border-collapse: collapse; font-size: 11px; }
.insp-tbl th { background: #f5f5f5; color: #9e9e9e; font-size: 9px; text-transform: uppercase; padding: 7px 10px; border-bottom: 1px solid #e8e8e8; font-weight: 700; letter-spacing: 0.3px; white-space: nowrap; text-align: left; }
.insp-tbl td { padding: 5px 6px; border-bottom: 1px solid #f0f0f0; }
.insp-tbl tbody tr:hover td { background: #fafafa; }
.ic-ro { color: #515151; font-size: 11px; padding: 5px 10px !important; }
.ic-input { border: 1px solid #e0e0e0; border-radius: 3px; font-family: 'Poppins', sans-serif; font-size: 11px; color: #515151; padding: 4px 7px; height: 26px; outline: none; width: 100%; }
.ic-input:focus { border-color: #1565c0; }
.ic-num { text-align: right; width: 72px; }
.ic-select { border: 1px solid #e0e0e0; border-radius: 3px; font-family: 'Poppins', sans-serif; font-size: 11px; color: #515151; padding: 4px 5px; height: 26px; outline: none; width: 100%; }
.ic-chk { width: 14px; height: 14px; cursor: pointer; accent-color: #1565c0; }
.cond-damaged  { border-color: #ffb74d; color: #e65100; background: #fff8e1; }
.cond-rejected { border-color: #ef9a9a; color: #c62828; background: #ffebee; }
.lc-warn     td { background: #fff8e1 !important; }
.lc-rejected td { background: #ffebee !important; }
.lc-temp-fail td { background: #fff3e0 !important; }

/* Temperature-sensitive check */
.temp-tag { display: block; font-size: 8px; color: #e65100; font-weight: 600; letter-spacing: 0.2px; margin-top: 2px; }
.temp-input-wrap { display: flex; flex-direction: column; gap: 2px; }
.temp-range { font-size: 9px; color: #9e9e9e; white-space: nowrap; }
.temp-fail-input { border-color: #ef9a9a !important; background: #ffebee !important; color: #c62828 !important; }
.temp-fail-label { font-size: 9px; color: #c62828; font-weight: 600; display: flex; align-items: center; gap: 3px; }

.insp-signoff-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 12px; padding: 14px; }
.isf-col { display: flex; flex-direction: column; gap: 4px; }
.isf-full { grid-column: 1 / -1; }
.isf-col .isf-lbl { font-size: 10px; font-weight: 600; color: #515151; }
.ic-input-lg { border: 1px solid #c3c6d4; border-radius: 3px; font-family: 'Poppins', sans-serif; font-size: 11px; color: #515151; padding: 5px 8px; height: 30px; outline: none; }
.ic-input-lg:focus { border-color: #1565c0; box-shadow: 0 0 0 2px rgba(21,101,192,.1); }
.ic-textarea { border: 1px solid #c3c6d4; border-radius: 3px; font-family: 'Poppins', sans-serif; font-size: 11px; color: #515151; padding: 6px 8px; outline: none; resize: vertical; width: 100%; }
.ic-textarea:focus { border-color: #1565c0; box-shadow: 0 0 0 2px rgba(21,101,192,.1); }
.req { color: #e53935; }

/* QC Fail correction notice */
.insp-card-fail-notice { border-color: #ef9a9a; }
.fail-title { background: linear-gradient(0deg, #ffcdd2 0%, #ffebee 100%) !important; color: #c62828 !important; border-color: #ef9a9a !important; }
.fail-notice-body { padding: 14px 16px; display: flex; flex-direction: column; gap: 8px; }
.fail-notice-body p { font-size: 11px; color: #515151; line-height: 1.6; }

.insp-footer { background: #fff; border-top: 1px solid #c3c6d4; padding: 10px 16px; display: flex; align-items: center; justify-content: space-between; flex-shrink: 0; }
.insp-footer-right { display: flex; gap: 8px; }
.insp-btn { border-radius: 4px; cursor: pointer; font-family: 'Poppins', sans-serif; font-size: 11px; font-weight: 600; height: 32px; padding: 0 16px; display: flex; align-items: center; gap: 6px; border: 1px solid transparent; }
.insp-cancel { background: #f5f5f5; border-color: #c3c6d4; color: #515151; }
.insp-cancel:hover { background: #e8e8e8; }
.insp-reject  { background: #fff; border-color: #ef9a9a; color: #c62828; }
.insp-reject:hover  { background: #ffebee; }
.insp-approve { background: #1565c0; color: #fff; }
.insp-approve:hover { background: #1976d2; }
.insp-correct { background: #fff3e0; border-color: #ffb74d; color: #e65100; }
.insp-correct:hover { background: #ffe0b2; }

/* ── Filter / search bar ── */
.filter-bar { background: #fff; border-bottom: 1px solid #e8e8e8; padding: 6px 16px; display: flex; align-items: center; gap: 8px; flex-shrink: 0; flex-wrap: wrap; }
.fl { font-size: 10px; color: #9e9e9e; font-weight: 600; text-transform: uppercase; letter-spacing: 0.4px; }
.f-search { border: 1px solid #c3c6d4; border-radius: 3px; font-family: 'Poppins', sans-serif; font-size: 11px; padding: 4px 8px; color: #515151; outline: none; height: 28px; }
.f-search:focus { border-color: #1565c0; }
.f-select { border: 1px solid #c3c6d4; border-radius: 3px; font-family: 'Poppins', sans-serif; font-size: 11px; padding: 4px 8px; color: #515151; outline: none; height: 28px; background: #fff; }
.f-select:focus { border-color: #1565c0; }
.f-btn-primary { background: #1565c0; border: none; border-radius: 3px; color: #fff; cursor: pointer; font-family: 'Poppins', sans-serif; font-size: 11px; font-weight: 600; height: 28px; padding: 0 12px; display: flex; align-items: center; gap: 5px; }
.f-btn-primary:hover { background: #1976d2; }
.f-btn-outline { background: #fff; border: 1px solid #c3c6d4; border-radius: 3px; color: #515151; cursor: pointer; font-family: 'Poppins', sans-serif; font-size: 11px; height: 28px; padding: 0 12px; display: flex; align-items: center; gap: 5px; }
.f-btn-outline:hover { background: #f5f5f5; }
.result-count { font-size: 10px; color: #9e9e9e; }

/* ── Action bar ── */
.action-bar { background: #fff; border-bottom: 1px solid #e8e8e8; padding: 6px 16px; display: flex; align-items: center; gap: 6px; flex-shrink: 0; }
.act-btn { background: #f5f5f5; border: 1px solid #c3c6d4; border-radius: 3px; cursor: pointer; font-family: 'Poppins', sans-serif; font-size: 11px; font-weight: 600; height: 28px; padding: 0 12px; display: flex; align-items: center; gap: 5px; transition: background .12s; color: #515151; }
.act-btn:disabled { opacity: 0.4; cursor: not-allowed; }
.act-new    { background: #e8f5e9; border-color: #a5d6a7; color: #388E3C; } .act-new:hover:not(:disabled)    { background: #c8e6c9; }
.act-edit   { background: #e3f2fd; border-color: #90caf9; color: #1565c0; } .act-edit:hover:not(:disabled)   { background: #bbdefb; }
.act-delete { background: #ffebee; border-color: #ef9a9a; color: #e53935; } .act-delete:hover:not(:disabled) { background: #ffcdd2; }
.act-export { background: #fff9c4; border-color: #fdd835; color: #f9a825; } .act-export:hover { background: #fff59d; }
.act-icon-btn { background: #f5f5f5; border: 1px solid #c3c6d4; border-radius: 3px; color: #757575; cursor: pointer; height: 28px; width: 28px; display: flex; align-items: center; justify-content: center; font-size: 11px; }
.act-icon-btn:hover { background: #e3f2fd; color: #1565c0; border-color: #90caf9; }

/* ── Body / table ── */
.body-full { display: flex; flex-direction: column; padding: 10px; flex: 1; overflow: hidden; min-height: 0; }
.pc { background: #fff; border: 1px solid #c3c6d4; border-radius: 6px; overflow: hidden; display: flex; flex-direction: column; }
.ph { padding: 8px 12px; font-size: 11px; font-weight: 700; color: #515151; text-transform: uppercase; letter-spacing: 0.4px; flex-shrink: 0; background: linear-gradient(0deg, #d7d7d7 0%, #fff 100%); border-bottom: 1px solid #c3c6d4; display: flex; align-items: center; justify-content: space-between; }
.ph-r { font-size: 10px; font-weight: 400; color: #9e9e9e; text-transform: none; letter-spacing: 0; }
.tbl-wrap { flex: 1; overflow: auto; }
.tbl-wrap::-webkit-scrollbar { width: 4px; height: 4px; }
.tbl-wrap::-webkit-scrollbar-thumb { background: #c3c6d4; border-radius: 2px; }
table { width: 100%; border-collapse: collapse; font-size: 11px; min-width: 1200px; }
th { background: linear-gradient(0deg, #d7d7d7 0%, #fff 100%); color: #515151; font-size: 10px; text-transform: uppercase; padding: 7px 10px; text-align: left; border-bottom: 2px solid #c3c6d4; position: sticky; top: 0; z-index: 1; letter-spacing: 0.3px; white-space: nowrap; font-weight: 700; }
td { padding: 7px 10px; border-bottom: 1px solid #e8e8e8; color: #515151; white-space: nowrap; font-size: 11px; }
tbody tr:nth-child(even) td { background: #f9f9f9; }
tbody tr.row-clickable { cursor: pointer; }
tbody tr.row-clickable:hover td { background: #e3f2fd !important; }
tbody tr.row-sel td { background: #e3f2fd; }
tbody tr.row-fail td { background: #fff5f5 !important; }
tbody tr.row-fail:hover td { background: #ffe0e0 !important; }
.col-chk { width: 36px; text-align: center; } .col-id { width: 56px; text-align: center; font-weight: 700; } .col-actions { width: 88px; text-align: center; }
.dash { color: #c3c6d4; }
.row-edit, .row-del, .row-correct { cursor: pointer; font-size: 10px; font-weight: 600; display: inline-flex; align-items: center; gap: 3px; line-height: 1.8; }
.row-edit    { color: #1565c0; } .row-edit:hover    { text-decoration: underline; }
.row-del     { color: #e53935; } .row-del:hover     { text-decoration: underline; }
.row-correct { color: #e65100; } .row-correct:hover { text-decoration: underline; }
.method-badge { font-size: 9px; font-weight: 700; padding: 2px 6px; border-radius: 3px; letter-spacing: 0.3px; }
.badge-scan { background: #e3f2fd; color: #1565c0; } .badge-rfid { background: #f3e5f5; color: #7b1fa2; }

/* QC Status badges */
.qc-badge { font-size: 9px; font-weight: 700; padding: 2px 8px; border-radius: 10px; letter-spacing: 0.3px; display: inline-block; }
.qc-na          { color: #bdbdbd; }
.qc-in_progress { background: #fff3e0; color: #e65100; border: 1px solid #ffcc02; }
.qc-passed      { background: #e8f5e9; color: #2e7d32; border: 1px solid #a5d6a7; }
.qc-failed      { background: #ffebee; color: #c62828; border: 1px solid #ef9a9a; }

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
.detail-panel { position: fixed; top: 0; right: 0; bottom: 0; width: 560px; max-width: 95vw; background: #fff; border-left: 1px solid #c3c6d4; z-index: 101; display: flex; flex-direction: column; box-shadow: -4px 0 20px rgba(0,0,0,.12); }
.panel-slide-enter-active, .panel-slide-leave-active { transition: transform .25s ease; }
.panel-slide-enter-from, .panel-slide-leave-to { transform: translateX(100%); }
.dp-header { padding: 0 16px; height: 52px; border-bottom: 1px solid #c3c6d4; display: flex; align-items: center; justify-content: space-between; flex-shrink: 0; background: linear-gradient(0deg, #d7d7d7 0%, #fff 100%); }
.dp-header-left { display: flex; align-items: center; gap: 10px; }
.dp-badge { font-size: 9px; font-weight: 700; letter-spacing: 0.5px; padding: 3px 8px; border-radius: 3px; }
.badge-edit    { background: #e3f2fd; color: #1565c0; }
.badge-new     { background: #e8f5e9; color: #388E3C; }
.badge-correct { background: #fff3e0; color: #e65100; }
.dp-title { font-size: 13px; font-weight: 700; color: #515151; }
.dp-close { background: none; border: 1px solid #c3c6d4; border-radius: 3px; color: #757575; cursor: pointer; font-size: 13px; height: 26px; width: 26px; display: flex; align-items: center; justify-content: center; }
.dp-close:hover { background: #ffebee; border-color: #ef9a9a; color: #e53935; }
.dp-correction-banner { background: #fff3e0; border-bottom: 1px solid #ffb74d; padding: 8px 16px; font-size: 10px; color: #e65100; font-weight: 600; display: flex; align-items: flex-start; gap: 6px; flex-shrink: 0; line-height: 1.5; }
.dp-body { flex: 1; overflow-y: auto; padding: 16px; }
.dp-body::-webkit-scrollbar { width: 4px; }
.dp-body::-webkit-scrollbar-thumb { background: #c3c6d4; border-radius: 2px; }
.dp-section-title { font-size: 10px; font-weight: 700; color: #9e9e9e; text-transform: uppercase; letter-spacing: 0.5px; margin-bottom: 10px; padding-bottom: 6px; border-bottom: 1px solid #e8e8e8; display: flex; align-items: center; justify-content: space-between; }
.dp-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 10px; }
.dp-field { display: flex; flex-direction: column; gap: 4px; } .dp-field-full { grid-column: 1 / -1; }
.dp-field label { font-size: 10px; font-weight: 600; color: #515151; }
.dp-field input, .dp-field select { border: 1px solid #c3c6d4; border-radius: 3px; font-family: 'Poppins', sans-serif; font-size: 11px; color: #515151; padding: 5px 8px; height: 30px; outline: none; background: #fff; }
.dp-field input:focus, .dp-field select:focus { border-color: #1565c0; box-shadow: 0 0 0 2px rgba(21,101,192,.1); }
.dp-field input:disabled { background: #f5f5f5; color: #9e9e9e; }
.li-add-btn { background: #e8f5e9; border: 1px solid #a5d6a7; border-radius: 3px; color: #388E3C; cursor: pointer; font-family: 'Poppins', sans-serif; font-size: 9px; font-weight: 700; height: 22px; padding: 0 8px; display: flex; align-items: center; gap: 4px; }
.li-add-btn:hover { background: #c8e6c9; }
.li-table-wrap { border: 1px solid #e8e8e8; border-radius: 4px; overflow: auto; max-height: 200px; }
.li-table { width: 100%; border-collapse: collapse; font-size: 10px; min-width: 500px; }
.li-table th { background: #f5f5f5; color: #9e9e9e; font-size: 9px; text-transform: uppercase; padding: 5px 8px; border-bottom: 1px solid #e8e8e8; font-weight: 700; letter-spacing: 0.3px; white-space: nowrap; text-align: left; }
.li-table td { padding: 4px 4px; border-bottom: 1px solid #f0f0f0; }
.li-input { border: 1px solid #e0e0e0; border-radius: 3px; font-family: 'Poppins', sans-serif; font-size: 10px; color: #515151; padding: 3px 6px; height: 24px; outline: none; width: 100%; }
.li-input:focus { border-color: #1565c0; }
.li-num { text-align: right; }
.li-select { border: 1px solid #e0e0e0; border-radius: 3px; font-family: 'Poppins', sans-serif; font-size: 10px; color: #515151; padding: 3px 4px; height: 24px; outline: none; width: 100%; }
.li-del-btn { background: none; border: none; color: #bdbdbd; cursor: pointer; font-size: 11px; padding: 2px 4px; border-radius: 3px; }
.li-del-btn:hover { background: #ffebee; color: #e53935; }
.dp-footer { padding: 10px 16px; border-top: 1px solid #c3c6d4; display: flex; justify-content: flex-end; gap: 8px; flex-shrink: 0; background: #fafafa; }
.fp-btn { border-radius: 3px; cursor: pointer; font-family: 'Poppins', sans-serif; font-size: 11px; font-weight: 600; height: 30px; padding: 0 14px; display: flex; align-items: center; gap: 5px; border: 1px solid transparent; }
.fp-cancel { background: #f5f5f5; border-color: #c3c6d4; color: #515151; } .fp-cancel:hover { background: #e8e8e8; }
.fp-save   { background: #1565c0; color: #fff; }                            .fp-save:hover   { background: #1976d2; }
.fp-delete { background: #e53935; color: #fff; }                            .fp-delete:hover { background: #c62828; }

/* ── Delete modal ── */
.del-modal { position: fixed; top: 50%; left: 50%; transform: translate(-50%, -50%); background: #fff; border: 1px solid #c3c6d4; border-radius: 8px; padding: 28px 24px; width: 360px; z-index: 102; text-align: center; box-shadow: 0 8px 32px rgba(0,0,0,.18); }
.modal-pop-enter-active, .modal-pop-leave-active { transition: all .2s ease; }
.modal-pop-enter-from, .modal-pop-leave-to { opacity: 0; transform: translate(-50%, -48%) scale(.95); }
.del-icon { font-size: 32px; color: #f9a825; margin-bottom: 10px; }
.del-title { font-size: 14px; font-weight: 700; color: #515151; margin-bottom: 8px; }
.del-msg { font-size: 11px; color: #757575; line-height: 1.6; margin-bottom: 20px; }
.del-actions { display: flex; justify-content: center; gap: 10px; }

/* QC-linked field indicators */
.qc-hint { font-size: 8px; font-weight: 700; background: #fff3e0; color: #e65100; border: 1px solid #ffb74d; border-radius: 3px; padding: 1px 5px; letter-spacing: 0.3px; cursor: help; margin-left: 4px; text-transform: uppercase; vertical-align: middle; }
.qc-hint-stock { background: #e8f5e9; color: #2e7d32; border-color: #a5d6a7; }
.li-qc-notice { font-size: 8px; font-weight: 700; background: #fff3e0; color: #e65100; border: 1px solid #ffb74d; border-radius: 3px; padding: 2px 6px; letter-spacing: 0.3px; cursor: help; display: inline-flex; align-items: center; gap: 3px; text-transform: uppercase; }
.qc-col-dot { display: inline-block; width: 5px; height: 5px; border-radius: 50%; background: #fb8c00; margin-left: 3px; vertical-align: middle; cursor: help; flex-shrink: 0; }
.qc-col-dot-stock { background: #43a047; }
.li-table-qc-accent { border-left: 3px solid #ffb74d !important; }

/* Stock Accepted card */
.insp-card-stock-ok { border-color: #a5d6a7; }
.stock-ok-title { background: linear-gradient(0deg, #c8e6c9 0%, #e8f5e9 100%) !important; color: #2e7d32 !important; border-color: #a5d6a7 !important; }
.stock-ok-body { padding: 10px 14px; }
.stock-ok-tbl { width: 100%; border-collapse: collapse; font-size: 10px; }
.stock-ok-tbl th { background: #f1f8e9; color: #558b2f; font-size: 9px; text-transform: uppercase; padding: 5px 8px; border-bottom: 1px solid #c8e6c9; font-weight: 700; text-align: left; white-space: nowrap; }
.stock-ok-tbl td { padding: 4px 8px; border-bottom: 1px solid #f0f0f0; color: #515151; white-space: nowrap; font-size: 10px; }
</style>
