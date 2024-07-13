import React, { useState, useEffect } from 'react';
import Modal from '../../Components/Modal/Modal';
import { ProjectVm } from '../../Models/ProjectVm';

interface ProjectModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSave: (project: { name: string }) => void;
  initialProject?: { name: string };
  title: string;
}

const ProjectModal: React.FC<ProjectModalProps> = ({ isOpen, onClose, onSave, initialProject, title }) => {
  const [projectName, setProjectName] = useState('');

  useEffect(() => {
    if (initialProject) {
      setProjectName(initialProject.name); // Set project name only when the modal is opened
    } else {
      setProjectName(''); // Reset project name when modal is closed
    }
  }, [isOpen, initialProject]);

  const handleSave = () => {
    onSave({ name: projectName });
    onClose();
  };

  return (
    <Modal title={title} isOpen={isOpen} onClose={onClose}>
      <input
        type="text"
        placeholder="Project name"
        value={projectName}
        onChange={(e) => setProjectName(e.target.value)}
        className="border p-2 w-full mb-4 rounded"
      />
      <div className="flex justify-end">
        <button
          onClick={handleSave}
          className="bg-green-500 text-white px-4 py-2 rounded hover:bg-green-600 mr-2"
        >
          Save
        </button>
        <button
          onClick={onClose}
          className="bg-red-500 text-white px-4 py-2 rounded hover:bg-red-600"
        >
          Cancel
        </button>
      </div>
    </Modal>
  );
};

export default ProjectModal;
