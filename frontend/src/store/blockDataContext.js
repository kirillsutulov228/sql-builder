import { createContext, useContext, useState } from "react";

const BlockDataContext = createContext([])

export function useBlockData () {
  return useContext(BlockDataContext)
}

export function BlockDataProvider ({ children }) {
  const [blockData, setBlockData] = useState([])
  return (
    <BlockDataContext.Provider value={[blockData, setBlockData]}>
      {children}
    </BlockDataContext.Provider>
  )
}
