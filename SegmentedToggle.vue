<template>
  <div class="seg-tabs">
    <button
      v-for="tab in tabs" :key="tab.id"
      class="seg-tab"
      :class="{ active: modelValue === tab.id }"
      @click="$emit('update:modelValue', tab.id)"
    >{{ tab.label }}</button>
  </div>
</template>

<script setup lang="ts">
interface SegmentItem { id: string; label: string }

defineProps<{
  tabs: readonly SegmentItem[]
  modelValue: string
}>()
defineEmits<{ (e: 'update:modelValue', value: string): void }>()
</script>

<style scoped>
.seg-tabs { display: flex; gap: 0; margin-left: 16px; border-bottom: 2px solid #c3c6d4; align-self: stretch; }
.seg-tab  { background: #fff; border: none; border-right: 1px solid #e0e0e0; color: #7f7f7f; cursor: pointer; font-family: 'Poppins', sans-serif; font-size: 12px; font-weight: 600; padding: 6px 18px; transition: background .15s, color .15s; letter-spacing: 0.4px; }
.seg-tab:hover  { background: #f2f2f2; color: #333; }
.seg-tab.active { background: #fff; color: #333; border-bottom: 2px solid #1565c0; margin-bottom: -2px; }
</style>
