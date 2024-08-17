import React, { useEffect, useState } from 'react';
import { useAuth } from '../../Context/useAuth';
import { getUserProjects, deleteProject, updateProject, createProject } from '../../Services/ProjectService';
import { ProjectVm } from '../../Models/ProjectVm';
import ConfirmationModal from '../../Components/ConfirmationModal/ConfirmationModal';
import ProjectList from '../ProjectList/ProjectList';
import ProjectModal from '../ProjectModal/ProjectModal';
import { notifyError, notifySuccess, toastPromise } from '../../Utils/toastUtils';
import '@flaticon/flaticon-uicons/css/all/all.css';


const Projects: React.FC = () => {
  const [projects, setProjects] = useState<ProjectVm[]>([]);
  const { isLoggedIn, token } = useAuth();
  const [selectedProject, setSelectedProject] = useState<ProjectVm | null>(null);
  const [isCreateModalOpen, setIsCreateModalOpen] = useState(false);
  const [isConfirmModalOpen, setIsConfirmModalOpen] = useState(false);
  const [projectToDelete, setProjectToDelete] = useState<string | null>(null);

  useEffect(() => {
    if (isLoggedIn() && token) {
      getUserProjects()
        .then((projects) => setProjects(projects))
        .catch((error) => console.error('Error fetching projects:', error));
    }
  }, [isLoggedIn, token]);

  const updateProjectList = async () => {
    if (isLoggedIn() && token) {
      try {
        const projects = await getUserProjects();
        setProjects(projects);
      } catch (error) {
        console.error('Error fetching projects:', error);
        notifyError('Error fetching projects');
      }
    }
  };

  const handleDeleteProject = async () => {
    if (projectToDelete) {
      try {
        await toastPromise(
          deleteProject(projectToDelete),
          'Deleting project',
          'Project deleted successfully!',
          'Failed to delete project'
        );
        setProjects(prevProjects => prevProjects.filter(project => project.id !== projectToDelete));
        setIsConfirmModalOpen(false);
        setProjectToDelete(null);
      } catch (error) {
        console.error('Error deleting project:', error);
      }
    }
  };

  const handleSaveProject = async (project: { name: string; description: string }) => {
    if (selectedProject) {
      try {
        await toastPromise(
          updateProject(selectedProject.id, project.name, project.description),
          'Updating project',
          'Project has been updated!',
          'Failed to update project'
        );
        setProjects(prevProjects =>
          prevProjects.map(p => p.id === selectedProject.id ? { ...p, name: project.name } : p)
        );
        setSelectedProject(null);
        updateProjectList();
      } catch (error) {
        console.error('Error updating project:', error);
      }
    } else {
      try {
        await toastPromise(
          createProject(project.name, project.description),
          'Creating project',
          'Project created successfully!',
          'Failed to create project'
        );
        setIsCreateModalOpen(false);
        updateProjectList();
      } catch (error) {
        console.error('Error creating project:', error);
      }
    }
  };

  const closeConfirmModal = () => {
    setIsConfirmModalOpen(false);
    setProjectToDelete(null);
  };

  return (
    <div key={token} className="p-4 font-sans max-w-md w-full max-h-[80vh] bg-black bg-opacity-25 rounded-lg shadow-lg scrollbar-hide">
      <div className='flex justify-center items-center'>
        <button
            onClick={() => setIsCreateModalOpen(true)}
            className="bg-beige shadow-md shadow-black rounded transform transition-transform duration-200 hover:bg-beige-dark active:bg-beige-light"
          >
            <img src="https://cdn-icons-png.flaticon.com/512/3917/3917177.png" draggable="false" width="56" height="56" className='hover:scale-110 active:scale-90'/>
          </button>
          <h2 className="text-4xl ml-2 text-beige font-bold">PROJECTS</h2>
      </div>
      
      <ProjectList projects={projects} />

      <ProjectModal
        isOpen={isCreateModalOpen}
        onClose={() => setIsCreateModalOpen(false)}
        onSave={handleSaveProject}
        title="Create Project"
      />

      <ProjectModal
        isOpen={!!selectedProject}
        onClose={() => setSelectedProject(null)}
        onSave={handleSaveProject}
        initialProject={selectedProject ? { name: selectedProject.name, description: selectedProject.description } : undefined}
        title="Update Project"
      />

      <ConfirmationModal
        isOpen={isConfirmModalOpen}
        onClose={closeConfirmModal}
        onConfirm={handleDeleteProject}
        title="Confirm Project Deletion"
        message="Are you sure you want to delete this project?"
      />
    </div>
  );
};

export default Projects;
