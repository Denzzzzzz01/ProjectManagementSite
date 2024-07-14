import React, { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import { getProjectById, updateProjectStatus } from '../../Services/ProjectService';
import { DetailedProject } from '../../Models/DetailedProject';
import { Status } from '../../Enums/StatusEnum';
import { Priority } from '../../Enums/PriorityEnum';
import TaskList from '../../Components/TaskList/TaskList';
import StatusDropdown from '../../Components/StatusDropdown/StatusDropdown';
import TaskModal from '../../Components/TaskModal/TaskModal';
import { UpdateTaskDto } from '../../Models/UpdateTaskDto';
import { addTask, markTaskDone, removeTask, updateTask } from '../../Services/TaskService';
import ConfirmationModal from '../../Components/ConfirmationModal/ConfirmationModal';
import { notifyError, notifySuccess, toastPromise } from '../../Utils/toastUtils';

const ProjectDetailPage: React.FC = () => {
  const { projectId } = useParams<{ projectId: string }>();
  const [project, setProject] = useState<DetailedProject | null>(null);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [updateTaskId, setUpdateTaskId] = useState<string | null>(null); 
  const [updateTaskForm, setUpdateTaskForm] = useState<UpdateTaskDto>({
    title: '',
    description: '',
    priority: Priority.Low,
  });
  const [isUpdateModalOpen, setIsUpdateModalOpen] = useState(false);
  const [isDeleteModalOpen, setIsDeleteModalOpen] = useState(false);
  const [taskToDeleteId, setTaskToDeleteId] = useState<string | null>(null);

  useEffect(() => {
    if (projectId) {
      getProjectById(projectId)
        .then((project) => setProject(project))
        .catch((error) => console.error('Error fetching project:', error));
    }
  }, [projectId]);

  const handleAddTask = async (task: { title: string; description: string; priority: Priority }) => {
    if (projectId) {
      try {
        await toastPromise(
          addTask({ ...task, projectId }),
          'Adding task',
          'Task added successfully!',
          'Failed to add task'
        );
        const updatedProject = await getProjectById(projectId);
        setProject(updatedProject);
      } catch (error) {
        console.error('Error adding task:', error);
      }
    }
  };

  const handleRemoveTask = async (taskId: string) => {
    if (projectId) {
      try {
        await toastPromise(
          removeTask(taskId, projectId),
          'Removing task',
          'Task removed successfully!',
          'Failed to remove task'
        );
        const updatedProject = await getProjectById(projectId);
        setProject(updatedProject);
        setIsDeleteModalOpen(false);
        setTaskToDeleteId(null);
      } catch (error) {
        console.error('Error removing task:', error);
      }
    }
  };

  const handleMarkTaskDone = async (taskId: string, isDone: boolean) => {
    if (projectId) {
      try {
        await toastPromise(
          markTaskDone(taskId, projectId, isDone),
          'Updating task status',
          'Task status updated successfully!',
          'Failed to update task status'
        );
        const updatedProject = await getProjectById(projectId);
        setProject(updatedProject);
      } catch (error) {
        console.error('Error marking task done:', error);
      }
    }
  };

  const handleOpenUpdateModal = (taskId: string) => {
    const taskToUpdate = project?.tasks.find((task) => task.id === taskId);
    if (taskToUpdate) {
      setUpdateTaskId(taskId); 
      setUpdateTaskForm({
        title: taskToUpdate.title,
        description: taskToUpdate.description,
        priority: taskToUpdate.priority,
      });
      setIsUpdateModalOpen(true);
    }
  };

  const handleCloseUpdateModal = () => {
    setIsUpdateModalOpen(false);
    setUpdateTaskId(null);
    setUpdateTaskForm({ title: '', description: '', priority: Priority.Low });
  };

  const handleUpdateTask = async (task: { title: string; description: string; priority: Priority }) => {
    if (projectId && updateTaskId) {
      try {
        await toastPromise(
          updateTask(updateTaskId, projectId, task),
          'Updating task',
          'Task has been updated!',
          'Failed to update task'
        );
        const updatedProject = await getProjectById(projectId);
        setProject(updatedProject);
        setIsUpdateModalOpen(false);
      } catch (error) {
        console.error('Error updating task:', error);
      }
    }
  };

  const handleStatusChange = async (newStatus: Status) => {
    if (projectId) {
      try {
        await toastPromise(
          updateProjectStatus(projectId, newStatus),
          'Updating project status',
          'Project status changed successfully',
          'Failed to update project status'
        );
        const updatedProject = await getProjectById(projectId);
        setProject(updatedProject);
      } catch (error) {
        console.error('Error updating project status:', error);
      }
    }
  };

  const confirmDeleteTask = (taskId: string) => {
    setTaskToDeleteId(taskId);
    setIsDeleteModalOpen(true);
  };

  const closeDeleteModal = () => {
    setIsDeleteModalOpen(false);
    setTaskToDeleteId(null);
  };

  if (!project) {
    return <div>Loading...</div>;
  }

  return (
    <div className="p-4">
      <h2 className="text-2xl mb-4">{project.name}</h2>
      <StatusDropdown currentStatus={project.status as Status} handleStatusChange={handleStatusChange} />
      <p className="mb-2">Created Time: {new Date(project.createdTime).toLocaleString()}</p>
      <h3 className="text-xl mt-4 mb-2">Tasks:</h3>
      <TaskList 
        project={project} 
        handleRemoveTask={confirmDeleteTask} 
        handleMarkTaskDone={handleMarkTaskDone} 
        handleOpenUpdateModal={handleOpenUpdateModal} 
      />
      <button
        onClick={() => setIsModalOpen(true)}
        className="bg-blue-500 text-white px-4 py-2 rounded mt-6"
      >
        Add Task
      </button>

      <TaskModal 
        title="Add New Task" 
        isOpen={isModalOpen} 
        onClose={() => setIsModalOpen(false)} 
        onSave={handleAddTask} 
      />

      <TaskModal 
        title="Update Task" 
        isOpen={isUpdateModalOpen} 
        onClose={handleCloseUpdateModal} 
        onSave={handleUpdateTask} 
        initialTask={updateTaskForm} 
      />

      <ConfirmationModal
        isOpen={isDeleteModalOpen}
        onClose={closeDeleteModal}
        onConfirm={() => taskToDeleteId && handleRemoveTask(taskToDeleteId)}
        title='Confirm Task Deletion'
        message="Are you sure you want to delete this task?"  />
    </div>
  );
};

export default ProjectDetailPage;
