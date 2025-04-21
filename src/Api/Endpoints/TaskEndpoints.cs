
using Application.Commands;

namespace Api.Endpoints
{
    public static class TaskEndpoints
    {
        public static void MapTaskEndpoints(this WebApplication app)
        {
            // POST endpoint for creating a task
            app.MapPost("/api/tasks", async (CreateTaskCommand command, CreateTaskHandler handler, CreateTaskValidator validator) =>
            {
                var validationResult = await validator.ValidateAsync(command);
                if (!validationResult.IsValid)
                    return Results.BadRequest(validationResult.Errors);

                var response = await handler.Handle(command);
                return Results.Created($"/api/tasks/{response.Id}", response);
            });

            // GET endpoint for retrieving tasks
            app.MapGet("/api/tasks", async (GetAllTasksHandler handler) =>
            {
                var response = await handler.Handle();
                return Results.Ok(response);
            });
        }
    }
}
