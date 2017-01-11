# NetworkService 


**NetworkService** es una utilidad para poder llamar a diferentes APIs desde **C#**. Está basado en la clase _HttpClient_ de _System.Net.Http_. Para ello la clase _NetworkService_ del proyecto _NetworkService.Impl_ implementa toda la funcionalidad cumpliendo el interfaz _INetworkService_ que se puede acceder a él en el proyecto _NetworkService.Contracts_.

La interfaz que expone la clase es la siguiente:

```csharp
namespace NetworkService.Contracts
{
    public interface INetworkService
    {
        void InitToken(IToken token);
        Task<TResponse> Get<TResponse, TError>(string url)
            where TError : IError;
        Task<TResponse> PostWithFormUrlEncoded<TResponse, TError>(string url, IDictionary<string, string> parameters)
            where TError : IError;
        Task<TResponse> Post<TResponse, TRequest, TError>(string url, TRequest parameter)
            where TError : IError;
        Task<TResponse> Put<TResponse, TRequest, TError>(string url, TRequest parameter)
            where TError : IError;
        Task<T> Delete<T, TError>(string url)
            where TError : IError;
        Task<T> Patch<T, TError>(string url, T parameter)
            where TError : IError;
        Task<TResponse> PostFile<TResponse, TError>(string url, IFile file)
            where TError : IError;
        Task GetAuthToken<TokenResponse>(string authUrl, IDictionary<string, string> configurationParameters)
            where TokenResponse : IToken;
        Task<TResponse> NetworkCallWithoutAuthAndRetry<TResponse>(Func<Task<TResponse>> action);
        Task<TResponse> NetworkCallRetryWithoutAuth<TResponse>(Func<Task<TResponse>> action, TimeSpan sleepPeriod, int retryCount);
        Task<TResponse> NetworkCallWithAuthAndRetry<TResponse>(Func<Task<TResponse>> action, Func<Task> AuthAction, TimeSpan sleepPeriod, int retryCount);
    }
}
```

# Características

* Hacer llamadas **GET, POST, PUT, DELETE y PATCH** a una url, añadiendo un objeto _TError_ que cumpla el interfaz _IError_ expuesto en el proyecto _NetworkService.Contracts_ para recoger los mensajes de error que se puedan devolver de la llamada. En el caso de las llamadas PUT y POST se podrá definir el tipo de objeto del parámetro que va en el cuerpo de la llamada y pasar el mismo como un parámetro más.
* Hacer llamadas **POST** con los parámetros por **form-urlencoded**. Tiene las características de las llamadas anteriores.
* Hacer llamadas **GET, POST, PUT, DELETE y PATCH** cuyo resultado sea un archivo. Para ello deben cumplir el interfaz _IFile_ el objeto de vuelta. 
* Hacer llamadas del tipo de las anteriores pasandolas como **Func** como parámetro y además tener **reintento** y **autorización** en ellas. Para eso se usan los tres últimos métodos, que según el que elijamos hará un número de reintentos y tendrán un delay entre sí como el que indiquemos en los parámetros que pasemos.
* Se puede inicializar directamente un **Token** con InitToken o pedirlo con GetAuthToken. El objeto que se obtiene tiene que cumplir el interfaz _IToken_. Una vez inicializado este _Token_ si usamos la llamada con autorización, el mismo se añadirá a la llamada.

# Getting Started

## Instalación

Para hacer uso del NetworkService sólo tienes que llevarte el código del mismo a tu proyecto y actualizar los paquetes de Nuget.

Es importante mantener la coherencia entre los paquetes usados. Así el proyecto NetworkService usa los paquetes de Nuget:

* Microsoft.Net.Http versión 2.2.29
* Newtonson.Json versión 9.0.1

## Modo de uso

### Llamada directa

Se pueden llamar directamente al api con los métodos Get, Put, Post, Delete y Patch como por ejemplo:

```csharp
var url = "http://localhost:56421/api/test/get-test-boolean";
try
{
    return await _networkService.Get<bool, ErrorModel>(url);
}
catch(Exception e)
{
    return e;
}
```

### Reintento y autorización

Pero lo normal es que se usen los métodos wrapeados con una Func de los anteriores para que éstos tengan reintento y autenticación por defecto. Se puede añadir un Token con InitToken o se puede indicar la Func de autenticación en el mismo:

#### Con autorización inicializada

```csharp
var url = "http://localhost:56421/api/test/get-test-boolean";
_networkService.InitToken(token);
await _networkService.NetworkCallRetryWithoutAuth(async () => await _networkService.Get<bool, ErrorModel>(url), new TimeSpan(0,0,3), 2);
```

#### Con llamada de autorización

```csharp
var url = "http://localhost:56421/api/test/get-test-boolean";
var configurationParameters = new Dictionary<string, string>
{
    { "grant_type", "password" },
    { "username", "username" },
    { "password", "1234" }
};
await _networkService.NetworkCallWithAuthAndRetry(
    async () => await _networkService.Get<bool, ErrorModel>(url),
    async () => await _networkService.GetAuthToken<TokenResponse>(url, configurationParameters),
    new System.TimeSpan(0,0,3), 2);
```

### Excepciones soportadas

Las excepciones, implementadas en el proyecto _NetworkService.Contracts_ que se pueden recibir son:


* Errores específicos del API:
  * **NetworkServiceBadRequestException**: Error 400 del API, BadRequest.
  * **NetworkServiceUnauthorizedException**: Error 401 del API, Unhautorized.
  * **NetworkServiceForbiddenException**: Error 403 del API, Forbidden.
  * **NetworkServiceNotFoundException**: Error 404 del API, NotFound
  * **NetworkServiceInternalServerErrorException**: Error 500 del API, Internal Server Error.

* Errores del API:
  * **NetworkServiceInformationalException**: Error con código 1xx procesando la llamada al API.
  * **NetworkServiceRedirectionException**: Error con código 3xx procesando la llamada al API.
  * **NetworkServiceClientErrorException**: Error con código 4xx procesando la llamada al API.
  * **NetworkServiceServerErrorException**: Error con código 5xx procesando la llamada al API.

* Errores del resultado de la llamada:
  * **NetworkServiceContentTypeException**: El resultado de la llamada no es un json y no cumple el interfaz _IFile_ el objeto esperado.
* Errores genéricos:
  * **NetworkServiceException**: Error en la llamada.


# Test

El proyecto contiene una carpeta test con todos los test. Se separan en los test de unidad y los test funcionales.

## Test Unidad

Los test de unidad copian la estructura de src para en cada uno de sus respectivos proyectos tener los tests pertinentes.

## Test Funcionales

La carpeta TestRunner contiene una aplicación WPF para ver el un ejemplo de funcionamiento del NetworkService. Esta aplicación llama a la api que está implementada en la carpeta Api del propio TestRunner, con lo que para poder probarlo hay que poner en marcha ambos proyectos.

La clase _NetworkServiceViewModel_ tiene el código de ejemplo del NetworkService.

# Contrubuir

El proyecto de Network Service está ubicado en el [Foils Web](https://plainconcepts.visualstudio.com/PlainConcepts.Foils.Web/_git/NetworkService) del equipo Windows Platform Team.