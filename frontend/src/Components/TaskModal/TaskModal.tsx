import React, { useState, useEffect } from 'react';
import Modal from '../../Components/Modal/Modal';
import { Priority, PriorityLabels } from '../../Enums/PriorityEnum';

interface TaskModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSave: (task: { title: string; description: string; priority: Priority }) => void;
  initialTask?: { title: string; description: string; priority: Priority };
  title: string;
}

const TaskModal: React.FC<TaskModalProps> = ({ isOpen, onClose, onSave, initialTask, title }) => {
  const [task, setTask] = useState({ title: '', description: '', priority: Priority.Low });

  useEffect(() => {
    if (initialTask) {
      setTask(initialTask); 
    } else {
      setTask({ title: '', description: '', priority: Priority.Low });
    }
  }, [isOpen, initialTask]);

  const handleSave = () => {
    onSave(task);
    onClose();
  };

  return (
    <Modal title={title} isOpen={isOpen} onClose={onClose}>
      <input
        type="text"
        placeholder="Title"
        value={task.title}
        onChange={(e) => setTask({ ...task, title: e.target.value })}
        className="border px-2 py-1 mr-2"
      />
      <input
        type="text"
        placeholder="Description"
        value={task.description}
        onChange={(e) => setTask({ ...task, description: e.target.value })}
        className="border px-2 py-1 mr-2"
      />
      <select
        value={task.priority}
        onChange={(e) => setTask({ ...task, priority: parseInt(e.target.value) })}
        className="border px-2 py-1 mr-2"
      >
        {Object.keys(PriorityLabels).map((key) => (
          <option key={key} value={key}>
            {PriorityLabels[parseInt(key) as Priority]}
          </option>
        ))}
      </select>
      <button
        onClick={handleSave}
        className="bg-blue-500 text-white px-4 py-2 rounded mt-2"
      >
        Save
      </button>
    </Modal>
  );
};

export default TaskModal;
    