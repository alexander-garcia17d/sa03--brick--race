using System;
using System.Threading;
using System.Threading.Tasks;

namespace sa03_brick_race
{
    internal class Jogo
    {
        private string posicaoCarro = "esquerda";
        private int pontos = 0;
        private int nivel = 1;
        private int vidas = 3;
        private double velocidade = 200;
        private int obstaculosDesviados = 0;

        private const int LARGURA_PISTA = 27;
        private const int LARGURA_PAINEL = 32;

        private const int ALTURA_PISTA = 12;
        private const int LINHA_COLISAO = 11;

        private GerenciadorObstaculos gerenciadorObstaculos;

        public Jogo()
        {
            gerenciadorObstaculos = new GerenciadorObstaculos(2, LINHA_COLISAO);
        }

        public void Iniciar()
        {
            bool jogoAtivo = true;

            while (jogoAtivo && vidas > 0)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo tecla = Console.ReadKey(true);

                    if (tecla.Key == ConsoleKey.LeftArrow || tecla.Key == ConsoleKey.A)
                        posicaoCarro = "esquerda";
                    else if (tecla.Key == ConsoleKey.RightArrow || tecla.Key == ConsoleKey.D)
                        posicaoCarro = "direita";
                    else if (tecla.Key == ConsoleKey.Escape)
                        jogoAtivo = false;
                }

                gerenciadorObstaculos.Atualizar(posicaoCarro, ref pontos, ref vidas, ref obstaculosDesviados);
                AtualizarNivel();
                ExibirTela();
                Task.Delay(TimeSpan.FromMilliseconds(velocidade)).Wait();
            }

            ExibirFimDeJogo();
        }

        private void AtualizarNivel()
        {
            int novoNivel = (pontos / 100) + 1;

            if (novoNivel > nivel)
            {
                nivel = novoNivel;
                velocidade = Math.Max(60, 250 - (nivel - 1) * 20);
            }
        }

        private string BordaTopo()
        {
            return "╔" + new string('═', LARGURA_PISTA + LARGURA_PAINEL + 1) + "╗";
        }

        private string BordaMeio()
        {
            return "╠" + new string('═', LARGURA_PISTA) + "╦" + new string('═', LARGURA_PAINEL) + "╣";
        }

        private string BordaBase()
        {
            return "╚" + new string('═', LARGURA_PISTA) + "╩" + new string('═', LARGURA_PAINEL) + "╝";
        }

        private string LinhaTitulo(string titulo)
        {
            int largura = LARGURA_PISTA + LARGURA_PAINEL + 1;
            int espacosEsquerda = (largura - titulo.Length) / 2;
            string conteudo = new string(' ', espacosEsquerda) + titulo;
            conteudo = conteudo.PadRight(largura);
            return "║" + conteudo + "║";
        }

        private string Linha(string pista, string painel)
        {
            string colPista = pista.PadRight(LARGURA_PISTA);
            string colPainel = painel.PadRight(LARGURA_PAINEL);
            return "║" + colPista + "║" + colPainel + "║";
        }

        private string FaixaNaLinha(int linhaAtual)
        {
            string conteudoEsquerda = "";
            string conteudoDireita = "";

            foreach (Obstaculo obstaculo in gerenciadorObstaculos.Todos)
            {
                if (obstaculo.Ativo && obstaculo.Linha == linhaAtual)
                {
                    if (obstaculo.Pista == "esquerda")
                        conteudoEsquerda = Obstaculo.Forma;
                    else
                        conteudoDireita = Obstaculo.Forma;
                }
            }

            int inicioCarro = ALTURA_PISTA - 4;

            if (linhaAtual >= inicioCarro)
            {
                int linhaCarro = linhaAtual - inicioCarro;
                string[] formaCarro = { "  █", " ███", "  █", " █ █" };
                string desenho = formaCarro[linhaCarro];

                if (posicaoCarro == "esquerda")
                    conteudoEsquerda = desenho;
                else
                    conteudoDireita = desenho;
            }

            return "  │" + conteudoEsquerda.PadRight(8) + "│" + conteudoDireita.PadRight(8) + "│";
        }

        private void ExibirTela()
        {
            Console.Clear();

            string[] textoPainel =
            {
                "  NIVEL   : " + nivel.ToString("D2"),
                "  VIDAS   : " + vidas,
                "  VELOC.  : " + velocidade + " ms",
                "",
                "",
                "",
                "",
                "",
                "  A ou seta esquerda",
                "  D ou seta direita",
                "  ESC = sair",
                "",
            };

            Console.WriteLine(BordaTopo());
            Console.WriteLine(LinhaTitulo("BRICK RACE"));
            Console.WriteLine(BordaMeio());
            Console.WriteLine(Linha("  ┌────────┬────────┐", "  PONTOS  : " + pontos.ToString("D6")));

            for (int linha = 0; linha < ALTURA_PISTA; linha++)
            {
                string painel = linha < textoPainel.Length ? textoPainel[linha] : "";
                Console.WriteLine(Linha(FaixaNaLinha(linha), painel));
            }

            Console.WriteLine(Linha("  └────────┴────────┘", ""));
            Console.WriteLine(BordaBase());
        }

        private void ExibirFimDeJogo()
        {
            Console.Clear();
            Console.WriteLine("╔═════════════════════════════════════════════════════╗");
            Console.WriteLine("║               FIM DE JOGO                           ║");
            Console.WriteLine("╠═════════════════════════════════════════════════════╣");
            Console.WriteLine("║ Pontuacao final      : " + pontos.ToString("D6") + "                       ║");
            Console.WriteLine("║ Nivel alcancado       : " + nivel.ToString("D2") + "                          ║");
            Console.WriteLine("║ Obstaculos desviados  : " + obstaculosDesviados + "                           ║");
            Console.WriteLine("║                                                     ║");
            Console.WriteLine("║ Pressione qualquer tecla para voltar                ║");
            Console.WriteLine("║ ao menu principal.                                  ║");
            Console.WriteLine("╚═════════════════════════════════════════════════════╝");
            Console.ReadKey(true);
        }
    }
}