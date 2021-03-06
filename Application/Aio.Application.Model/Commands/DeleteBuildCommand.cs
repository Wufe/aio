using MediatR;

namespace Aio.Application.Model.Commands {
    public class DeleteBuildCommand : IRequest {
        public string ID { get; private set; }

        public DeleteBuildCommand(string id)
        {
            ID = id;
        }
    }
}