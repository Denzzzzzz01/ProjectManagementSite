import React, { useState } from 'react';
import { ProjectVm } from '../../Models/ProjectVm';
import { updateProject } from '../../Services/ProjectService';

interface UpdateProjectModalProps {
  project: ProjectVm;
  setProjects: React.Dispatch<React.SetStateAction<ProjectVm[]>>;
  closeModal: () => void;
}

const UpdateProjectModal: React.FC<UpdateProjectModalProps> = ({ project, setProjects, closeModal }) => {
  const [projectName, setProjectName] = useState(project.name);

  const handleUpdateProject = async () => {
    try {
      const updatedProject = await updateProject(project.id, projectName);
      setProjects(prevProjects =>
        prevProjects.map(p => p.id === updatedProject.id ? updatedProject : p)
      );
      closeModal();
      window.location.reload(); // Reload the page
    } catch (error) {
      console.error('Error updating project:', error);
    }
  };

  return (
    <div className="fixed inset-0 bg-gray-600 bg-opacity-50 flex justify-center items-center z-50">
      <div className="bg-white p-6 rounded shadow-lg max-w-sm w-full">
        <h2 className="text-xl mb-4">Update Project</h2>
        <input
          type="text"
          value={projectName}
          onChange={(e) => setProjectName(e.target.value)}
          placeholder="Project name"
          className="border p-2 w-full mb-4 rounded"
        />
        <div className="flex justify-end">
          <button
            onClick={handleUpdateProject}
            className="bg-green-500 text-white px-4 py-2 rounded hover:bg-green-600 mr-2"
          >
            Update
          </button>
          <button
            onClick={closeModal}
            className="bg-red-500 text-white px-4 py-2 rounded hover:bg-red-600"
          >
            Cancel
          </button>
        </div>
      </div>
    </div>
  );
};

export default UpdateProjectModal;
