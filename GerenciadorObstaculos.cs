using System;

namespace sa03_brick_race
{
    internal class GerenciadorObstaculos
    {
        private Obstaculo[] obstaculos;
        private int linhaColisao;

        public GerenciadorObstaculos(int quantidadeMaxima, int linhaColisao)
        {
            this.linhaColisao = linhaColisao;
            obstaculos = new Obstaculo[quantidadeMaxima];

            for (int i = 0; i < obstaculos.Length; i++)
            {
                obstaculos[i] = new Obstaculo();
            }
        }

        public Obstaculo[] Todos => obstaculos;

        public void Atualizar(string posicaoCarro, ref int pontos, ref int vidas, ref int obstaculosDesviados)
        {
            TentarCriarObstaculo();

            for (int i = 0; i < obstaculos.Length; i++)
            {
                if (!obstaculos[i].Ativo)
                {
                    continue;
                }

                obstaculos[i].Descer();

                if (obstaculos[i].Linha == linhaColisao)
                {
                    if (obstaculos[i].Pista == posicaoCarro)
                    {
                        vidas--;
                    }
                    else
                    {
                        pontos += 10;
                        obstaculosDesviados++;
                    }
                }

                if (obstaculos[i].Linha > linhaColisao)
                {
                    obstaculos[i].Desativar();
                }
            }
        }

        private void TentarCriarObstaculo()
        {
            if (ContarAtivos() >= obstaculos.Length)
            {
                return;
            }

            if (Random.Shared.Next(0, 100) >= 15)
            {
                return;
            }

            bool esquerdaOcupada = false;
            bool direitaOcupada = false;

            foreach (Obstaculo obstaculo in obstaculos)
            {
                if (obstaculo.Ativo && obstaculo.Linha <= 2)
                {
                    if (obstaculo.Pista == "esquerda") esquerdaOcupada = true;
                    else direitaOcupada = true;
                }
            }

            if (esquerdaOcupada && direitaOcupada)
            {
                return;
            }

            string novaPista;
            if (esquerdaOcupada) novaPista = "direita";
            else if (direitaOcupada) novaPista = "esquerda";
            else novaPista = Random.Shared.Next(0, 2) == 0 ? "esquerda" : "direita";

            foreach (Obstaculo obstaculo in obstaculos)
            {
                if (!obstaculo.Ativo)
                {
                    obstaculo.Ativar(novaPista);
                    return;
                }
            }
        }

        private int ContarAtivos()
        {
            int total = 0;
            foreach (Obstaculo obstaculo in obstaculos)
            {
                if (obstaculo.Ativo)
                {
                    total++;
                }
            }
            return total;
        }
    }
}