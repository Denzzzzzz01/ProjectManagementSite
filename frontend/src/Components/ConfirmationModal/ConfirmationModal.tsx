import React from 'react';
import Modal from '../../Components/Modal/Modal';

interface ConfirmationModalProps {
  isOpen: boolean;
  onClose: () => void;
  onConfirm: () => void;
  title: string;
  message: string;
}

const ConfirmationModal: React.FC<ConfirmationModalProps> = ({ isOpen, onClose, onConfirm, title, message }) => {
  return (
    <Modal title={title} isOpen={isOpen} onClose={onClose}>
      <p>{message}</p>
      <div className="mt-4 flex justify-end">
        <button
          onClick={onClose}
          className="bg-gray-500 text-white px-4 py-2 rounded mr-2"
        >
          Cancel
        </button>
        <button
          onClick={onConfirm}
          className="bg-red-500 text-white px-4 py-2 rounded"
        >
          Confirm
        </button>
      </div>
    </Modal>
  );
};

export default ConfirmationModal;
