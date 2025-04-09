const appInTossUtilityLib = {
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
}

mergeInto(LibraryManager.library, appInTossUtilityLib);