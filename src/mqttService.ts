import { ref } from 'vue'

export type MqttStatus = 'connecting' | 'connected' | 'disconnected'
export const mqttStatus = ref<MqttStatus>('connecting')

setTimeout(() => { mqttStatus.value = 'connected' }, 600)

export function mqttSubscribe(_topic: string, _handler: (payload: string) => void) {
}

export function mqttUnsubscribe(_topic: string, _handler: (payload: string) => void) {
}

export function mqttPublish(_topic: string, _payload: string) {
}
