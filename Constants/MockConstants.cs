namespace quick_ai.Constants
{
    internal class MockConstants
    {
        public const string RESPONSE_GENERAL = @"
Para listar os arquivos de um diretório no PowerShell, você pode usar o cmdlet `Get-ChildItem`. Aqui está o comando básico:

```powershell
Get-ChildItem
```

Esse comando lista todos os arquivos e subdiretórios no diretório atual.

### Exemplos:

1. **Listar arquivos em um diretório específico**:
    ```powershell
    Get-ChildItem -Path ""C:\Seu\Diretorio""
    ```

2. **Listar apenas os arquivos (excluindo diretórios)**:
    ```powershell
    Get-ChildItem -Path ""C:\Seu\Diretorio"" -File
    ```

Esses comandos são bastante flexíveis e podem ser combinados para atender às suas necessidades.
";
        public const string RESPONSE_GRAMMAR = @"{""revisedText"": ""Buenas tardes, ¿cómo están?"",""summary"": [{""before"": ""Buena tardies"", ""after"": ""Buenas tardes""},{""before"": ""como están?"", ""after"": ""¿cómo están?""}]}";
    }
}
