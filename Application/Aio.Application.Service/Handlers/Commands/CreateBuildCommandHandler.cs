using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Aio.Application.Model.Commands;
using Aio.Domain.Service.Interface;
using Aio.Persistence.Model;
using Aio.Persistence.Repository.Interface;
using MediatR;

namespace Aio.Application.Service.Handlers.Commands {
    public class CreateBuildCommandHandler : IRequestHandler<CreateBuildCommand, string>
    {
        private readonly IMapper _mapper;
        private readonly IBuildDomainService _buildService;
        private readonly IRepository<BuildPersistenceModel> _buildRepository;

        public CreateBuildCommandHandler(
            IMapper mapper,
            IBuildDomainService buildService,
            IRepository<BuildPersistenceModel> buildRepository)
        {
            _mapper = mapper;
            _buildService = buildService;
            _buildRepository = buildRepository;
        }

        public Task<string> Handle(CreateBuildCommand request, CancellationToken cancellationToken)
        {
            // Create domain entity
            var buildDomainEntity = _buildService.CreateBuild(request.Name);
            if (request.RepositoryURL != null)
                buildDomainEntity.SetRepository(request.RepositoryURL);

            // Convert domain entity to persistence model
            var buildPersistenceModel = _mapper.Map<BuildPersistenceModel>(buildDomainEntity);

            // Store persistence model
            _buildRepository.Insert(buildPersistenceModel);
            return Task.FromResult(buildPersistenceModel.ID);
        }
    }
}