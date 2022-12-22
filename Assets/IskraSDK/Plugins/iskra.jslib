
var iskraWebPlugin = {
               
     $state: {
       window: null,
       messageListener: null
     },
     
     Open: function(url, query, redirectUrl, obj) {
       var urlStr = Pointer_stringify(url);
       var queryStr = Pointer_stringify(query);
       var redirectUrlStr = Pointer_stringify(redirectUrl);
       var origin = window.location.origin;
       
       var url = urlStr + queryStr + "&origin=" + origin;
       var target = "IskraWindow";
       var windowFeatures = "menubar=1,resizable=1,width=400,height=800";
       //state.window = window.open(urlStr+ "?appId="+ appIdStr +"&origin=" + origin,"mywindow","menubar=1,resizable=1,width=320,height=800");
       state.window = window.open(url, target, windowFeatures);
       state.messageListener = function(event) {
         if (event.origin !== redirectUrlStr)
           return ; 
             
         if (event.data === 'close') {
           Module['dynCall_vi'](obj, null);
           return ;
         }
           
         // data
         var rawdata = event.data;
         var bufferSize = lengthBytesUTF8(rawdata) + 1;
         //Allocate memory space
         var buffer = _malloc(bufferSize);
         //Copy old data to the new one then return it
         stringToUTF8(rawdata, buffer, bufferSize);
           
         // https://velog.io/@fgprjs/Unity%EC%97%90%EC%84%9C-JavaScript-Plugin-%EB%A7%8C%EB%93%A4%EA%B8%B0-3-JS-Websocket-%EC%8B%9C%ED%96%89%EC%B0%A9%EC%98%A4
         // < unity 2021.2
         //Runtime.dynCall('vi', obj, buffer);
         // unity 2021.2
         Module['dynCall_vi'](obj, buffer); 
       };
       window.addEventListener("message", state.messageListener, false);
     },
     
     Close: function() {
       if (state.window) {
         state.window.close();
         window.removeEventListener("message", state.messageListener, false);
         state.window = null;
         state.messageListener = null;
       }
     },

};
autoAddDeps(iskraWebPlugin, '$state');
mergeInto(LibraryManager.library, iskraWebPlugin);