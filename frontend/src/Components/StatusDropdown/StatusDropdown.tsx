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
    setIsDropdownOpen(false); 
  };

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

  return (
    <div className="relative mb-2 text-2xl shadow-black shadow-sm rounded" >
      <button
        onClick={toggleDropdown}
        className={`px-2 py-1 rounded cursor-pointer hover:bg-opacity-70 ${getStatusBgClass(currentStatus)}`}
      >
        {getStatusLabel(currentStatus)}
      </button>
      {isDropdownOpen && (
        <ul className="absolute bg-beige border mt-1 rounded shadow-md shadow-black z-10">
          {Object.keys(Status).filter(key => !isNaN(Number(key))).map(key => {
            const status = Number(key) as Status;
            return (
              <li
                key={key}
                onClick={() => handleStatusSelect(status)}
                className={`px-4 py-2 cursor-pointer hover:bg-beige bg-gray-200`}
              >
                {StatusLabels[status]}
              </li>
            );
          })}
        </ul>
      )}
    </div>
  );
};

export default StatusDropdown;
