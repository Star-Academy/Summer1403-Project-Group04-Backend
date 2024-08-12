﻿using System.ComponentModel.DataAnnotations;

namespace RelationshipAnalysis.Models;

public class Role
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Name { get; set; }

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}