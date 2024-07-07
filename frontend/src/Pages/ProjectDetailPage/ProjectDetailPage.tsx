import React, { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import { getProjectById } from '../../Services/ProjectService';
import { DetailedProject } from '../../Models/DetailedProject';
import { Status } from '../../Enums/StatusEnum';
import { Priority, PriorityLabels } from '../../Enums/PriorityEnum';
import { getStatusLabel } from '../../Helpers/StatusHelpers';

const ProjectDetailPage: React.FC = () => {
  const { projectId } = useParams<{ projectId: string }>();
  const [project, setProject] = useState<DetailedProject | null>(null);

  useEffect(() => {
    if (projectId) {
      getProjectById(projectId)
        .then((project) => setProject(project))
        .catch((error) => console.error('Error fetching project:', error));
    }
  }, [projectId]);

  if (!project) {
    return <div>Loading...</div>;
  }

  return (
    <div className="p-4">
      <h2 className="text-2xl mb-4">{project.name}</h2>
      <p className="mb-2">Status: {getStatusLabel(project.status as Status)}</p>
      <p className="mb-2">Created Time: {new Date(project.createdTime).toLocaleString()}</p>
      <h3 className="text-xl mt-4 mb-2">Tasks:</h3>
      <ul className="list-disc pl-5">
        {project.tasks.map((task) => (
          <li key={task.id}>
            <div className="mb-2">
              <strong>{task.title}</strong>
              <p>Added Time: {new Date(task.addedTime).toLocaleString()}</p>
              <p>Priority: {PriorityLabels[task.priority]}</p>
              <p>Status: {task.isDone ? 'Done' : 'Not Done'}</p>
            </div>
          </li>
        ))}
      </ul>
    </div>
  );
};

export default ProjectDetailPage;
