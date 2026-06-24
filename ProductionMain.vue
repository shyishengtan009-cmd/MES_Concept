<template>
  <TabBarShell title="Production" :tabs="tabs" v-model="activeTab">
    <SalesOrder          v-if="activeTab === 'sales'"    />
    <WorkOrder           v-if="activeTab === 'work'"     />
    <BillOfMaterials     v-if="activeTab === 'bom'"      />
    <ProductionPlanning  v-if="activeTab === 'planning'" />
    <ProductionAnalysis  v-if="activeTab === 'analysis'" />
    <ShiftManagement     v-if="activeTab === 'shift'"    />
  </TabBarShell>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import TabBarShell        from './TabBarShell.vue'
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
