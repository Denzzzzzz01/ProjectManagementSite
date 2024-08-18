import React, { useState, useEffect } from 'react';
import { DetailedProject } from '../../Models/DetailedProject';
import { PriorityLabels } from '../../Enums/PriorityEnum';
import '@flaticon/flaticon-uicons/css/all/all.css';

interface TaskListProps {
  project: DetailedProject;
  handleRemoveTask: (taskId: string) => void;
  handleMarkTaskDone: (taskId: string, isDone: boolean) => void;
  handleOpenUpdateModal: (taskId: string) => void;
  className?: string;
}

const TaskList: React.FC<TaskListProps> = ({
  project,
  handleRemoveTask,
  handleMarkTaskDone,
  handleOpenUpdateModal,
  className,
}) => {
  const [sortedTasks, setSortedTasks] = useState(project.tasks);
  const [sortOption, setSortOption] = useState('date');
  const [sortOrder, setSortOrder] = useState('asc');
  const [filterOption, setFilterOption] = useState('all');
  const [expandedTaskIds, setExpandedTaskIds] = useState<string[]>([]);

  useEffect(() => {
    let tasks = [...project.tasks];

    if (filterOption === 'done') {
      tasks = tasks.filter((task) => task.isDone);
    } else if (filterOption === 'notDone') {
      tasks = tasks.filter((task) => !task.isDone);
    }

    const sortMultiplier = sortOrder === 'asc' ? 1 : -1;

    if (sortOption === 'date') {
      tasks.sort(
        (a, b) =>
          (new Date(a.addedTime).getTime() - new Date(b.addedTime).getTime()) * sortMultiplier
      );
    } else if (sortOption === 'priority') {
      tasks.sort((a, b) => {
        if (a.priority === b.priority) {
          return (
            (new Date(a.addedTime).getTime() - new Date(b.addedTime).getTime()) * sortMultiplier
          );
        }
        return (a.priority - b.priority) * sortMultiplier;
      });
    }

    setSortedTasks(tasks);
  }, [project.tasks, sortOption, sortOrder, filterOption]);

  const toggleSortOrder = () => {
    setSortOrder((prevOrder) => (prevOrder === 'asc' ? 'desc' : 'asc'));
  };

  const toggleDescription = (taskId: string) => {
    setExpandedTaskIds((prevIds) =>
      prevIds.includes(taskId)
        ? prevIds.filter((id) => id !== taskId)
        : [...prevIds, taskId]
    );
  };

  return (
    <div className={`flex-1 overflow-hidden ${className}`}>
      <div className="mb-4 p-1 flex gap-4 items-center bg-gray-200 rounded text-lg">
        <button
          onClick={toggleSortOrder}
          className="px-2 py-2 bg-beige rounded shadow-sm shadow-black hover:bg-beige-dark cursor-pointer flex items-center justify-center"
        >
          <img
            src="https://cdn-icons-png.flaticon.com/512/3916/3916919.png"
            width="16"
            height="16"
            alt="Sort Icon"
            className={`transition-transform duration-200 ${sortOrder === 'asc' ? '' : 'rotate-180'}`}
            style={{ transformOrigin: 'center' }}
          />
        </button>
        <div className="flex items-center">
          <label className="mr-2">Sort by:</label>
          <select
            className="bg-beige shadow-sm shadow-black hover:bg-beige-dark px-2 py-1 rounded cursor-pointer"
            onChange={(e) => setSortOption(e.target.value)}
            value={sortOption}
          >
            <option value="date">Date</option>
            <option value="priority">Priority</option>
          </select>
        </div>
        <div className="flex items-center">
          <label className="mr-2">Filter:</label>
          <select
            className="bg-beige shadow-sm shadow-black hover:bg-beige-dark px-2 py-1 rounded cursor-pointer"
            onChange={(e) => setFilterOption(e.target.value)}
            value={filterOption}
          >
            <option value="all">All Tasks</option>
            <option value="done">Only Done</option>
            <option value="notDone">Only Not Done</option>
          </select>
        </div>
      </div>

      <ul className="list-none pb-16 max-h-full h-[100%] overflow-y-auto scrollbar-hide">
        {sortedTasks.map((task) => (
          <li key={task.id} className="mb-6 bg-white rounded shadow-black shadow-sm relative">
            <div className="px-4 pt-1.5 flex">
              <button
                onClick={() => handleMarkTaskDone(task.id, !task.isDone)}
                className={`relative w-12 h-12 flex items-center justify-center shadow-sm shadow-black rounded mt-2 mr-4 ${
                  task.isDone ? 'bg-green-500' : 'bg-gray-500'
                } hover:bg-opacity-75`}
              >
                <img
                  src="https://cdn-icons-png.flaticon.com/512/3917/3917749.png"
                  draggable="false"
                  width="36"
                  height="36"
                  alt="Status Icon"
                  className={`transition-transform duration-200 ease-in-out transform ${
                    task.isDone ? 'filter invert' : ''
                  } hover:scale-110 active:scale-90`}
                />
              </button>
              <div className="flex flex-col justify-between w-[80%]">
                <div>
                  <strong className="text-2xl break-words">{task.title}</strong>
                  <div className="t-1 flex items-center gap-16">
                    <p>Priority: {PriorityLabels[task.priority]}</p>
                    <p>Status: {task.isDone ? 'Done' : 'Not Done'}</p>
                  </div>
                  <div className=''>
                    <button
                      className="px-1 py-0.5 text-black mt-2 flex items-center font-bold text-lg bg-beige rounded shadow-gray-700 shadow-sm 
                      hover:bg-beige-dark active:bg-beige-light"
                      onClick={() => toggleDescription(task.id)}
                    >
                      Description
                      <img
                        src="https://cdn-icons-png.flaticon.com/512/3916/3916919.png"
                        width="16"
                        height="16"
                        alt="Expand Icon"
                        className={`ml-1 transition-transform duration-300 ${
                          expandedTaskIds.includes(task.id) ? 'rotate-180' : 'rotate-0'
                        }`}
                        style={{ transformOrigin: 'center' }}
                      />
                    </button>

                    <div
                      className={`mt-2 overflow-hidden transition-max-height duration-500 ease-in-out ${
                        expandedTaskIds.includes(task.id) ? 'max-h-96' : 'max-h-0'
                      }`}
                    >
                      {expandedTaskIds.includes(task.id) && (
                        <div className="opacity-100 transition-opacity duration-500 ease-in-out whitespace-pre-wrap break-words">
                          {task.description}
                        </div>
                      )}
                    </div>
                  </div>
                </div>
              </div>
            </div>

            <div className="bg-beige-gradient pt-0.5 pb-1 rounded-b mt-3 flex items-center justify-center">
              <p className="text-center text-sm">
                Added Time: {new Date(task.addedTime).toLocaleString()}
              </p>
              <button
                onClick={() => handleOpenUpdateModal(task.id)}
                className="bg-gray-200 text-black px-1.5 py-0.5 active:pt-1 active:pb-0 shadow-sm shadow-black rounded mt-0.5 ml-2 transform transition-transform duration-100 hover:bg-gray-300 active:shadow-inner active:shadow-black"
              >
                <i className="fi fi-rs-pencil"></i>
              </button>
              <button
                onClick={() => handleRemoveTask(task.id)}
                className="bg-gray-200 text-black px-1.5 py-0.5 active:pt-1 active:pb-0 shadow-sm shadow-black rounded mt-0.5 ml-2 transform transition-transform duration-100 hover:bg-gray-300 active:shadow-inner active:shadow-black"
              >
                <i className="fi fi-rs-trash"></i>
              </button>
            </div>
          </li>
        ))}
      </ul>
    </div>
  );
};

export default TaskList;
