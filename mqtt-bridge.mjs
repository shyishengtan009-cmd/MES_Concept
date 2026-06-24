// Bridges local NanoMQ (plain 1883) → EMQX Cloud (TLS 8883)
// Subscribes to all topics on local broker, re-publishes to cloud
// Run with: node mqtt-bridge.mjs

import mqtt from 'mqtt'

const LOCAL  = 'mqtt://REDACTED-LOCAL-IP:1883'
const CLOUD  = 'mqtts://REDACTED-BROKER-HOST:8883'
const TOPICS = ['decoded', 'hias/#']

const local = mqtt.connect(LOCAL, {
  clientId: 'hias-bridge-local',
  clean: true,
  reconnectPeriod: 3000,
})

const cloud = mqtt.connect(CLOUD, {
  clientId: 'hias-bridge-cloud',
  username: 'REDACTED-USER',
  password: 'REDACTED-PASSWORD',
  clean: true,
  reconnectPeriod: 3000,
  rejectUnauthorized: false,
})

local.on('connect', () => {
  console.log('[Bridge] Connected to local NanoMQ (REDACTED-LOCAL-IP:1883)')
  TOPICS.forEach(t => {
    local.subscribe(t, { qos: 0 }, err => {
      if (err) console.error(`[Bridge] Subscribe error (${t}):`, err.message)
      else console.log(`[Bridge] Subscribed locally to: ${t}`)
    })
  })
})

cloud.on('connect', () => {
  console.log('[Bridge] Connected to EMQX Cloud')
})

local.on('message', (topic, payload) => {
  if (!cloud.connected) return
  cloud.publish(topic, payload, { qos: 0 }, err => {
    if (err) console.error('[Bridge] Forward error:', err.message)
    else console.log(`[${new Date().toLocaleTimeString()}] Forwarded → ${topic}: ${payload.toString().slice(0, 80)}`)
  })
})

local.on('error',   err => console.error('[Local] Error:', err.message))
local.on('offline', ()  => console.log('[Local] Disconnected from NanoMQ'))

cloud.on('error',   err => console.error('[Cloud] Error:', err.message))
cloud.on('offline', ()  => console.log('[Cloud] Disconnected from EMQX Cloud'))
