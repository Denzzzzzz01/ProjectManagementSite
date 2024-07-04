import React, { useEffect, useState } from "react";
import { useAuth } from "../../Context/useAuth";
import { getUserProjects } from "../../Services/ProjectService";
import { ProjectVm } from "../../Models/ProjectVm";
import { Status } from "../../Enums/StatusEnum";
import { getStatusLabel } from "../../Helpers/StatusHelpers";

const Projects: React.FC = () => {
  const [projects, setProjects] = useState<ProjectVm[]>([]);
  const { isLoggedIn, token } = useAuth();

  useEffect(() => {
    if (isLoggedIn() && token) {
      getUserProjects()
        .then((projects) => setProjects(projects))
        .catch((error) => console.error("Error fetching projects:", error));
    }
  }, [isLoggedIn, token]);

  return (
    <div>
      <h2>Your Projects</h2>
      <ul>
        {projects.map((project) => (
          <li key={project.id}>
            {project.name} - {getStatusLabel(project.status as Status)}
          </li>
        ))}
      </ul>
    </div>
  );
};

export default Projects;
