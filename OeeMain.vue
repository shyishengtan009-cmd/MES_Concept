<template>
  <TabBarShell title="Dashboard" :tabs="tabs" v-model="activeTab">
    <OeeDashboard          v-if="activeTab === 'oee'"  />
    <ShopFloorMonitoring   v-if="activeTab === 'shop'" />
    <PreventiveMaintenance v-if="activeTab === 'pm'"   />
    <MesWesOverview        v-if="activeTab === 'mes'"  />
  </TabBarShell>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import TabBarShell           from './TabBarShell.vue'
import OeeDashboard          from './OeeDashboard.vue'
import ShopFloorMonitoring   from './ShopFloorMonitoring.vue'
import PreventiveMaintenance from './PreventiveMaintenance.vue'
import MesWesOverview        from './MesWesOverview.vue'

const activeTab = ref<'oee' | 'shop' | 'pm' | 'mes'>('oee')

const tabs = [
  { id: 'oee',  label: 'OEE',                   icon: 'fa-solid fa-industry'        },
  { id: 'shop', label: 'Shop Floor Monitoring',  icon: 'fa-solid fa-warehouse'       },
  { id: 'pm',   label: 'Preventive Maintenance', icon: 'fa-solid fa-wrench'          },
  { id: 'mes',  label: 'MES / WES Overview',     icon: 'fa-solid fa-diagram-project' },
] as const
</script>
