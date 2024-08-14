using System.ComponentModel.DataAnnotations.Schema;

namespace CMM.PolicyFilter.Entities;

[Table("users")]
public class User
{
    [Column("id")]
    public long Id { get; set; }
    
    [Column("username")]
    public string Username { get; set; }
    
    [Column("registration_date")]
    public DateTime RegistrationDate { get; set; }
}