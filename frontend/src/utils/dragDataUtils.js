import { produce } from "immer"

export const dragContexts = {
  menu: "MENU",
  grid: "GRID",
  nested: "NESTED"
}

export function getBlockItemById(blockData, id) {
  let result = null
  function traverseNode (item) {
    if (item.id === id) {
      result = item
    }

    if (result || !item.children) return

    for (const child of item.children) {
      traverseNode(child)
    }
  }
  for (const item of blockData) {
    traverseNode(item)
  }
  return result
}

export const withAddChildById = produce((blockData, id, child) => {
  const selectedBlock = getBlockItemById(blockData, id)
  if (!selectedBlock.children) selectedBlock.children = []
  selectedBlock.children.push(child) 
})

export const withoutChildById = produce((blockData, id) => {
  const child = getBlockItemById(blockData, id)
  const parent = getBlockItemById(child.parentId)
  parent.children = parent.children.filter(item => item.id !== id)
})

export const getDragDataFromEvent = (event) => {
  const dataString = event.dataTransfer.getData("text/plain")
  if (!dataString) return {}
  const data = JSON.parse(dataString)
  const rect = event.currentTarget.getBoundingClientRect()
  const x = event.clientX - rect.left - (data.offsetX || 0)
  const y = event.clientY - rect.top - (data.offsetY || 0)
  return { data, x, y }
}