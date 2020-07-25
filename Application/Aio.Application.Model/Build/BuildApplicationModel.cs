using System;
using System.Collections.Generic;

namespace Aio.Application.Model.Build {
    public class BuildApplicationModel {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string RepositoryURL { get; set; }
        public string RepositoryTrigger { get; set; }
        public int? Order { get; private set; }
        public IEnumerable<BuildStepApplicationModel> Steps { get; set; }

        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }
    }
}