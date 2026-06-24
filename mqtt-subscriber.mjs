// Subscribes to the client's local MQTT broker and prints incoming data
// Run with: node mqtt-subscriber.mjs

import mqtt from 'mqtt'

const client = mqtt.connect('mqtt://192.168.0.98:1883', {
  clientId: 'hias-subscriber',
  clean: true,
  reconnectPeriod: 3000,
})

client.on('connect', () => {
  console.log('[Subscriber] Connected to 192.168.0.98:1883')
  client.subscribe('decoded', { qos: 0 }, (err) => {
    if (err) console.error('[Subscriber] Subscribe error:', err.message)
    else console.log('[Subscriber] Subscribed to topic: decoded — waiting for data...')
  })
})

client.on('message', (topic, payload) => {
  console.log(`[${new Date().toLocaleTimeString()}] Topic: ${topic} | Data: ${payload.toString()}`)
})

client.on('error', err => console.error('[Subscriber] Error:', err.message))
client.on('offline', () => console.log('[Subscriber] Disconnected'))
client.on('reconnect', () => console.log('[Subscriber] Reconnecting...'))
