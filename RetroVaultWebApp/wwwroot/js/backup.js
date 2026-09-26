(function () {
    'use strict';

    const DB_NAME = 'RetroVaultBackup';
    const DB_STORE = 'handles';
    const DB_KEY = 'backupDirHandle';

    const btnSelectFolder = document.getElementById('btn-select-folder');
    const btnBackup = document.getElementById('btn-backup');
    const folderName = document.getElementById('folder-name');
    const backupProgress = document.getElementById('backup-progress');
    const backupProgressBar = document.getElementById('backup-progress-bar');
    const backupProgressText = document.getElementById('backup-progress-text');
    const backupResult = document.getElementById('backup-result');
    const fsSupportAlert = document.getElementById('fs-support-alert');
    const folderSection = document.getElementById('folder-section');

    let currentDirHandle = null;

    function isSupported() {
        return 'showDirectoryPicker' in window;
    }

    function openDB() {
        return new Promise(function (resolve, reject) {
            const request = indexedDB.open(DB_NAME, 1);
            request.onupgradeneeded = function (e) {
                e.target.result.createObjectStore(DB_STORE);
            };
            request.onsuccess = function (e) {
                resolve(e.target.result);
            };
            request.onerror = function () {
                reject(request.error);
            };
        });
    }

    async function storeHandle(handle) {
        const db = await openDB();
        return new Promise(function (resolve, reject) {
            const tx = db.transaction(DB_STORE, 'readwrite');
            tx.objectStore(DB_STORE).put(handle, DB_KEY);
            tx.oncomplete = function () { resolve(); };
            tx.onerror = function () { reject(tx.error); };
        });
    }

    async function loadHandle() {
        const db = await openDB();
        return new Promise(function (resolve, reject) {
            const tx = db.transaction(DB_STORE, 'readonly');
            const request = tx.objectStore(DB_STORE).get(DB_KEY);
            request.onsuccess = function () { resolve(request.result); };
            request.onerror = function () { reject(request.error); };
        });
    }

    async function clearHandle() {
        const db = await openDB();
        return new Promise(function (resolve, reject) {
            const tx = db.transaction(DB_STORE, 'readwrite');
            tx.objectStore(DB_STORE).delete(DB_KEY);
            tx.oncomplete = function () { resolve(); };
            tx.onerror = function () { reject(tx.error); };
        });
    }

    async function verifyPermission(handle) {
        const options = { mode: 'readwrite' };
        const permission = await handle.queryPermission(options);
        if (permission === 'granted') {
            return true;
        }
        const result = await handle.requestPermission(options);
        return result === 'granted';
    }

    function showProgress(percent, text) {
        backupProgress.classList.remove('d-none');
        backupProgressBar.style.width = percent + '%';
        backupProgressText.textContent = text;
    }

    function hideProgress() {
        backupProgress.classList.add('d-none');
        backupProgressBar.style.width = '0%';
    }

    function showResult(message, isError) {
        backupResult.classList.remove('d-none', 'alert-success', 'alert-danger');
        backupResult.classList.add('alert', isError ? 'alert-danger' : 'alert-success');
        backupResult.textContent = message;
    }

    function hideResult() {
        backupResult.classList.add('d-none');
    }

    async function selectFolder() {
        try {
            const handle = await window.showDirectoryPicker({ mode: 'readwrite' });
            const granted = await verifyPermission(handle);
            if (!granted) {
                showResult('Permission to access the folder was denied.', true);
                return;
            }
            currentDirHandle = handle;
            await storeHandle(handle);
            folderName.textContent = handle.name;
            btnBackup.disabled = false;
            hideResult();
        } catch (err) {
            if (err.name !== 'AbortError') {
                showResult('Could not select folder: ' + err.message, true);
            }
        }
    }

    async function tryRestoreHandle() {
        try {
            const handle = await loadHandle();
            if (!handle) {
                return;
            }
            const granted = await verifyPermission(handle);
            if (granted) {
                currentDirHandle = handle;
                folderName.textContent = handle.name;
                btnBackup.disabled = false;
            } else {
                await clearHandle();
            }
        } catch {
            await clearHandle();
        }
    }

    async function getDirHandle(dirHandle, name) {
        try {
            return await dirHandle.getDirectoryHandle(name, { create: true });
        } catch {
            return null;
        }
    }

    async function fileExists(dirHandle, filename) {
        try {
            await dirHandle.getFileHandle(filename);
            return true;
        } catch {
            return false;
        }
    }

    async function writeFile(dirHandle, filename, data) {
        const fileHandle = await dirHandle.getFileHandle(filename, { create: true });
        const writable = await fileHandle.createWritable();
        await writable.write(data);
        await writable.close();
    }

    async function generateFilename(dateStr, dirHandle) {
        for (let seq = 1; seq <= 99; seq++) {
            const padded = seq.toString().padStart(2, '0');
            const name = 'vault-' + dateStr + '_' + padded + '.json';
            if (!await fileExists(dirHandle, name)) {
                return name;
            }
        }
        throw new Error('Too many backups in one day (max 99)');
    }

    async function backupNow() {
        if (!currentDirHandle) {
            return;
        }

        btnBackup.disabled = true;
        btnSelectFolder.disabled = true;
        hideResult();

        try {
            const granted = await verifyPermission(currentDirHandle);
            if (!granted) {
                showResult('Permission to access the folder was lost. Please select it again.', true);
                return;
            }

            showProgress(5, 'Fetching vault items...');

            const itemsResponse = await fetch('?handler=Items');
            if (!itemsResponse.ok) {
                throw new Error('Failed to fetch vault items: ' + itemsResponse.status);
            }
            const itemCount = parseInt(itemsResponse.headers.get('X-Item-Count') || '0', 10);
            const itemsData = await itemsResponse.text();

            showProgress(15, 'Writing vault.json...');
            const today = new Date().toISOString().slice(0, 10);
            const itemsFilename = await generateFilename(today, currentDirHandle);
            await writeFile(currentDirHandle, itemsFilename, itemsData);

            showProgress(20, 'Fetching thumbnail list...');
            const thumbnailsResponse = await fetch('?handler=Thumbnails');
            if (!thumbnailsResponse.ok) {
                throw new Error('Failed to fetch thumbnail list: ' + thumbnailsResponse.status);
            }
            const thumbnailFilenames = await thumbnailsResponse.json();

            const thumbnailsDir = await getDirHandle(currentDirHandle, 'thumbnails');
            if (!thumbnailsDir) {
                throw new Error('Could not create thumbnails directory');
            }

            let newCount = 0;
            let skippedCount = 0;
            const total = thumbnailFilenames.length;

            for (let i = 0; i < total; i++) {
                const filename = thumbnailFilenames[i];
                const percent = 20 + Math.round((i / total) * 80);
                showProgress(percent, 'Processing thumbnails (' + (i + 1) + '/' + total + ')...');

                if (await fileExists(thumbnailsDir, filename)) {
                    skippedCount++;
                    continue;
                }

                const id = filename.replace('.png', '');
                const imgResponse = await fetch('?handler=Thumbnail&id=' + encodeURIComponent(id));
                if (!imgResponse.ok) {
                    continue;
                }
                const blob = await imgResponse.blob();
                await writeFile(thumbnailsDir, filename, blob);
                newCount++;
            }

            showProgress(100, 'Done!');
            hideProgress();
            const msg = 'Backup completed: ' + itemCount + ' items saved! ' + newCount + ' new thumbnail(s) saved, ' + skippedCount + ' already existed.';
            showResult(msg, false);
        } catch (err) {
            hideProgress();
            showResult('Backup failed: ' + err.message, true);
        } finally {
            btnBackup.disabled = false;
            btnSelectFolder.disabled = false;
        }
    }

    if (!isSupported()) {
        fsSupportAlert.classList.remove('d-none');
        folderSection.classList.add('d-none');
        return;
    }

    btnSelectFolder.addEventListener('click', selectFolder);
    btnBackup.addEventListener('click', backupNow);

    tryRestoreHandle();
})();
