using Domain.Interfaces;

namespace Application.Commands
{
    public class GetAllTasksHandler
    {
        private readonly ITaskRepository _repository;

        public GetAllTasksHandler(ITaskRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<TaskDto>> Handle()
        {
            // Get all tasks from the repository
            var tasks = await _repository.GetAllAsync();

            // Manually map TaskItem to TaskDto
            var taskDtos = tasks.Select(task => new TaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description
            }).ToList();

            return taskDtos;
        }
    }
}
