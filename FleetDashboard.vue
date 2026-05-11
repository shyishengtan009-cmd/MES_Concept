<template>
  <div id="hias-root">

    <!-- TOP NAV -->
    <header class="hias-topnav">
      <div class="topnav-inner">

        <div class="topnav-left">
          <button class="btn-flat-round" @click="showSidebar = !showSidebar" title="Toggle menu">
            <i class="fas fa-bars"></i>
          </button>
          <div class="hias-logo">
            <svg viewBox="0 0 120 40" xmlns="http://www.w3.org/2000/svg" class="hias-logo-svg">
              <rect x="0" y="1"  width="36" height="11" rx="2" fill="#e53935"/>
              <rect x="0" y="14" width="36" height="11" rx="2" fill="#16A34A"/>
              <rect x="0" y="27" width="36" height="11" rx="2" fill="#1f2a58"/>
              <text x="44" y="28" font-family="inherit" font-weight="700" font-size="22" fill="#222">HIAS</text>
            </svg>
          </div>
        </div>

        <div class="menu-section">
          <div class="global-search">
            <input type="text" placeholder="Search HIAS" />
            <i class="fas fa-magnifying-glass search-append-icon"></i>
          </div>

          <button class="org-btn">
            <span v-html="corporateFareSVG" class="org-svg"></span>
            <span>Toppleware</span>
            <i class="fas fa-chevron-down org-chevron"></i>
          </button>

          <button class="bell-btn">
            <i class="fas fa-bell bell-icon-unread"></i>
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
            <i class="fas fa-angle-down user-chevron"></i>
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
                activeIcon:   isActiveParent(menu.name) && (isOpen(menu.name) || !menu.children.length),
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
              <i class="fas fa-angle-down"></i>
            </div>
          </div>

          <div class="nav-children-wrap" :class="{ 'nav-children--open': isOpen(menu.name) }">
            <div
              v-for="child in menu.children"
              :key="child"
              class="nav-child-row"
              :class="{
                activeNavItem: isActiveChild(child),
                orangeBorder:  isActiveChild(child),
                activeIcon:    isActiveChild(child),
              }"
              @click="selectChild(child, menu.name)"
            >
              <div
                class="nav-icon-section"
                :class="{
                  orangeBorder: isActiveChild(child),
                  activeIcon:   isActiveChild(child),
                }"
              ></div>
              <div class="nav-label-section">
                <div class="nav-label-text childInset">{{ child }}</div>
              </div>
            </div>
          </div>
        </div>
      </aside>

      <!-- PAGE -->
      <div class="hias-page-container">

        <!-- Page Header -->
        <div class="bg-white header-app hias-header-fix">
          <div>
            <div class="text-h4 text-weight-medium">Fleet Dashboard</div>
            <div class="breadcrumb q-mt-sm">
              <span>Monitoring</span>
              <span class="bc-sep">&nbsp;|&nbsp;</span>
              <span>Fleet</span>
              <span class="bc-sep">&nbsp;|&nbsp;</span>
              <span class="bc-active">Fleet Dashboard</span>
            </div>
          </div>
          <div class="header-slot-btns">
            <button class="q-btn-dense-outline">Dashboard</button>
            <button class="q-btn-dense-outline">Device Settings</button>
            <button class="q-btn-dense-outline">Real Time Monitoring</button>
            <button class="q-btn-dense-green7">
              <i class="fa-solid fa-plus"></i> Add Device
            </button>
          </div>
        </div>

        <!-- Main content area (blank) -->
        <div class="app-container q-mx-lg q-mt-lg q-mb-sm">
        </div>

      </div><!-- /.hias-page-container -->
    </div><!-- /.hias-body -->

  </div><!-- /#hias-root -->
</template>

<script setup lang="ts">
import { ref } from 'vue'

