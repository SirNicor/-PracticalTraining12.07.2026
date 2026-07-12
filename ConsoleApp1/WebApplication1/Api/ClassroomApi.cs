using DomainCore;
using EFRepository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;  

namespace WebApplication1.Api;

public static class ClassroomApi
{
    public static void AddedApiClassroom(this WebApplication app)
    {
        app.MapGet("/api/classrooms", async ([FromServices]IClassroomRepository repository) =>
        {
            var classrooms = await repository.GetAllClassroomsAsync();
            return Results.Ok(classrooms ?? new List<Classroom>());
        });

        app.MapGet("/api/classrooms/{number}", async (string number, [FromServices]IClassroomRepository repository) =>
        {
            var classroom = await repository.GetClassroomForNumberAsync(number);
            return classroom == null ? Results.NotFound() : Results.Ok(classroom);
        });

        app.MapPost("/api/classrooms", async (Classroom classroom, [FromServices]IClassroomRepository repository) =>
        {
            if (classroom == null || string.IsNullOrWhiteSpace(classroom.NumberClassroom))
                return Results.BadRequest(new { message = "Номер аудитории обязателен" });

            var id = await repository.CreateAsync(classroom);
            if (id == null)
                return Results.Conflict(new { message = $"Аудитория '{classroom.NumberClassroom}' уже существует или произошла ошибка" });

            return Results.Created($"/api/classrooms/{id}", classroom);
        });

        app.MapPut("/api/classrooms/{id}", async (int id, Classroom classroom, [FromServices]IClassroomRepository repository) =>
        {
            if (id != classroom.ID)
                return Results.BadRequest(new { message = "ID в URL и теле запроса не совпадают" });

            if (string.IsNullOrWhiteSpace(classroom.NumberClassroom))
                return Results.BadRequest(new { message = "Номер аудитории обязателен" });

            var result = await repository.UpdateAsync(classroom);
            if (result == null)
                return Results.NotFound(new { message = $"Аудитория с ID {id} не найдена" });

            return Results.Ok(result);
        });

        app.MapDelete("/api/classrooms/{id}", async (int id, [FromServices]IClassroomRepository repository) =>
        {
            var result = await repository.DeleteAsync(id);
            if (result == null)
                return Results.NotFound(new { message = $"Аудитория с ID {id} не найдена" });

            return Results.Ok(result);
        });
    }
}