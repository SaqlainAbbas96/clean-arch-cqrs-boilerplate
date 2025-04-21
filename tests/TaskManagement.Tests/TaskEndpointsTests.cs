using Application.Commands;
using Domain.Entities;
using Domain.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Net;
using System.Net.Http.Json;

namespace TaskManagement.Tests
{
    public class TaskEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly Mock<ITaskRepository> _mockRepo;

        public TaskEndpointsTests(WebApplicationFactory<Program> factory)
        {
            // Create a mock of the repository
            _mockRepo = new Mock<ITaskRepository>();

            // Use the factory to set up a custom WebHostBuilder with the mock repository
            var testFactory = factory.WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Testing");
                builder.ConfigureServices(services =>
                {
                    // Replace the actual repository with the mock
                    services.AddSingleton(_mockRepo.Object);
                });
            });

            _client = testFactory.CreateClient();
        }

        [Fact]
        public async Task Post_ValidTask_ShouldReturn201()
        {
            // Arrange
            var request = new CreateTaskCommand { Title = "My Task", Description = "Task Description" };

            // Mock the repository call - Assume the task is added and assigned an ID
            _mockRepo.Setup(r => r.AddAsync(It.IsAny<TaskItem>()))
                     .Callback<TaskItem>(task => task.Id = Guid.NewGuid())
                     .Returns(Task.CompletedTask);

            // Act
            var response = await _client.PostAsJsonAsync("/api/tasks", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var responseData = await response.Content.ReadFromJsonAsync<CreateTaskResponse>();
            responseData.Should().NotBeNull();
            responseData.Title.Should().Be("My Task");
        }

        [Fact]
        public async Task Post_InvalidTask_ShouldReturn400()
        {
            // Arrange
            var request = new CreateTaskCommand { Title = "", Description = "" };

            // Act
            var response = await _client.PostAsJsonAsync("/api/tasks", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetAllTasks_ShouldReturn200()
        {
            // Arrange
            var mockTasks = new List<TaskDto>
    {
        new TaskDto { Id = Guid.NewGuid(), Title = "Task 1", Description = "Task Description 1" },
        new TaskDto { Id = Guid.NewGuid(), Title = "Task 2", Description = "Task Description 2" }
    };

            // Make sure you're mocking the method that returns Task<List<TaskDto>>
            _mockRepo.Setup(r => r.GetAllAsync())
         .ReturnsAsync(mockTasks.Select(task => new TaskItem
         {
             Id = task.Id,
             Title = task.Title,
             Description = task.Description
         }).ToList());

            // Act
            var response = await _client.GetAsync("/api/tasks");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseData = await response.Content.ReadFromJsonAsync<List<TaskDto>>();
            responseData.Should().NotBeNull();
            responseData.Count.Should().Be(2); // Ensure that two tasks are returned
        }
    }
}
