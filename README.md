<p align="center">
  <picture>
    <source media="(prefers-color-scheme: dark)" srcset="docs/logo-dark.png">
    <img src="docs/logo.png" alt="LogConverter" width="460">
  </picture>
</p>

<p align="center">
  Conversor em C# que transforma logs de acesso de CDN no formato "MINHA CDN" em logs no formato "Agora".
</p>

<p align="center">
  <a href="https://github.com/uhitlei/AgileContent/actions/workflows/ci.yml"><img alt="CI" src="https://github.com/uhitlei/AgileContent/actions/workflows/ci.yml/badge.svg"></a>
  <img alt=".NET Framework 4.8" src="https://img.shields.io/badge/.NET%20Framework-4.8-512BD4">
  <img alt="Testes NUnit" src="https://img.shields.io/badge/testes-NUnit%203-2E8B57">
  <a href="LICENSE"><img alt="Licença MIT" src="https://img.shields.io/badge/licen%C3%A7a-MIT-blue"></a>
</p>

---

## Sobre

Projeto criado em dezembro de 2017 como solução de um desafio técnico de processo seletivo. O desafio pedia duas coisas:

1. **Conversão de logs:** ler um log de acesso de uma CDN fictícia ("MINHA CDN"), em que cada linha tem os campos separados por `|`, e gravá-lo no formato "Agora", com cabeçalho e campos em outra ordem.
2. **Maior número com os mesmos dígitos:** dado um inteiro `N`, devolver o maior número que se forma reordenando seus dígitos, ou `-1` se o resultado passar de 100.000.000 (`SolutionClass`).

O nome do repositório vem do desafio original. O projeto não tem vínculo com nenhuma empresa.

## Como funciona

Cada linha do log MINHA CDN tem cinco campos:

```
tamanho da resposta | status HTTP | status do cache | "requisição" | tempo de resposta
```

A conversão (`AgoraLog.ConvertLine`) faz o seguinte com cada linha:

| Campo Agora | Origem |
|---|---|
| `provider` | Sempre `"MINHA CDN"` |
| `http-method` | Primeira palavra da requisição (`GET`, `POST`...) |
| `status-code` | Status HTTP, sem alteração |
| `uri-path` | Segunda palavra da requisição (o caminho) |
| `time-taken` | Tempo de resposta sem a parte decimal (`100.2` vira `100`) |
| `response-size` | Tamanho da resposta, sem alteração |
| `cache-status` | `INVALIDATE` vira `REFRESH_HIT`; os demais (`HIT`, `MISS`) são mantidos |

Espaços em volta do `|`, finais de linha Windows (`\r\n`) e linhas em branco são aceitos. Uma linha que não tenha cinco campos, que não tenha método e caminho na requisição ou que traga um valor numérico inválido interrompe a conversão com uma `FormatException` que informa o número da linha.

O arquivo gerado começa com três linhas de cabeçalho (`#Version`, `#Date` no formato `dd/MM/yyyy HH:mm:ss` e `#Fields`), seguidas de uma linha por acesso, com os campos separados por tabulação.

### Exemplo

Entrada (MINHA CDN):

```
312|200|HIT|"GET /robots.txt HTTP/1.1"|100.2
101|200|MISS|"POST /myImages HTTP/1.1"|319.4
199|404|MISS|"GET /not-found HTTP/1.1"|142.9
312|200|INVALIDATE|"GET /robots.txt HTTP/1.1"|245.1
```

Saída (Agora):

```
#Version: 1.0
#Date: 15/12/2017 23:01:06
#Fields:provider	http-method	status-code	uri-path	time-taken	response-size	cache-status
"MINHA CDN"	GET	200	/robots.txt	100	312	HIT
"MINHA CDN"	POST	200	/myImages	319	101	MISS
"MINHA CDN"	GET	404	/not-found	142	199	MISS
"MINHA CDN"	GET	200	/robots.txt	245	312	REFRESH_HIT
```

Este é exatamente o resultado verificado pelo teste `ConvertContent_LogCompleto_GeraCabecalhoELinhasNoFormatoAgora`.

## Como usar

