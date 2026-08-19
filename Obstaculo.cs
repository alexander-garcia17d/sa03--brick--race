namespace sa03_brick_race
{
    internal class Obstaculo
    {
        public string Pista { get; set; }
        public int Linha { get; set; }
        public bool Ativo { get; set; }

        // Formato em "X": diagonal cruzada, 3 linhas
        public static readonly string[] Formas = { " █ █", "  █ ", " █ █" };

        public Obstaculo()
        {
            Ativo = false;
        }

        public void Ativar(string pista)
        {
            Ativo = true;
            Pista = pista;
            Linha = 0;
        }

        public void Desativar()
        {
            Ativo = false;
        }

        public void Descer()
        {
            Linha++;
        }
    }
}