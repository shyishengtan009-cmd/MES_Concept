<template>
  <TabBarShell title="Document" :tabs="tabs" v-model="activeTab">
    <DocCompliance         v-if="activeTab === 'compliance'" />
    <DocMonitoringLog      v-if="activeTab === 'monitoring'" />
    <DocPestControl        v-if="activeTab === 'pest'"       />
    <DocMachineMaintenance v-if="activeTab === 'machine'"    />
  </TabBarShell>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import TabBarShell           from './TabBarShell.vue'
import DocCompliance         from './DocCompliance.vue'
import DocMonitoringLog      from './DocMonitoringLog.vue'
import DocPestControl        from './DocPestControl.vue'
import DocMachineMaintenance from './DocMachineMaintenance.vue'

const activeTab = ref<'compliance' | 'monitoring' | 'pest' | 'machine'>('compliance')

const tabs = [
  { id: 'compliance', label: 'Compliance Document',     icon: 'fa-solid fa-file-shield'        },
  { id: 'monitoring',  label: 'Monitoring Log & Report', icon: 'fa-solid fa-clipboard-list'     },
  { id: 'pest',        label: 'Pest Control',            icon: 'fa-solid fa-bug'                },
  { id: 'machine',     label: 'Machine Maintenance',     icon: 'fa-solid fa-screwdriver-wrench' },
] as const
</script>
