# Mini-ETRM: Crude Pricing & P&L API

A Proof of Concept (PoC) for a simplified Energy Trading and Risk Management (ETRM) system, designed to handle crude oil trading operations (WTI and Brent) and calculate Mark-to-Market P&L (Profit and Loss) in real time, interacting with a high-frequency price simulator.

## 📌 What does this program do?

The system simulates the core of an energy trading platform:
1. **Live Market Simulation:** It uses a background engine (`BackgroundService`) that implements a *Random Walk* algorithm to fluctuate WTI and Brent prices every 500 milliseconds.
2. **Trade Execution:** It allows you to register buy or sell orders for barrels. At the time of execution, the system captures the exact market price at that millisecond and persists the contract.
3. **Real-Time P&L Calculation:** Allows you to consult the financial status of a past transaction by comparing the historical execution price against the live price in the simulator (Mark-to-Market), calculating the exact profit or loss instantly.

## 🏗️ Architecture: Clean Architecture + CQRS

The project is designed strictly following the principles of **Clean Architecture**, structured in 4 layers (Domain, Application, Infrastructure, API), and implements the **CQRS** (Command Query Responsibility Segregation) pattern.

### Why this architecture?

In financial and trading systems, business rules (Domain) are critical and must be isolated from temporary or infrastructure-related technological decisions.

* **Decoupling:** The P&L calculation logic and the *Trade* entities are unaware of the existence of SQL Server or Entity Framework.
* **Isolated Performance:** Enables the pricing engine (high in-memory frequency) to operate without blocking database transactions, using domain-defined contracts (interfaces).
* **Scalability and Testing:** By separating commands (writes) from queries (reads) using CQRS, it's possible to optimize, scale, and test each flow independently.

## 💻 Technology Stack and Reasons

* **C# and .NET 10:** Chosen for its extremely high performance, robust ecosystem for enterprise applications, and native support for dependency injection and background workers.
* **Entity Framework Core & SQL Server:** For persisting trades. EF Core enables seamless mapping and injects the transactional security and consistency (ACID) required for storing financial contracts.
* **MediatR (CQRS Pattern):** Simplifies the Application layer by delegating each use case to a specific Handler, keeping API controllers extremely clean and respecting the Single Responsibility Principle (SRP).
* **ConcurrentDictionary (In-Memory Caching):** Since market prices change every 500ms, storing each tick in SQL Server would create an I/O (Input/Output) bottleneck. This thread-safe structure was used, acting as a Singleton to allow concurrent and atomic reads in constant time, simulating the behavior of an ultra-fast Redis cache.
* **IHostedService / BackgroundService:** Used to build the Market Data simulator. Being integrated into the .NET lifecycle, it allows running processes parallel to the web API without the need to set up additional infrastructure (such as an external worker daemon).
* **Docker & Docker Compose:** Used for containerization and local orchestration of the project. Through a multi-stage Dockerfile, we ensure that the application is compiled and runs in an isolated and deterministic environment, eliminating the classic "it works on my machine" problem. Furthermore, Docker Compose allows you to simultaneously launch the API and its infrastructure dependency (the SQL Server container) with a single command, reducing friction to zero for any developer or tester who wants to try the Proof of Concept (PoC).
* **Scalar:** Implemented as the interactive interface for REST API documentation. Unlike traditional interfaces, Scalar offers a much more modern, lightweight, and developer-focused view of the OpenAPI specification. It allows for very intuitive testing of operation execution endpoints and P&L (profit and loss) queries, also providing automatically generated clients and consumption examples in multiple languages ​​directly from the browser.

## 📂 Project Structure

* `MiniETRM.Domain`: Core entities (`Trade`), Value Objects (`MarketTick`), Enums, and Repository/Service abstractions. It has no dependencies.
* `MiniETRM.Application`: Business use cases (MediatR handlers for commands and queries) and DTOs.
* `MiniETRM.Infrastructure`: Implementation of persistence (EF Core DbContext), the in-memory cache engine, and the market simulator (`BackgroundService`).
* `MiniETRM.Api`: RESTful endpoints, Dependency Injection configuration, Middleware, and Swagger.

## 🚀 How to run the project (Docker)

The project is fully Dockerized. You don't need to install the .NET SDK or configure a local SQL Server.

### Prerequisites:
* Docker Desktop installed.

### Steps:
1. Clone the repository or download only the `docker-compose.yml` file.
2. Open a terminal in the project root (where the `docker-compose.yml` file is located).
3. Run the following command:
    ```bash 
    docker-compose up -d
4. Wait a few seconds for the database to initialize.
5. Navigate to http://localhost:8080/scalar to interact with the endpoints:

    GET /api/marketdata/latest: View real-time price fluctuations.

    POST /api/trades: Execute a new trade.
    
    GET /api/trades/{id}/pnl: Check live profit/loss.

6. To stop and clean up the containers, run:
    ```bash
    docker-compose down
<details>
<summary>Ver versión en Español</summary>

# Mini-ETRM: Crude Pricing & P&L API

Una Prueba de Concepto (PoC) de un sistema ETRM (Energy Trading and Risk Management) simplificado, diseñada para manejar operaciones de trading de crudo (WTI y Brent) y calcular el Mark-to-Market P&L (Profit and Loss) en tiempo real, interactuando con un simulador de precios de alta frecuencia.

## 📌 ¿Qué hace este programa?

