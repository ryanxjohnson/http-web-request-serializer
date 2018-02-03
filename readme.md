# Http Web Request Serializer

## What this libary will accomplish:
This library will parse raw HTTP request text to a serialized object.
Client code can save all the information about a particular web request in permanent storage, decoupling the request properties from any particular framework.

This libary will also build a .NET `HttpWebRequest` object from the raw request text or the JSON object.

Serialization options allows the client to ignore certain headers like cookies.

## Parsing Usage:

Sample Raw Web Request
```
var sampleGet = @"
GET https://httpbin.org/get HTTP/1.1
Host: httpbin.org
Connection: keep-alive
User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/62.0.3202.94 Safari/537.36
Upgrade-Insecure-Requests: 1
Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8
Accept-Encoding: gzip, deflate, br
Accept-Language: en-US,en;q=0.9";
```

#### Note on SerializationOptions:
You can apecify what the parser should not serialize. Pass SerializationOptions as an optional parameter to the parser. For example, it might not make sense to serialize cookies if you are dynamically adding them later.

```
SerializationOptions options = new SerializationOptions(new [] { SerializationOptionKey.Cookie });
```

or

```
SerializationOptions options = new SerializationOptions();
options.IgnoreKey(SerializationOptionKey.Cookie);
```

### Parse to JSON:
```
var json = HttpParser.GetRawRequestAsJson(sampleGet);

returns "{""Uri"":""https://httpbin.org/get"",""Headers"":{""Method"":""GET"",""HttpVersion"":""HTTP/1.1"",""Host"":""httpbin.org"",""Connection"":""keep-alive"",""User-Agent"":""Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/62.0.3202.94 Safari/537.36"",""Upgrade-Insecure-Requests"":""1"",""Accept"":""text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8"",""Accept-Encoding"":""gzip, deflate, br"",""Accept-Language"":""en-US,en;q=0.9""},""Cookie"":{},""RequestData"":null}"
```

### Parse to IDictionary<string, object>:
```
var dict = HttpParser.GetRawRequestAsDictionary(sampleGet);
```

### Parse to ParsedRequest Object:
```
var parsed = HttpParser.GetParsedRequest(sampleGet);
```

## Build .NET HttpWebRequest Usage:
```
var request = RequestBuilder.CreateWebRequestFromJson(json);
```

```
var request = RequestBuilder.CreateWebRequestFromDictionary(dict);
```

```
var request = RequestBuilder.CreateWebRequestFromParsedRequest(parsed);
```

### Execute Web Request and capture response:
```
string response = request.GetResponseAsString();
```