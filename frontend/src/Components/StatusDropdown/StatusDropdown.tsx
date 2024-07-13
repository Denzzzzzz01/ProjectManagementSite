import React, { useState } from 'react';
import { Status, StatusLabels } from '../../Enums/StatusEnum';
import { getStatusLabel } from '../../Helpers/StatusHelpers';

interface StatusDropdownProps {
  currentStatus: Status;
  handleStatusChange: (newStatus: Status) => void;
}

const StatusDropdown: React.FC<StatusDropdownProps> = ({ currentStatus, handleStatusChange }) => {
  const [isDropdownOpen, setIsDropdownOpen] = useState(false);

  const toggleDropdown = () => {
    setIsDropdownOpen(!isDropdownOpen);
  };

  const handleStatusSelect = (status: Status) => {
    handleStatusChange(status);
    setIsDropdownOpen(false); // Закрываем выпадающий список после выбора
  };

  return (
    <div className="relative mb-2">
      <span>Status: </span>
      <button
        onClick={toggleDropdown}
        className="bg-gray-200 px-2 py-1 rounded cursor-pointer"
      >
        {getStatusLabel(currentStatus)}
      </button>
      {isDropdownOpen && (
        <ul className="absolute bg-white border mt-1 rounded shadow-lg z-10">
          {Object.keys(Status).filter(key => !isNaN(Number(key))).map(key => (
            <li
              key={key}
              onClick={() => handleStatusSelect(Number(key) as Status)}
              className="px-4 py-2 cursor-pointer hover:bg-gray-200"
            >
              {StatusLabels[Number(key) as Status]}
            </li>
          ))}
        </ul>
      )}
    </div>
  );
};

export default StatusDropdown;
