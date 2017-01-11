# NetworkService 


**NetworkService** es una utilidad para poder llamar a diferentes APIs desde **C#**. Est� basado en la clase _HttpClient_ de _System.Net.Http_. Para ello la clase _NetworkService_ del proyecto _NetworkService.Impl_ implementa toda la funcionalidad cumpliendo el interfaz _INetworkService_ que se puede acceder a �l en el proyecto _NetworkService.Contracts_.

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

# Caracter�sticas

* Hacer llamadas **GET, POST, PUT, DELETE y PATCH** a una url, a�adiendo un objeto _TError_ que cumpla el interfaz _IError_ expuesto en el proyecto _NetworkService.Contracts_ para recoger los mensajes de error que se puedan devolver de la llamada. En el caso de las llamadas PUT y POST se podr� definir el tipo de objeto del par�metro que va en el cuerpo de la llamada y pasar el mismo como un par�metro m�s.
* Hacer llamadas **POST** con los par�metros por **form-urlencoded**. Tiene las caracter�sticas de las llamadas anteriores.
* Hacer llamadas **GET, POST, PUT, DELETE y PATCH** cuyo resultado sea un archivo. Para ello deben cumplir el interfaz _IFile_ el objeto de vuelta. 
* Hacer llamadas del tipo de las anteriores pasandolas como **Func** como par�metro y adem�s tener **reintento** y **autorizaci�n** en ellas. Para eso se usan los tres �ltimos m�todos, que seg�n el que elijamos har� un n�mero de reintentos y tendr�n un delay entre s� como el que indiquemos en los par�metros que pasemos.
* Se puede inicializar directamente un **Token** con InitToken o pedirlo con GetAuthToken. El objeto que se obtiene tiene que cumplir el interfaz _IToken_. Una vez inicializado este _Token_ si usamos la llamada con autorizaci�n, el mismo se a�adir� a la llamada.

# Getting Started

## Instalaci�n

Para hacer uso del NetworkService s�lo tienes que llevarte el c�digo del mismo a tu proyecto y actualizar los paquetes de Nuget.

Es importante mantener la coherencia entre los paquetes usados. As� el proyecto NetworkService usa los paquetes de Nuget:

* Microsoft.Net.Http versi�n 2.2.29
* Newtonson.Json versi�n 9.0.1

## Modo de uso

### Llamada directa

Se pueden llamar directamente al api con los m�todos Get, Put, Post, Delete y Patch como por ejemplo:

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

### Reintento y autorizaci�n

Pero lo normal es que se usen los m�todos wrapeados con una Func de los anteriores para que �stos tengan reintento y autenticaci�n por defecto. Se puede a�adir un Token con InitToken o se puede indicar la Func de autenticaci�n en el mismo:

#### Con autorizaci�n inicializada

```csharp
var url = "http://localhost:56421/api/test/get-test-boolean";
_networkService.InitToken(token);
await _networkService.NetworkCallRetryWithoutAuth(async () => await _networkService.Get<bool, ErrorModel>(url), new TimeSpan(0,0,3), 2);
```

#### Con llamada de autorizaci�n

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


* Errores espec�ficos del API:
  * **NetworkServiceBadRequestException**: Error 400 del API, BadRequest.
  * **NetworkServiceUnauthorizedException**: Error 401 del API, Unhautorized.
  * **NetworkServiceForbiddenException**: Error 403 del API, Forbidden.
  * **NetworkServiceNotFoundException**: Error 404 del API, NotFound
  * **NetworkServiceInternalServerErrorException**: Error 500 del API, Internal Server Error.

* Errores del API:
  * **NetworkServiceInformationalException**: Error con c�digo 1xx procesando la llamada al API.
  * **NetworkServiceRedirectionException**: Error con c�digo 3xx procesando la llamada al API.
  * **NetworkServiceClientErrorException**: Error con c�digo 4xx procesando la llamada al API.
  * **NetworkServiceServerErrorException**: Error con c�digo 5xx procesando la llamada al API.

* Errores del resultado de la llamada:
  * **NetworkServiceContentTypeException**: El resultado de la llamada no es un json y no cumple el interfaz _IFile_ el objeto esperado.
* Errores gen�ricos:
  * **NetworkServiceException**: Error en la llamada.


# Test

El proyecto contiene una carpeta test con todos los test. Se separan en los test de unidad y los test funcionales.

## Test Unidad

Los test de unidad copian la estructura de src para en cada uno de sus respectivos proyectos tener los tests pertinentes.

## Test Funcionales

La carpeta TestRunner contiene una aplicaci�n WPF para ver el un ejemplo de funcionamiento del NetworkService. Esta aplicaci�n llama a la api que est� implementada en la carpeta Api del propio TestRunner, con lo que para poder probarlo hay que poner en marcha ambos proyectos.

La clase _NetworkServiceViewModel_ tiene el c�digo de ejemplo del NetworkService.

# Contrubuir

El proyecto de Network Service est� ubicado en el [Foils Web](https://plainconcepts.visualstudio.com/PlainConcepts.Foils.Web/_git/NetworkService) del equipo Windows Platform Team.