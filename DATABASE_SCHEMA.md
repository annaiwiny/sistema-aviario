# Modelagem do Banco de Dados

Abaixo está a representação da modelagem de dados atual do sistema **FarmSystem**, mostrando as tabelas e seus relacionamentos.

## Diagrama Entidade-Relacionamento (ERD)

```mermaid
erDiagram
    %% Tabelas Principais
    User ||--o{ Farm : "Possui (1:N)"
    Farm ||--o{ Lot : "Contém (1:N)"
    Lot ||--o{ LotItem : "Possui (1:N)"

    %% Tabelas de Monitoramento e Registros (Vinculadas ao Lote)
    Lot ||--o{ Vaccination : "Recebe (1:N)"
    Lot ||--o{ Mortality : "Registra (1:N)"
    Lot ||--o{ CollectEgg : "Produz (1:N)"
    Lot ||--o{ Sale : "Gera (1:N)"

    %% Definição das Entidades
    User {
        int Id
        string Name
        string Email
        string Password
        string Cpf
        string State
        string City
        string Phone
        string Address
    }

    Farm {
        int Id
        string Name
        int OwnerId
        datetime CreationDate
    }

    Lot {
        int Id
        datetime AccommodationDate
        int FarmId
    }

    LotItem {
        int Id
        string Race
        int Quantity
        int LotId
    }

    Vaccination {
        int Id
        int LotId
        datetime ApplicationDate
        string VaccineType
        decimal ApplicationValue
        int ApplicationQuantity
    }

    Mortality {
        int Id
        int LotId
        datetime DeathDate
        int Quantity
        string Reason
    }

    CollectEgg {
        int Id
        int LotId
        datetime CollectionDate
        int Quantity
        string EggType
    }

    Sale {
        int Id
        int LotId
        datetime SaleDate
        int EggQuantity
        decimal UnitValue
        decimal TotalValue
    }
```

## Descrição das Relações

1.  **User -> Farm**: Um usuário pode ter múltiplas granjas (embora a regra de negócio atual limite a uma por simplicidade inicial).
2.  **Farm -> Lot**: Uma granja pode ter vários lotes de aves ao longo do tempo.
3.  **Lot -> LotItem**: Um lote pode ser composto por múltiplas linhagens (raças) diferentes, cada uma com sua quantidade inicial.
4.  **Lot -> Monitoramentos (Vaccination, Mortality, CollectEgg, Sale)**: Todos os eventos diários e financeiros estão atrelados a um **Lote** específico, permitindo o rastreamento detalhado da produtividade e saúde daquele grupo de aves.
