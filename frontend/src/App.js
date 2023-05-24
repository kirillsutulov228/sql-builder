import { useCallback, useEffect, useState } from "react";
import BuildBlock from "./components/BuildBlock/BuildBlock";
import { useBlockData } from "./store/blockDataContext";
import { buildPropertiesByBlockType, dragContexts, getBlockItemById, getDragDataFromEvent, mapBlockData, withRecalculatedNestedPositions, withoutChildById } from "./utils/dragDataUtils";
import { v4 as uuid } from 'uuid';
import { syntaxHighlight } from "./utils/jsonUtils";
import { useSelectContext } from "./store/selectContext";
import { useDebouncedCallback } from "./utils/hooks";
import { api } from "./utils/axios";
import { produce } from "immer";

function App() {
  const [blockData, setBlockData] = useBlockData()
  const [selectData, setSelectData] = useSelectContext()
  const [result, setResult] = useState(null);

  const fetchQuery = useDebouncedCallback(async () => {
    if (!blockData) return
    try {
      const { data } = await api.post('/query/parse', blockData);
      setResult(data.item1)
      console.log({ data });
    } catch (err) {
      console.error(err)
    }
  }, 500, [blockData])

  useEffect(() => {
    fetchQuery();
  }, [fetchQuery])

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
        properties: buildPropertiesByBlockType(data.blockType)
      }])
    }

    if (data.dragContext === dragContexts.grid) {
      const newItemIndex = blockData.findIndex(v => v.id === data.id)
      const newData = [...blockData]
      newData[newItemIndex] = { ...newData[newItemIndex], x, y }
      setBlockData(newData)
    }
    if (data.dragContext === dragContexts.nested) {
      const selected = getBlockItemById(blockData, data.id)
      const withoitChild = [...withoutChildById(blockData, data.id)]
      const newObj = { ...selected, x, y }
      delete newObj.parentId
      withoitChild.push(newObj)
      return setBlockData(withoitChild)
    }
  }, [setBlockData, blockData])

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
          properties={child.properties}
          draggable
        >
          {renderChildren(child.children)}
        </BuildBlock>
      ))
    }

    return blockData.map(item => (
      <BuildBlock
        id={item.id}
        key={item.id}
        properties={item.properties}
        blockType={item.blockType}
        className={'placed-block root-block'}
        dragContext={dragContexts.grid}
        style={{ '--pos-x': `${item.x}px`, '--pos-y': `${item.y}px` }}
        data-block-type={item.blockType}
        draggable
      >
        {renderChildren(item.children)}
      </BuildBlock>
    ))
  }, [blockData])

  const deleteSelectedById = useCallback(() => {
    setBlockData(withoutChildById(blockData, selectData.id))
    setSelectData(null)
  }, [blockData, selectData, setBlockData, setSelectData])

  useEffect(() => {
    if (selectData) {
      const itemContainer = document.querySelector('.info-menu .item-container')
      itemContainer.innerHTML = ''
      const elem = selectData.element.cloneNode(true)
      const childrens = Array.from(elem.querySelectorAll('*'))
      childrens.forEach(c => c.id = '')
      elem.draggable = false
      elem.id = ''
      itemContainer.appendChild(elem)
    }
  }, [selectData])

  const setPropertyById = useCallback((id, propKey, value) => {
    setBlockData(() => mapBlockData(blockData, (item) => {
      if (item.id !== id) return item
      const changedItem = produce(item, (newItem) => { newItem.properties[propKey].value = value })
      return changedItem
    }))
    
    setSelectData(produce(newSelect => { newSelect.properties[propKey].value = value }))
    setTimeout(() => setBlockData((data) => withRecalculatedNestedPositions(data)))
  }, [blockData, setBlockData, setSelectData])
  
  return (
    <div className="app">
      <header className="main-header">
        <div className="logo">SQL Builder</div>
      </header>
      <div className="content">
        <nav role="menu" className="menu">
          <div className="block-select-menu">
            <p className="block-select-menu-title">Блоки</p>
            <BuildBlock className={'select-block'} blockType="QUERY"/>
            <BuildBlock className={'select-block'} blockType="SELECT"/>
            <BuildBlock className={'select-block'} blockType="FROM"/>
            <BuildBlock className={'select-block'} blockType="TABLE_NAME"/>
            <BuildBlock className={'select-block'} blockType={"FIELD_NAME"}/>
            <BuildBlock className={'select-block'} blockType={"WHERE"}/>
            <BuildBlock className={'select-block'} blockType={"CONDITION"}/>
            <BuildBlock className={'select-block'} blockType={"AND"}/>
            <BuildBlock className={'select-block'} blockType={"OR"}/>
          </div>
        </nav>
        <div className="build-section">
          <div className="build-panel-wrapper">
            <div className="build-panel">
              {selectData && (
                <div className="info-menu">
                  <div className="item-container" />
                  <div className="input-block">
                    {Object.entries(selectData.properties).map(([key, option]) => {
                      return (
                        <div className="text-input-field" key={key}>
                          <label htmlFor={key}>{option.title || key}</label>
                          <input
                            type="text"
                            name={key}
                            value={option.value ?? ''}
                            onChange={(event) => setPropertyById(selectData.id, key, event.target.value)} />
                        </div>
                      )
                    })}
                  </div>
                  <button className="item-delete-button" type="button" onClick={deleteSelectedById}>Удалить</button>
                </div>
              )}
              <div className="grid"
                onDrop={onDropHandler}
                onDragOver={onDragOverHandler}
                onDragEnter={onDragEnterHandler}>
                {renderBlockTree()}
              </div>
            </div>
          </div>
          <div className="result">
            {<pre dangerouslySetInnerHTML={{__html: result}}></pre>}
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
