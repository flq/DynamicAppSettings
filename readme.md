# DynamicAppSettings

This package gives you dynamic access to your AppSettings or some other arbitrary NameValueCollection. 

It supports nesting a la _Server.Port_, _Server.Address_ and allows casting to other types by writing e.g. 

    int port = Settings.Server.Port 
    
The functionality is based on TypeConverters, therefore a QuickConverter is provided as a base class for your convenience.

## Licensing ##

Copyright 2012 Frank-Leonardo Quednau ([realfiction.net](http://realfiction.net)) 
Licensed under the Apache License, Version 2.0 (the "License"); 
you may not use this solution except in compliance with the License. 
You may obtain a copy of the License at 

http://www.apache.org/licenses/LICENSE-2.0 

Unless required by applicable law or agreed to in writing, 
software distributed under the License is distributed on an "AS IS" 
BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
See the License for the specific language governing permissions and limitations under the License. 