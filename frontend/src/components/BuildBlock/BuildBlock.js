import classNames from 'classnames'
import './BuildBlock.scss'
import { useCallback, useState } from 'react'
import { dragContexts, withAddChildById } from '../../utils/dragDataUtils'
import { useBlockData } from '../../store/blockDataContext'
import { v4 as uuid } from 'uuid'

export default function BuildBlock ({
  className,
  children,
  blockType = "NONE",
  dragContext = dragContexts.menu,
  id,
  draggable = true,
  ...rest
}) {
  const [blockData, setBlockData] = useBlockData()
  const [isDragging, setIsDragging] = useState(false)

  const onDragStartHandler = useCallback((event) => {
    const rect = event.target.getBoundingClientRect()
    const offsetX = event.clientX - rect.left
    const offsetY = event.clientY - rect.top
    event.dataTransfer.setData('text/plain', JSON.stringify({ blockType, dragContext, id, offsetX, offsetY }))
    setIsDragging(true)
  }, [blockType, dragContext, id])

  const onDragEndHandler = useCallback((event) => {
    setIsDragging(false)
  }, [])

  const onDropHandler = useCallback((event) => {
    const data = JSON.parse(event.dataTransfer.getData('text/plain'))
    if ((dragContext === dragContexts.grid || dragContext === dragContexts.nested) && data.dragContext === dragContexts.menu) {
      event.stopPropagation()
      const newBlockDataState = withAddChildById(blockData, id, {
        id: uuid(),
        blockType: data.blockType,
        parentId: id 
      })
      setBlockData(newBlockDataState)
    }
  }, [blockData, dragContext, id, setBlockData])

  return (
    <div
      id={id}
      className={classNames('build-block', className)}
      data-dragging={isDragging}
      onDragStart={onDragStartHandler}
      onDragEnd={onDragEndHandler}
      onDrop={onDropHandler}
      draggable={draggable}
      {...rest}>
        <div className='block-type'>{blockType}</div>
      {children}
    </div>
  )
}
