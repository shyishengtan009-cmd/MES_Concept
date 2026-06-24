<template>
  <TabBarShell title="ESG" :tabs="tabs" v-model="activeTab">
    <EsgEnergyWater   v-if="activeTab === 'energy'"    />
    <EsgWasteEmission v-if="activeTab === 'waste'"     />
    <EsgWorkerSafety  v-if="activeTab === 'safety'"    />
    <EsgReporting     v-if="activeTab === 'reporting'" />
  </TabBarShell>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import TabBarShell      from './TabBarShell.vue'
import EsgEnergyWater   from './EsgEnergyWater.vue'
import EsgWasteEmission from './EsgWasteEmission.vue'
import EsgWorkerSafety  from './EsgWorkerSafety.vue'
import EsgReporting     from './EsgReporting.vue'

const activeTab = ref<'energy' | 'waste' | 'safety' | 'reporting'>('energy')

const tabs = [
  { id: 'energy',    label: 'Energy & Water Monitoring',         icon: 'fa-solid fa-droplet'      },
  { id: 'waste',     label: 'Waste & Emission Tracking',          icon: 'fa-solid fa-recycle'      },
  { id: 'safety',    label: 'Worker Health & Safety Compliance',  icon: 'fa-solid fa-helmet-safety' },
  { id: 'reporting', label: 'ESG Reporting & Analytics',          icon: 'fa-solid fa-chart-pie'    },
] as const
</script>
