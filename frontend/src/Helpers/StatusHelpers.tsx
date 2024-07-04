import { Status } from '../Enums/StatusEnum';

export const getStatusLabel = (status: Status): string => {
    switch (status) {
        case Status.InProgress:
            return 'In Progress';
        case Status.Finished:
            return 'Finished';
        case Status.Canceled:
            return 'Canceled';
        case Status.Deferred:
            return 'Deferred';
        default:
            return 'Unknown';
    }
};
