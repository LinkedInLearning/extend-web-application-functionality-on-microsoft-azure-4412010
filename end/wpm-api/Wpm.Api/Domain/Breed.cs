﻿namespace Wpm.Api.Domain;
public class Breed
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public decimal IdealMaxWeight { get; set; }
    public Species Species { get; set; }
    public int SpeciesId { get; set; }
}