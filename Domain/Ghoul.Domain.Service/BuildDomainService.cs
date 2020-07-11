using Ghoul.Domain.Entity;
using Ghoul.Domain.Service.Interface;

namespace Ghoul.Domain.Service {
    public class BuildDomainService : IBuildDomainService
    {
        public BuildDomainEntity CreateBuild(string name)
        {
            return BuildDomainEntity.Build(name);
        }
    }
}