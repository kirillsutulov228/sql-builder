import { produce } from "immer"

export const dragContexts = {
  menu: "MENU",
  grid: "GRID",
  nested: "NESTED"
}

export const blockTypes = {
  SELECT: 'SELECT',
  FIELD_NAME: 'FIELD_NAME',
  TABLE_NAME: 'TABLE_NAME',
  CONDITION: 'CONDITION',
  JOIN: 'JOIN',
  ORDER_BY: "ORDER_BY",
  GROUP_BY: "GROUP_BY",
  COUNT: 'COUNT',
  MAX: 'MAX',
  MIN: 'MIN',
  SUM: 'SUM',
  AVG: 'AVG',
}

export function buildPropertiesByBlockType(blockType) {
  switch (blockType) {
    case blockTypes.FIELD_NAME:
      return {
        value: { title: 'Имя столбца', value: null }
      }
    case blockTypes.TABLE_NAME:
      return {
        value: { title: 'Имя таблицы', value: null }
      }
    case blockTypes.CONDITION:
      return {
        first: { title: 'Значение 1', value: null },
        operation: { title: 'Операция', value: null },
        second: { title: 'Значение 2', value: null }
      }
	case blockTypes.JOIN:
	  return {
		type: { title: 'Вид соеденинения', value: null }
	  }
	case blockTypes.COUNT:
	  return {
		value: { title: 'Псевдоним', value: null }
	  }
	case blockTypes.MAX:
	  return {
		value: { title: 'Псевдоним', value: null }
	  }
	case blockTypes.MIN:
	  return {
		value: { title: 'Псевдоним', value: null }
	  }
	case blockTypes.SUM:
	  return {
		value: { title: 'Псевдоним', value: null }
	  }
	case blockTypes.AVG:
	  return {
		value: { title: 'Псевдоним', value: null }
	  }
    default:
      return {}
  }
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

  if (blockData instanceof Array) {
    for (const item of blockData) {
      traverseNode(item)
    }
  } else {
    traverseNode(blockData)
  }

  return result
}

export const withAddChildById = produce((blockData, id, child) => {
  const selectedBlock = getBlockItemById(blockData, id)
  if (!selectedBlock.children) selectedBlock.children = []
  selectedBlock.children.push(child)
  selectedBlock.children.sort((a, b) => a.x - b.x)
})

export const withoutChildById = produce((blockData, id) => {
  const child = getBlockItemById(blockData, id)
  if (!child.parentId) {
    return blockData.filter(item => item.id !== id)
  }
  const parent = getBlockItemById(blockData, child.parentId)
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

export const mapBlockData = (blockData, mapper) => {
  const newTree = []
  const traverse = (item) => {
    const result = mapper({ ...item })
    if (item.children) {
      result.children = []
      for (const child of item.children) {
        result.children.push(traverse(child))
      }
    }
    return result
  }
  for (const item of blockData) {
    newTree.push(traverse(item))
  }
  return newTree
}

export const withRecalculatedNestedPositions = (blockData) => {
  return mapBlockData(blockData, (item) => {
    if (!item.parentId) return item
    const containerRect = document.getElementById(item.id).getBoundingClientRect()
    const parentContainerRect = document.getElementById(item.parentId).getBoundingClientRect()
    
    return {
      ...item,
      x: containerRect.x - parentContainerRect.x,
      y: containerRect.y -parentContainerRect.y
    }
  })
}
