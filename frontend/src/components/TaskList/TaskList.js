import { useEffect, useState } from 'react'
import './TaskList.scss'
import { api } from './../../utils/axios';
import { NavLink } from 'react-router-dom';

export default function TaskList() {
  const [tasks, setTasks] = useState([])

  useEffect(() => {
    const fetchTasks = async () => {
      const { data } = await api.get('/tasks')
      setTasks(data)
    }

    fetchTasks()
  }, [])

  return (
    <div className="task-list-page">
      <h1>Список доступных задач</h1>
      <div className='task-list'>
        {tasks.map((task) => <div className='task-item' key={task.taskNum}>
          <h2>Задача #{task.taskNum} "{task.taskName}" <NavLink to={`/tasks/${task.taskNum}`}>Перейти</NavLink></h2>
          <p>{task.taskDescription}</p>
        </div>)}
      </div>
    </div>
  ) 
}