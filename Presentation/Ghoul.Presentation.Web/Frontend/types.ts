export type TBuild = {
    _notFound?: boolean;
    id: string;
    name: string;
    repositoryURL?: string;
    repositoryEvent?: string;
    steps: TStep[];
}

export const getUndefinedBuild = (id: string): TBuild => ({
    _notFound: true,
    id,
    name: 'undefined',
    steps: []
})

export type TStep = {
    name: string;
    commandExecutable?: string;
    commandArguments?: string;
    environmentVariables?: string[];
    workingDirectory?: string;
    fireAndForget?: boolean;
    haltOnError?: boolean;
}