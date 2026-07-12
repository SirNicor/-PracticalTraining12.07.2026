using DomainCore;
using EFRepository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Api;

public static class SubjectApi
{
    public static void AddedApiSubject(this WebApplication app)
    {
        app.MapGet("/api/subjects", async ([FromServices]ISubjectRepository repository) =>
        {
            var subjects = await repository.GetAllSubjectsAsync();
            return Results.Ok(subjects ?? new List<Subject>());
        });

        app.MapGet("/api/subjects/{name}", async (string name, [FromServices]ISubjectRepository repository) =>
        {
            var subject = await repository.GetSubjectsForNameAsync(name);
            return subject == null ? Results.NotFound() : Results.Ok(subject);
        });

        app.MapPost("/api/subjects", async (Subject subject, [FromServices]ISubjectRepository repository) =>
        {
            if (subject == null || string.IsNullOrWhiteSpace(subject.NameSubjects))
                return Results.BadRequest(new { message = "Название предмета обязательно" });

            var id = await repository.CreateAsync(subject);
            if (id == null)
                return Results.Conflict(new { message = $"Предмет '{subject.NameSubjects}' уже существует или произошла ошибка" });

            return Results.Created($"/api/subjects/{id}", subject);
        });

        app.MapPut("/api/subjects/{id}", async (int id, Subject subject, [FromServices]ISubjectRepository repository) =>
        {
            if (id != subject.ID)
                return Results.BadRequest(new { message = "ID в URL и теле запроса не совпадают" });

            if (string.IsNullOrWhiteSpace(subject.NameSubjects))
                return Results.BadRequest(new { message = "Название предмета обязательно" });

            var result = await repository.UpdateAsync(subject);
            if (result == null)
                return Results.NotFound(new { message = $"Предмет с ID {id} не найден" });

            return Results.Ok(result);
        });

        app.MapDelete("/api/subjects/{id}", async (int id,[FromServices]ISubjectRepository repository) =>
        {
            var result = await repository.DeleteAsync(id);
            if (result == null)
                return Results.NotFound(new { message = $"Предмет с ID {id} не найден" });

            return Results.Ok(result);
        });
    }
}