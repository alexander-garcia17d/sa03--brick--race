using System;
 
namespace sa03_brick_race
{
    internal class GerenciadorObstaculos
    {
        private Obstaculo[] obstaculos;
        private int linhaColisao;
        private string ultimaPistaCriada = null;
 
        // Mesma distância mínima pra pista igual e pista diferente:
        // isso elimina qualquer viés que fazia os obstáculos grudarem sempre do mesmo lado,
        // e o valor maior garante tempo de reação real (inclusive nos 2 primeiros obstáculos).
        private const int DISTANCIA_MINIMA_MESMA_PISTA = 6;
 
        private const int DIST_DIFERENTE_MIN_PADRAO = 6;
        private const int DIST_DIFERENTE_MAX_PADRAO = 7;
 
        private const int DIST_DIFERENTE_MIN_NIVEL_ALTO = 4;
        private const int DIST_DIFERENTE_MAX_NIVEL_ALTO = 5;
 
        private const int NIVEL_PARA_REDUZIR_DISTANCIA = 55;
 
        private const int MINIMO_OBSTACULOS_TELA = 2;
        private const int MAX_OBSTACULOS_NORMAL = 5;
        private const int MAX_OBSTACULOS_RARO = 7;
 
        private const int CHANCE_SPAWN_NORMAL = 40;    // % de chance por atualização, até 5 obstáculos (era 25)
        private const int CHANCE_SPAWN_RARO = 2;        // % de chance de "estourar" até 7, raríssimo
        private const int CHANCE_PISTA_OPOSTA = 80;      // % de chance de priorizar pista diferente da última
 
        public GerenciadorObstaculos(int linhaColisao)
        {
            this.linhaColisao = linhaColisao;
            obstaculos = new Obstaculo[MAX_OBSTACULOS_RARO];
 
            for (int i = 0; i < obstaculos.Length; i++)
            {
                obstaculos[i] = new Obstaculo();
            }
        }
 
        public Obstaculo[] Todos => obstaculos;
 
        public void Atualizar(string posicaoCarro, int nivelAtual, ref int pontos, ref int vidas, ref int obstaculosDesviados)
        {
            TentarCriarObstaculo(nivelAtual);
 
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
 
        private void TentarCriarObstaculo(int nivelAtual)
        {
            int ativos = ContarAtivos();
 
            if (ativos >= MAX_OBSTACULOS_RARO)
            {
                return; // teto absoluto, nunca ultrapassa
            }
 
            bool abaixoDoMinimo = ativos < MINIMO_OBSTACULOS_TELA;
 
            bool podeTentar;
            if (abaixoDoMinimo)
            {
                // Abaixo do mínimo: tenta em TODO frame (em vez de depender da % de chance),
                // mas sem pular a checagem de distância — assim o 2º obstáculo nasce assim
                // que houver espaço de verdade, nunca colado/impossível de desviar.
                podeTentar = true;
            }
            else if (ativos < MAX_OBSTACULOS_NORMAL)
            {
                podeTentar = Random.Shared.Next(0, 100) < CHANCE_SPAWN_NORMAL;
            }
            else
            {
                // Entre o normal (5) e o raro (7): só libera com chance bem pequena
                podeTentar = Random.Shared.Next(0, 100) < CHANCE_SPAWN_RARO;
            }
 
            if (!podeTentar)
            {
                return;
            }
 
            string primeiraTentativa = EscolherPistaComPrioridade();
            string segundaTentativa = primeiraTentativa == "esquerda" ? "direita" : "esquerda";
 
            // Sorteia a distância mínima exigida entre pistas diferentes UMA vez por tentativa,
            // pra dar variabilidade sem perder consistência dentro dessa checagem
            int distanciaMinimaPistasDiferentes = SortearDistanciaMinimaPistasDiferentes(nivelAtual);
 
            if (PistaValida(primeiraTentativa, distanciaMinimaPistasDiferentes))
            {
                CriarNaPista(primeiraTentativa);
                return;
            }
 
            if (PistaValida(segundaTentativa, distanciaMinimaPistasDiferentes))
            {
                CriarNaPista(segundaTentativa);
            }
 
            // Se nenhuma pista respeita a distância mínima, não nasce ninguém agora
            // (mesmo estando abaixo do mínimo — evita 2 obstáculos colados/impossíveis de desviar)
        }
 
        // 80% de chance de preferir a pista OPOSTA à última criada; 20% pode repetir
        private string EscolherPistaComPrioridade()
        {
            if (ultimaPistaCriada == null)
            {
                return Random.Shared.Next(0, 2) == 0 ? "esquerda" : "direita";
            }
 
            string oposta = ultimaPistaCriada == "esquerda" ? "direita" : "esquerda";
            bool prioridadeParaOposta = Random.Shared.Next(0, 100) < CHANCE_PISTA_OPOSTA;
 
            return prioridadeParaOposta ? oposta : ultimaPistaCriada;
        }
 
        private int SortearDistanciaMinimaPistasDiferentes(int nivelAtual)
        {
            if (nivelAtual >= NIVEL_PARA_REDUZIR_DISTANCIA)
            {
                return Random.Shared.Next(DIST_DIFERENTE_MIN_NIVEL_ALTO, DIST_DIFERENTE_MAX_NIVEL_ALTO + 1);
            }
 
            return Random.Shared.Next(DIST_DIFERENTE_MIN_PADRAO, DIST_DIFERENTE_MAX_PADRAO + 1);
        }
 
        private bool PistaValida(string pista, int distanciaMinimaPistasDiferentes)
        {
            foreach (Obstaculo obstaculo in obstaculos)
            {
                if (!obstaculo.Ativo)
                {
                    continue;
                }
 
                int distancia = obstaculo.Linha; // novo obstáculo nasceria na linha 0
 
                if (obstaculo.Pista == pista)
                {
                    if (distancia < DISTANCIA_MINIMA_MESMA_PISTA)
                    {
                        return false;
                    }
                }
                else
                {
                    if (distancia < distanciaMinimaPistasDiferentes)
                    {
                        return false;
                    }
                }
            }
 
            return true;
        }
 
        private void CriarNaPista(string pista)
        {
            foreach (Obstaculo obstaculo in obstaculos)
            {
                if (!obstaculo.Ativo)
                {
                    obstaculo.Ativar(pista);
                    ultimaPistaCriada = pista;
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
 