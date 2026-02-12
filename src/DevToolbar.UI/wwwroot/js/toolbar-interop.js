// DevToolbar JS Interop helpers

window.DevToolbar = {
    /**
     * Downloads a string as a CSV file.
     * @param {string} filename - The filename for the download.
     * @param {string} csvContent - The CSV content to download.
     */
    downloadCsv: function (filename, csvContent) {
        var blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
        var link = document.createElement('a');
        var url = URL.createObjectURL(blob);
        link.href = url;
        link.download = filename;
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
        setTimeout(function () { URL.revokeObjectURL(url); }, 1000);
    }
};
