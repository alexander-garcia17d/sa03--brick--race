using System;

namespace sa03_brick_race
{
    public class Menu
    {
        public void Mostrar()
        {
            bool isRunning = true;

            while (isRunning)
            {
                ExibirMenu();
                string opcao = Console.ReadLine();

                switch (opcao)
                {
                    case "1":
                        Jogo jogo = new Jogo();
                        jogo.Iniciar();
                        break;
                    case "2":
                        MostrarInstrucoes();
                        break;
                    case "3":
                        MostrarUltimoResultado();
                        break;
                    case "0":
                        Console.WriteLine("saindo do jogo...volte logo!");
                        isRunning = false;
                        break;
                    default:
                        Console.WriteLine("comando inválido!presione qualquer tecla para tentar de novo :)");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private void ExibirMenu()
        {
            Console.Clear();
            Console.WriteLine("╔═════════════════════════════════════════════╗");
            Console.WriteLine("║               BRICK RACE - C#               ║");
            Console.WriteLine("║          Corrida em C# - SA 03              ║");
            Console.WriteLine("╠═════════════════════════════════════════════╣");
            Console.WriteLine("║ 1 - Iniciar jogo                            ║");
            Console.WriteLine("║ 2 - Instruções                              ║");
            Console.WriteLine("║ 3 - Ver último resultado                    ║");
            Console.WriteLine("║ 0 - Sair                                    ║");
            Console.WriteLine("╚═════════════════════════════════════════════╝");
            Console.Write("Escolha uma opção: ");
        }

        private void IniciarJogo()
        {
            Console.Clear();
            Console.WriteLine("Iniciando o jogo... (lógica da partida entra aqui)");
            Console.WriteLine("Pressione qualquer tecla para voltar ao menu.");
            Console.ReadKey();
        }

        private void MostrarInstrucoes()
        {
            Console.Clear();
            Console.WriteLine("=== INSTRUÇÕES ===");
            Console.WriteLine("OBJETIVO:");
            Console.WriteLine("Desvie dos obstaculos trocando entre a pista esquerda e a pista direita.");
            Console.WriteLine();
            Console.WriteLine("CONTROLES:");
            Console.WriteLine("A ou seta esquerda = mover para a esquerda");
            Console.WriteLine("D ou seta direita = mover para a direita");
            Console.WriteLine("ESC = sair da partida");
            Console.WriteLine();
            Console.WriteLine("REGRAS:");
            Console.WriteLine("Voce comeca com 3 vidas.");
            Console.WriteLine("Cada obstaculo desviado aumenta sua pontuacao.");
            Console.WriteLine("Ao bater em um obstaculo, voce perde uma vida.");
            Console.WriteLine("Quando as vidas chegam a zero, a partida termina.");
            Console.WriteLine();
            Console.WriteLine("Pressione qualquer tecla para voltar ao menu.");
            Console.ReadKey();
        }

        private void MostrarUltimoResultado()
        {
            Console.Clear();
            Console.WriteLine("===ÚLTIMO RESULTADO===");
            Console.WriteLine("Nenhuma partida foi jogada ainda.");
            Console.WriteLine();
            Console.WriteLine("Pressione qualquer tecla para voltar ao menu.");
            Console.ReadKey();
        }
    }
}