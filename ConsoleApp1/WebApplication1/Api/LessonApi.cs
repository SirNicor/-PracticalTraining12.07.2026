using DomainCore.DTO;
using EFRepository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Api;

public static class LessonApi
{
    public static void AddedApiLesson(this WebApplication app)
    {
        app.MapGet("/api/lessons/{id}", async (int id, [FromServices]ILessonRepository repository) =>
        {
            var lesson = await repository.GetLessonInfoForIDAsync(id);
            return lesson == null ? Results.NotFound() : Results.Ok(lesson);
        });

        app.MapGet("/api/lessons/group/{name}", async (string name, [FromServices]ILessonRepository repository) =>
        {
            var lessons = await repository.GetLessonInfoForNameGroupAsync(name);
            return Results.Ok(lessons ?? new List<LessonDTOInfo>());
        });

        app.MapGet("/api/lessons/teacher/{name}", async (string name, [FromServices]ILessonRepository repository) =>
        {
            var lessons = await repository.GetLessonInfoForNameTeacherAsync(name);
            return Results.Ok(lessons ?? new List<LessonDTOInfo>());
        });

        app.MapGet("/api/lessons/classroom/{number}", async (string number, [FromServices]ILessonRepository repository) =>
        {
            var lessons = await repository.GetLessonInfoForNumberClassroomAsync(number);
            return Results.Ok(lessons ?? new List<LessonDTOInfo>());
        });

        app.MapPost("/api/lessons", async (LessonDTOCreate dto, [FromServices]ILessonRepository repository) =>
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.TeacherName) || string.IsNullOrWhiteSpace(dto.ClassroomNumber))
                return Results.BadRequest(new { message = "Данные занятия обязательны" });

            var id = await repository.CreateLessonAsync(dto);
            if (id == null)
                return Results.Conflict(new { message = "Не удалось создать занятие (конфликт расписания или ошибка данных)" });

            return Results.Created($"/api/lessons/{id}", dto);
        });

        app.MapPut("/api/lessons/{id}", async (int id, LessonDTOCreate dto, [FromServices]ILessonRepository repository) =>
        {
            if (id != dto.ID)
                return Results.BadRequest(new { message = "ID в URL и теле запроса не совпадают" });

            var result = await repository.UpdateLessonAsync(dto);
            if (result == null)
                return Results.NotFound(new { message = $"Занятие с ID {id} не найдено" });

            return Results.Ok(result);
        });

        app.MapDelete("/api/lessons/{id}", async (int id, [FromServices]ILessonRepository repository) =>
        {
            var result = await repository.DeleteLessonAsync(id);
            if (result == null)
                return Results.NotFound(new { message = $"Занятие с ID {id} не найдено" });

            return Results.Ok(result);
        });
    }
}