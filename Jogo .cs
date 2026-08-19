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
        private double velocidade = 150;
        private int obstaculosDesviados = 0;

        private const int LARGURA_PISTA = 27;
        private const int LARGURA_PAINEL = 32;

        private const int ALTURA_PISTA = 23;
        private const int LINHA_COLISAO = ALTURA_PISTA - 4;

        // --- Boost de velocidade (W ou seta pra cima) ---
        private const double MULTIPLICADOR_BOOST = 1.75; // +75% de velocidade
        private static readonly TimeSpan LIMIAR_BOOST = TimeSpan.FromMilliseconds(200);
        private DateTime ultimoToqueBoost = DateTime.MinValue;

        private GerenciadorObstaculos gerenciadorObstaculos;

        public int PontosFinais => pontos;
        public int NivelFinal => nivel;
        public int ObstaculosDesviadosFinais => obstaculosDesviados;

        public Jogo()
        {
            gerenciadorObstaculos = new GerenciadorObstaculos(LINHA_COLISAO);
        }

        public void Iniciar()
        {
            bool jogoAtivo = true;

            while (jogoAtivo && vidas > 0)
            {
                // Drena TODAS as teclas disponíveis nesse frame (não só uma).
                // Isso evita que o key-repeat do W/seta-cima "engula" o A/D enquanto o boost está ativo.
                while (Console.KeyAvailable)
                {
                    ConsoleKeyInfo tecla = Console.ReadKey(true);

                    if (tecla.Key == ConsoleKey.LeftArrow || tecla.Key == ConsoleKey.A)
                        posicaoCarro = "esquerda";
                    else if (tecla.Key == ConsoleKey.RightArrow || tecla.Key == ConsoleKey.D)
                        posicaoCarro = "direita";
                    else if (tecla.Key == ConsoleKey.UpArrow || tecla.Key == ConsoleKey.W)
                        ultimoToqueBoost = DateTime.Now;
                    else if (tecla.Key == ConsoleKey.Escape)
                        jogoAtivo = false;
                }

                bool acelerando = (DateTime.Now - ultimoToqueBoost) < LIMIAR_BOOST;
                double velocidadeEfetiva = acelerando ? velocidade / MULTIPLICADOR_BOOST : velocidade;

                gerenciadorObstaculos.Atualizar(posicaoCarro, nivel, ref pontos, ref vidas, ref obstaculosDesviados);
                AtualizarNivel();
                ExibirTela(acelerando);
                Task.Delay(TimeSpan.FromMilliseconds(velocidadeEfetiva)).Wait();
            }

            ExibirFimDeJogo();
        }

        private void AtualizarNivel()
        {
            int novoNivel = (pontos / 100) + 1;

            if (novoNivel > nivel)
            {
                nivel = novoNivel;
                velocidade = Math.Max(60, 150 - (nivel - 1) * 20);
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
                if (!obstaculo.Ativo)
                {
                    continue;
                }

                int offset = linhaAtual - obstaculo.Linha;
                if (offset >= 0 && offset < Obstaculo.Formas.Length)
                {
                    string desenho = Obstaculo.Formas[offset];

                    if (obstaculo.Pista == "esquerda")
                        conteudoEsquerda = desenho;
                    else
                        conteudoDireita = desenho;
                }
            }

            // Carro agora no formato "X", 3 linhas (mesmo formato dos obstáculos)
            int inicioCarro = ALTURA_PISTA - 3;

            if (linhaAtual >= inicioCarro)
            {
                int linhaCarro = linhaAtual - inicioCarro;
                string[] formaCarro = { " █ █", "  █ ", " █ █" };
                string desenho = formaCarro[linhaCarro];

                if (posicaoCarro == "esquerda")
                    conteudoEsquerda = desenho;
                else
                    conteudoDireita = desenho;
            }

            return "  │" + conteudoEsquerda.PadRight(8) + "│" + conteudoDireita.PadRight(8) + "│";
        }

        private void ExibirTela(bool acelerando)
        {
            Console.Clear();

            string[] textoPainel =
            {
                "  NIVEL   : " + nivel.ToString("D2"),
                "  VIDAS   : " + vidas,
                "  VELOC.  : " + velocidade + " ms",
                acelerando ? "  >>> BOOST ATIVO <<<" : "",
                "",
                "",
                "",
                "",
                "  A ou seta esquerda",
                "  D ou seta direita",
                "  W ou seta cima = boost",
                "  ESC = sair",
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
            string titulo = "FIM DE JOGO";
            string linha1 = " Pontuacao final      : " + pontos.ToString("D6");
            string linha2 = " Nivel alcancado       : " + nivel.ToString("D2");
            string linha3 = " Obstaculos desviados  : " + obstaculosDesviados;
            string linha4 = " Pressione qualquer tecla para voltar";
            string linha5 = " ao menu principal.";

            int largura = titulo.Length;
            largura = Math.Max(largura, linha1.Length);
            largura = Math.Max(largura, linha2.Length);
            largura = Math.Max(largura, linha3.Length);
            largura = Math.Max(largura, linha4.Length);
            largura = Math.Max(largura, linha5.Length);
            largura += 4; // folga extra nas bordas

            Console.Clear();
            Console.WriteLine("╔" + new string('═', largura) + "╗");
            Console.WriteLine(LinhaCentralizadaFim(titulo, largura));
            Console.WriteLine("╠" + new string('═', largura) + "╣");
            Console.WriteLine(LinhaConteudoFim(linha1, largura));
            Console.WriteLine(LinhaConteudoFim(linha2, largura));
            Console.WriteLine(LinhaConteudoFim(linha3, largura));
            Console.WriteLine(LinhaConteudoFim("", largura));
            Console.WriteLine(LinhaConteudoFim(linha4, largura));
            Console.WriteLine(LinhaConteudoFim(linha5, largura));
            Console.WriteLine("╚" + new string('═', largura) + "╝");
            Console.ReadKey(true);
        }

        private string LinhaConteudoFim(string conteudo, int largura)
        {
            return "║" + conteudo.PadRight(largura) + "║";
        }

        private string LinhaCentralizadaFim(string texto, int largura)
        {
            int espacosEsquerda = Math.Max(0, (largura - texto.Length) / 2);
            string conteudo = new string(' ', espacosEsquerda) + texto;
            return "║" + conteudo.PadRight(largura) + "║";
        }
    }
}