Compile a solução (veja abaixo) ou baixe o `LogConverter-win.zip` da [página de releases](https://github.com/uhitlei/AgileContent/releases), quando houver uma publicada, e rode:

```powershell
AgileContent.Console.exe <log de entrada> <arquivo de saída>
```

- O log de entrada pode ser um arquivo local ou uma URL.
- A pasta do arquivo de saída é criada se não existir.
- Sem argumentos, o programa usa o arquivo público do desafio original (se ainda estiver no ar) e grava em `./output/minhaCdn1.csv`.

O console também imprime o resultado de `SolutionClass` para os valores do enunciado (`147`, `483`, `647`, `100000000` e `100000001`), depois o conteúdo do arquivo convertido. Em caso de erro, mostra a mensagem e termina com código de saída `1`.

Para usar a conversão em outro código, referencie `AgileContent.Library`:

```csharp
var conversor = new ConvertAgoraClass();

// De arquivo (ou URL) para arquivo
conversor.ConvertLog("input-01.txt", "output/agora.txt");

// Ou em memória
string agora = conversor.ConvertContent(conteudoMinhaCdn, DateTime.Now);
```

## Estrutura

```
AgileContent.sln
├── AgileContent/            Aplicação console (AgileContent.Console.exe)
├── AgileContent.Library/    AgoraLog (conversão de uma linha), ConvertAgoraClass (arquivo completo) e SolutionClass
└── AgileContent.Test/       Testes unitários (NUnit)
```

## Compilar e testar

Com Visual Studio 2019 ou superior no Windows, abra `AgileContent.sln`, restaure os pacotes NuGet e compile. Os testes aparecem no Test Explorer.

Pela linha de comando:

```powershell
nuget restore AgileContent.sln
msbuild AgileContent.sln /p:Configuration=Release
packages\NUnit.ConsoleRunner.3.11.1\tools\nunit3-console.exe AgileContent.Test\bin\Release\AgileContent.Test.dll
```

Em macOS ou Linux, o build, os testes e o próprio conversor rodam com Mono via Docker:

```bash
docker run --rm -v "$PWD":/src -w /src mono:6.12 bash -c '
  nuget restore AgileContent.sln &&
  msbuild AgileContent.sln /p:Configuration=Release /p:DebugSymbols=false /p:DebugType=none /v:m &&
  mono packages/NUnit.ConsoleRunner.3.11.1/tools/nunit3-console.exe \
    AgileContent.Test/bin/Release/AgileContent.Test.dll'
```

O teste `TestConvert_CndToAgora`, que baixa o log original da internet, é marcado como `Explicit` e só roda quando chamado pelo nome.

## CI/CD

O workflow [`ci.yml`](.github/workflows/ci.yml) roda no GitHub Actions a cada push e pull request:

1. Restaura os pacotes e compila a solução em um runner Windows.
2. Executa os testes e guarda o `TestResult.xml` como artefato.
3. Empacota o executável em `LogConverter-win.zip`.
4. Quando o push é de uma tag `v*` (ex.: `v1.1.0`), publica uma release no GitHub com o zip anexado.

Para lançar uma versão:

```bash
git tag v1.1.0
git push origin v1.1.0
```

## Melhorias de 2026

Ao revisitar o projeto, foram feitas estas correções e adições:

- Os campos da linha não eram aparados: com espaços em volta do `|`, `INVALIDATE` nunca virava `REFRESH_HIT` e o método HTTP saía vazio. Agora os espaços são removidos.
- Linhas malformadas derrubavam a conversão com `IndexOutOfRangeException`; agora geram uma `FormatException` com o número da linha e o motivo.
- O cabeçalho `#Date` usava o relógio de 12 horas sem AM/PM (`hh`); agora usa 24 horas e cultura invariante.
- Gravar a saída em um caminho sem pasta (ex.: `saida.txt`) lançava exceção; agora funciona.
- `WebClient` e `StreamWriter` passaram a ser liberados com `using`, e os blocos `catch { throw e; }`, que apagavam o stack trace, foram removidos.
- A conversão foi separada do download (`ConvertContent`), e a entrada pode ser um arquivo local.
- `SolutionClass` recusa `N` negativo com `ArgumentOutOfRangeException` em vez de falhar na conversão do texto.
- O console aceita entrada e saída por argumento, retorna código de erro e só espera uma tecla quando a entrada não está redirecionada.
- O teste antigo de conversão comparava um `StringBuilder` com uma `string` e passava sempre; foi corrigido e marcado como `Explicit` por depender da internet.
- Testes migrados de MSTest para NUnit 3, com novos casos para a conversão e linhas malformadas.
- Projetos migrados de .NET Framework 4.5 para 4.8.
- Pipeline de CI/CD, licença MIT e esta documentação.

## Licença

Distribuído sob a [licença MIT](LICENSE).