const corporateFareSVG = `<svg xmlns="http://www.w3.org/2000/svg" enable-background="new 0 0 24 24" height="18px" viewBox="0 0 24 24" width="18px"><rect fill="none" height="30" width="28"/><path fill="currentColor" d="M12,7V3H2v18h20V7H12z M10,19H4v-2h6V19z M10,15H4v-2h6V15z M10,11H4V9h6V11z M10,7H4V5h6V7z M20,19h-8V9h8V19z M18,11h-4v2h4V11z M18,15h-4v2h4V15z"/></svg>`
const userAvatarSVG    = `<svg width="38px" height="38px" viewBox="0 0 16 16" xmlns="http://www.w3.org/2000/svg" fill="currentColor"><path d="M3 14s-1 0-1-1 1-4 6-4 6 3 6 4-1 1-1 1H3zm5-6a3 3 0 1 0 0-6 3 3 0 0 0 0 6z"/></svg>`

const showSidebar  = ref(true)
const openMenus    = ref(['Monitoring'])
const activeParent = ref('Monitoring')
const activeChild  = ref('Cold Truck Real-Time Monitoring')

const menus = [
  { name: 'Dashboard',                 icon: 'fas fa-chart-bar',           children: [] },
  { name: 'Document',                  icon: 'fas fa-folder-open',          children: ['Document List', 'Document Category'] },
  { name: 'Certification',             icon: 'fas fa-certificate',          children: ['Halal Certification', 'Industry Certification'] },
  { name: 'Audit',                     icon: 'fas fa-clipboard-check',      children: ['Internal Audit', 'Supplier Audit', 'Third-Party Audit'] },
  { name: 'Non-Conformance',           icon: 'fas fa-exclamation-triangle',  children: ['NC List', 'NC Report'] },
  { name: 'AI Matching',               icon: 'fas fa-brain',                children: ['Smart Matching', 'Matching Settings'] },
  { name: 'Operation',                 icon: 'fas fa-cogs',                 children: ['Product', 'Raw Material', 'Manufacturing'] },
  { name: 'Purchasing',                icon: 'fas fa-shopping-cart',        children: ['Supplier', 'Purchase Order'] },
  { name: 'Collaborator',              icon: 'fas fa-handshake',            children: ['Collaborator List', 'Collaboration Requests'] },
  { name: 'Report',                    icon: 'fas fa-chart-line',           children: ['Audit Report', 'Compliance Report', 'Export Report'] },
  { name: 'Monitoring',                icon: 'fas fa-satellite-dish',       children: ['Temperature & Humidity', 'Typhoid Vaccination', 'Cold Truck Real-Time Monitoring', 'Alert'] },
  { name: 'Human Resource',            icon: 'fas fa-users',                children: ['Employee', 'Training', 'Competency'] },
  { name: 'Customer Voice Management', icon: 'fas fa-comments',             children: [] },
  { name: 'User Management',           icon: 'fas fa-user-cog',             children: [] },
]

function toggleMenu(name: string, hasChildren: boolean) {
  if (!hasChildren) { activeParent.value = name; return }
  if (openMenus.value.includes(name)) {
    openMenus.value = openMenus.value.filter(m => m !== name)
  } else {
    openMenus.value = [name]
    activeParent.value = name
  }
}
function isOpen(name: string)       { return openMenus.value.includes(name) }
function isActiveParent(name: string) { return activeParent.value === name }
function isActiveChild(child: string) { return activeChild.value === child }
function selectChild(child: string, parent: string) {
  activeChild.value  = child
  activeParent.value = parent
}
</script>

<style scoped>
/* ── External fonts/icons (load via index.html or main.ts in production) ── */

/* ── Reset & base ─────────────────────────────────────────────────────── */
*, *::before, *::after { box-sizing: border-box; }

/* ── HIAS global styles ───────────────────────────────────────────────── */
.app-container {
  padding: 1px;
  position: relative;
  border-radius: 5px;
  margin-bottom: 19px;
  border: 1px solid #c3c6d4;
}

.header-app {
  margin: -24px;
  margin-bottom: unset;
  min-height: 89px;
  border-bottom: 1px solid #c3c6d4;
  box-sizing: border-box;
}

