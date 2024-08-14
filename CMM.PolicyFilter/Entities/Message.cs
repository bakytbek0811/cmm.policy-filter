using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CMM.PolicyFilter.Entities;

[Table("messages")]
public class Message
{
    [Column("id")]
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [Column("content")]
    [Required]
    [JsonPropertyName("content")]
    public string Content { get; set; }

    [Column("original_content")]
    [Required]
    [JsonPropertyName("originalContent")]
    public string OriginalContent { get; set; }

    [Column("from_user_id")]
    [Required]
    [JsonPropertyName("fromUserId")]
    public long FromUserId { get; set; }

    [ForeignKey("FromUserId")]
    public User User { get; set; }

    [Column("created_at")]
    [Required]
    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }
}