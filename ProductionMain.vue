<template>
  <div class="root">
    <div class="tabbar">
      <button
        v-for="tab in tabs"
        :key="tab.id"
        class="tab-btn"
        :class="{ 'tab-btn--active': activeTab === tab.id }"
        @click="activeTab = tab.id"
      >
        <i :class="tab.icon"></i>
        <span>{{ tab.label }}</span>
      </button>
    </div>
    <div class="tab-content">
      <SalesOrder          v-if="activeTab === 'sales'"    />
      <WorkOrder           v-if="activeTab === 'work'"     />
      <BillOfMaterials     v-if="activeTab === 'bom'"      />
      <ProductionPlanning  v-if="activeTab === 'planning'" />
      <ProductionAnalysis  v-if="activeTab === 'analysis'" />
      <ShiftManagement     v-if="activeTab === 'shift'"    />
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import SalesOrder         from './SalesOrder.vue'
import WorkOrder          from './WorkOrder.vue'
import BillOfMaterials    from './BillOfMaterials.vue'
import ProductionPlanning from './ProductionPlanning.vue'
import ProductionAnalysis from './ProductionAnalysis.vue'
import ShiftManagement    from './ShiftManagement.vue'

const activeTab = ref<'sales' | 'work' | 'bom' | 'planning' | 'analysis' | 'shift'>('sales')

const tabs = [
  { id: 'sales',    label: 'Sales Order',        icon: 'fa-solid fa-file-invoice-dollar' },
  { id: 'work',     label: 'Work Order',          icon: 'fa-solid fa-list-check'          },
  { id: 'bom',      label: 'Bill of Materials',   icon: 'fa-solid fa-sitemap'             },
  { id: 'planning', label: 'Production Planning', icon: 'fa-solid fa-calendar-days'       },
  { id: 'analysis', label: 'Production Analysis', icon: 'fa-solid fa-chart-bar'           },
  { id: 'shift',    label: 'Shift Management',    icon: 'fa-solid fa-user-clock'          },
] as const
</script>

<style scoped>
.root { height: 100%; display: flex; flex-direction: column; overflow: hidden; font-family: 'Poppins', sans-serif; }

.tabbar {
  background: #fff;
  border-bottom: 2px solid #c3c6d4;
  display: flex;
  flex-shrink: 0;
  overflow-x: auto;
  padding: 0 8px;
}
.tabbar::-webkit-scrollbar { height: 3px; }
.tabbar::-webkit-scrollbar-thumb { background: #c3c6d4; border-radius: 2px; }

.tab-btn {
  align-items: center;
  background: transparent;
  border: none;
  border-bottom: 2px solid transparent;
  color: #757575;
  cursor: pointer;
  display: flex;
  font-family: 'Poppins', sans-serif;
  font-size: 11px;
  font-weight: 500;
  gap: 6px;
  margin-bottom: -2px;
  padding: 10px 16px;
  transition: color .15s, border-color .15s;
  white-space: nowrap;
}
.tab-btn:hover { color: #1565c0; }
.tab-btn--active { border-bottom-color: #1565c0; color: #1565c0; font-weight: 600; }

.tab-content { flex: 1; overflow: hidden; }
</style>
