# MouseSwitcher

Este projeto tem como objetivo monitorar e identificar dispositivos de entrada bruta conectados ao sistema, especificamente focado no reconhecimento de movimentos do mouse e touchpad. O aplicativo executa a detecção de movimentos e centraliza o cursor na tela apropriada, dependendo do dispositivo de entrada detectado.
Ou seja, se utilzar o touchpad, ele centralizará na tela 1 (index 0), se utilizar o mouse, centralizara na tela 2 (index 1).

## Funcionalidades Atuais

- **Detecção de Mouse e Touchpad**: Identifica quando o movimento do mouse ou touchpad ocorre e distingue qual dispositivo está sendo utilizado.
- **Centralização do Cursor**: O cursor é centralizado automaticamente na tela apropriada, com base no dispositivo de entrada detectado (Tela 1 para touchpad e Tela 2 para mouse externo).
- **Execução em Segundo Plano**: O aplicativo roda em segundo plano, não interferindo diretamente na interface do usuário. A janela principal é invisível e posicionada fora da tela para não atrapalhar a experiência.

## Observações

- **Em Melhoria**: Este projeto ainda está em processo de desenvolvimento e melhoria. Algumas funcionalidades podem não ser perfeitas e ajustes ainda são necessários para garantir uma detecção mais precisa e a centralização eficiente do cursor.
## Tecnologias Usadas

- **C#**: Linguagem de programação principal.
- **.NET Framework**: Para construção do aplicativo de desktop.
- **Raw Input API**: Para capturar eventos de dispositivos de entrada.

## Melhorias Planejadas

- **Suporte a Mais Dispositivos**
- **Personalização**
  
## Contribuições

Sinta-se à vontade para contribuir com melhorias ou sugestões. Para isso, faça um **fork** deste repositório e envie um **pull request**.

:)

Link = banana.com
