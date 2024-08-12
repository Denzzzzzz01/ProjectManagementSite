import React from 'react';
import { Link } from 'react-router-dom';
import { ProjectVm } from '../../Models/ProjectVm';
import { Status } from '../../Enums/StatusEnum';
import { getStatusLabel } from '../../Helpers/StatusHelpers';
import '@flaticon/flaticon-uicons/css/all/all.css';
import './ProjectList.css';

interface ProjectListProps {
  projects: ProjectVm[];
}

const ProjectList: React.FC<ProjectListProps> = ({ projects }) => {
  return (
    <div className="max-h-96 overflow-y-auto pr-2 scrollbar-hide">
      <ul className="list-disc pl-5 pr-5">
        {projects.map((project) => (
          <li
            key={project.id}
            className="mb-2 mx-2 flex items-center bg-white  rounded shadow hover:shadow-lg transition-transform transform hover:scale-105"
          >
            <Link to={`/project/${project.id}`} className="p-4 flex-grow">
              {project.name}
              <p>{project.description}</p> 

              <span className="ml-4 cursor-pointer">
                {getStatusLabel(project.status as Status)}
              </span>
            </Link>
          </li>
        ))}
      </ul>
    </div>
  );
};

export default ProjectList;
