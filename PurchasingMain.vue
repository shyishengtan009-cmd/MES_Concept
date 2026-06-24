<template>
  <TabBarShell title="Purchasing" :tabs="tabs" v-model="activeTab">
    <PurSupplierManagement  v-if="activeTab === 'supplier'"   />
    <PurRawMaterialPurchase v-if="activeTab === 'rmpurchase'" />
  </TabBarShell>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import TabBarShell            from './TabBarShell.vue'
import PurSupplierManagement  from './PurSupplierManagement.vue'
import PurRawMaterialPurchase from './PurRawMaterialPurchase.vue'

const activeTab = ref<'supplier' | 'rmpurchase'>('supplier')

const tabs = [
  { id: 'supplier',   label: 'Supplier Management',  icon: 'fa-solid fa-truck-field'   },
  { id: 'rmpurchase', label: 'Raw Material Purchase', icon: 'fa-solid fa-cart-shopping' },
] as const
</script>
