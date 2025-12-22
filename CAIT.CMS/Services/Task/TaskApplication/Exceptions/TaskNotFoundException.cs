using BuildingBlocks.Shared.Exceptions;

namespace TaskApplication.Exceptions
{
    public class TaskNotFoundException : NotFoundException
    {
        public TaskNotFoundException(Guid id) : base("Task", id)
        {
        }
    }
}
