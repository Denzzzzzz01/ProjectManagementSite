import React from 'react';
import { DetailedProject } from '../../Models/DetailedProject';
import { PriorityLabels } from '../../Enums/PriorityEnum';

interface TaskListProps {
  project: DetailedProject;
  handleRemoveTask: (taskId: string) => void;
  handleMarkTaskDone: (taskId: string, isDone: boolean) => void;
  handleOpenUpdateModal: (taskId: string) => void;
}

const TaskList: React.FC<TaskListProps> = ({ project, handleRemoveTask, handleMarkTaskDone, handleOpenUpdateModal }) => {
  return (
    <ul className="list-disc pl-5">
      {project.tasks.map((task) => (
        <li key={task.id} className="mb-4">
          <div>
            <strong>{task.title}</strong>
            <p>Added Time: {new Date(task.addedTime).toLocaleString()}</p>
            <p>Priority: {PriorityLabels[task.priority]}</p>
            <p>Status: {task.isDone ? 'Done' : 'Not Done'}</p>
            <button
              onClick={() => handleRemoveTask(task.id)}
              className="bg-red-500 text-white px-2 py-1 rounded mt-2 mr-2"
            >
              Remove Task
            </button>
            <button
              onClick={() => handleMarkTaskDone(task.id, !task.isDone)}
              className={`px-2 py-1 rounded mt-2 ${task.isDone ? 'bg-gray-500' : 'bg-green-500'} text-white`}
            >
              {task.isDone ? 'Undo' : 'Mark as Done'}
            </button>
            <button
              onClick={() => handleOpenUpdateModal(task.id)}
              className="bg-blue-500 text-white px-2 py-1 rounded mt-2 ml-2"
            >
              Update Task
            </button>
          </div>
        </li>
      ))}
    </ul>
  );
};

export default TaskList;
