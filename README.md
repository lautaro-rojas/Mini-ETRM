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

1. Clonar el repositorio.
2. Abrir una terminal en la raíz del proyecto (donde se encuentra el archivo `docker-compose.yml`).
3. Ejecutar el siguiente comando:
   ```bash
   docker-compose up --build -d
4. Esperar unos segundos a que la base de datos se inicialice.
5. Navegar a http://localhost:8080/scalar para interactuar con los endpoints:

    GET /api/marketdata/latest: Ver la fluctuación de precios en tiempo real.

    POST /api/trades: Ejecutar una nueva operación.

    GET /api/trades/{id}/pnl: Consultar la ganancia/pérdida en vivo.
6. Para detener y limpiar los contenedores, ejecuta:
    ```bash
    docker-compose down