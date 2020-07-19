using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Ghoul.Application.Model.Commands;
using Ghoul.Domain.Entity.Build;
using Ghoul.Domain.Service.Interface;
using Ghoul.Persistence.Model;
using Ghoul.Persistence.Repository.Interface;
using MediatR;

namespace Ghoul.Application.Service.Handlers.Commands {
    public class StopBuildRunCommandHandler : IRequestHandler<StopBuildRunCommand>
    {
        private readonly IMapper _mapper;
        private readonly IRepository<BuildPersistenceModel> _buildRepository;
        private readonly IRepository<RunPersistenceModel> _runRepository;
        private readonly IBuildDomainService _buildService;

        public StopBuildRunCommandHandler(
            IMapper mapper,
            IRepository<BuildPersistenceModel> buildRepository,
            IRepository<RunPersistenceModel> runRepository,
            IBuildDomainService buildService
        )
        {
            _mapper = mapper;
            _buildRepository = buildRepository;
            _runRepository = runRepository;
            _buildService = buildService;
        }

        public Task<Unit> Handle(StopBuildRunCommand request, CancellationToken cancellationToken)
        {
            // Retrieval
            var buildPersistenceModel = _buildRepository.Find(request.BuildID);
            if (buildPersistenceModel == null)
                throw new ArgumentException($"Build with id \"{request.BuildID}\" does not exist.");
            var runPersistenceModel = _runRepository.Find(request.RunID);
            if (runPersistenceModel == null)
                throw new ArgumentException($"Run with id \"{request.RunID}\" does not exist.");

            // Conversion to domain entity
            var buildDomainEntity = _mapper.Map<BuildDomainEntity>(buildPersistenceModel);
            var runDomainEntity = _mapper.Map<RunDomainEntity>(runPersistenceModel);

            // Stop run via domain logic
            var buildRunContainer = _buildService.StopBuildRun(buildDomainEntity, runDomainEntity);

            // Conversion to persistence model
            buildPersistenceModel = _mapper.Map<BuildPersistenceModel>(buildRunContainer.Build);
            runPersistenceModel = _mapper.Map<RunPersistenceModel>(buildRunContainer.Run);

            // Persist
            _buildRepository.Update(buildPersistenceModel);
            _runRepository.Update(runPersistenceModel);

            return Task.FromResult(new Unit());
        }
    }
}