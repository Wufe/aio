using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Ghoul.Application.Model.Build;
using Ghoul.Application.Model.Queries;
using Ghoul.Domain.Service.Interface;
using Ghoul.Persistence.Model;
using MediatR;

namespace Ghoul.Application.Service.Handlers.Queries {
    public class GetIdleRunQueryHandler : IRequestHandler<GetIdleRunQuery, GetIdleRunQueryResponse>
    {
        private readonly IMapper _mapper;
        private readonly IBuildDomainService _buildService;

        public GetIdleRunQueryHandler(
            IMapper mapper,
            IBuildDomainService buildService
        )
        {
            _mapper = mapper;
            _buildService = buildService;
        }

        public Task<GetIdleRunQueryResponse> Handle(GetIdleRunQuery request, CancellationToken cancellationToken)
        {
            var buildRunContainer = _buildService.GetNextIdleRun();

            if (buildRunContainer == null)
                return Task.FromResult<GetIdleRunQueryResponse>(null);

            var buildPersistenceModel = _mapper.Map<BuildPersistenceModel>(buildRunContainer.Build);
            var runPersistenceModel = _mapper.Map<RunPersistenceModel>(buildRunContainer.Run);

            var buildApplicationModel = _mapper.Map<BuildApplicationModel>(buildPersistenceModel);
            var runApplicationModel = _mapper.Map<RunApplicationModel>(runPersistenceModel);

            return Task.FromResult(new GetIdleRunQueryResponse(buildApplicationModel, runApplicationModel));
        }
    }
}