function dataURIToBlob(dataURI) {
    const splitDataURI = dataURI.split(',')
    const byteString = splitDataURI[0].indexOf('base64') >= 0 ? atob(splitDataURI[1]) : decodeURI(splitDataURI[1])
    const mimeString = splitDataURI[0].split(':')[1].split(';')[0]

    const ia = new Uint8Array(byteString.length)
    for (let i = 0; i < byteString.length; i++)
        ia[i] = byteString.charCodeAt(i)

    return new Blob([ia], { type: mimeString })
}

function GetErrorMessage(statusCode, content) {
    if (statusCode >= 200 && statusCode <= 299)
        return null;

    if (content.errors) {
        return `${content.title ?? content} (${content.errors[0].message})`;
    }

    return content.detail ?? content.title ?? content;
}

function sleep(time) {
    return new Promise((resolve) => {
        setTimeout(resolve, time);
    });
}

async function copyToClipboard(element, text) {

    let tooltip = bootstrap.Tooltip.getInstance(element);
    tooltip.hide();

    navigator.clipboard.writeText(text);

    element.setAttribute('data-bs-title', 'Copied!');

    tooltip = new bootstrap.Tooltip(element);
    tooltip.show();

    await sleep(3000);
    tooltip.hide();

    // Resets the tooltip title
    element.setAttribute('data-bs-title', 'Copy to clipboard');
    new bootstrap.Tooltip(element);
}

async function getUserAsync(accessToken, language) {
    var response = await fetch('api/me', {
        method: "GET",
        headers: {
            "Content-type": "application/json",
            "Accept-Language": language,
            "Authorization": `bearer ${accessToken}`
        }
    });

    var content = await response.json();
    var errorMessage = GetErrorMessage(response.status, content);
}