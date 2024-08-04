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

  const handleSaveProject = async (project: { name: string }) => {
    if (selectedProject) {
      try {
        await toastPromise(
          updateProject(selectedProject.id, project.name),
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
          createProject(project.name),
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

  // const handleEditProject = (project: ProjectVm) => {
  //   setSelectedProject(project);
  // };

  // const confirmDeleteProject = (projectId: string) => {
  //   setProjectToDelete(projectId);
  //   setIsConfirmModalOpen(true);
  // };

  const closeConfirmModal = () => {
    setIsConfirmModalOpen(false);
    setProjectToDelete(null);
  };

  return (
    <div className="p-4">
      <h2 className="text-2xl mb-4">Your Projects</h2>
      <ProjectList projects={projects} />

      <button
        onClick={() => setIsCreateModalOpen(true)}
        className="bg-blue-500 text-white px-4 py-2 rounded hover:bg-blue-600"
      >
        Create Project
      </button>

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
        initialProject={selectedProject ? { name: selectedProject.name } : undefined}
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
