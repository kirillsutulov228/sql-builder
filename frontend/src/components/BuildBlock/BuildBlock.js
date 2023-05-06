import classNames from 'classnames'
import './BuildBlock.scss'
import { useCallback, useRef, useState } from 'react'
import { dragContexts, getBlockItemById, getDragDataFromEvent, withAddChildById, withRecalculatedNestedPositions, withoutChildById } from '../../utils/dragDataUtils'
import { useBlockData } from '../../store/blockDataContext'
import { v4 as uuid } from 'uuid'
import { useSelectContext } from '../../store/selectContext'

export default function BuildBlock ({
  className,
  children,
  blockType = "NONE",
  dragContext = dragContexts.menu,
  id,
  draggable = true,
  ...rest
}) {
  const elemRef = useRef(null)
  const [blockData, setBlockData] = useBlockData()
  const [isDragging, setIsDragging] = useState(false)
  const [selectData, setSelectData] = useSelectContext()

  const onDragStartHandler = useCallback((event) => {
    const rect = event.target.getBoundingClientRect()
    const offsetX = event.clientX - rect.left
    const offsetY = event.clientY - rect.top
    event.dataTransfer.setData('text/plain', JSON.stringify({ blockType, dragContext, id, offsetX, offsetY }))
    setIsDragging(true)
    event.stopPropagation()
  }, [blockType, dragContext, id])

  const onDragEndHandler = useCallback((event) => {
    setIsDragging(false)
  }, [])

  const onDropHandler = useCallback((event) => {
    if (dragContext === dragContexts.menu) return

    const { data, x, y } = getDragDataFromEvent(event)
    if (data.id === id) return
    if (data.dragContext === dragContexts.menu) {
      event.stopPropagation()
      const item = {
        id: uuid(),
        blockType: data.blockType,
        parentId: id,
        x,
        y
      }
      const newBlockDataState = withAddChildById(
        withRecalculatedNestedPositions(
          blockData
        ), id, item
      )
      setBlockData(() => newBlockDataState)
      return setTimeout(() => setBlockData((data) => withRecalculatedNestedPositions(data)))
    }
    
    if (data.dragContext === dragContexts.nested || data.dragContext === dragContexts.grid) {
      event.stopPropagation()
      const recalculated = withRecalculatedNestedPositions(blockData)
      let moved = getBlockItemById(recalculated, data.id)
      const withoutMoved = withoutChildById(recalculated, moved.id)
      try {
        const newBlockData = withAddChildById(withoutMoved, id, {
          ...moved,
          parentId: id,
          x,
          y
        })
        setBlockData(newBlockData)
        setTimeout(() => setBlockData((data) => withRecalculatedNestedPositions(data)))
      } catch (err) {
        console.warn('Нарушение уровня вложенности')
      }
    }
  }, [blockData, dragContext, id, setBlockData])

  const selectHandler = useCallback((event) => {
    if (dragContext === dragContexts.grid || dragContext === dragContexts.nested) {
      setSelectData({ element: elemRef.current, id, dragContext })
      event.stopPropagation()
    }
  }, [dragContext, id, setSelectData])

  return (
    <div
      id={id}
      className={classNames('build-block', className)}
      data-dragging={isDragging}
      onDragStart={onDragStartHandler}
      onDragEnd={onDragEndHandler}
      onDrop={onDropHandler}
      draggable={draggable}
      onClick={selectHandler}
      ref={elemRef}
      {...rest}>
        <div className='block-type'>{blockType}</div>
      {children}
    </div>
  )
}
