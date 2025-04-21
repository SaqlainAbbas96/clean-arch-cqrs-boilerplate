using Domain.Entities;
using Domain.Interfaces;

namespace Application.Commands
{
    public class CreateTaskHandler
    {
        private readonly ITaskRepository _repository;

        public CreateTaskHandler(ITaskRepository repository)
        {
            _repository = repository;
        }

        public async Task<CreateTaskResponse> Handle(CreateTaskCommand command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));
 
            // Manually map CreateTaskCommand to TaskItem
            var task = new TaskItem
            {
                Title = command.Title ?? string.Empty, // Ensure Title is never null
                Description = command.Description
            };

            // Save the task to the repository
            await _repository.AddAsync(task);

            // Return a response
            return new CreateTaskResponse
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description
            };
        }
    }
}
