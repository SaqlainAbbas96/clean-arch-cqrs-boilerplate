using Application.Commands;
using Domain.Entities;
using Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace TaskManagement.Tests;

public class CreateTaskHandlerTests
{
    private readonly Mock<ITaskRepository> _mockRepo;
    private readonly CreateTaskHandler _handler;

    public CreateTaskHandlerTests()
    {
        _mockRepo = new Mock<ITaskRepository>();
        _handler = new CreateTaskHandler(_mockRepo.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldReturnResponse()
    {
        // Arrange
        var fakeId = Guid.NewGuid();
        var command = new CreateTaskCommand { Title = "New Task", Description = "Desc" };

        _mockRepo.Setup(r => r.AddAsync(It.IsAny<TaskItem>()))
                 .Callback<TaskItem>(task => task.Id = fakeId)
                 .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(fakeId);
        result.Title.Should().Be(command.Title);
    }

    [Fact]
    public async Task Handle_EmptyTitle_ShouldStillReturnResponse()
    {
        // Arrange
        var command = new CreateTaskCommand { Title = "", Description = "Something" };

        // Mock the repository to return a task with a valid Id
        _mockRepo.Setup(r => r.AddAsync(It.IsAny<TaskItem>()))
                 .Callback<TaskItem>(task => task.Id = Guid.NewGuid())
                 .Returns(Task.CompletedTask); // Simulate a successful task addition

        // Act
        var result = await _handler.Handle(command);

        // Assert
        result.Should().NotBeNull(); // Ensure the result is not null
        result.Title.Should().Be(""); // Ensure the title is still an empty string as expected
    }

    [Fact]
    public async Task Handle_NullCommand_ShouldThrowException()
    {
        Func<Task> act = async () => await _handler.Handle(null);
        await act.Should().ThrowAsync<ArgumentNullException>();
    }
}
