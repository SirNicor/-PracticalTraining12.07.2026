using DomainCore.DTO;
using EFRepository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Api;

public static class GroupApi
{
    public static void AddedApiGroup(this WebApplication app)
    {
        app.MapGet("/api/groups", async ([FromServices]IGroupRepository repository) =>
        {
            var groups = await repository.GetAllGroupsAsync();
            return Results.Ok(groups ?? new List<GroupDto>());
        });

        app.MapGet("/api/groups/{name}", async (string name, [FromServices]IGroupRepository repository) =>
        {
            var group = await repository.GetGroupForNameAsync(name);
            return group == null ? Results.NotFound() : Results.Ok(group);
        });

        app.MapPost("/api/groups", async (GroupDto groupDto, [FromServices]IGroupRepository repository) =>
        {
            if (groupDto == null || string.IsNullOrWhiteSpace(groupDto.NameGroup))
                return Results.BadRequest(new { message = "Название группы обязательно" });

            var id = await repository.CreateAsync(groupDto);
            if (id == null)
                return Results.Conflict(new { message = $"Группа '{groupDto.NameGroup}' уже существует/Неверные названия для каких-либо предметов" });

            return Results.Created($"/api/groups/{id}", groupDto);
        });

        app.MapPut("/api/groups/{id}", async (int id, GroupDto groupDto, [FromServices]IGroupRepository repository) =>
        {
            if (id != groupDto.ID)
                return Results.BadRequest(new { message = "ID в URL и теле запроса не совпадают" });

            if (string.IsNullOrWhiteSpace(groupDto.NameGroup))
                return Results.BadRequest(new { message = "Название группы обязательно" });

            var result = await repository.UpdateAsync(groupDto);
            if (result == null)
                return Results.NotFound(new { message = $"Группа с ID {id} не найдена/Неверные названия для каких-либо предметов" });

            return Results.Ok(result);
        });

        app.MapDelete("/api/groups/{id}", async (int id, [FromServices]IGroupRepository repository) =>
        {
            var result = await repository.DeleteAsync(id);
            if (result == null)
                return Results.NotFound(new { message = $"Группа с ID {id} не найдена" });

            return Results.Ok(result);
        });
    }
}