<template>
  <div id="hias-root">

    <!-- TOP NAV -->
    <header class="hias-topnav">
      <div class="topnav-inner">

        <div class="topnav-left">
          <button class="btn-flat-round" @click="showSidebar = !showSidebar" title="Toggle menu">
            <i class="fa-solid fa-bars"></i>
          </button>
          <div class="hias-logo">
            <img src="/hias-logo.png" alt="HIAS Logo" />
          </div>
        </div>

        <div class="menu-section">
          <div class="global-search">
            <input type="text" placeholder="Search HIAS" v-model="searchQuery" />
            <i class="fa-solid fa-magnifying-glass search-append-icon"></i>
          </div>

          <button class="org-btn">
            <span v-html="corporateFareSVG" class="org-svg"></span>
            <span>Toppleware</span>
            <i class="fa-solid fa-chevron-down org-chevron"></i>
          </button>

          <button class="bell-btn">
            <i class="fa-solid fa-bell bell-icon-unread"></i>
            <span class="bell-badge">13426</span>
          </button>

          <button class="user-btn">
            <div class="user-avatar-wrap">
              <span v-html="userAvatarSVG" class="avatar-svg"></span>
            </div>
            <div class="user-text">
              <div class="user-name">Saliza Rina</div>
              <div class="user-role">Halal Executive</div>
            </div>
            <i class="fa-solid fa-angle-down user-chevron"></i>
          </button>
        </div>

      </div>
    </header>

    <!-- BODY -->
    <div class="hias-body" :class="{ 'sidebar-hidden': !showSidebar }">

      <!-- SIDEBAR -->
      <aside class="hias-drawer" v-show="showSidebar">
        <div v-for="menu in menus" :key="menu.name" class="nav-exp-item">
          <div
            class="nav-item-row"
            :class="{ activeNavItem: isActiveParent(menu.name) && (isOpen(menu.name) || !menu.children.length) }"
            @click="toggleMenu(menu.name, menu.children.length > 0)"
          >
            <div
              class="nav-icon-section"
              :class="{
                orangeBorder: isActiveParent(menu.name) && (isOpen(menu.name) || !menu.children.length),
                activeIcon:   isActiveParent(menu.name) && (isOpen(menu.name) || !menu.children.length)
              }"
            >
              <i :class="menu.icon" class="nav-icon-el"></i>
            </div>
            <div class="nav-label-section">
              <div class="nav-label-text">{{ menu.name }}</div>
            </div>
            <div
              v-if="menu.children.length"
              class="nav-expand-icon"
              :class="{ 'nav-expand-icon--open': isOpen(menu.name) }"
            >
              <i class="fa-solid fa-angle-down"></i>
            </div>
          </div>

          <div class="nav-children-wrap" :class="{ 'nav-children--open': isOpen(menu.name) }">
            <div
              v-for="child in menu.children"
              :key="child.name"
              class="nav-child-row"
              :class="{
                activeNavItem: isActiveChild(child.name),
                orangeBorder:  isActiveChild(child.name),
                activeIcon:    isActiveChild(child.name)
              }"
              @click="selectChild(child.name, menu.name)"
            >
              <div
                class="nav-icon-section"
                :class="{ orangeBorder: isActiveChild(child.name), activeIcon: isActiveChild(child.name) }"
              ></div>
              <div class="nav-label-section">
                <div class="nav-label-text childInset">{{ child.name }}</div>
              </div>
            </div>
          </div>
        </div>
      </aside>

      <!-- PAGE CONTENT -->
      <div class="hias-page-container">
        <OeeMain          v-if="currentPage === 'oee-main'"           />
        <Inventory        v-if="currentPage === 'inventory'"          />
        <CostAnalysis     v-if="currentPage === 'cost-analysis'"      />
        <IncomingRecords  v-if="currentPage === 'inventory-incoming'" />
        <OutgoingRecords  v-if="currentPage === 'inventory-outgoing'" />
        <ProductionMain   v-if="currentPage === 'production'"         />
      </div>

    </div><!-- /.hias-body -->

    <!-- Chat FAB -->
    <button class="chat-fab" title="Support">
      <i class="fa-solid fa-comment-dots"></i>
    </button>

  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import OeeMain          from './OeeMain.vue'
