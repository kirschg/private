using System;
using System.Collections.Generic;

namespace vizsgaremek.Models;

public partial class User
{
    public int Id { get; set; }

    public string FelhasznaloNev { get; set; } = null!;

    public string TeljesNev { get; set; } = null!;

    public string Salt { get; set; } = null!;

    public string Hash { get; set; } = null!;

    public string Email { get; set; } = null!;

    public int Jogosultsag { get; set; }

    public int Aktiv { get; set; }

    public DateTime? RegisztracioDatuma { get; set; }

    public string ProfilKepUtvonal { get; set; } = null!;
}
