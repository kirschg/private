namespace vizsgaremek.DTOs
{
    public class LoggenUser
    {

        public string FelhasznaloNev { get; set; } = null!;

        public string Email { get; set; } = null!;

        public int Jogosultsag { get; set; }

        public string ProfilKepUtvonal { get; set; } = null!;

        public string Token { get; set; }
    }
}