/* ── Layout ───────────────────────────────────────────────────────────── */
#hias-root {
  height: 100vh;
  display: flex;
  flex-direction: column;
  overflow: hidden;
  font-size: 14px;
  font-family: system-ui, -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
  color: #191919;
}
.hias-body { display: flex; flex: 1; overflow: hidden; }

/* ── Top nav ──────────────────────────────────────────────────────────── */
.hias-topnav {
  min-height: 60px;
  background: white;
  color: #333;
  border-bottom: 1px solid rgba(50, 50, 50, 0.15);
  box-shadow: 0 1px 5px rgba(0,0,0,.2), 0 2px 2px rgba(0,0,0,.14);
  flex-shrink: 0;
  z-index: 200;
}
.topnav-inner {
  display: flex; align-items: center; justify-content: space-between;
  padding: 0 12px; min-height: 60px;
}
.topnav-left { display: flex; align-items: center; gap: 4px; }

.btn-flat-round {
  width: 40px; height: 40px; border-radius: 50%;
  background: none; border: none; cursor: pointer;
  font-size: 16px; color: #333;
  display: flex; align-items: center; justify-content: center;
}
.btn-flat-round:hover { background: #f2f2f2; }

.hias-logo { display: flex; align-items: center; cursor: pointer; }
.hias-logo-svg { width: 100px; height: 36px; }

.menu-section { display: flex; align-items: center; margin-right: 30px; }

.global-search { position: relative; }
.global-search input {
  height: 36px; border: 1px solid rgba(0,0,0,.24); border-radius: 4px;
  padding: 0 36px 0 10px; font-size: 14px; font-family: inherit;
  background: white; outline: none; width: 220px; color: #333;
  transition: border-color .15s;
}
.global-search input:focus { border-color: #16a34a; border-width: 2px; }
.global-search input::placeholder { color: #aaa; }
.global-search .search-append-icon {
  position: absolute; right: 10px; top: 50%;
  transform: translateY(-50%); font-size: 13px; color: #aaa; pointer-events: none;
}

.org-btn {
  display: flex; align-items: center; gap: 6px;
  border: none; background: none; cursor: pointer;
  font-family: inherit; font-size: 14px; font-weight: 500;
  padding: 0 8px; border-radius: 8px; height: 40px; margin-left: 16px; color: #333;
}
.org-btn:hover { background: #f2f2f2; }
.org-btn .org-svg { display: flex; align-items: center; color: #7f7f7f; }
.org-btn .org-chevron { font-size: 11px; color: rgba(0,0,0,.54); }

.bell-btn {
  position: relative; width: 40px; height: 40px; border-radius: 50%;
  background: #f2f2f2; border: none; cursor: pointer;
  display: flex; align-items: center; justify-content: center; margin-left: 17px;
}
.bell-icon-unread { color: #ec808d; font-size: 16px; }
.bell-badge {
  position: absolute; top: -2px; right: -4px;
  background: #ef4444; color: #fff; font-size: 9px; font-weight: 700;
  padding: 1px 4px; border-radius: 10px; min-width: 18px; text-align: center; line-height: 1.6;
}

.user-btn {
  display: flex; align-items: center; gap: 8px;
  border: none; background: none; cursor: pointer;
  font-family: inherit; padding: 4px 8px; border-radius: 8px;
  margin-left: 8px; margin-right: -40px;
}
.user-btn:hover { background: #f2f2f2; }
.user-avatar-wrap {
  border: 1.5px solid #333; border-radius: 100px;
  overflow: hidden; width: 38px; height: 38px;
  display: flex; align-items: center; justify-content: center;
  background: white; flex-shrink: 0; color: #555;
}
.user-avatar-wrap .avatar-svg { display: flex; align-items: center; transform: translateY(4px); }
.user-text { line-height: 1.3; text-align: left; }
.user-name { font-size: 14px; font-weight: 500; }
.user-role { font-size: 12px; color: #7f7f7f; }
.user-chevron { font-size: 11px; color: rgba(0,0,0,.54); }

/* ── Sidebar ──────────────────────────────────────────────────────────── */
.hias-drawer {
  width: 255px; background: white;
  border-right: 1px solid #e0e0e0;
  overflow-y: auto; overflow-x: hidden; flex-shrink: 0;
}
.hias-drawer::-webkit-scrollbar { width: 3px; }
.hias-drawer::-webkit-scrollbar-thumb { background: #e6e7ea; border-radius: 3px; }
.sidebar-hidden .hias-drawer { display: none; }

.nav-exp-item { font-weight: 600; }
.nav-item-row {
  display: flex; align-items: center; height: 48px;
  cursor: pointer; user-select: none; transition: background .1s;
}
.nav-item-row:hover { background: #f2f2f2; }
.activeNavItem { background-color: rgba(229, 243, 234, 1); }

.nav-icon-section {
  width: 56px; flex-shrink: 0; height: 100%;
  border-left: 8px solid transparent; padding-left: 16px;
  display: flex; align-items: center; justify-content: flex-start;
}
.orangeBorder { border-left-color: #f44b08 !important; }
.activeIcon .nav-icon-el,
.orangeBorder .nav-icon-el { color: #f44b08; }
.nav-icon-el { font-size: 14px; color: #7f7f7f; }

.nav-label-section { flex: 1; overflow: hidden; }
.nav-label-text {
  font-size: 14px; color: #333;
  white-space: nowrap; overflow: hidden; text-overflow: ellipsis;
}

.nav-expand-icon {
  width: 32px; text-align: center; font-size: 14px; color: #7f7f7f;
  transition: transform .2s; display: flex; align-items: center; justify-content: center;
}
.nav-expand-icon--open { transform: rotate(180deg); }

.nav-children-wrap { overflow: hidden; max-height: 0; transition: max-height .25s ease; }
.nav-children--open { max-height: 600px; }

.nav-child-row {
  display: flex; align-items: center; height: 44px;
  cursor: pointer; font-weight: 400; transition: background .1s;
  border-left: 8px solid transparent;
}
.nav-child-row:hover { background: #f2f2f2; }
.childInset { padding-left: 20px; font-size: 14px; color: #333; }

/* ── Page container ───────────────────────────────────────────────────── */
.hias-page-container {
  flex: 1; overflow-y: auto; background: #F5F5F5;
}
.hias-page-container::-webkit-scrollbar { width: 5px; }
.hias-page-container::-webkit-scrollbar-thumb { background: #e6e7ea; border-radius: 4px; }

/* ── Page header ──────────────────────────────────────────────────────── */
.hias-header-fix {
  margin: 0 !important;
  padding: 0 32px;
  display: flex; align-items: center; justify-content: space-between;
}
.breadcrumb .bc-sep { color: #aaa; }
.bc-active { color: #16a34a; }

.text-h4 { font-size: 1.875rem; line-height: 1.25; letter-spacing: .0073529412em; }
.text-weight-medium { font-weight: 500; }
.q-mt-sm { margin-top: 8px !important; }

.header-slot-btns { display: flex; align-items: center; gap: 8px; }

.q-btn-dense-outline {
  height: 28px; padding: 0 12px;
  border: 1px solid rgba(0,0,0,.24); border-radius: 4px;
  background: white; cursor: pointer; font-family: inherit;
  font-size: 13px; font-weight: 600; color: #555; white-space: nowrap;
}
.q-btn-dense-outline:hover { background: #f2f2f2; }

.q-btn-dense-green7 {
  height: 28px; padding: 0 12px;
  border: none; border-radius: 4px; background: #388E3C;
  cursor: pointer; font-family: inherit;
  font-size: 13px; font-weight: 600; color: #fff;
  display: flex; align-items: center; gap: 5px; white-space: nowrap;
}
.q-btn-dense-green7:hover { background: #2E7D32; }

.q-mx-lg { margin-left: 16px !important; margin-right: 16px !important; }
.q-mt-lg { margin-top: 16px !important; }
.q-mb-sm { margin-bottom: 8px !important; }
.bg-white { background: white; }
</style>
