# Mediator Caching  

For more information and context, see my blog [post](https://www.kaels-kabbage.com/post/mediatr-caching/).  

----  

The idea is that one would reuse Mediatr queries throughout an application, many of which would be repeated as time goes on.    
For performance reasons, you want to cache something somewhere.  
This demonstrates a way to fairly easily add caching and invalidation to your system.  
It provides several caching and invalidation methods.  