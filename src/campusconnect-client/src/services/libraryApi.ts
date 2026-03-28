const API_BASE_URL = 'http://localhost:5099/api';

// Helper pentru a obține token-ul
const getAuthToken = () => {
  return localStorage.getItem('token');
};

// Helper pentru headere cu autentificare
// ATENȚIE: pentru upload (FormData) NU punem Content-Type, browserul îl pune singur.
const getAuthHeaders = () => {
  const token = getAuthToken();
  return {
    'Content-Type': 'application/json',
    ...(token && { Authorization: `Bearer ${token}` })
  };
};

const getAuthHeadersNoContentType = () => {
  const token = getAuthToken();
  return {
    ...(token && { Authorization: `Bearer ${token}` })
  };
};

export interface LibraryFolder {
  id: string; // Guid
  name: string;
  createdAtUtc: string;
  itemsCount?: number;
}

export interface LibraryItem {
  id: string; // Guid
  title: string;
  type: number | string; // poate veni 1/2 sau "File"/"Link"
  url?: string | null;

  storedFileName?: string | null;
  originalFileName?: string | null;
  contentType?: string | null;
  sizeBytes?: number | null;

  createdByUserId: string;
  createdAtUtc: string;
}

export interface CreateFolderRequest {
  name: string;
}

export interface CreateLinkRequest {
  title: string;
  url: string;
}

export const libraryApi = {
  // FOLDERS
    // getFolders: async (): Promise<LibraryFolder[]> => {
    //     const response = await fetch(`${API_BASE_URL}/library/folders`, {
    //         headers: getAuthHeaders()
    //     });
    //     if (!response.ok) throw new Error('Failed to fetch library folders');
    //     return response.json();
    // },
    getFolders: async (): Promise<LibraryFolder[]> => {
        const response = await fetch(`${API_BASE_URL}/library/folders`, {
            headers: getAuthHeaders()
        });

        if (!response.ok) {
            const errText = await response.text().catch(() => '');
            throw new Error(
                `Failed to fetch library folders (${response.status}) ${errText || response.statusText}`
            );
        }

        return response.json();
    },


  createFolder: async (data: CreateFolderRequest): Promise<LibraryFolder> => {
    const response = await fetch(`${API_BASE_URL}/library/folders`, {
      method: 'POST',
      headers: getAuthHeaders(),
      body: JSON.stringify(data)
    });

    if (!response.ok) {
      const err = await response.text().catch(() => '');
      throw new Error(err || 'Failed to create folder');
    }

    return response.json();
  },

  deleteFolder: async (folderId: string): Promise<void> => {
    const response = await fetch(`${API_BASE_URL}/library/folders/${folderId}`, {
      method: 'DELETE',
      headers: getAuthHeaders()
    });
    if (!response.ok) throw new Error('Failed to delete folder');
  },

  // ITEMS
  getItems: async (folderId: string): Promise<LibraryItem[]> => {
    const response = await fetch(`${API_BASE_URL}/library/folders/${folderId}/items`, {
      headers: getAuthHeaders()
    });
    if (!response.ok) throw new Error('Failed to fetch library items');
    return response.json();
  },

  addLink: async (folderId: string, data: CreateLinkRequest): Promise<{ id: string }> => {
    const response = await fetch(`${API_BASE_URL}/library/folders/${folderId}/items/link`, {
      method: 'POST',
      headers: getAuthHeaders(),
      body: JSON.stringify(data)
    });
    if (!response.ok) throw new Error('Failed to add link');
    return response.json();
  },

  uploadFile: async (folderId: string, title: string, file: File): Promise<{ id: string }> => {
    const form = new FormData();
    form.append('title', title);
    form.append('file', file);

    const response = await fetch(`${API_BASE_URL}/library/folders/${folderId}/items/file`, {
      method: 'POST',
      headers: getAuthHeadersNoContentType(),
      body: form
    });

    if (!response.ok) {
      const err = await response.text().catch(() => '');
      throw new Error(err || 'Failed to upload file');
    }

    return response.json();
  },

  deleteItem: async (itemId: string): Promise<void> => {
    const response = await fetch(`${API_BASE_URL}/library/items/${itemId}`, {
      method: 'DELETE',
      headers: getAuthHeaders()
    });
    if (!response.ok) throw new Error('Failed to delete item');
  },

  // Download: returnează Blob, fiindcă trebuie token în header
  downloadItem: async (itemId: string): Promise<Blob> => {
    const response = await fetch(`${API_BASE_URL}/library/items/${itemId}/download`, {
      headers: getAuthHeadersNoContentType()
    });
    if (!response.ok) throw new Error('Failed to download file');
    return response.blob();
  }
};
