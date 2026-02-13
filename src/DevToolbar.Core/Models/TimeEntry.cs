namespace DevToolbar.Core.Models;

using System.Text.Json.Serialization;

/// <summary>
/// Represents a time tracking entry.
/// </summary>
public class TimeEntry
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [JsonPropertyName("projectId")]
    public string ProjectId { get; set; } = string.Empty;

    [JsonPropertyName("workItemId")]
    public string? WorkItemId { get; set; }

    [JsonPropertyName("startTime")]
    public DateTime StartTime { get; set; }

    [JsonPropertyName("endTime")]
    public DateTime? EndTime { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonIgnore]
    public TimeSpan Duration => (EndTime ?? DateTime.Now) - StartTime;
}
