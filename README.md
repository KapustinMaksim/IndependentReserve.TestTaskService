# Requirements:
1. Visual studio 2019
2. dotnet-core 3.0 ([download])

# API

To check the operation, use the method: **/Public/GetAllOrders** wich allow two parameters:
1. primaryCurrencyCode
2. secondaryCurrencyCode

For more informationt see [independentreserve API]


Example full url like: 
`http://localhost:64656/Public/GetAllOrders?primaryCurrencyCode=xbt&secondaryCurrencyCode=aud`

[download]: https://dotnet.microsoft.com/download/dotnet-core/3.0
[independentreserve API]: https://www.independentreserve.com/api#GetAllOrders
