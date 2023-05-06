import { createContext, useContext, useState } from "react";

const SelectContext = createContext(null)

export const useSelectContext = () => {
  return useContext(SelectContext)
}

export const SelectContextProvider = ({ children }) => {
  const [selectData, setSelectData] = useState(null)
  return <SelectContext.Provider value={[selectData, setSelectData]}>{children}</SelectContext.Provider>
}
