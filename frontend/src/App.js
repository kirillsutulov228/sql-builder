import { useCallback } from "react";
import BuildBlock from "./components/BuildBlock/BuildBlock";
import { useBlockData } from "./store/blockDataContext";
import { dragContexts, getDragDataFromEvent } from "./utils/dragDataUtils";
import { v4 as uuid } from 'uuid';
import { syntaxHighlight } from "./utils/jsonUtils";

function App() {
  const [blockData, setBlockData] = useBlockData()

  console.log({ blockData })

  const onDragOverHandler = useCallback((event) => {
    event.preventDefault()
  }, [])

  const onDropHandler = useCallback((event) => {
    event.preventDefault()
    event.stopPropagation()

    const { data, x, y } = getDragDataFromEvent(event)
    
    if (data.dragContext === dragContexts.menu) {
      return setBlockData((prev) => [...prev, {
        id: uuid(),
        blockType: data.blockType,
        x,
        y,
      }])
    }

    if (data.dragContext === dragContexts.grid) {
      return setBlockData(prev => {
        const newItemIndex = prev.findIndex(v => v.id === data.id)
        const newData = [...prev]
        newData[newItemIndex] = { ...newData[newItemIndex], x, y }
        return newData
      })
    }
  }, [setBlockData])

  const onDragEnterHandler = useCallback((event) => {
    event.preventDefault()
  }, [])

  const renderBlockTree = useCallback(() => {
    const renderChildren = (children) => {
      if (!children) return null
      return children.map(child => (
        <BuildBlock
          id={child.id}
          key={child.id}
          blockType={child.blockType}
          className={'placed-block nested-block'}
          dragContext={dragContexts.nested}
          data-block-type={child.blockType}
          draggable={false}
        >
          {renderChildren(child.children)}
        </BuildBlock>
      ))
    }

    return blockData.map(item => (
      <BuildBlock
        id={item.id}
        key={item.id}
        blockType={item.blockType}
        className={'placed-block root-block'}
        dragContext={dragContexts.grid}
        style={{ '--pos-x': `${item.x}px`, '--pos-y': `${item.y}px` }}
        data-block-type={item.blockType}
      >
        {renderChildren(item.children)}
      </BuildBlock>
    ))
  }, [blockData])

  return (
    <div className="app">
      <header className="main-header">
        <div className="logo">SQL Builder</div>
      </header>
      <div className="content">
        <nav role="menu" className="menu">
          <div className="block-select-menu">
            <p className="block-select-menu-title">Блоки</p>
            <BuildBlock className={'select-block'} data-block-type={"NONE"}/>
            <BuildBlock className={'select-block'} data-block-type={"NONE"}/>
            <BuildBlock className={'select-block'} data-block-type={"NONE"}/>
            <BuildBlock className={'select-block'} data-block-type={"NONE"}/>
          </div>
        </nav>
        <div className="build-section">
          <div className="build-panel-wrapper">
            <div
              className="build-panel">
              <div className="grid"
              onDrop={onDropHandler}
              onDragOver={onDragOverHandler}
              onDragEnter={onDragEnterHandler}>
                {renderBlockTree()}
              </div>
            </div>
          </div>
          <div className="result">
            <pre
            className="json"
            dangerouslySetInnerHTML={{ __html: syntaxHighlight(JSON.stringify(blockData, null, 4))}}></pre>
          </div>
        </div>
      </div>
    </div>
  );
}

export default App;
