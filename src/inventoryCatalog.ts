// Shared finished-goods catalog — single source of truth for product/batch data
// referenced across Inventory, Knowledge Retention, and Document pages.

export interface FinishedGood {
  code: string
  name: string
  category: 'sauce' | 'paste' | 'season'
  uom: string
  latestBatch: string
}

export const FINISHED_GOODS: FinishedGood[] = [
  { code: 'FG-001', name: 'Chili Sauce Pouch 250g',     category: 'sauce',  uom: 'pcs', latestBatch: 'B2026-0411' },
  { code: 'FG-002', name: 'Curry Paste Tub 500g',        category: 'paste',  uom: 'pcs', latestBatch: 'B2026-0409' },
  { code: 'FG-003', name: 'Sambal Belacan Jar 200g',     category: 'sauce',  uom: 'pcs', latestBatch: 'B2026-0410' },
  { code: 'FG-004', name: 'Rendang Paste Tub 400g',      category: 'paste',  uom: 'pcs', latestBatch: 'B2026-0412' },
  { code: 'FG-005', name: 'Seasoning Blend A Sachet',    category: 'season', uom: 'pcs', latestBatch: 'B2026-0408' },
  { code: 'FG-006', name: 'Tomato Chili Sauce Bottle',   category: 'sauce',  uom: 'pcs', latestBatch: 'B2026-0407' },
  { code: 'FG-007', name: 'Asam Pedas Paste Tub 400g',   category: 'paste',  uom: 'pcs', latestBatch: 'B2026-0413' },
  { code: 'FG-008', name: 'Lemongrass Seasoning Mix',    category: 'season', uom: 'pcs', latestBatch: 'B2026-0406' },
]

export function findByName(name: string): FinishedGood | undefined {
  return FINISHED_GOODS.find(p => p.name === name)
}

export function findByBatch(batch: string): FinishedGood | undefined {
  return FINISHED_GOODS.find(p => p.latestBatch === batch)
}
