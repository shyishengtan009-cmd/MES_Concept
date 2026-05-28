import { reactive } from 'vue'

export interface StockEntry {
  skuCode: string
  skuName: string
  qty: number
  unit: string
  batchNo: string
  expiryDate: string
  warehouseName: string
  incomingId: number
}

// Tracks which incoming record IDs have already contributed to stock
const stockedRecordIds = new Set<number>()

// Pre-seeded from already-passed incoming records (IDs 1, 4, 6, 9)
export const stockPool = reactive<StockEntry[]>([
  { skuCode: 'SKU-0019', skuName: 'Sodium Benzoate E211',    qty: 80,  unit: 'carton', batchNo: 'BT-1A', expiryDate: '2027-06-01', warehouseName: 'Zone D', incomingId: 1 },
  { skuCode: 'SKU-0034', skuName: 'Corrugated Box 400×300',  qty: 20,  unit: 'carton', batchNo: 'BT-4A', expiryDate: '',           warehouseName: 'Zone B', incomingId: 4 },
  { skuCode: 'SKU-0034', skuName: 'Corrugated Box 400×300',  qty: 4,   unit: 'carton', batchNo: 'BT-4B', expiryDate: '',           warehouseName: 'Zone B', incomingId: 4 },
  { skuCode: 'SKU-0012', skuName: 'Curry Powder (1kg bag)',  qty: 200, unit: 'carton', batchNo: 'BT-6A', expiryDate: '2026-09-01', warehouseName: 'Zone A', incomingId: 6 },
  { skuCode: 'SKU-0021', skuName: 'Palm Olein Oil (5L)',     qty: 100, unit: 'pcs',    batchNo: 'BT-9A', expiryDate: '2027-04-15', warehouseName: 'Zone A', incomingId: 9 },
])

// Mark pre-seeded records so they don't get double-added
;[1, 4, 6, 9].forEach(id => stockedRecordIds.add(id))

export function addToStockPool(incomingId: number, items: Omit<StockEntry, 'incomingId'>[]) {
  if (stockedRecordIds.has(incomingId)) return
  stockedRecordIds.add(incomingId)
  items.forEach(item => {
    stockPool.push({ ...item, incomingId })
  })
}

export function removeFromStockPool(incomingId: number) {
  const idx = stockPool.findIndex(s => s.incomingId === incomingId)
  while (stockPool.findIndex(s => s.incomingId === incomingId) !== -1) {
    stockPool.splice(stockPool.findIndex(s => s.incomingId === incomingId), 1)
  }
  stockedRecordIds.delete(incomingId)
}
