// Run with: node mqtt-publisher.mjs
// Publishes simulated sensor data to the local NanoMQ broker

import mqtt from 'mqtt'

const client = mqtt.connect('wss://REDACTED-BROKER-HOST:8084/mqtt', {
  clientId: 'hias-publisher',
  username: 'REDACTED-USER',
  password: 'REDACTED-PASSWORD',
})

function rnd(base, delta, dec = 1) {
  const v = base + (Math.random() * 2 - 1) * delta
  return dec === 0 ? Math.round(v) : parseFloat(v.toFixed(dec))
}

client.on('connect', () => {
  console.log('[Publisher] Connected — sending sensor data...')

  // ── OEE Dashboard (FFS line) ────────────────────────────────
  setInterval(() => {
    client.publish('hias/oee/ffs/availability',  JSON.stringify(rnd(91.2, 2)))
    client.publish('hias/oee/ffs/performance',   JSON.stringify(rnd(83.7, 3)))
    client.publish('hias/oee/ffs/quality',       JSON.stringify(rnd(98.4, 0.5)))
    client.publish('hias/oee/ffs/packcount',     JSON.stringify(rnd(18640, 30, 0)))
    client.publish('hias/oee/ffs/linespeed',     JSON.stringify(rnd(62, 4, 0)))
  }, 2000)

  // ── OEE Dashboard (Batch line) ──────────────────────────────
  setInterval(() => {
    client.publish('hias/oee/batch/availability', JSON.stringify(rnd(88.5, 2)))
    client.publish('hias/oee/batch/performance',  JSON.stringify(rnd(79.2, 3)))
    client.publish('hias/oee/batch/quality',      JSON.stringify(rnd(97.1, 0.5)))
  }, 2000)

  // ── Shop Floor — FFS Machines ───────────────────────────────
  setInterval(() => {
    client.publish('hias/shopfloor/m01/fillweight', JSON.stringify(rnd(250.4, 1.5)))
    client.publish('hias/shopfloor/m01/sealtemp',   JSON.stringify(rnd(182.5, 2)))
    client.publish('hias/shopfloor/m02/fillweight', JSON.stringify(rnd(249.8, 1.5)))
    client.publish('hias/shopfloor/m02/sealtemp',   JSON.stringify(rnd(181.0, 2)))
    client.publish('hias/shopfloor/m03/fillweight', JSON.stringify(rnd(251.2, 1.5)))
    client.publish('hias/shopfloor/m03/sealtemp',   JSON.stringify(rnd(183.2, 2)))
  }, 3000)

  // ── Shop Floor — Batching Plant ─────────────────────────────
  setInterval(() => {
    client.publish('hias/shopfloor/bp01/batchweight', JSON.stringify(rnd(2450, 50, 0)))
    client.publish('hias/shopfloor/bp01/rpm',         JSON.stringify(rnd(18.4, 0.5)))
    client.publish('hias/shopfloor/bp02/batchweight', JSON.stringify(rnd(2380, 50, 0)))
    client.publish('hias/shopfloor/bp02/rpm',         JSON.stringify(rnd(17.8, 0.5)))
  }, 3000)

  // ── MES / WES ───────────────────────────────────────────────
  setInterval(() => {
    client.publish('hias/mes/wo/WO-2026-041/progress', JSON.stringify(rnd(82, 3, 0)))
    client.publish('hias/wes/storage',                 JSON.stringify(rnd(2840, 100, 0)))
    client.publish('hias/wes/picking',                 JSON.stringify(rnd(14, 3, 0)))
    client.publish('hias/wes/agv01/battery',           JSON.stringify(rnd(88, 2, 0)))
  }, 4000)

  console.log('[Publisher] Intervals started. Press Ctrl+C to stop.')
})

client.on('error', err => console.error('[Publisher] Error:', err.message))
