<template>
  <div class="tabshell-root">
    <div class="tab-bar-wrap">
      <span class="tab-bar-title" :title="title">&#9632; {{ title }}</span>
      <button class="tab-scroll-btn" @click="scrollTabs(-1)"><i class="fa-solid fa-chevron-left"></i></button>
      <div class="tab-bar" ref="tabBarEl" @wheel="onWheel">
        <button
          v-for="tab in tabs" :key="tab.id"
          class="tab"
          :class="{ active: modelValue === tab.id }"
          :title="tab.label"
          @click="$emit('update:modelValue', tab.id)"
        >
          <i v-if="tab.icon" :class="tab.icon"></i>
          <span class="tab-label">{{ tab.label }}</span>
        </button>
      </div>
      <button class="tab-scroll-btn" @click="scrollTabs(1)"><i class="fa-solid fa-chevron-right"></i></button>
    </div>
    <div class="tab-content">
      <slot />
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'

interface TabItem { id: string; label: string; icon?: string }

defineProps<{
  title: string
  tabs: readonly TabItem[]
  modelValue: string
}>()
defineEmits<{ (e: 'update:modelValue', value: string): void }>()

const tabBarEl = ref<HTMLDivElement | null>(null)

function onWheel(e: WheelEvent) {
  if (!tabBarEl.value) return
  tabBarEl.value.scrollLeft += e.deltaY
  e.preventDefault()
}

function scrollTabs(dir: 1 | -1) {
  tabBarEl.value?.scrollBy({ left: dir * 220, behavior: 'smooth' })
}
</script>

<style scoped>
*, *::before, *::after { box-sizing: border-box; margin: 0; padding: 0; }

.tabshell-root { height: 100%; display: flex; flex-direction: column; font-family: 'Poppins', sans-serif; background: #f5f5f5; }

.tab-bar-wrap {
  display: flex; align-items: center; gap: 4px;
  background: #ffffff; border-bottom: 2px solid #c3c6d4;
  padding: 0 12px 0 20px; height: 46px; flex-shrink: 0;
  box-shadow: 0 1px 3px rgba(0,0,0,.06);
  min-width: 0;
}
.tab-bar {
  display: flex; align-items: center;
  overflow-x: auto; overflow-y: hidden; scrollbar-width: none;
  min-width: 0; flex: 1; height: 100%;
}
.tab-bar::-webkit-scrollbar { display: none; height: 0; }
.tab-bar-title { font-size: 13px; font-weight: 700; color: #515151; letter-spacing: 0.5px; margin-right: 16px; text-transform: uppercase; flex-shrink: 0; width: 200px; white-space: nowrap; display: inline-flex; align-items: center; justify-content: flex-start; }
.tab-scroll-btn {
  background: transparent; border: none; color: #9e9e9e; cursor: pointer;
  width: 26px; height: 100%; flex-shrink: 0; font-size: 12px;
  display: flex; align-items: center; justify-content: center;
}
.tab-scroll-btn:hover { color: #1565c0; background: #f5f5f5; }
.tab {
  background: transparent; border: none; border-bottom: 3px solid transparent;
  color: #7f7f7f; cursor: pointer; font-family: 'Poppins', sans-serif;
  font-size: 13px; font-weight: 600; padding: 0 18px; height: 46px;
  letter-spacing: 0.3px; transition: color 0.15s, border-color 0.15s;
  display: flex; align-items: center; justify-content: center; gap: 7px;
  flex-shrink: 0; min-width: 130px; max-width: 220px; overflow: hidden;
}
.tab-label { white-space: nowrap; overflow: hidden; text-overflow: ellipsis; min-width: 0; }
.tab:hover { color: #333; background: #f5f5f5; }
.tab.active { color: #1565c0; border-bottom-color: #1565c0; background: linear-gradient(180deg, transparent 50%, rgba(21,101,192,.07) 100%); box-shadow: inset 0 -3px 6px -3px rgba(21,101,192,.3); }
.tab-content { flex: 1; overflow: hidden; position: relative; }
</style>
