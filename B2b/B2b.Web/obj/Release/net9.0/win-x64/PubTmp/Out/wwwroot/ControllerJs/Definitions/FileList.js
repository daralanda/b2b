document.addEventListener('DOMContentLoaded', () => {
    // DOM Elemanları
    const searchInput = document.getElementById('fileSearchInput');
    const sortByEl = document.getElementById('sortBy');
    const sortOrderEl = document.getElementById('sortOrder');
    const btnRefresh = document.getElementById('btnRefresh');
    const fileGrid = document.getElementById('fileGrid');

    // Modal Elemanları
    const previewModalEl = document.getElementById('filePreviewModal');
    const previewModal = new bootstrap.Modal(previewModalEl);
    const previewModalLabel = document.getElementById('filePreviewModalLabel');
    const previewContainer = document.getElementById('previewContainer');
    const previewFileInfo = document.getElementById('previewFileInfo');
    const btnPreviewDownload = document.getElementById('btnPreviewDownload');

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

    // Grid İçi Tıklamalar İçin Event Delegation
    fileGrid.addEventListener('click', (e) => {
        const btn = e.target.closest('[data-action]');
        if (!btn) return;

        // Sayfa yenilenmesini engelle
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

    // JWT / Auth Header
    function getAuthHeader() {
        const token = localStorage.getItem('token');
        return token ? { 'Authorization': `Bearer ${token}` } : {};
    }

    // API'den Dosyaları Getirme
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

    // 6'lı Grid Yapısı ile Kartları Çizdirme
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

            // Masaüstü ekranlarda tam 6'lı dizilim: col-xl-2 (12/2 = 6)
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

    // Kart Üzerinde Thumbnail Oluşturma
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

    // Modal Penceresinde Canlı Ön İzleme
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

    // Link Kopyalama
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

    // Ad Değiştirme (SweetAlert2)
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

    // Dosya Silme (SweetAlert2)
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