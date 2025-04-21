using Application.Commands;
using Domain.Entities;
using Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace TaskManagement.Tests
{
    public class GetAllTasksHandlerTests
    {
        private readonly Mock<ITaskRepository> _mockRepo;
        private readonly GetAllTasksHandler _handler;

        public GetAllTasksHandlerTests()
        {
            _mockRepo = new Mock<ITaskRepository>();
            _handler = new GetAllTasksHandler(_mockRepo.Object);
        }

        [Fact]
        public async Task Handle_ReturnsListOfTaskDtos()
        {
            // Arrange
            var data = new List<TaskItem>
            {
                new TaskItem { Id = Guid.NewGuid(), Title = "Task 1", Description = "Desc 1" },
                new TaskItem { Id = Guid.NewGuid(), Title = "Task 2", Description = "Desc 2" }
            };

            _mockRepo.Setup(x => x.GetAllAsync()).ReturnsAsync(data);

            // Act
            var result = await _handler.Handle();

            // Assert
            result.Should().HaveCount(2);
            result[0].Title.Should().Be("Task 1");
        }

        [Fact]
        public async Task Handle_EmptyList_ReturnsEmpty()
        {
            _mockRepo.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<TaskItem>());
            var result = await _handler.Handle();
            result.Should().BeEmpty();
        }
    }
}
