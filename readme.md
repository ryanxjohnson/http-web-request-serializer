# Http Web Request Serializer

## What this libary will accomplish
This library will parse raw HTTP request text to a serialized object.
Client code can save all the information about a particular web request in permanent storage, decoupling the request properties from any particular framework.

This libary will also build a .NET `HttpWebRequest` object from the raw request text or the JSON object.

Serialization options allows the client to ignore certain headers like cookies.

