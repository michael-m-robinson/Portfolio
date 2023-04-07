
//Image upload handler callback
const image_handler = (blobInfo, progress) => new Promise((resolve, reject) => {
    //AntiForgeryToken
    const token = $(`input[name='__RequestVerificationToken']`).val();
    const xhr = new XMLHttpRequest();

    xhr.withCredentials = false;
    xhr.open('POST', '/Posts/UploadPostImageFile');

    xhr.upload.onprogress = (e) => {
        progress(e.loaded / e.total * 100);
    };

    xhr.onload = () => {
        if (xhr.status === 403) {
            reject({ message: 'HTTP Error: ' + xhr.status, remove: true });
            return;
        }

        if (xhr.status < 200 || xhr.status >= 300) {
            reject('HTTP Error: ' + xhr.status);
            return;
        }

        const json = JSON.parse(xhr.responseText);

        if (!json || typeof json.location != 'string') {
            reject('Invalid JSON: ' + xhr.responseText);
            return;
        }

        resolve(json.location);
    };

    xhr.onerror = () => {
        reject('Image upload failed due to a XHR Transport error. Code ' + xhr.status);
    };

    const formData = new FormData();
    formData.append('__RequestVerificationToken', token);
    formData.append('file', blobInfo.blob(), blobInfo.filename());
    console.log(formData);
    xhr.send(formData);
});

$('document').ready(function () {
    let content = '';
    if (document.getElementById('ArticleData') !== undefined) {
        content = document.getElementById('ArticleData').value;
    }

    //Initialize tinymce
    tinymce.init({
        selector: `[name='Post.Content']`,
        plugins: 'image',
        images_upload_handler: image_handler,
        setup: function (editor) {
            editor.on('init', function (e) {
                editor.setContent(content);
            });
        },
    });
});



