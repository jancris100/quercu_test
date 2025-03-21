﻿using System.ComponentModel.DataAnnotations;

public class PropertyUpdateDto
{
    [Required]
    public int Id { get; set; }

    [Required]
    public int PropertyTypeId { get; set; }

    [Required]
    public int OwnerId { get; set; }

    [Required, MinLength(1)]
    public string Number { get; set; } = string.Empty;

    [Required, MinLength(5)]
    public string Address { get; set; } = string.Empty;

    [Required, Range(0.1, double.MaxValue)]
    public decimal Area { get; set; }

    public decimal? ConstructionArea { get; set; }
}