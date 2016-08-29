Unity Version: 5.3.2f1 Personal


Cena Inicial (e única): Main Scene


No canto inferior esquerdo existem botoes para Iniciar o Jogo, 
ativar modo de ondas infinitas, reiniciar level e chamar uma onda extra.

No lado esquerdo ao centro da tela existem dois botoes(com os icones das torres) para selecionar qual torre a ser construída 
(a opçao default eh a torre azul)

O jogo utiliza apenas 3D Objects primitivos do Unity como graficos (Cubes, Spheres e Cylinders)

Exitem dois tipos de torres, que são diferenciadas pelo Alcance e Dano;

Existem dois tipos de inimigos, um com mais hp e mais lento, e outro rapido e com menos hp; O seu caminho eh definido utilizando o 
algoritimo de Djikstra e eh atualizado e leva em consideracao as torres no caminho. 

Caso 10 inimigos cheguem ao objetivo, o jogo termina em derrota e o level eh reiniciado;
Caso o jogador derrote as 5 Waves e não exista mais nenhum inimigo vivo ele vence o jogo (caso waves infinitas esteja desativado)

A maior parte das configuracoes estao nos scripts GameManager e TileMap (ambos sao parte do GameObject GameMaster), dentre elas:
TileMap: Start position, Target position, Map size X e Map size Y; 

Tentei manter uma configuracao similar ao game Desktop Tower Defense no modo facil. 
Foi uma atividade divertida, espero que gostem =).
