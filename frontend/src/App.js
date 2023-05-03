import { useCallback } from "react";
import BuildBlock from "./components/BuildBlock/BuildBlock";
import { useBlockData } from "./store/blockDataContext";
import { dragContexts } from "./utils/gragDataUtils";
import { v4 as uuid } from 'uuid';

const getDragDataFromEvent = (event) => {
  const dataString = event.dataTransfer.getData("text/plain")
  if (!dataString) return {}
  const data = JSON.parse(dataString)
  const rect = event.currentTarget.getBoundingClientRect()
  const x = event.clientX - rect.left - (data.offsetX || 0)
  const y = event.clientY - rect.top - (data.offsetY || 0)
  return { data, x, y }
}

function App() {
  const [blockData, setBlockData] = useBlockData()

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

  return (
    <div className="app">
      <header className="main-header">
        <div className="logo">SQL Builder</div>
      </header>
      <div className="content">
        <nav role="menu" className="menu">
          <div className="block-select-menu">
            <p className="block-select-menu-title">Блоки</p>
            <BuildBlock className={'select-block'}/>
            <BuildBlock className={'select-block'}/>
            <BuildBlock className={'select-block'}/>
            <BuildBlock className={'select-block'}/>
          </div>
        </nav>
        <div className="build-panel-wrapper">
          <div
            className="build-panel">
            <div className="grid"
             onDrop={onDropHandler}
             onDragOver={onDragOverHandler}
             onDragEnter={onDragEnterHandler}>
              {blockData.map(item => (
                <BuildBlock
                  id={item.id}
                  key={item.id}
                  blockType={item.blockType}
                  className={'placed-block root-block'}
                  dragContext={dragContexts.grid}
                  style={{ '--pos-x': `${item.x}px`, '--pos-y': `${item.y}px` }}/>)
              )}
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}

export default App;
