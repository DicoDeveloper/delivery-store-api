# DeliveryStore API

## Overview
Este projeto é uma API web que integra com serviço de consulta de CEP externo para realizar calculo para o valor do frete. Ele inclui consultas, criação, edição, exclusão de produtos. Para vendas cadastro, cancelamento e consultas com filtros.

## Tech Stack
- **ASP.NET Core 8**: Para construir a API web.
- **Entity Framework Core**: Para operações de banco de dados.
- **SQLite**: Como o banco de dados.
- **Moq**: Para testes unitários.
- **xUnit**: Como framework de testes.
- **HttpClient**: Para fazer requisições HTTP a APIs externas.
- **Swagger**: Para documentação e testes da minimal API.

## Features
1. **Product Create**: Realiza a criação de um produto novo.
2. **Product Update**: Realiza a atualização de um produto.
3. **Product Delete**: Realiza a exclusão lógica do registro na base de dados.
4. **Sale Create**: Realiza a criação de uma venda nova atrelada a itens com seus produtos.
5. **Sale Cancel**: Realiza o cancelamento de uma venda existente.
6. **Shipping Cost**: Realiza a consulta do valor do frete da venda com base em um CEP informado.

## Endpoints
1. **Product Create**
    - `POST /products`
2. **Product Update**
    - `Put /products`
3. **Product Delete**
    - `Delete /products`
4. **Sale Create**
    - `Post /sales`
5. **Sale Cancel**
    - `Post /sales-cancel`
6. **Shipping Cost**
    - `Post /sales-shipping-cost`

## Instruções de Setup

### Pré-requisito
- .NET SDK 8.0 ou superior
- SQLite instalado ou incluído no projeto

### Configuração
1. **Configure o Setup**
    - Crie um `appsettings.json` arquivo na pasta raiz do projeto com o conteúdo:
    ```json
    {
        "ConnectionStrings": {
            "DefaultConnection": "Data Source=deliverystore.db"
        },
        "ViaCEP": {
            "BaseUrl": "https://viacep.com.br/"
        },
        "Logging": {
            "LogLevel": {
            "Default": "Information",
            "Microsoft.AspNetCore": "Warning",
            "Microsoft.Hosting.Lifetime": "Information"
            }
        },
    }
    ```

### Executando a aplicação
1. **Build e execute a aplicação.**
    ```sh
    docker build -t deliverystore . ; docker run -p 5000:5000 deliverystore
    ```
2. **Acesse a documentação da API**
    - Navegue para `http://localhost:5000/swagger` no seu navegador para visualizar e interagir com a minimal API usando o Swagger..

### Testando
1. **Executando os testes unitários**
    ```sh
    dotnet test
    ```
## Uso
- **Criar novo Produto**
    - Envie uma requisição `POST` para `/products` com os detalhes do produto.
- **Atualizar Produto**
    - Envie uma requisição `PUT` para `/products` com os detalhes do produto.
- **Deletar um Produto**
    - Envie uma requisição `DELETE` para `/products` com o Id do produto.
- **Criar nova Venda**
    - Envie uma requisição `POST` para `/sales` com os detalhes da venda e dos itens da venda.
- **Cancelar Venda**
    - Envie uma requisição `POST` para `/sales:cancel` com o Id da venda.
- **Consultar valor do frete**
    - Envie uma requisição `GET` para `/sales:shipping-cost` com o CEP para onde será enviado a venda.

---