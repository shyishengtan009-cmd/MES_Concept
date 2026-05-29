import mqtt from 'mqtt'
import { ref } from 'vue'

const BROKER_URL = 'wss://REDACTED-BROKER-HOST:8084/mqtt'

let client: mqtt.MqttClient | null = null
const subscribers: Map<string, Set<(payload: string) => void>> = new Map()

export type MqttStatus = 'connecting' | 'connected' | 'disconnected'
export const mqttStatus = ref<MqttStatus>('connecting')

export function getMqttClient(): mqtt.MqttClient {
  if (client) return client

  client = mqtt.connect(BROKER_URL, {
    clientId: `hias-concept-${Math.random().toString(16).slice(2, 8)}`,
    username: 'REDACTED-USER',
    password: 'REDACTED-PASSWORD',
    clean: true,
    reconnectPeriod: 3000,
  })

  client.on('connect', () => {
    mqttStatus.value = 'connected'
    subscribers.forEach((_, topic) => client!.subscribe(topic))
  })

  client.on('message', (topic, payload) => {
    const handlers = subscribers.get(topic)
    if (handlers) handlers.forEach(fn => fn(payload.toString()))
  })

  client.on('error', () => { mqttStatus.value = 'disconnected' })
  client.on('offline', () => { mqttStatus.value = 'disconnected' })
  client.on('reconnect', () => { mqttStatus.value = 'connecting' })

  return client
}

export function mqttSubscribe(topic: string, handler: (payload: string) => void) {
  if (!subscribers.has(topic)) subscribers.set(topic, new Set())
  subscribers.get(topic)!.add(handler)
  const c = getMqttClient()
  if (c.connected) c.subscribe(topic)
}

export function mqttUnsubscribe(topic: string, handler: (payload: string) => void) {
  subscribers.get(topic)?.delete(handler)
}

export function mqttPublish(topic: string, payload: string) {
  getMqttClient().publish(topic, payload)
}
