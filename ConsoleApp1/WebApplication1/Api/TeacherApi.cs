using DomainCore;
using DomainCore.DTO;
using EFRepository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Api;

public static class TeacherApi
{
    public static void AddedApiTeacher(this WebApplication app)
    {
        app.MapGet("/api/teachers", async ([FromServices]ITeacherRepository repository) =>
        {
            var teachers = await repository.GetAllTeachersAsync();
            return Results.Ok(teachers ?? new List<TeacherDto>());
        });

        app.MapGet("/api/teachers/{name}", async (string name, [FromServices]ITeacherRepository repository) =>
        {
            var teacher = await repository.GetTeacherForNameAsync(name);
            return teacher == null ? Results.NotFound() : Results.Ok(teacher);
        });

        app.MapPost("/api/teachers", async (TeacherDto teacherDto, [FromServices]ITeacherRepository repository) =>
        {
            if (teacherDto == null || string.IsNullOrWhiteSpace(teacherDto.FIO))
                return Results.BadRequest(new { message = "ФИО преподавателя обязательно" });

            var id = await repository.CreateAsync(teacherDto);
            if (id == null)
                return Results.Conflict(new { message = $"Преподаватель '{teacherDto.FIO}' уже существует или произошла ошибка" });

            return Results.Created($"/api/teachers/{id}", teacherDto);
        });

        app.MapPut("/api/teachers/{id}", async (int id, TeacherDto teacherDto, [FromServices]ITeacherRepository repository) =>
        {
            if (id != teacherDto.ID)
                return Results.BadRequest(new { message = "ID в URL и теле запроса не совпадают" });

            if (string.IsNullOrWhiteSpace(teacherDto.FIO))
                return Results.BadRequest(new { message = "ФИО преподавателя обязательно" });

            var result = await repository.UpdateAsync(teacherDto);
            if (result == null)
                return Results.NotFound(new { message = $"Преподаватель с ID {id} не найден" });

            return Results.Ok(result);
        });

        app.MapDelete("/api/teachers/{id}", async (int id, [FromServices]ITeacherRepository repository) =>
        {
            var result = await repository.DeleteAsync(id);
            if (result == null)
                return Results.NotFound(new { message = $"Преподаватель с ID {id} не найден" });

            return Results.Ok(result);
        });
    }
}