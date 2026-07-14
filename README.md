# Brick Race - SA 03

## Descrição
Este projeto é um jogo em modo texto inspirado no estilo Brick Game, desenvolvido em C#.

## Regras do jogo
- O jogador controla um carro em duas pistas.
- Use A ou seta esquerda para mover para a pista esquerda.
- Use D ou seta direita para mover para a pista direita.
- Pressione ESC para sair da partida.
- Cada obstáculo desviado aumenta a pontuação.
- Colidir com um obstáculo reduz uma vida.
- Quando as vidas chegam a zero, a partida termina.

## Estruturas utilizadas
- Estruturas de repetição para atualização contínua da tela
- Estruturas de decisão para movimentação e colisão.
- Vetores para armazenar os obstáculos.
- Recursividade em uma função de escolha de faixa livre.

## Testes manuais
| Nº | Funcionalidade | Situação simulada | Resultado esperado | Resultado obtido | Status |
|---|---|---|---|---|---|
| 01 | Menu | Digitar opção inválida | Exibir aviso e retornar ao menu | Conforme esperado | OK |
| 02 | Movimento | Mover para pista esquerda | Carro aparece na pista esquerda | Conforme esperado | OK |
| 03 | Colisão | Obstáculo chega à pista do carro | Jogador perde uma vida | Conforme esperado | OK |
| 04 | Pontuação | Desviar de obstáculo | Pontuação aumenta | Conforme esperado | OK |
| 05 | Fim de jogo | Vidas chegam a zero | Exibir tela de fim de jogo | Conforme esperado | OK |


