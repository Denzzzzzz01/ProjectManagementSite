export enum Status {
    InProgress = 1,
    Finished = 2,
    Canceled = 3,
    Deferred = 4
};

export const StatusLabels = {
    [Status.InProgress]: 'In Progress',
    [Status.Finished]: 'Finished',
    [Status.Canceled]: 'Canceled',
    [Status.Deferred]: 'Deferred',
};