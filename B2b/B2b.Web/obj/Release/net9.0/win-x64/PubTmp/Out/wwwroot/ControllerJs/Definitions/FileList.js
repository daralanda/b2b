document.addEventListener('DOMContentLoaded', () => {
    // DOM Elemanları
    const searchInput = document.getElementById('fileSearchInput');
    const sortByEl = document.getElementById('sortBy');
    const sortOrderEl = document.getElementById('sortOrder');
    const btnRefresh = document.getElementById('btnRefresh');
    const fileGrid = document.getElementById('fileGrid');

    // Ön İzleme Modalı Elemanları
    const previewModalEl = document.getElementById('filePreviewModal');
    const previewModal = new bootstrap.Modal(previewModalEl);
    const previewModalLabel = document.getElementById('filePreviewModalLabel');
    const previewContainer = document.getElementById('previewContainer');
    const previewFileInfo = document.getElementById('previewFileInfo');
    const btnPreviewDownload = document.getElementById('btnPreviewDownload');

    // Dosya Yükleme Modalı Elemanları
    const uploadModalEl = document.getElementById('fileUploadModal');
    const fileInput = document.getElementById('fileInput');
    const dropZone = document.getElementById('dropZone');
    const selectedFilesContainer = document.getElementById('selectedFilesContainer');
    const selectedFilesList = document.getElementById('selectedFilesList');
    const btnUploadSubmit = document.getElementById('btnUploadSubmit');
    const uploadProgressContainer = document.getElementById('uploadProgressContainer');
    const uploadProgressBar = document.getElementById('uploadProgressBar');

    let selectedFiles = [];
    let searchTimeout = null;

    // İlk Yükleme
    loadFiles();

    // Event Listener'lar
    sortByEl.addEventListener('change', loadFiles);
    sortOrderEl.addEventListener('change', loadFiles);
    btnRefresh.addEventListener('click', loadFiles);

    // Live Search (300ms Debounce)
    searchInput.addEventListener('input', () => {
        clearTimeout(searchTimeout);
        searchTimeout = setTimeout(() => {
            loadFiles();
        }, 300);
    });

    // --- YÜKLEME VE SÜRÜKLE-BIRAK İŞLEMLERİ ---

    // Dosya seçimi değiştiğinde
    fileInput.addEventListener('change', (e) => {
        handleFileSelection(Array.from(e.target.files));
    });

    // Sürükle - Bırak Efektleri
    ['dragenter', 'dragover'].forEach(eventName => {
        dropZone.addEventListener(eventName, (e) => {
            e.preventDefault();
            e.stopPropagation();
            dropZone.classList.add('border-primary', 'bg-soft-primary');
        }, false);
    });

    ['dragleave', 'drop'].forEach(eventName => {
        dropZone.addEventListener(eventName, (e) => {
            e.preventDefault();
            e.stopPropagation();
            dropZone.classList.remove('border-primary', 'bg-soft-primary');
        }, false);
    });

    dropZone.addEventListener('drop', (e) => {
        const dt = e.dataTransfer;
        const files = dt.files;
        handleFileSelection(Array.from(files));
    });

    function handleFileSelection(files) {
        if (!files || files.length === 0) return;

        // Mevcut seçilenlerin üzerine ekle
        selectedFiles = [...selectedFiles, ...files];
        renderSelectedFiles();
    }

    function renderSelectedFiles() {
        selectedFilesList.innerHTML = '';
        if (selectedFiles.length === 0) {
            selectedFilesContainer.classList.add('d-none');
            btnUploadSubmit.disabled = true;
            return;
        }

        selectedFilesContainer.classList.remove('d-none');
        btnUploadSubmit.disabled = false;

        selectedFiles.forEach((file, index) => {
            const li = document.createElement('li');
            li.className = 'list-group-item d-flex align-items-center justify-content-between p-2 font-size-12';
            li.innerHTML = `
                <div class="text-truncate me-2">
                    <i class="bx bx-file me-1 text-primary"></i>
                    <strong>${escapeHtml(file.name)}</strong> 
                    <span class="text-muted">(${formatBytes(file.size)})</span>
                </div>
                <button type="button" class="btn btn-sm btn-soft-danger py-0 px-1" data-remove-index="${index}">
                    <i class="bx bx-x"></i>
                </button>
            `;
            selectedFilesList.appendChild(li);
        });
    }

    // Seçilen listeden dosya çıkarma
    selectedFilesList.addEventListener('click', (e) => {
        const btn = e.target.closest('[data-remove-index]');
        if (!btn) return;
        const index = parseInt(btn.dataset.removeIndex, 10);
        selectedFiles.splice(index, 1);
        renderSelectedFiles();
    });

    // Yüklemeyi Başlat Butonu
    btnUploadSubmit.addEventListener('click', () => {
        if (selectedFiles.length === 0) return;

        const formData = new FormData();
        selectedFiles.forEach(file => {
            formData.append('files', file);
        });

        btnUploadSubmit.disabled = true;
        uploadProgressContainer.classList.remove('d-none');
        uploadProgressBar.style.width = '0%';

        // XMLHttpRequest ile progress takibi yapabilen yükleme
        const xhr = new XMLHttpRequest();
        xhr.open('POST', '/api/FileListApi/UploadFiles', true);

        // JWT token ekle
        const token = localStorage.getItem('token');
        if (token) {
            xhr.setRequestHeader('Authorization', `Bearer ${token}`);
        }

        xhr.upload.onprogress = (e) => {
            if (e.lengthComputable) {
                const percentComplete = Math.round((e.loaded / e.total) * 100);
                uploadProgressBar.style.width = percentComplete + '%';
            }
        };

        xhr.onload = function () {
            btnUploadSubmit.disabled = false;
            uploadProgressContainer.classList.add('d-none');

            if (xhr.status === 200) {
                const res = JSON.parse(xhr.responseText);
                if (res.Success || res.success) {
                    Swal.fire('Başarılı!', res.Message || res.message, 'success');

                    // Modalı kapat ve değişkenleri sıfırla
                    resetUploadModal();
                    const uploadModalInstance = bootstrap.Modal.getInstance(uploadModalEl);
                    if (uploadModalInstance) uploadModalInstance.hide();

                    loadFiles();
                } else {
                    Swal.fire('Hata!', res.Message || res.message || 'Yükleme başarısız.', 'error');
                }
            } else {
                Swal.fire('Hata!', 'Dosya yüklenirken sunucuda bir hata oluştu.', 'error');
            }
        };

        xhr.onerror = function () {
            btnUploadSubmit.disabled = false;
            uploadProgressContainer.classList.add('d-none');
            Swal.fire('Hata!', 'Ağ hatası oluştu.', 'error');
        };

        xhr.send(formData);
    });

    function resetUploadModal() {
        selectedFiles = [];
        fileInput.value = '';
        renderSelectedFiles();
        uploadProgressBar.style.width = '0%';
        uploadProgressContainer.classList.add('d-none');
    }

    // Modal kapandığında sıfırla
    uploadModalEl.addEventListener('hidden.bs.modal', resetUploadModal);


    // --- GRID TIKLAMALARI VE YARDIMCI FONKSİYONLAR ---

    fileGrid.addEventListener('click', (e) => {
        const btn = e.target.closest('[data-action]');
        if (!btn) return;

        if (btn.tagName === 'A') {
            e.preventDefault();
        }

        const action = btn.dataset.action;
        const filename = btn.dataset.filename;
        const url = btn.dataset.url;
        const ext = btn.dataset.ext;
        const size = btn.dataset.size;

        if (action === 'preview') {
            openPreviewModal(filename, url, ext, size);
        } else if (action === 'copy-link') {
            copyToClipboard(url);
        } else if (action === 'rename') {
            renameFile(filename);
        } else if (action === 'delete') {
            deleteFile(filename);
        }
    });

    function getAuthHeader() {
        const token = localStorage.getItem('token');
        return token ? { 'Authorization': `Bearer ${token}` } : {};
    }

    function loadFiles() {
        const search = searchInput.value.trim();
        const sortBy = sortByEl.value;
        const order = sortOrderEl.value;

        const queryParams = new URLSearchParams({
            search: search,
            sortBy: sortBy,
            order: order
        });

        fetch(`/api/FileListApi/GetFiles?${queryParams.toString()}`, {
            method: 'GET',
            headers: {
                ...getAuthHeader(),
                'Content-Type': 'application/json'
            }
        })
            .then(res => res.json())
            .then(res => {
                const isSuccess = res.Success !== undefined ? res.Success : res.success;
                const fileData = res.Data !== undefined ? res.Data : res.data;

                if (isSuccess) {
                    renderGrid(fileData);
                } else {
                    Swal.fire('Hata!', res.Message || res.message || 'Dosyalar yüklenemedi.', 'error');
                }
            })
            .catch(() => {
                Swal.fire('Hata!', 'Sunucuyla iletişim kurulurken bir sorun oluştu.', 'error');
            });
    }

    function renderGrid(files) {
        fileGrid.innerHTML = '';

        if (!files || files.length === 0) {
            fileGrid.innerHTML = `
                <div class="col-12 text-center py-5">
                    <i class="bx bx-folder-open display-4 text-muted"></i>
                    <p class="text-muted mt-2">Aramanıza uygun dosya bulunamadı.</p>
                </div>`;
            return;
        }

        files.forEach(file => {
            const fileName = file.Name || file.name;
            const fileFormattedSize = file.FormattedSize || file.formattedSize;
            const fileCreatedDate = file.CreatedDate || file.createdDate;
            const fileUrl = file.Url || file.url;
            const fileExtension = (file.Extension || file.extension || '').replace('.', '').toLowerCase();

            const cardCol = document.createElement('div');
            cardCol.className = 'col-xl-2 col-lg-3 col-md-4 col-sm-6 mb-3';

            cardCol.innerHTML = `
                <div class="card border shadow-none mb-0 h-100">
                    <div class="position-relative bg-light rounded-top d-flex align-items-center justify-content-center overflow-hidden" 
                         style="height: 110px; cursor: pointer;" 
                         data-action="preview" data-filename="${fileName}" data-url="${fileUrl}" data-ext="${fileExtension}" data-size="${fileFormattedSize}">
                        ${renderThumbnail(fileUrl, fileExtension)}
                        <span class="position-absolute top-0 end-0 m-1 badge bg-dark bg-opacity-50 font-size-10">${fileExtension.toUpperCase()}</span>
                    </div>

                    <div class="card-body p-2">
                        <div class="d-flex align-items-start justify-content-between">
                            <div class="overflow-hidden me-1">
                                <h5 class="font-size-13 text-truncate mb-1" title="${fileName}">
                                    <a href="#" class="text-dark" data-action="preview" data-filename="${fileName}" data-url="${fileUrl}" data-ext="${fileExtension}" data-size="${fileFormattedSize}">${fileName}</a>
                                </h5>
                                <p class="text-muted font-size-11 mb-0">${fileFormattedSize}</p>
                            </div>
                            
                            <div class="dropdown">
                                <a class="font-size-16 text-muted dropdown-toggle p-0" href="#" role="button" data-bs-toggle="dropdown" aria-haspopup="true">
                                    <i class="bx bx-dots-vertical-rounded"></i>
                                </a>
                                <div class="dropdown-menu dropdown-menu-end">
                                    <a class="dropdown-item" href="#" data-action="preview" data-filename="${fileName}" data-url="${fileUrl}" data-ext="${fileExtension}" data-size="${fileFormattedSize}">
                                        <i class="bx bx-show me-1"></i> Ön İzleme
                                    </a>
                                    <a class="dropdown-item" href="#" data-action="copy-link" data-url="${fileUrl}">
                                        <i class="bx bx-link me-1"></i> Link Kopyala
                                    </a>
                                    <a class="dropdown-item" href="#" data-action="rename" data-filename="${fileName}">
                                        <i class="bx bx-edit me-1"></i> Ad Değiştir
                                    </a>
                                    <div class="dropdown-divider"></div>
                                    <a class="dropdown-item text-danger" href="#" data-action="delete" data-filename="${fileName}">
                                        <i class="bx bx-trash me-1"></i> Sil
                                    </a>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            `;
            fileGrid.appendChild(cardCol);
        });
    }

    function renderThumbnail(url, ext) {
        const imageExts = ['jpg', 'jpeg', 'png', 'gif', 'webp', 'svg'];
        if (imageExts.includes(ext)) {
            return `<img src="${url}" alt="Preview" class="img-fluid w-100 h-100" style="object-fit: cover;">`;
        }

        let iconClass = 'bx bx-file';
        if (['pdf'].includes(ext)) iconClass = 'bx bxs-file-pdf text-danger';
        else if (['doc', 'docx'].includes(ext)) iconClass = 'bx bxs-file-doc text-primary';
        else if (['xls', 'xlsx'].includes(ext)) iconClass = 'bx bxs-file-json text-success';
        else if (['zip', 'rar', '7z'].includes(ext)) iconClass = 'bx bxs-file-archive text-warning';
        else if (['mp3', 'wav', 'ogg'].includes(ext)) iconClass = 'bx bxs-music text-info';
        else if (['mp4', 'webm'].includes(ext)) iconClass = 'bx bxs-video text-secondary';
        else if (['txt', 'json', 'xml', 'log', 'css', 'js'].includes(ext)) iconClass = 'bx bx-code-alt text-dark';

        return `<i class="${iconClass} display-4"></i>`;
    }

    function openPreviewModal(filename, url, ext, size) {
        previewModalLabel.textContent = filename;
        previewFileInfo.textContent = `Boyut: ${size} | Tip: ${ext.toUpperCase()}`;
        btnPreviewDownload.href = url;
        previewContainer.innerHTML = '';

        const imageExts = ['jpg', 'jpeg', 'png', 'gif', 'webp', 'svg'];
        const videoExts = ['mp4', 'webm', 'ogg'];
        const audioExts = ['mp3', 'wav', 'ogg'];
        const textExts = ['txt', 'json', 'xml', 'log', 'css', 'js'];

        if (imageExts.includes(ext)) {
            previewContainer.innerHTML = `<img src="${url}" class="img-fluid rounded" style="max-height: 65vh; object-fit: contain;" alt="${filename}">`;
        }
        else if (ext === 'pdf') {
            previewContainer.innerHTML = `<iframe src="${url}" class="w-100 rounded" style="height: 65vh; border: none;"></iframe>`;
        }
        else if (videoExts.includes(ext)) {
            previewContainer.innerHTML = `
                <video controls class="w-100 rounded" style="max-height: 65vh;">
                    <source src="${url}" type="video/${ext}">
                    Tarayıcınız video oynatmayı desteklemiyor.
                </video>`;
        }
        else if (audioExts.includes(ext)) {
            previewContainer.innerHTML = `
                <div class="py-5">
                    <i class="bx bxs-music display-1 text-info mb-3"></i>
                    <audio controls class="w-100 mt-3">
                        <source src="${url}" type="audio/${ext}">
                        Tarayıcınız ses oynatmayı desteklemiyor.
                    </audio>
                </div>`;
        }
        else if (textExts.includes(ext)) {
            previewContainer.innerHTML = `<div class="spinner-border text-primary" role="status"></div>`;
            fetch(url)
                .then(res => res.text())
                .then(text => {
                    previewContainer.innerHTML = `<pre class="text-start w-100 bg-dark text-light p-3 rounded" style="max-height: 60vh; overflow: auto;"><code>${escapeHtml(text)}</code></pre>`;
                })
                .catch(() => {
                    previewContainer.innerHTML = `<p class="text-danger">Metin içeriği yüklenemedi.</p>`;
                });
        }
        else {
            previewContainer.innerHTML = `
                <div class="py-4">
                    <i class="bx bx-file display-1 text-muted mb-3"></i>
                    <p class="text-muted">Bu dosya türü için doğrudan ön izleme desteklenmiyor.</p>
                </div>`;
        }

        previewModal.show();
    }

    function escapeHtml(text) {
        return text
            .replace(/&/g, "&amp;")
            .replace(/</g, "&lt;")
            .replace(/>/g, "&gt;")
            .replace(/"/g, "&quot;")
            .replace(/'/g, "&#039;");
    }

    function formatBytes(bytes, decimals = 2) {
        if (!+bytes) return '0 Bytes';
        const k = 1024;
        const dm = decimals < 0 ? 0 : decimals;
        const sizes = ['Bytes', 'KB', 'MB', 'GB', 'TB'];
        const i = Math.floor(Math.log(bytes) / Math.log(k));
        return `${parseFloat((bytes / Math.pow(k, i)).toFixed(dm))} ${sizes[i]}`;
    }

    function copyToClipboard(text) {
        navigator.clipboard.writeText(text).then(() => {
            Swal.fire({
                toast: true,
                position: 'top-end',
                icon: 'success',
                title: 'Link kopyalandı!',
                showConfirmButton: false,
                timer: 2000
            });
        }).catch(() => {
            Swal.fire('Hata!', 'Link kopyalanamadı.', 'error');
        });
    }

    function renameFile(oldName) {
        Swal.fire({
            title: 'Yeniden Adlandır',
            input: 'text',
            inputValue: oldName,
            showCancelButton: true,
            confirmButtonText: 'Kaydet',
            cancelButtonText: 'İptal',
            customClass: {
                confirmButton: 'btn btn-primary w-xs me-2',
                cancelButton: 'btn btn-secondary w-xs'
            },
            buttonsStyling: false,
            inputValidator: (value) => {
                if (!value || value.trim() === '') {
                    return 'Lütfen geçerli bir dosya adı girin!';
                }
            }
        }).then((result) => {
            if (result.isConfirmed && result.value !== oldName) {
                fetch('/api/FileListApi/RenameFile', {
                    method: 'POST',
                    headers: {
                        ...getAuthHeader(),
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify({ OldName: oldName, NewName: result.value.trim() })
                })
                    .then(res => res.json())
                    .then(res => {
                        const isSuccess = res.Success !== undefined ? res.Success : res.success;
                        const message = res.Message || res.message;
                        if (isSuccess) {
                            Swal.fire('Başarılı!', message, 'success');
                            loadFiles();
                        } else {
                            Swal.fire('Hata!', message, 'error');
                        }
                    })
                    .catch(() => Swal.fire('Hata!', 'İşlem gerçekleştirilemedi.', 'error'));
            }
        });
    }

    function deleteFile(fileName) {
        Swal.fire({
            title: 'Emin misiniz?',
            text: `"${fileName}" dosyasını silmek istediğinize emin misiniz?`,
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Evet, Sil!',
            cancelButtonText: 'İptal',
            customClass: {
                confirmButton: 'btn btn-danger w-xs me-2',
                cancelButton: 'btn btn-secondary w-xs'
            },
            buttonsStyling: false
        }).then((result) => {
            if (result.isConfirmed) {
                fetch('/api/FileListApi/DeleteFile', {
                    method: 'POST',
                    headers: {
                        ...getAuthHeader(),
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify({ FileName: fileName })
                })
                    .then(res => res.json())
                    .then(res => {
                        const isSuccess = res.Success !== undefined ? res.Success : res.success;
                        const message = res.Message || res.message;
                        if (isSuccess) {
                            Swal.fire('Silindi!', message, 'success');
                            loadFiles();
                        } else {
                            Swal.fire('Hata!', message, 'error');
                        }
                    })
                    .catch(() => Swal.fire('Hata!', 'Silme işlemi gerçekleştirilemedi.', 'error'));
            }
        });
    }
});