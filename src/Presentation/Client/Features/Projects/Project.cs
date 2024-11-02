namespace Thaniwasi.Client.Features.Projects;

public record Project
{
    public int Id { get; init; }
    public string Title { get; init; }
    public bool Active { get; init; }
}