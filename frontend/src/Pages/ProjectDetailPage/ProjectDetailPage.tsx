import React, { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { getProjectById, updateProjectStatus, deleteProject, updateProject } from '../../Services/ProjectService';
import { DetailedProject } from '../../Models/DetailedProject';
import { Status } from '../../Enums/StatusEnum';
import { Priority } from '../../Enums/PriorityEnum';
import TaskList from '../../Components/TaskList/TaskList';
import StatusDropdown from '../../Components/StatusDropdown/StatusDropdown';
import TaskModal from '../../Components/TaskModal/TaskModal';
import { UpdateTaskDto } from '../../Models/UpdateTaskDto';
import { addTask, markTaskDone, removeTask, updateTask } from '../../Services/TaskService';
import ConfirmationModal from '../../Components/ConfirmationModal/ConfirmationModal';
import ProjectModal from '../../Components/ProjectModal/ProjectModal';
import { toastPromise } from '../../Utils/toastUtils';
import '@flaticon/flaticon-uicons/css/all/all.css';
import ProjectMembersList from '../../Components/ProjectMembersList/ProjectMembersList';
import UserSearchModal from '../../Components/UserSearchModal/UserSearchModal';
const ProjectDetailPage: React.FC = () => {
  const { projectId } = useParams<{ projectId: string }>();
  const navigate = useNavigate();
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
  const [isDeleteProjectModalOpen, setIsDeleteProjectModalOpen] = useState(false);
  const [isUpdateProjectModalOpen, setIsUpdateProjectModalOpen] = useState(false);

  const [isUserSearchModalOpen, setIsUserSearchModalOpen] = useState(false);  

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

  const handleDeleteProject = async () => {
    if (projectId) {
      try {
        await toastPromise(
          deleteProject(projectId),
          'Deleting project',
          'Project deleted successfully!',
          'Failed to delete project'
        );
        navigate('/');
      } catch (error) {
        console.error('Error deleting project:', error);
      }
    }
  };

  const confirmDeleteProject = () => {
    setIsDeleteProjectModalOpen(true);
  };

  const closeDeleteProjectModal = () => {
    setIsDeleteProjectModalOpen(false);
  };

  const handleUpdateProject = async (project: { name: string, description: string }) => {
    if (projectId) {
      try {
        await toastPromise(
          updateProject(projectId, project.name, project.description),
          'Updating project',
          'Project updated successfully!',
          'Failed to update project'
        );
        const updatedProject = await getProjectById(projectId);
        setProject(updatedProject);
        setIsUpdateProjectModalOpen(false);
      } catch (error) {
        console.error('Error updating project:', error);
      }
    }
  };

  
  const closeUserSearchModal = () => {
    setIsUserSearchModalOpen(false);
  };

  if (!project) {
    return <div>Loading...</div>;
  }

  return (
    <div className="flex flex-col lg:flex-row gap-24 w-full max-w-[70%] h-[80vh] font-sans">
      <div className="flex flex-col items-center justify-center h-min w-auto lg:w-[50%] bg-white p-4 rounded shadow">
        <div className='flex items-start justify-between w-[100%]'>
          <h2 className="text-4xl font-bold">{project.name}</h2>
          <StatusDropdown currentStatus={project.status as Status} handleStatusChange={handleStatusChange}/>
        </div>
        <p className="mt-6">Created Time: {new Date(project.createdTime).toLocaleString()}</p>
        <p className="mt-4">{project.description}</p>
        
        <button
          onClick={() => setIsUserSearchModalOpen(true)}
          className="bg-green-500 text-white px-4 py-2 rounded hover:bg-green-600"
        >
          Add Users
        </button>

        <UserSearchModal
          isOpen={isUserSearchModalOpen}
          onClose={closeUserSearchModal}
          projectId={projectId || ''}
        />
        {projectId && <ProjectMembersList projectId={projectId} />}

        <div className="w-[100%] flex justify-end mt-8">
          <button
            onClick={() => setIsUpdateProjectModalOpen(true)}
            className="bg-beige text-white px-4 py-2 rounded shadow-gray-700 shadow-sm mr-2 transform transition-transform duration-200 active:shadow-inner active:shadow-gray-700 active:pt-2.5 active:pb-0.5 hover:bg-beige-dark"
          >
            <i className="fi fi-rs-pencil"></i>
          </button>
          <button
            onClick={confirmDeleteProject}
            className="bg-beige text-white px-4 py-2 rounded shadow-gray-700 shadow-sm transform transition-transform duration-200 active:shadow-inner active:shadow-gray-700 active:pt-2.5 active:pb-0.5 hover:bg-beige-dark"
          >
            <i className="fi fi-rs-trash"></i>
          </button>
        </div>
      </div>

      <div className="w-full lg:w-[50%] p-4 h-full max-h-full overflow-y-auto bg-black bg-opacity-25 rounded-lg shadow-lg scrollbar-hide">
        <h1 className="font-bold mb-6 text-center text-gray-200 flex justify-center items-center">
          <button
            onClick={() => setIsModalOpen(true)}
            className="bg-beige mr-2 shadow-md shadow-black rounded transform transition-transform duration-200 hover:bg-beige-dark active:bg-beige-light"
          >
            <img src="https://cdn-icons-png.flaticon.com/512/3917/3917177.png" draggable="false" width="56" height="56" className='hover:scale-110 active:scale-90'/>
          </button>
          <p className='text-4xl'>TASKS</p>
        </h1>
        <TaskList
        className="h-[85%] scrollbar-hide" 
          project={project}
          handleRemoveTask={confirmDeleteTask}
          handleMarkTaskDone={handleMarkTaskDone}
          handleOpenUpdateModal={handleOpenUpdateModal}
        />
        <div className="flex justify-end mt-6"></div>
      </div>

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

      <ProjectModal
        title="Update Project"
        isOpen={isUpdateProjectModalOpen}
        onClose={() => setIsUpdateProjectModalOpen(false)}
        onSave={handleUpdateProject}
        initialProject={{ name: project.name, description: project.description }}
      />

      <ConfirmationModal
        isOpen={isDeleteModalOpen}
        onClose={closeDeleteModal}
        onConfirm={() => taskToDeleteId && handleRemoveTask(taskToDeleteId)}
        title="Confirm Task Deletion"
        message="Are you sure you want to delete this task?"
      />

      <ConfirmationModal
        isOpen={isDeleteProjectModalOpen}
        onClose={closeDeleteProjectModal}
        onConfirm={handleDeleteProject}
        title="Confirm Project Deletion"
        message="Are you sure you want to delete this project?"
      />
    </div>
  );
};

export default ProjectDetailPage;