import Inventory        from './Inventory.vue'
import CostAnalysis     from './CostAnalysis.vue'
import IncomingRecords  from './IncomingRecords.vue'
import OutgoingRecords  from './OutgoingRecords.vue'
import ProductionMain   from './ProductionMain.vue'

const corporateFareSVG = `<svg xmlns="http://www.w3.org/2000/svg" enable-background="new 0 0 24 24" height="23px" viewBox="0 0 24 24" width="23px"><rect fill="none" height="24" width="24"/><path fill="currentColor" d="M12,7V3H2v18h20V7H12z M10,19H4v-2h6V19z M10,15H4v-2h6V15z M10,11H4V9h6V11z M10,7H4V5h6V7z M20,19h-8V9h8V19z M18,11h-4v2h4V11z M18,15h-4v2h4V15z"/></svg>`
const userAvatarSVG    = `<svg width="24px" height="24px" viewBox="0 0 16 16" xmlns="http://www.w3.org/2000/svg" fill="currentColor"><path d="M3 14s-1 0-1-1 1-4 6-4 6 3 6 4-1 1-1 1H3zm5-6a3 3 0 1 0 0-6 3 3 0 0 0 0 6z"/></svg>`

const showSidebar  = ref(true)
const searchQuery  = ref('')
const openMenus    = ref<string[]>(['MES'])
const activeParent = ref('MES')
const activeChild  = ref('Dashboard')
const currentPage  = ref('oee-main')

const pageMap: Record<string, string> = {
  'Dashboard':         'oee-main',
  'Cost Analysis':     'cost-analysis',
  'Inventory':         'inventory',
  'Incoming Records':  'inventory-incoming',
  'Outgoing Records':  'inventory-outgoing',
  'Production':        'production',
}

type MenuItem = { name: string; icon: string; children: { name: string; icon?: string }[] }

const menus: MenuItem[] = [
  { name: 'Dashboard',                 icon: 'fa-solid fa-square-poll-vertical', children: [] },
  { name: 'Document',                  icon: 'fa-solid fa-folder',               children: [{ name: 'Document List' }, { name: 'Document Category' }] },
  { name: 'Certification',             icon: 'fa-solid fa-folder',               children: [{ name: 'Halal Certification' }, { name: 'Industry Certification' }] },
  { name: 'Audit',                     icon: 'fa-solid fa-clipboard-check',      children: [{ name: 'Internal Audit' }, { name: 'Supplier Audit' }, { name: 'Third-Party Audit' }] },
  { name: 'Non-Conformance',           icon: 'fa-solid fa-triangle-exclamation', children: [{ name: 'NC List' }, { name: 'NC Report' }] },
  { name: 'AI Matching',               icon: 'fa-solid fa-wand-magic-sparkles',  children: [{ name: 'Smart Matching' }, { name: 'Matching Settings' }] },
  { name: 'Operation',                 icon: 'fa-solid fa-briefcase',            children: [{ name: 'Product' }, { name: 'Raw Material' }, { name: 'Manufacturing' }] },
  { name: 'Purchasing',                icon: 'fa-solid fa-file-invoice-dollar',  children: [{ name: 'Supplier' }, { name: 'Purchase Order' }] },
  { name: 'MES',                       icon: 'fa-solid fa-diagram-project',      children: [{ name: 'Dashboard' }, { name: 'Cost Analysis' }, { name: 'Production' }] },
  { name: 'Inventory',                 icon: 'fa-solid fa-warehouse',            children: [{ name: 'Inventory' }, { name: 'Incoming Records' }, { name: 'Outgoing Records' }] },
  { name: 'Collaborator',              icon: 'fa-solid fa-building',             children: [{ name: 'Collaborator List' }, { name: 'Collaboration Requests' }] },
  { name: 'Report',                    icon: 'fa-solid fa-list',                 children: [{ name: 'Audit Report' }, { name: 'Compliance Report' }, { name: 'Export Report' }] },
  { name: 'Monitoring',                icon: 'fa-solid fa-eye',                  children: [{ name: 'Typhoid Vaccination' }, { name: 'Cold Truck Real-Time Monitoring' }, { name: 'Alert' }] },
  { name: 'Human Resource',            icon: 'fa-solid fa-people-group',         children: [{ name: 'Employee' }, { name: 'Training' }, { name: 'Competency' }] },
  { name: 'Customer Voice Management', icon: 'fa-solid fa-comments',             children: [] },
  { name: 'User Management',           icon: 'fa-solid fa-user',                 children: [] },
]

