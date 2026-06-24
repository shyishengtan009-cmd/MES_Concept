<template>
  <TabBarShell title="Knowledge Retention" :tabs="tabs" v-model="activeTab">
    <KrCostCalculation  v-if="activeTab === 'costcalc'" />
    <KrRecipeManagement v-if="activeTab === 'recipe'"   />
    <KrSopManagement    v-if="activeTab === 'sop'"      />
    <CostAnalysis       v-if="activeTab === 'cost'"     />
  </TabBarShell>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import TabBarShell        from './TabBarShell.vue'
import KrCostCalculation  from './KrCostCalculation.vue'
import KrRecipeManagement from './KrRecipeManagement.vue'
import KrSopManagement    from './KrSopManagement.vue'
import CostAnalysis       from './CostAnalysis.vue'

const activeTab = ref<'costcalc' | 'recipe' | 'sop' | 'cost'>('costcalc')

const tabs = [
  { id: 'costcalc', label: 'Product / Food Cost Calculation', icon: 'fa-solid fa-calculator' },
  { id: 'recipe',   label: 'Recipe Management',                icon: 'fa-solid fa-book-open'  },
  { id: 'sop',      label: 'SOP Management',                    icon: 'fa-solid fa-list-check' },
  { id: 'cost',     label: 'Cost Analysis',                     icon: 'fa-solid fa-chart-line' },
] as const
</script>