El sistema simula el núcleo de una plataforma de trading de energía:
1. **Simulación de Mercado en Vivo:** Utiliza un motor en segundo plano (`BackgroundService`) que implementa un algoritmo de *Random Walk* para fluctuar los precios del WTI y el Brent cada 500 milisegundos.
2. **Ejecución de Trades (Operaciones):** Permite registrar operaciones de compra o venta de barriles. Al momento de la ejecución, el sistema captura el precio exacto del mercado en ese milisegundo y persiste el contrato.
3. **Cálculo de P&L en Tiempo Real:** Permite consultar el estado financiero de una operación pasada cruzando el precio histórico de ejecución contra el precio en vivo del simulador (Mark-to-Market), calculando la ganancia o pérdida exacta al instante.

## 🏗️ Arquitectura: Clean Architecture + CQRS

El proyecto está diseñado siguiendo estrictamente los principios de **Clean Architecture** estructurado en 4 capas (Domain, Application, Infrastructure, API) e implementa el patrón **CQRS** (Command Query Responsibility Segregation).

### ¿Por qué esta arquitectura?

En sistemas financieros y de trading, las reglas de negocio (Dominio) son críticas y deben estar aisladas de las decisiones tecnológicas temporales o de infraestructura. 
* **Desacoplamiento:** La lógica de cálculo de P&L y las entidades de *Trade* no saben de la existencia de SQL Server o Entity Framework.
* **Rendimiento Aislado:** Permite que el motor de precios (alta frecuencia en memoria) opere sin bloquear las transacciones de la base de datos, usando contratos (Interfaces) definidos por el Dominio.
* **Escalabilidad y Testing:** Al separar Comandos (escrituras) de Consultas (lecturas) mediante CQRS, es posible optimizar, escalar y testear cada flujo de forma independiente.

## 💻 Stack Tecnológico y Motivos

* **C# y .NET 10:** Elegido por su altísimo rendimiento, su ecosistema robusto para aplicaciones empresariales y su soporte nativo para inyección de dependencias y workers en segundo plano.
* **Entity Framework Core & SQL Server:** Para la persistencia de las operaciones (`Trades`). EF Core permite un mapeo fluido e inyecta la seguridad y consistencia transaccional (ACID) que requiere el almacenamiento de contratos financieros.
* **MediatR (Patrón CQRS):** Simplifica la capa de Aplicación delegando cada caso de uso a un *Handler* específico, manteniendo los controladores de la API sumamente limpios y respetando el Principio de Responsabilidad Única (SRP).
* **ConcurrentDictionary (In-Memory Caching):** Como los precios del mercado cambian cada 500ms, guardar cada *tick* en SQL Server generaría un cuello de botella por I/O (Input/Output). Se utilizó esta estructura *thread-safe* actuando como un Singleton para permitir lecturas concurrentes y atómicas en tiempo constante, simulando el comportamiento de un caché de Redis ultra-rápido.
* **IHostedService / BackgroundService:** Utilizado para construir el simulador de *Market Data*. Al estar integrado en el ciclo de vida de .NET, permite ejecutar procesos paralelos a la API web sin necesidad de levantar infraestructura adicional (como un worker daemon externo).
* **Docker & Docker Compose:**  Utilizados para la contenedorización y orquestación local del proyecto. Mediante un Dockerfile multi-stage, garantizamos que la aplicación se compile y ejecute en un entorno aislado y determinista, eliminando el clásico problema de "en mi máquina funciona". Además, docker-compose permite levantar simultáneamente la API y su dependencia de infraestructura (el contenedor de SQL Server) con un solo comando, reduciendo a cero la fricción para cualquier desarrollador o evaluador que desee probar la PoC.
* **Scalar:** Implementado como la interfaz interactiva para la documentación de la API REST. A diferencia de las interfaces tradicionales, Scalar ofrece una visualización de la especificación OpenAPI mucho más moderna, ligera y centrada en la experiencia del desarrollador. Permite testear los endpoints de ejecución de operaciones y consultas de P&L (profit and loss) de forma muy intuitiva, proporcionando además clientes y ejemplos de consumo generados automáticamente en múltiples lenguajes directamente desde el navegador.

## 📂 Estructura del Proyecto

* `MiniETRM.Domain`: Entidades core (`Trade`), Value Objects (`MarketTick`), Enums y abstracciones de Repositorios/Servicios. No tiene dependencias.
* `MiniETRM.Application`: Casos de uso de negocio (Handlers de MediatR para Comandos y Consultas) y DTOs.
* `MiniETRM.Infrastructure`: Implementación de la persistencia (EF Core DbContext), el motor de caché en memoria y el simulador de mercado (`BackgroundService`).
* `MiniETRM.Api`: Endpoints RESTful, configuración de Inyección de Dependencias, Middleware y Swagger.

## 🚀 Cómo ejecutar el proyecto (Docker)

El proyecto está completamente dockerizado. No necesitas instalar el SDK de .NET ni configurar un servidor de SQL Server localmente.

### Requisitos previos:
* Docker Desktop instalado.

### Pasos:

1. Clonar el repositorio o descargar solo el archivo `docker-compose.yml`.
2. Abrir una terminal en la raíz del proyecto (donde se encuentra el archivo `docker-compose.yml`).
3. Ejecutar el siguiente comando:
   ```bash
   docker-compose up -d
4. Esperar unos segundos a que la base de datos se inicialice.
5. Navegar a http://localhost:8080/scalar para interactuar con los endpoints:

    GET /api/marketdata/latest: Ver la fluctuación de precios en tiempo real.

    POST /api/trades: Ejecutar una nueva operación.

    GET /api/trades/{id}/pnl: Consultar la ganancia/pérdida en vivo.
6. Para detener y limpiar los contenedores, ejecuta:
    ```bash
    docker-compose down

<details>