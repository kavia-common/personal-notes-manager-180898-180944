using System.ComponentModel.DataAnnotations;
using NotesApp.Contracts;
using NotesApp.Models;
using NotesApp.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddEndpointsApiExplorer();
// OpenAPI with NSwag (already referenced)
builder.Services.AddOpenApiDocument(config =>
{
    config.Title = "Notes App API";
    config.Version = "v1";
    config.Description = "REST API for managing personal notes.";
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.SetIsOriginAllowed(_ => true)
              .AllowCredentials()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Dependency Injection for repository
builder.Services.AddSingleton<INoteRepository, InMemoryNoteRepository>();

var app = builder.Build();

// Use CORS
app.UseCors("AllowAll");

// Configure OpenAPI/Swagger
app.UseOpenApi();
app.UseSwaggerUi(config =>
{
    config.Path = "/docs";
});

// Seed data in development
if (app.Environment.IsDevelopment())
{
    var repo = app.Services.GetRequiredService<INoteRepository>();
    if (repo is InMemoryNoteRepository memoryRepo)
    {
        memoryRepo.SeedIfEmpty();
    }
}

// Health check endpoint
app.MapGet("/", () => new { message = "Healthy" });

var notesTag = "Notes";

// PUBLIC_INTERFACE
/// <summary>
/// Get all notes.
/// </summary>
/// <returns>Array of Note objects.</returns>
app.MapGet("/api/notes", (INoteRepository repo) =>
{
    var notes = repo.GetAll();
    return Results.Ok(notes);
})
.WithTags(notesTag)
.WithName("GetNotes");

// PUBLIC_INTERFACE
/// <summary>
/// Get a note by id.
/// </summary>
/// <param name="id">Note GUID</param>
/// <returns>200 with note or 404 if not found.</returns>
app.MapGet("/api/notes/{id:guid}", (Guid id, INoteRepository repo) =>
{
    var note = repo.GetById(id);
    return note is null ? Results.NotFound() : Results.Ok(note);
})
.Produces<Note>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound)
.WithTags(notesTag)
.WithName("GetNoteById");

// PUBLIC_INTERFACE
/// <summary>
/// Create a new note.
/// </summary>
/// <param name="request">Create payload</param>
/// <param name="repo">Repository</param>
/// <returns>201 Created with Location header and created note body, or 400 on validation errors.</returns>
app.MapPost("/api/notes", (CreateNoteRequest request, INoteRepository repo, HttpContext http) =>
{
    // Model validation (Minimal APIs don't auto-validate DTOs with DataAnnotations)
    var validationResults = new List<ValidationResult>();
    var ctx = new ValidationContext(request, serviceProvider: null, items: null);
    if (!Validator.TryValidateObject(request, ctx, validationResults, validateAllProperties: true))
    {
        var errors = validationResults
            .GroupBy(v => v.MemberNames.FirstOrDefault() ?? "")
            .ToDictionary(g => g.Key, g => g.Select(v => v.ErrorMessage ?? "Invalid").ToArray());
        return Results.ValidationProblem(errors);
    }

    var now = DateTime.UtcNow;
    var note = new Note
    {
        Id = Guid.NewGuid(),
        Title = request.Title!.Trim(),
        Content = string.IsNullOrWhiteSpace(request.Content) ? null : request.Content,
        CreatedAt = now,
        UpdatedAt = now
    };

    repo.Create(note);

    var location = $"{http.Request.Scheme}://{http.Request.Host}/api/notes/{note.Id}";
    return Results.Created(location, note);
})
.ProducesValidationProblem()
.Produces<Note>(StatusCodes.Status201Created)
.WithTags(notesTag)
.WithName("CreateNote");

// PUBLIC_INTERFACE
/// <summary>
/// Update a note by id.
/// </summary>
/// <param name="id">Note GUID</param>
/// <param name="request">Update payload</param>
/// <param name="repo">Repository</param>
/// <returns>200 with updated note, 400 on validation error, or 404 if not found.</returns>
app.MapPut("/api/notes/{id:guid}", (Guid id, UpdateNoteRequest request, INoteRepository repo) =>
{
    // Validate request
    var validationResults = new List<ValidationResult>();
    var ctx = new ValidationContext(request, null, null);
    if (!Validator.TryValidateObject(request, ctx, validationResults, validateAllProperties: true))
    {
        var errors = validationResults
            .GroupBy(v => v.MemberNames.FirstOrDefault() ?? "")
            .ToDictionary(g => g.Key, g => g.Select(v => v.ErrorMessage ?? "Invalid").ToArray());
        return Results.ValidationProblem(errors);
    }

    var existing = repo.GetById(id);
    if (existing is null)
    {
        return Results.NotFound();
    }

    existing.Title = request.Title!.Trim();
    existing.Content = string.IsNullOrWhiteSpace(request.Content) ? null : request.Content;
    existing.UpdatedAt = DateTime.UtcNow;

    repo.Update(existing);
    return Results.Ok(existing);
})
.ProducesValidationProblem()
.Produces<Note>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound)
.WithTags(notesTag)
.WithName("UpdateNote");

// PUBLIC_INTERFACE
/// <summary>
/// Delete a note by id.
/// </summary>
/// <param name="id">Note GUID</param>
/// <param name="repo">Repository</param>
/// <returns>204 on success or 404 if not found.</returns>
app.MapDelete("/api/notes/{id:guid}", (Guid id, INoteRepository repo) =>
{
    var deleted = repo.Delete(id);
    return deleted ? Results.NoContent() : Results.NotFound();
})
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status404NotFound)
.WithTags(notesTag)
.WithName("DeleteNote");

app.Run();