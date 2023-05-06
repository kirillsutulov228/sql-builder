import { blockTypes } from "../../../utils/dragDataUtils"
import BuildBlock from "../BuildBlock"

export const SelectBlock = ({ id, children, dragContext }) => {
  return (
    <BuildBlock
      className='select-block'
      id={id}
      blockType={blockTypes.SELECT}
      dragContext={dragContext}
      draggable>
      {children}
    </BuildBlock>
  )
}