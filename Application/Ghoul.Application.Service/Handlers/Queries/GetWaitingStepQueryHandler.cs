using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Ghoul.Application.Model.Build;
using Ghoul.Application.Model.Queries;
using Ghoul.Domain.Entity.Build;
using Ghoul.Persistence.Model;
using Ghoul.Persistence.Repository.Interface;
using MediatR;

namespace Ghoul.Application.Service.Handlers.Queries {
    public class GetWaitingStepQueryHandler : IRequestHandler<GetWaitingStepQuery, GetWaitingStepQueryResponse>
    {
        private readonly IMapper _mapper;
        private readonly IReadRepository<BuildPersistenceModel> _buildRepository;

        public GetWaitingStepQueryHandler(
            IMapper mapper,
            IReadRepository<BuildPersistenceModel> buildRepository
        )
        {
            _mapper = mapper;
            _buildRepository = buildRepository;
        }

        public Task<GetWaitingStepQueryResponse> Handle(GetWaitingStepQuery request, CancellationToken cancellationToken)
        {
            // Build retrieval
            var buildPersistenceModel = _buildRepository.Find(request.BuildID);
            if (buildPersistenceModel == null)
                throw new ArgumentException($"Build with id \"{request.BuildID}\" does not exist.");

            // Conversion
            var buildDomainEntity = _mapper.Map<BuildDomainEntity>(buildPersistenceModel);

            // Retrieval via business logic
            var buildStepContainer = buildDomainEntity.GetNextWaitingStep();
            if (buildStepContainer == null)
                return Task.FromResult<GetWaitingStepQueryResponse>(null);

            // Conversion to application model
            var stepPersistenceModel = _mapper.Map<BuildStepPersistenceModel>(buildStepContainer.Step);
            var stepApplicationModel = _mapper.Map<BuildStepApplicationModel>(stepPersistenceModel);
            return Task.FromResult(new GetWaitingStepQueryResponse(stepApplicationModel, buildStepContainer.Index));
        }
    }
}