<template>
  <div class="oee-main-root">

    <!-- Tab bar -->
    <div class="tab-bar">
      <span class="tab-bar-title">&#9632; Dashboard</span>
      <button
        v-for="tab in tabs" :key="tab.id"
        class="tab"
        :class="{ active: activeTab === tab.id }"
        @click="activeTab = tab.id"
      >
        <i :class="tab.icon"></i> {{ tab.label }}
      </button>
    </div>

    <!-- Content panels -->
    <div class="tab-content">
      <OeeDashboard        v-if="activeTab === 'oee'"  />
      <ShopFloorMonitoring v-if="activeTab === 'shop'" />
      <PreventiveMaintenance v-if="activeTab === 'pm'" />
      <MesWesOverview      v-if="activeTab === 'mes'"  />
    </div>

  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import OeeDashboard          from './OeeDashboard.vue'
import ShopFloorMonitoring   from './ShopFloorMonitoring.vue'
import PreventiveMaintenance from './PreventiveMaintenance.vue'
import MesWesOverview        from './MesWesOverview.vue'

const activeTab = ref<'oee' | 'shop' | 'pm' | 'mes'>('oee')

const tabs = [
  { id: 'oee',  label: 'OEE',                    icon: 'fa-solid fa-industry'          },
  { id: 'shop', label: 'Shop Floor Monitoring',   icon: 'fa-solid fa-warehouse'         },
  { id: 'pm',   label: 'Preventive Maintenance',  icon: 'fa-solid fa-wrench'            },
  { id: 'mes',  label: 'MES / WES Overview',      icon: 'fa-solid fa-diagram-project'   },
] as const
</script>

<style scoped>
*, *::before, *::after { box-sizing: border-box; margin: 0; padding: 0; }

.oee-main-root {
  height: 100%;
  display: flex;
  flex-direction: column;
  font-family: 'Poppins', sans-serif;
  background: #f5f5f5;
}

.tab-bar {
  display: flex;
  align-items: center;
  gap: 4px;
  background: #ffffff;
  border-bottom: 2px solid #c3c6d4;
  padding: 0 20px;
  height: 46px;
  flex-shrink: 0;
  box-shadow: 0 1px 3px rgba(0,0,0,.06);
}

.tab-bar-title {
  font-size: 13px;
  font-weight: 700;
  color: #515151;
  letter-spacing: 0.5px;
  margin-right: 16px;
  text-transform: uppercase;
}

.tab {
  background: transparent;
  border: none;
  border-bottom: 3px solid transparent;
  color: #7f7f7f;
  cursor: pointer;
  font-family: 'Poppins', sans-serif;
  font-size: 13px;
  font-weight: 600;
  padding: 0 18px;
  height: 46px;
  letter-spacing: 0.3px;
  transition: color 0.15s, border-color 0.15s;
  display: flex;
  align-items: center;
  gap: 7px;
}

.tab:hover { color: #333; background: #f5f5f5; }
.tab.active { color: #1565c0; border-bottom-color: #1565c0; }

.tab-content {
  flex: 1;
  overflow: hidden;
  position: relative;
}
</style>
