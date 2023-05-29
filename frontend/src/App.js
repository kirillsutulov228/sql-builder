import { Route, Routes } from "react-router"
import IndexLayout from "./components/IndexLayout/IndexLayout"
import Builder from "./Builder"
import TaskList from "./components/TaskList/TaskList"

const App = () => {
  return (
    <div className="app">
      <Routes>
        <Route element={<IndexLayout />}>
          <Route path='/' element={<Builder />} />
          <Route path='/tasks' element={<TaskList />} />
          <Route path='/tasks/:taskNum' element={<Builder taskMode/>} />
        </Route>
      </Routes>
    </div>
  )
}

export default App
