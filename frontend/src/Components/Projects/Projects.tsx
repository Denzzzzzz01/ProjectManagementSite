import React, { useEffect, useState } from 'react';
import { useAuth } from '../../Context/useAuth';
import { getUserProjects, deleteProject } from '../../Services/ProjectService';
import { ProjectVm } from '../../Models/ProjectVm';
import { Status } from '../../Enums/StatusEnum';
import { getStatusLabel } from '../../Helpers/StatusHelpers';
import CreateProjectModal from '../CreateProjectModal/CreateProjectModal';

const Projects: React.FC = () => {
  const [projects, setProjects] = useState<ProjectVm[]>([]);
  const { isLoggedIn, token } = useAuth();

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

  return (
    <div className="p-4">
      <h2 className="text-2xl mb-4">Your Projects</h2>
      <ul className="list-disc pl-5">
        {projects.map((project) => (
          <li key={project.id} className="mb-2 flex items-center">
            <span className="flex-grow">
              {project.name} - {getStatusLabel(project.status as Status)}
            </span>
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
    </div>
  );
};

export default Projects;
