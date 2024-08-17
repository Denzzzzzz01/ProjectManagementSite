import React, { useState, useEffect } from 'react';
import { createPortal } from 'react-dom';
import { Link } from 'react-router-dom';
import { ProjectVm } from '../../Models/ProjectVm';
import { Status, StatusLabels } from '../../Enums/StatusEnum';
import { getStatusLabel } from '../../Helpers/StatusHelpers';
import '@flaticon/flaticon-uicons/css/all/all.css';
import './ProjectList.css';

interface ProjectListProps {
  projects: ProjectVm[];
}

const getStatusBgClass = (status: Status) => {
  switch (status) {
    case Status.InProgress:
      return 'bg-green-500';
    case Status.Finished:
      return 'bg-blue-400';
    case Status.Canceled:
      return 'bg-red-400';
    case Status.Deferred:
      return 'bg-yellow-400';
    default:
      return 'bg-gray-200';
  }
};

const ProjectList: React.FC<ProjectListProps> = ({ projects }) => {
  const [sortedProjects, setSortedProjects] = useState<ProjectVm[]>(projects);
  const [sortOption, setSortOption] = useState('date');
  const [sortOrder, setSortOrder] = useState('asc');
  const [statusFilter, setStatusFilter] = useState<Status | 'all'>('all');
  const [hoveredProject, setHoveredProject] = useState<{ id: string, position: { x: number, y: number } } | null>(null);

  useEffect(() => {
    const sorted = [...projects];
    const sortMultiplier = sortOrder === 'asc' ? 1 : -1;

    if (sortOption === 'date') {
      sorted.sort((a, b) => (new Date(b.createdTime).getTime() - new Date(a.createdTime).getTime()) * sortMultiplier);
    } else if (sortOption === 'name') {
      sorted.sort((a, b) => a.name.localeCompare(b.name) * sortMultiplier);
    }

    if (statusFilter === 'all') {
      setSortedProjects(sorted);
    } else {
      setSortedProjects(sorted.filter(project => project.status === statusFilter));
    }
  }, [projects, sortOption, sortOrder, statusFilter]);

  const handleMouseEnter = (projectId: string, event: React.MouseEvent) => {
    const rect = (event.target as HTMLElement).getBoundingClientRect();
    setHoveredProject({
      id: projectId,
      position: { x: rect.left + window.scrollX, y: rect.bottom + window.scrollY }
    });
  };

  const handleMouseLeave = () => {
    setHoveredProject(null);
  };

  const toggleSortOrder = () => {
    setSortOrder((prevOrder) => (prevOrder === 'asc' ? 'desc' : 'asc'));
  };

  return (
    <div className="flex flex-col justify-center items-center mt-6 max-h-[80%] overflow-y-auto scrollbar-hide">
      <div className="w-[35vh] mb-4 p-1 flex gap-6 items-center bg-gray-200 rounded text-lg shadow-md shadow-black">
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
          <select 
            className="bg-beige shadow-sm shadow-black hover:bg-beige-dark px-2 py-1 rounded cursor-pointer"
            onChange={(e) => setSortOption(e.target.value)}
            value={sortOption}
          >
            <option value="date">Date</option>
            <option value="name">Name</option>
          </select>
        </div>
        <div className="flex items-center">
          <select
            className="bg-beige shadow-sm shadow-black hover:bg-beige-dark px-2 py-1 rounded cursor-pointer"
            onChange={(e) => setStatusFilter(e.target.value === 'all' ? 'all' : Number(e.target.value) as Status)}
            value={statusFilter}
          >
            <option value="all">Show All</option>
            {Object.entries(StatusLabels).map(([statusValue, label]) => (
              <option key={statusValue} value={statusValue}>
                {label}
              </option>
            ))}
          </select>
        </div>
      </div>

      {sortedProjects.length === 0 ? (
        <div className="text-center text-gray-500">No projects found for the selected status.</div>
      ) : (
        <ul className="list-disc px-5 overflow-y-auto max-h-[500px] scrollbar-hide">
          {sortedProjects.map((project) => (
            <li
              key={project.id}
              className="w-[35vh] mb-4 mx-2 flex justify-center items-center bg-white rounded shadow-md shadow-black transition-transform transform 
                hover:scale-105 hover:shadow-lg hover:shadow-black relative"
            >
              <Link to={`/project/${project.id}`} className="p-4 flex-col items-center justify-center text-center relative">
                <div className="mb-3 flex items-center justify-center relative">
                  <h3 className="text-xl font-bold">{project.name}</h3>
                  {project.description && (
                    <i
                      className="fi fi-br-interrogation text-sm ml-2 cursor-pointer hover:scale-125 transition-transform"
                      onMouseEnter={(e) => handleMouseEnter(project.id, e)}
                      onMouseLeave={handleMouseLeave}
                    />
                  )}
                </div>
                <span className={`px-2 py-1 rounded shadow-sm shadow-black ${getStatusBgClass(project.status)}`}>
                  {getStatusLabel(project.status as Status)}
                </span>
              </Link>
            </li>
          ))}
        </ul>
      )}
      {hoveredProject &&
        createPortal(
          <div
            className="absolute p-2 bg-white border border-gray-300 rounded shadow-md z-50 w-64 text-sm text-left whitespace-pre-wrap break-words"
            style={{ top: `${hoveredProject.position.y}px`, left: `${hoveredProject.position.x}px` }}
          >
            {projects.find(p => p.id === hoveredProject.id)?.description}
          </div>,
          document.body
        )}
    </div>
  );
};

export default ProjectList;
