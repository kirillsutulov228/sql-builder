import classNames from 'classnames'
import './BuildBlock.scss'
import { useCallback, useState } from 'react'
import { dragContexts } from '../../utils/gragDataUtils'

export default function BuildBlock ({
  className,
  children,
  blockType = "NONE",
  dragContext = dragContexts.menu,
  id,
  ...rest
}) {
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

  return (
    <div
      id={id}
      className={classNames('build-block', className)}
      data-dragging={isDragging}
      onDragStart={onDragStartHandler}
      onDragEnd={onDragEndHandler}
      draggable
      {...rest}>
      {children}
    </div>
  )
}
