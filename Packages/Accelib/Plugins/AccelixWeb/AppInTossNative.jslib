const appInTossUtilityLib = {

    GetDeviceId: function()
    {
        try
        {
            let returnStr = window.getDeviceId();
            let bufferSize = lengthBytesUTF8(returnStr) + 1;
            let buffer = _malloc(bufferSize);
            stringToUTF8(returnStr, buffer, bufferSize);
            return buffer;
        }catch(e)
        {
            console.error(e);
        }
    },

    GetOperationalEnvironment: function()
    {
        try
        {
            let returnStr = window.getOperationalEnvironment();
            let bufferSize = lengthBytesUTF8(returnStr) + 1;
            let buffer = _malloc(bufferSize);
            stringToUTF8(returnStr, buffer, bufferSize);
            return buffer;
        }catch(e)
        {
            console.error(e);
        }
    },

    GetTossAppVersion: function()
    {
        try
        {
            let returnStr = window.getTossAppVersion();
            let bufferSize = lengthBytesUTF8(returnStr) + 1;
            let buffer = _malloc(bufferSize);
            stringToUTF8(returnStr, buffer, bufferSize);
            return buffer;
        }catch(e)
        {
            console.error(e);
        }
    },

    GetPlatformOS: function()
    {
        try
        {
            let returnStr = window.getPlatformOS();
            let bufferSize = lengthBytesUTF8(returnStr) + 1;
            let buffer = _malloc(bufferSize);
            stringToUTF8(returnStr, buffer, bufferSize);
            return buffer;
        }catch(e)
        {
            console.error(e);
        }
    },

    GetSchemeUri: function()
    {
        try
        {
            let returnStr = window.getSchemeUri();
            let bufferSize = lengthBytesUTF8(returnStr) + 1;
            let buffer = _malloc(bufferSize);
            stringToUTF8(returnStr, buffer, bufferSize);
            return buffer;
        }catch(e)
        {
            console.error(e);
        }
    },

    GetLocale: function()
    {
        try
        {
            let returnStr = window.getLocale();
            let bufferSize = lengthBytesUTF8(returnStr) + 1;
            let buffer = _malloc(bufferSize);
            stringToUTF8(returnStr, buffer, bufferSize);
            return buffer;
        }catch(e)
        {
            console.error(e);
        }
    },

    HandleShare: function(msg)
    {
        try
        {
            window.handleShare(UTF8ToString(msg));
        }catch(e)
        {
            console.error(e);
        }
    },

    HandleHapticFeedback: function(typeName)
    {
        try
        {
            window.handleHapticFeedback(UTF8ToString(typeName));
        }catch(e)
        {
            console.error(e);
        }
    },

    GetSafeAreaInsets: function()
    {
        try
        {
            let returnStr = window.getSafeAreaInsets();
            let bufferSize = lengthBytesUTF8(returnStr) + 1;
            let buffer = _malloc(bufferSize);
            stringToUTF8(returnStr, buffer, bufferSize);
            return buffer;
        }catch(e)
        {
            console.error(e);
        }
    },

    CallAds: function(_unityCallerName, _unitId, _isLoad, _isInterstitial)
    {
        try
        {
            const unityCallerName = UTF8ToString(_unityCallerName);
            const unitId = UTF8ToString(_unitId);

            if(_isInterstitial)
            {
                if(_isLoad)
                    {window.aitAds.loadInterstitial(unityCallerName, unitId);}
                else
                    {window.aitAds.showInterstitial(unityCallerName, unitId);}
            }else
            {
                if(_isLoad)
                    {window.aitAds.loadRewarded(unityCallerName, unitId);}
                else
                    {window.aitAds.showRewarded(unityCallerName, unitId);}
            }
        }catch(e)
        {
            console.error(e);
        }
    },

}

mergeInto(LibraryManager.library, appInTossUtilityLib);