function toggleMenu(name: string, hasChildren: boolean) {
  if (!hasChildren) {
    activeParent.value = name
    if (pageMap[name]) currentPage.value = pageMap[name]
    return
  }
  if (openMenus.value.includes(name)) {
    openMenus.value = openMenus.value.filter(m => m !== name)
  } else {
    openMenus.value  = [name]
    activeParent.value = name
  }
}

function selectChild(child: string, parent: string) {
  activeChild.value  = child
  activeParent.value = parent
  if (pageMap[child]) currentPage.value = pageMap[child]
}

function isOpen        (name: string) { return openMenus.value.includes(name) }
function isActiveParent(name: string) { return activeParent.value === name }
function isActiveChild (child: string){ return activeChild.value === child }
</script>

<style scoped>
/* Pulled from hias-shell.css — scoped to this component */
*, *::before, *::after { box-sizing: border-box; margin: 0; padding: 0; }

#hias-root {
  height: 100vh;
  display: flex;
  flex-direction: column;
  font-family: 'Roboto', sans-serif;
  background: #f5f5f5;
  overflow: hidden;
}

/* ── Top Nav ── */
.hias-topnav {
  background: #ffffff;
  border-bottom: 1px solid #d9dce8;
  height: 50px;
  flex-shrink: 0;
  box-shadow: 0 1px 4px rgba(0,0,0,.08);
  z-index: 100;
}
.topnav-inner {
  height: 100%;
  display: flex;
  align-items: center;
  padding: 0 12px 0 0;
  gap: 8px;
}
.topnav-left {
  display: flex;
  align-items: center;
  gap: 4px;
  flex-shrink: 0;
}
.btn-flat-round {
  background: transparent;
  border: none;
  border-radius: 50%;
  color: #515151;
  cursor: pointer;
  font-size: 16px;
  height: 36px;
  width: 36px;
  display: flex;
  align-items: center;
  justify-content: center;
}
.btn-flat-round:hover { background: #f0f0f0; }
.hias-logo { display: flex; align-items: center; padding: 0 8px; }
.hias-logo img { height: 28px; }

.menu-section {
  display: flex;
  align-items: center;
  gap: 6px;
  margin-left: auto;
}
.global-search {
  position: relative;
  display: flex;
  align-items: center;
}
.global-search input {
  border: 1px solid #d0d3e0;
  border-radius: 20px;
  font-family: 'Roboto', sans-serif;
  font-size: 13px;
  height: 32px;
  outline: none;
  padding: 0 32px 0 14px;
  width: 220px;
  color: #333;
}
.global-search input:focus { border-color: #1565c0; }
.search-append-icon {
  color: #9e9e9e;
  font-size: 13px;
  position: absolute;
  right: 10px;
}

.org-btn {
  background: transparent;
  border: 1px solid #d0d3e0;
  border-radius: 4px;
  color: #515151;
  cursor: pointer;
  display: flex;
  align-items: center;
  gap: 5px;
  font-family: 'Roboto', sans-serif;
  font-size: 13px;
  height: 32px;
  padding: 0 10px;
}
.org-btn:hover { background: #f5f5f5; }
.org-svg { display: flex; align-items: center; }
.org-chevron { font-size: 10px; color: #9e9e9e; }

.bell-btn {
  background: transparent;
  border: none;
  cursor: pointer;
  height: 36px;
  width: 36px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  position: relative;
}
.bell-btn:hover { background: #f0f0f0; }
.bell-icon-unread { color: #515151; font-size: 16px; }
.bell-badge {
  background: #e53935;
  border-radius: 8px;
  color: #fff;
  font-size: 9px;
  font-weight: 700;
  min-width: 16px;
  padding: 1px 4px;
  position: absolute;
  right: 2px;
  top: 4px;
}

.user-btn {
  background: transparent;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  display: flex;
  align-items: center;
  gap: 6px;
  padding: 4px 8px;
}
.user-btn:hover { background: #f0f0f0; }
.user-avatar-wrap {
  width: 28px;
  height: 28px;
  border-radius: 50%;
  background: #e3f2fd;
  display: flex;
  align-items: center;
  justify-content: center;
  color: #1565c0;
}
.avatar-svg { display: flex; }
.user-text { text-align: left; }
.user-name { font-size: 12px; font-weight: 600; color: #333; line-height: 1.2; }
.user-role { font-size: 10px; color: #9e9e9e; line-height: 1.2; }
.user-chevron { font-size: 10px; color: #9e9e9e; }

/* ── Body ── */
.hias-body {
  display: flex;
  flex: 1;
  overflow: hidden;
}
.hias-body.sidebar-hidden .hias-drawer { display: none; }

/* ── Sidebar ── */
.hias-drawer {
  background: #ffffff;
  border-right: 1px solid #d9dce8;
  display: flex;
  flex-direction: column;
  flex-shrink: 0;
  overflow-y: auto;
  width: 220px;
}
.hias-drawer::-webkit-scrollbar { width: 4px; }
.hias-drawer::-webkit-scrollbar-thumb { background: #d0d3e0; border-radius: 2px; }

.nav-exp-item { border-bottom: 1px solid #f0f0f0; }

.nav-item-row {
  align-items: center;
  cursor: pointer;
  display: flex;
  height: 44px;
  padding-right: 8px;
  transition: background 0.12s;
}
.nav-item-row:hover { background: #f5f5f5; }
.nav-item-row.activeNavItem { background: #e3f2fd; }

.nav-icon-section {
  align-items: center;
  border-left: 3px solid transparent;
  display: flex;
  flex-shrink: 0;
  height: 100%;
  justify-content: center;
  width: 44px;
}
.nav-icon-section.orangeBorder { border-left-color: #f57c00; }
.nav-icon-el { color: #757575; font-size: 15px; }
.nav-icon-section.activeIcon .nav-icon-el { color: #1565c0; }

.nav-label-section { flex: 1; overflow: hidden; }
.nav-label-text { color: #515151; font-size: 12px; font-weight: 500; overflow: hidden; text-overflow: ellipsis; white-space: nowrap; }
.nav-item-row.activeNavItem .nav-label-text { color: #1565c0; font-weight: 600; }
.childInset { padding-left: 4px; }

.nav-expand-icon {
  color: #9e9e9e;
  font-size: 11px;
  transition: transform 0.2s;
}
.nav-expand-icon--open { transform: rotate(180deg); }

.nav-children-wrap { max-height: 0; overflow: hidden; transition: max-height 0.25s ease; }
.nav-children--open { max-height: 400px; }

.nav-child-row {
  align-items: center;
  background: #fafafa;
  cursor: pointer;
  display: flex;
  height: 38px;
  padding-right: 8px;
  transition: background 0.12s;
}
.nav-child-row:hover { background: #f0f0f0; }
.nav-child-row.activeNavItem { background: #e3f2fd; }

/* ── Page container ── */
.hias-page-container {
  flex: 1;
  overflow: hidden;
  position: relative;
}

/* ── Chat FAB ── */
.chat-fab {
  background: #1565c0;
  border: none;
  border-radius: 50%;
  bottom: 24px;
  box-shadow: 0 4px 12px rgba(21,101,192,.4);
  color: #fff;
  cursor: pointer;
  font-size: 20px;
  height: 52px;
  position: fixed;
  right: 24px;
  width: 52px;
  z-index: 200;
}
.chat-fab:hover { background: #1976d2; }
</style>
