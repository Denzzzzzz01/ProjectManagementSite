import React, { useEffect, useState } from 'react';
import { useAuth } from '../../Context/useAuth';
import { getUserProjects, deleteProject, updateProjectStatus } from '../../Services/ProjectService';
import { ProjectVm } from '../../Models/ProjectVm';
import { Status } from '../../Enums/StatusEnum';
import { getStatusLabel } from '../../Helpers/StatusHelpers';
import CreateProjectModal from '../CreateProjectModal/CreateProjectModal';
import UpdateProjectModal from '../UpdateProjectModal/UpdateProjectModal';
import { Link } from 'react-router-dom';

const Projects: React.FC = () => {
  const [projects, setProjects] = useState<ProjectVm[]>([]);
  const { isLoggedIn, token } = useAuth();
  const [selectedProject, setSelectedProject] = useState<ProjectVm | null>(null);
  const [showStatusDropdown, setShowStatusDropdown] = useState(false);

  useEffect(() => {
    if (isLoggedIn() && token) {
      getUserProjects()
        .then((projects) => setProjects(projects))
        .catch((error) => console.error('Error fetching projects:', error));
    }
  }, [isLoggedIn, token]);

  const handleDeleteProject = async (projectId: string) => {
    try {
      await deleteProject(projectId);
      setProjects(prevProjects => prevProjects.filter(project => project.id !== projectId));
    } catch (error) {
      console.error('Error deleting project:', error);
    }
  };

  const handleOpenUpdateModal = (project: ProjectVm) => {
    setSelectedProject(project);
  };

  const handleCloseUpdateModal = () => {
    setSelectedProject(null);
  };

  const handleStatusClick = (project: ProjectVm) => {
    setSelectedProject(project);
    setShowStatusDropdown(true);
  };

  const handleSelectStatus = async (status: Status) => {
    if (selectedProject) {
      try {
        await updateProjectStatus(selectedProject.id, status);
        setProjects(prevProjects =>
          prevProjects.map(proj =>
            proj.id === selectedProject.id ? { ...proj, status } : proj
          )
        );
        setShowStatusDropdown(false);
        setSelectedProject(null);
      } catch (error) {
        console.error('Error updating project status:', error);
      }
    }
  };

  return (
    <div className="p-4">
      <h2 className="text-2xl mb-4">Your Projects</h2>
      <ul className="list-disc pl-5">
        {projects.map((project) => (
          <li key={project.id} className="mb-2 flex items-center">
            <Link to={`/project/${project.id}`} className="flex-grow">
              {project.name}
            </Link>
            <span className="ml-4 cursor-pointer" onClick={() => handleStatusClick(project)}>
              {getStatusLabel(project.status as Status)}
            </span>
            {showStatusDropdown && selectedProject?.id === project.id && (
              <div className="absolute mt-2 bg-white shadow-lg rounded-md py-2 w-36 z-10">
                {Object.values(Status).map((value) => (
                  typeof value === 'number' && (
                    <button
                      key={value}
                      onClick={() => handleSelectStatus(value as Status)}
                      className="block w-full text-left px-4 py-2 text-sm hover:bg-gray-100"
                    >
                      {getStatusLabel(value as Status)}
                    </button>
                  )
                ))}
              </div>
            )}
            <button
              onClick={() => handleOpenUpdateModal(project)}
              className="ml-4 bg-yellow-500 text-white px-2 py-1 rounded hover:bg-yellow-600"
            >
              Update
            </button>
            <button
              onClick={() => handleDeleteProject(project.id)}
              className="ml-4 bg-red-500 text-white px-2 py-1 rounded hover:bg-red-600"
            >
              Delete
            </button>
          </li>
        ))}
      </ul>

      <CreateProjectModal setProjects={setProjects} />

      {selectedProject && (
        <UpdateProjectModal
          project={selectedProject}
          setProjects={setProjects}
          closeModal={handleCloseUpdateModal}
        />
      )}
    </div>
  );
};

export default Projects;
