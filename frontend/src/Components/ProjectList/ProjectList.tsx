import React from 'react';
import { Link } from 'react-router-dom';
import { ProjectVm } from '../../Models/ProjectVm';
import { Status } from '../../Enums/StatusEnum';
import { getStatusLabel } from '../../Helpers/StatusHelpers';

interface ProjectListProps {
  projects: ProjectVm[];
  onEdit: (project: ProjectVm) => void;
  onDelete: (projectId: string) => void;
}

const ProjectList: React.FC<ProjectListProps> = ({ projects, onEdit, onDelete }) => {
  return (
    <ul className="list-disc pl-5">
      {projects.map((project) => (
        <li key={project.id} className="mb-2 flex items-center">
          <Link to={`/project/${project.id}`} className="flex-grow">
            {project.name}
          </Link>
          <span className="ml-4 cursor-pointer">
            {getStatusLabel(project.status as Status)}
          </span>
          <button
            onClick={() => onEdit(project)}
            className="ml-4 bg-yellow-500 text-white px-2 py-1 rounded hover:bg-yellow-600"
          >
            Update
          </button>
          <button
            onClick={() => onDelete(project.id)}
            className="ml-4 bg-red-500 text-white px-2 py-1 rounded hover:bg-red-600"
          >
            Delete
          </button>
        </li>
      ))}
    </ul>
  );
};

export default ProjectList;
