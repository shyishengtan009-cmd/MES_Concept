<template>
  <TabBarShell title="Inventory" :tabs="tabs" v-model="activeTab">
    <InvFinishedGoods      v-if="activeTab === 'fg'"        />
    <InvSemiFinishedGoods  v-if="activeTab === 'sfg'"       />
    <InvRawMaterials       v-if="activeTab === 'rm'"        />
    <InvTradingProducts    v-if="activeTab === 'trading'"   />
    <InvLabellingGenerator v-if="activeTab === 'label'"     />
    <InvPackagingMaterials v-if="activeTab === 'packaging'" />
    <InvBMR                v-if="activeTab === 'bmr'"       />
    <InvSampleRetention    v-if="activeTab === 'sample'"    />
    <InvTraceability       v-if="activeTab === 'trace'"     />
  </TabBarShell>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import TabBarShell           from './TabBarShell.vue'
import InvFinishedGoods      from './InvFinishedGoods.vue'
import InvSemiFinishedGoods  from './InvSemiFinishedGoods.vue'
import InvRawMaterials       from './InvRawMaterials.vue'
import InvTradingProducts    from './InvTradingProducts.vue'
import InvLabellingGenerator from './InvLabellingGenerator.vue'
import InvPackagingMaterials from './InvPackagingMaterials.vue'
import InvBMR                from './InvBMR.vue'
import InvSampleRetention    from './InvSampleRetention.vue'
import InvTraceability       from './InvTraceability.vue'

const activeTab = ref<'fg' | 'sfg' | 'rm' | 'trading' | 'label' | 'packaging' | 'bmr' | 'sample' | 'trace'>('fg')

const tabs = [
  { id: 'fg',        label: 'Finished Goods / Products',          icon: 'fa-solid fa-box'                },
  { id: 'sfg',       label: 'Semi-Finished Goods',                 icon: 'fa-solid fa-boxes-stacking'     },
  { id: 'rm',        label: 'Raw Materials',                       icon: 'fa-solid fa-wheat-awn'           },
  { id: 'trading',   label: 'Trading Products',                    icon: 'fa-solid fa-handshake'          },
  { id: 'label',     label: 'Labelling Generator',                 icon: 'fa-solid fa-tag'                },
  { id: 'packaging', label: 'Packaging Materials',                 icon: 'fa-solid fa-box-open'           },
  { id: 'bmr',       label: 'Batch Manufacturing Record (BMR)',    icon: 'fa-solid fa-file-medical'       },
  { id: 'sample',    label: 'Sample Retention Management',         icon: 'fa-solid fa-vial'               },
  { id: 'trace',     label: 'Traceability',                        icon: 'fa-solid fa-route'              },
] as const
</script>
