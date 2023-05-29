import { useEffect, useState } from 'react'
import './TaskList.scss'
import { api } from './../../utils/axios';
import { NavLink } from 'react-router-dom';

export default function TaskList() {
  const [tasks, setTasks] = useState([])
  const [isTaskLoading, setIsTaskLoading] = useState(true)
  useEffect(() => {
    const fetchTasks = async () => {
      setIsTaskLoading(true)
      const { data } = await api.get('/tasks')
      setTasks(data)
      setIsTaskLoading(false)
    }

    fetchTasks()
  }, [])

  return (
    <div className="task-list-page">
      <h1>Список доступных задач</h1>
      {isTaskLoading && <p>Загрузка..</p>}
      <div className='task-list'>
        {tasks.map((task) => <div className='task-item' key={task.taskNum}>
          <h2>Задача #{task.taskNum} "{task.taskName}" <NavLink to={`/tasks/${task.taskNum}`}>Перейти</NavLink></h2>
          <p>{task.taskDescription}</p>
        </div>)}
      </div>
    </div>
  ) 
}