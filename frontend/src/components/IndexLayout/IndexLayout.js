import { Outlet } from "react-router"
import './IndexLayout.scss'
import { NavLink } from "react-router-dom"

const IndexLayout = () => {
  return (
    <div className="index-layout">
      <header className="main-header">
        <div className="logo">SQL Builder</div>
        <nav className="header-nav">
          <NavLink to='/'>Строитель</NavLink>
          <NavLink to='/tasks'>Задачи</NavLink>
        </nav>
      </header>
      <div className="index-content">
        <Outlet />
      </div>
    </div>
  )
}

export default IndexLayout
