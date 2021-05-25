namespace ChurchDataShow.Models
{
    class TextoBiblia
    {
        public int Id { get; set; }
        public int IdTraducao { get; set; }
        public int IdLivro { get; set; }
        public int Capitulo { get; set; }
        public int Versiculo { get; set; }
        public string Texto { get; set; }
    }
}
