mergeInto(LibraryManager.library, {
    GeneratePDFWebGL: function (filename, data, size) {
        var contentType = 'application/pdf';
        var bytes = new Uint8Array(size);
        for (var i = 0; i < size; i++) {
            bytes[i] = HEAPU8[data + i];
        }

        var blob = new Blob([bytes], { type: contentType });
        var url = URL.createObjectURL(blob);

        var element = document.createElement('a');
        element.href = url;
        element.download = UTF8ToString(filename);
        element.style.display = 'none';

        document.body.appendChild(element);
        element.click();
        document.body.removeChild(element);
        URL.revokeObjectURL(url);
    },
});